using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using nscreg.Data;
using nscreg.Data.Constants;
using nscreg.Data.Entities;
using nscreg.Resources.Languages;
using nscreg.Server.Common.Helpers;
using nscreg.Server.Common.Models.StatUnits;
using nscreg.Server.Common.Models.StatUnits.Create;
using nscreg.Utilities.Configuration.StatUnitAnalysis;
using nscreg.Utilities.Extensions;
using nscreg.Server.Common.Services.Contracts;
using nscreg.Utilities.Configuration;
using nscreg.Utilities.Configuration.DBMandatoryFields;
using nscreg.Utilities.Enums;
using Activity = nscreg.Data.Entities.Activity;
using EnterpriseGroup = nscreg.Data.Entities.EnterpriseGroup;
using LegalUnit = nscreg.Data.Entities.LegalUnit;
using LocalUnit = nscreg.Data.Entities.LocalUnit;
using Person = nscreg.Data.Entities.Person;

namespace nscreg.Server.Common.Services.StatUnit
{
    /// <summary>
    /// Class service creation
    /// </summary>
    public class CreateService
    {
        private readonly NSCRegDbContext _dbContext;
        private readonly IUserService _userService;
        private readonly CommonService _commonSvc;
        private readonly DataAccessService _dataAccessService;
        private const bool _shouldAnalyze = true;
        private readonly IMapper _mapper;
        private readonly IStatUnitAnalyzeService _analysisService;
        private readonly StatUnitCheckPermissionsHelper _statUnitCheckPermissionsHelper;
        private readonly StatUnitCreationHelper _statUnitCreationHelper;

        public CreateService(NSCRegDbContext dbContext, IMapper mapper, IUserService userService,
            CommonService commonSvc, DataAccessService dataAccessService, IStatUnitAnalyzeService analysisService,
            StatUnitCheckPermissionsHelper statUnitCheckPermissionsHelper, StatUnitCreationHelper statUnitCreationHelper)
        {
            _dbContext = dbContext;
            _userService = userService;
            _commonSvc = commonSvc;
            _dataAccessService = dataAccessService;
            _mapper = mapper;
            _analysisService = analysisService;
            _statUnitCheckPermissionsHelper = statUnitCheckPermissionsHelper;
            _statUnitCreationHelper = statUnitCreationHelper;
        }

        /// <summary>
        /// Legal unit creation method
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="userId">User Id</param>
        /// <returns></returns>
        public async Task<Dictionary<string, string[]>> CreateLegalUnit(LegalUnitCreateM data, string userId)
            => await CreateUnitContext<LegalUnit, LegalUnitCreateM>(data, userId, unit =>
            {
                var isAdmin = _userService.IsInRoleAsync(userId, DefaultRoleNames.Administrator).Result;
                if (!isAdmin)
                {
                    var regionIds = new List<int?>
                    {
                        data.Address?.RegionId,
                        data.ActualAddress?.RegionId,
                        data.PostalAddress?.RegionId
                    }.Where(x => x != null)
                    .Select(x => (int)x)
                    .ToList();
                    _statUnitCheckPermissionsHelper.CheckRegionOrActivityContains(userId, regionIds, data.Activities.Select(x => x.ActivityCategoryId).ToList());
                }
                if (_commonSvc.HasAccess<LegalUnit>(data.DataAccess, v => v.LocalUnits))
                {
                    if (data.LocalUnits != null && data.LocalUnits.Any())
                    {
                        var localUnits = _dbContext.LocalUnits.Where(x => data.LocalUnits.Contains(x.RegId));
                        foreach (var localUnit in localUnits)
                        {
                            unit.LocalUnits.Add(localUnit);
                        }
                        unit.HistoryLocalUnitIds = string.Join(",", data.LocalUnits);
                    }
                }
                return Task.CompletedTask;
            });

        /// <summary>
        /// Local unit creation method
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="userId">User Id</param>
        /// <returns></returns>
        public async Task<Dictionary<string, string[]>> CreateLocalUnit(LocalUnitCreateM data, string userId)
        {
            var isAdmin = _userService.IsInRoleAsync(userId, DefaultRoleNames.Administrator).Result;
            if (!isAdmin)
            {
                var regionIds = new List<int?>
                {
                    data.Address?.RegionId,
                    data.ActualAddress?.RegionId,
                    data.PostalAddress?.RegionId
                }.Where(x => x != null)
                .Select(x => (int)x)
                .ToList();
                _statUnitCheckPermissionsHelper.CheckRegionOrActivityContains(userId, regionIds, data.Activities.Select(x => x.ActivityCategoryId).ToList());
            }
            return await CreateUnitContext<LocalUnit, LocalUnitCreateM>(data, userId, null);
        }

        /// <summary>
        /// Enterprise creation method
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="userId">User Id</param>
        /// <returns></returns>
        public async Task<Dictionary<string, string[]>> CreateEnterpriseUnit(EnterpriseUnitCreateM data, string userId)
            => await CreateUnitContext<EnterpriseUnit, EnterpriseUnitCreateM>(data, userId, unit =>
            {
                var isAdmin = _userService.IsInRoleAsync(userId, DefaultRoleNames.Administrator).Result;
                if (!isAdmin)
                {
                    var regionIds = new List<int?>
                    {
                        data.Address?.RegionId,
                        data.ActualAddress?.RegionId,
                        data.PostalAddress?.RegionId
                    }.Where(x => x != null)
                    .Select(x => (int)x)
                    .ToList();
                    _statUnitCheckPermissionsHelper.CheckRegionOrActivityContains(userId, regionIds,data.Activities.Select(x => x.ActivityCategoryId).ToList());
                }
                if (data.LegalUnits != null && data.LegalUnits.Any())
                {
                    var legalUnits = _dbContext.LegalUnits.Where(x => data.LegalUnits.Contains(x.RegId)).ToList();
                    foreach (var legalUnit in legalUnits)
                    {
                        unit.LegalUnits.Add(legalUnit);
                    }
                    unit.HistoryLegalUnitIds = string.Join(",", data.LegalUnits);
                }
                return Task.CompletedTask;
            });

        /// <summary>
        /// Method for creating an enterprise group
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="userId">User Id</param>
        /// <returns></returns>
        public async Task<Dictionary<string, string[]>> CreateEnterpriseGroup(EnterpriseGroupCreateM data, string userId)
            => await CreateContext<EnterpriseGroup, EnterpriseGroupCreateM>(data, userId, unit =>
            {
                var isAdmin = _userService.IsInRoleAsync(userId, DefaultRoleNames.Administrator).Result;
                if (!isAdmin)
                {
                    var regionIds = new List<int?> { data.Address?.RegionId, data.ActualAddress?.RegionId, data.PostalAddress?.RegionId }.Where(x => x != null).Select(x => (int)x).ToList();
                    _statUnitCheckPermissionsHelper.CheckRegionOrActivityContains(userId, regionIds, new List<int>());
                }

                if (_commonSvc.HasAccess<EnterpriseGroup>(data.DataAccess, v => v.EnterpriseUnits))
                {
                    if (data.EnterpriseUnits != null && data.EnterpriseUnits.Any())
                    {
                        var enterprises = _dbContext.EnterpriseUnits.Where(x => data.EnterpriseUnits.Contains(x.RegId))
                            .ToList();
                        foreach (var enterprise in enterprises)
                        {
                            unit.EnterpriseUnits.Add(enterprise);
                        }
                        unit.HistoryEnterpriseUnitIds = string.Join(",", data.EnterpriseUnits);
                    }
                }
                
                return Task.CompletedTask;
            });

        /// <summary>
        /// Static unit context creation method
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="userId">User Id</param>
        /// <param name="work">In work</param>
        /// <returns></returns>
        private async Task<Dictionary<string, string[]>> CreateUnitContext<TUnit, TModel>( TModel data, string userId, Func<TUnit, Task> work)
            where TModel : StatUnitModelBase
            where TUnit : StatisticalUnit, new()
            => await CreateContext<TUnit, TModel>(data, userId, async unit =>
            {
                if (_commonSvc.HasAccess<TUnit>(data.DataAccess, v => v.Activities))
                {
                    var activitiesList = data.Activities ?? new List<ActivityM>();

                    unit.ActivitiesUnits.AddRange(activitiesList.Select(v =>
                        {
                            var activity = _mapper.Map<ActivityM, Activity>(v);
                            activity.Id = 0;
                            activity.ActivityCategoryId = v.ActivityCategoryId;
                            activity.UpdatedBy = userId;
                            return new ActivityStatisticalUnit {Activity = activity};
                        }
                    ));
                }

                var personList = data.Persons ?? new List<PersonM>();
                unit.PersonsUnits.AddRange(personList.Select(v =>
                {
                    if (v.Id.HasValue && v.Id > 0)
                    {
                        return new PersonStatisticalUnit { PersonId = (int)v.Id, PersonTypeId = v.Role };
                    }
                    var newPerson = _mapper.Map<PersonM, Person>(v);
                    return new PersonStatisticalUnit { Person = newPerson, PersonTypeId = v.Role };
                }));

                var statUnits = data.PersonStatUnits ?? new List<PersonStatUnitModel>();
                foreach (var unitM in statUnits)
                {
                    unit.PersonsUnits.Add(new PersonStatisticalUnit
                    {
                        EnterpriseGroupId = unitM.StatRegId == null ? unitM.GroupRegId : null,
                        PersonId = null,
                        PersonTypeId = unitM.RoleId
                    });
                }

                var countriesList = data.ForeignParticipationCountriesUnits ?? new List<int>();

                unit.ForeignParticipationCountriesUnits.AddRange(countriesList.Select(v => new CountryStatisticalUnit { CountryId = v }));

                unit.SizeId = unit.SizeId == 0 ? null : unit.SizeId;

                await (work?.Invoke(unit) ?? Task.CompletedTask);
            });

        /// <summary>
        /// Context сreation method
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="userId">User Id</param>
        /// <param name="work">In Work</param>
        /// <returns></returns>
        private async Task<Dictionary<string, string[]>> CreateContext<TUnit, TModel>( TModel data, string userId, Func<TUnit, Task> work)
            where TModel : IStatUnitM
            where TUnit : class, IStatisticalUnit, new()
        {
            var unit = new TUnit();
            if (_dataAccessService.CheckWritePermissions(userId, unit.UnitType))
            {
                return new Dictionary<string, string[]> { { nameof(UserAccess.UnauthorizedAccess), new[] { nameof(Resource.Error403) } } };
            }

            await _commonSvc.InitializeDataAccessAttributes(data, userId, unit.UnitType);
            _mapper.Map(data, unit);
            await _commonSvc.AddAddresses<TUnit>(unit, data);

            if (work != null)
            {
                await work(unit);
            }
            unit.UserId = userId;

            if (_shouldAnalyze)
            {
                var analyzeResult = await _analysisService.AnalyzeStatUnit(unit, isSkipCustomCheck: true);
                if (analyzeResult.Messages.Any()) return analyzeResult.Messages;
            }

            await _statUnitCreationHelper.CheckElasticConnect();
            if (unit is LocalUnit)
                await _statUnitCreationHelper.CreateLocalUnit(unit as LocalUnit);
            else if (unit is LegalUnit)
                await _statUnitCreationHelper.CreateLegalWithEnterpriseAndLocal(unit as LegalUnit);
            else if (unit is EnterpriseUnit)
                await _statUnitCreationHelper.CreateEnterpriseWithGroup(unit as EnterpriseUnit);
            else if (unit is EnterpriseGroup)
                await _statUnitCreationHelper.CreateGroup(unit as EnterpriseGroup);

            return null;
        }
    }
}
