using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using nscreg.Data.Constants;
using LogStatus = nscreg.Data.Constants.DataUploadingLogStatuses;
using nscreg.Server.Common.Services.DataSources;
using Newtonsoft.Json;
using nscreg.Data.Entities;
using nscreg.Utilities.Enums;
using nscreg.Business.Analysis.StatUnit;
using System.Linq;
using nscreg.Server.Common.Services;
using nscreg.Data;
using nscreg.Server.Common.Models.StatUnits;
using nscreg.Server.Common.Services.StatUnit;
using nscreg.Utilities.Configuration.StatUnitAnalysis;
using nscreg.Utilities.Configuration.DBMandatoryFields;
using nscreg.Utilities.Configuration;
using nscreg.Utilities.Extensions;
using nscreg.Resources.Languages;
using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;

namespace nscreg.Server.DataUploadSvc
{
    internal class ImportExecutor
    {
        public bool AnyWarnings { get; private set; }

        private readonly ILogger _logger;
        private readonly DbLogBuffer _logBuffer;
        private readonly StatUnitAnalysisRules _statUnitAnalysisRules;
        private readonly DbMandatoryFields _dbMandatoryFields;
        private readonly ValidationSettings _validationSettings;

        private AnalyzeService _analysisSvc;

        BlockingCollection<IReadOnlyDictionary<string, object>> _tasksQueue;

#if DEBUG
        public Stopwatch swPopulation = new Stopwatch();
        public long populationCount = 0;

        public Stopwatch swAnalyze = new Stopwatch();
        public long analyzeCount = 0;

        public Stopwatch swSave = new Stopwatch();
        public long saveCount = 0;

        public Stopwatch swDbLog = new Stopwatch();
        public long dbLogCount = 0;
#endif
        public ImportExecutor(StatUnitAnalysisRules _statUnitAnalysisRules, DbMandatoryFields _dbMandatoryFields, ValidationSettings _validationSettings, ILogger _logger, DbLogBuffer _logBuffer)
        {
            this._statUnitAnalysisRules = _statUnitAnalysisRules;
            this._dbMandatoryFields = _dbMandatoryFields;
            this._validationSettings = _validationSettings;
            this._logger = _logger;
            this._logBuffer = _logBuffer;
        }

        public void UseTasksQueue(BlockingCollection<IReadOnlyDictionary<string, object>> collection)
        {
            _tasksQueue = collection;
        }

        public Task Start(DataSourceQueue dequeued)
        {
            return Task.Run(async () =>
            {
                var dbContextHelper = new DbContextHelper();
                var _context = dbContextHelper.CreateDbContext(new string[] { });

                await InitializeCacheForLookups(_context);

                var userService = new UserService(_context);
                var commonSvc = new Common.Services.StatUnit.Common(_context);
                var permissions = await commonSvc.InitializeDataAccessAttributes<IStatUnitM>(userService, null, dequeued.UserId, dequeued.DataSource.StatUnitType);
                var populateService = new PopulateService(dequeued.DataSource.VariablesMappingArray, dequeued.DataSource.AllowedOperations, dequeued.DataSource.StatUnitType, _context, dequeued.UserId, permissions);
                _analysisSvc = new AnalyzeService(_context, _statUnitAnalysisRules, _dbMandatoryFields, _validationSettings);
                var saveService = await SaveManager.CreateSaveManager(_context, dequeued.UserId, permissions);

                var i = 0;
                foreach (var parsedUnit in _tasksQueue.GetConsumingEnumerable())
                {
                    //_logger.LogInformation("processing entity #{0} ({1:0.00} %)", i + 1, (double)i / parsed.Length * 100);
                    var startedAt = DateTime.Now;

                    /// Populate Unit

                    swPopulation.Start();
                    _logger.LogInformation("populating unit");
                    var (populated, isNew, populateError, historyUnit) = await populateService.PopulateAsync(parsedUnit);
                    swPopulation.Stop();
                    populationCount += 1;
                    if (populateError.HasValue())
                    {
                        _logger.LogInformation("error during populating of unit: {0}", populateError);
                        AnyWarnings = true;
                        await LogUpload(LogStatus.Error, populateError, analysisSummary: new List<string>() { populateError });
                        continue;
                    }

                    populated.DataSource = dequeued.DataSourceFileName;
                    populated.ChangeReason = ChangeReasons.Edit;
                    populated.EditComment = "Uploaded from data source file";

                    /// Analyze Unit

                    _logger.LogInformation(
                        "analyzing populated unit #{0} RegId={1}", i + 1,
                        populated.RegId > 0 ? populated.RegId.ToString() : "(new)");

                    swAnalyze.Start();

                    var (analysisError, (errors, summary)) = AnalyzeUnit(populated, dequeued);
                    swAnalyze.Stop();
                    analyzeCount += 1;

                    if (analysisError.HasValue())
                    {
                        _logger.LogInformation("analysis attempt failed with error: {0}", analysisError);
                        AnyWarnings = true;
                        await LogUpload(LogStatus.Error, analysisError);
                        continue;
                    }
                    if (errors.Any())
                    {
                        _logger.LogInformation("analysis revealed {0} errors", errors.Count);
                        errors.Values.ForEach(x => x.ForEach(e => _logger.LogInformation(Resource.ResourceManager.GetString(e.ToString()))));
                        AnyWarnings = true;
                        await LogUpload(LogStatus.Warning, string.Join(",", errors.SelectMany(c => c.Value)), errors, summary);
                        continue;
                    }

                    /// Save Unit

                    _logger.LogInformation("saving unit");

                    swSave.Start();
                    var (saveError, saved) = await saveService.SaveUnit(populated, dequeued.DataSource, dequeued.UserId, isNew, historyUnit);

                    swSave.Stop();
                    saveCount += 1;

                    if (saveError.HasValue())
                    {
                        _logger.LogError(saveError);
                        AnyWarnings = true;
                        await LogUpload(LogStatus.Warning, saveError);
                        continue;
                    }

                    if (!saved) AnyWarnings = true;
                    await LogUpload(saved ? LogStatus.Done : LogStatus.Warning);


                    async Task LogUpload(LogStatus status, string note = "",
                            IReadOnlyDictionary<string, string[]> analysisErrors = null,
                            IEnumerable<string> analysisSummary = null)
                    {
                        swDbLog.Start();
                        var rawUnit = JsonConvert.SerializeObject(dequeued.DataSource.VariablesMappingArray.ToDictionary(x => x.target, x =>
                        {
                            var tmp = x.source.Split('.', 2);
                            //if (parsed[i].ContainsKey(tmp[0]))
                            //    return JsonConvert.SerializeObject(parsed[i][tmp[0]]);
                            return tmp[0];
                        }));
                        await _logBuffer.LogUnitUpload(
                             dequeued.Id, rawUnit, startedAt, populated,
                             status, note ?? "", analysisErrors, analysisSummary);

                        swDbLog.Stop();
                    }
                }
            });
        }

        private (string, (IReadOnlyDictionary<string, string[]>, string[] test)) AnalyzeUnit(IStatisticalUnit unit, DataSourceQueue queueItem)
        {
            if (queueItem.DataSource.DataSourceUploadType != DataSourceUploadTypes.StatUnits)
                return (null, (new Dictionary<string, string[]>(), new string[0]));

            AnalysisResult analysisResult;
            try
            {
                analysisResult = _analysisSvc.AnalyzeStatUnit(unit, queueItem.DataSource.AllowedOperations == DataSourceAllowedOperation.Alter, true, false);
            }
            catch (Exception ex)
            {
                return (ex.Message, (null, null));
            }
            return (null, (
                analysisResult.Messages,
                analysisResult.SummaryMessages?.ToArray() ?? Array.Empty<string>()));
        }

        private static async Task InitializeCacheForLookups(NSCRegDbContext context)
        {
            await context.ActivityCategories.LoadAsync();
            await context.PersonTypes.LoadAsync();
            await context.RegistrationReasons.LoadAsync();
            await context.Regions.LoadAsync();
            await context.UnitsSize.LoadAsync();
            await context.Statuses.LoadAsync();
            await context.ReorgTypes.LoadAsync();
            await context.SectorCodes.LoadAsync();
            await context.DataSourceClassifications.LoadAsync();
            await context.LegalForms.LoadAsync();
            await context.ForeignParticipations.LoadAsync();
            await context.Countries.LoadAsync();
        }
    }
}
