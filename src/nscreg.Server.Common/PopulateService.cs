using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using nscreg.Business.DataSources;
using nscreg.Data;
using nscreg.Data.Constants;
using nscreg.Data.Entities;
using nscreg.Resources.Languages;
using nscreg.Server.Common.Services.DataSources;
using nscreg.Utilities.Extensions;

namespace nscreg.Server.Common
{
    /// <summary>
    /// Service for populating unit
    /// </summary>
    public class PopulateService
    {
        private readonly DataSourceAllowedOperation _allowedOperation;
        private readonly string _personRoleSource;
        private readonly DataSourceUploadTypes _uploadType;
        private readonly string _statIdSourceKey;
        private readonly StatUnitTypes _unitType;
        private readonly NSCRegDbContext _context;
        private readonly StatUnitPostProcessor _postProcessor;
        public PopulateService((string source, string target)[] propMapping, DataSourceAllowedOperation operation, DataSourceUploadTypes uploadType, StatUnitTypes unitType, NSCRegDbContext context)
        {
            _personRoleSource = propMapping.FirstOrDefault(c => c.target == "Persons.Person.Role").source;
            _statIdSourceKey = StatUnitKeyValueParser.GetStatIdSourceKey(propMapping) ?? throw new ArgumentNullException(nameof(propMapping), "StatId doesn't have source field(header)");
            _context = context;
            _unitType = unitType;
            _allowedOperation = operation;
            _uploadType = uploadType;
            _postProcessor = new StatUnitPostProcessor(context);
        }

        /// <summary>
        /// Method for populate unit
        /// </summary>
        /// <param name="raw">Parsed  data of a unit</param>
        /// <returns></returns>
        public async Task<(StatisticalUnit unit, bool isNew, string errors)> PopulateAsync(IReadOnlyDictionary<string, object> raw)
        {
            try
            {
                var (resultUnit, isNew) = await GetStatUnitBase(raw);

                // Check for operation errors
                if (_allowedOperation == DataSourceAllowedOperation.Create && !isNew)
                {
                    var statId = raw.GetValueOrDefault(_statIdSourceKey);
                    return (resultUnit, false, string.Format(Resource.StatisticalUnitWithSuchStatIDAlreadyExists, statId));
                }

                if (_allowedOperation == DataSourceAllowedOperation.Alter && isNew)
                {
                    return (resultUnit, true,
                        $"StatUnit failed with error: {Resource.StatUnitIdIsNotFound} ({resultUnit.StatId})");
                }

                raw = await TransformReferenceField(raw, "Persons.Person.Role", (value) =>
                {
                    // Todo: can be cached
                    return _context.PersonTypes.FirstOrDefaultAsync(x =>
                            x.Name == value || x.NameLanguage1 == value || x.NameLanguage2 == value);
                });

                StatUnitKeyValueParser.ParseAndMutateStatUnit(raw, resultUnit);

                var errors = await _postProcessor.FillIncompleteDataOfStatUnit(resultUnit, _uploadType);

                return (resultUnit, isNew, errors);
            }
            catch (Exception ex)
            {
                return (ex.Data["unit"] as StatisticalUnit, false, ex.Message);
            }
        }
        /// <summary>
        /// Returns existed or new stat unit
        /// </summary>
        /// <param name="operation">Data source operation(enum)</param>
        /// <param name="raw">Parsed data of a unit</param>
        /// <returns></returns>
        private async Task<(StatisticalUnit unit, bool isNew)> GetStatUnitBase(IReadOnlyDictionary<string, object> raw)
        {
            if (!_statIdSourceKey.HasValue() || !raw.TryGetValue(_statIdSourceKey, out var statId))
                return (GetStatUnitSetHelper.CreateByType(_unitType), true);

            var existing = await GetStatUnitSetHelper
                .GetStatUnitSet(_context, _unitType)
                .FirstOrDefaultAsync(x => x.StatId == statId.ToString());

            if (existing == null) return (GetStatUnitSetHelper.CreateByType(_unitType), true);
            _context.Entry(existing).State = EntityState.Detached;
            return (unit: existing, isNew: false);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="raw"></param>
        /// <param name="referenceField"></param>
        /// <param name="getEntityAction"></param>
        /// <returns></returns>
        private async Task<IReadOnlyDictionary<string, object>> TransformReferenceField<TEntity>(IReadOnlyDictionary<string, object> raw, string referenceField, Func<string, Task<TEntity>> getEntityAction)
     where TEntity : LookupBase
        {
            ///todo Разобраться с нужностью этого метода.
            Dictionary<string, object> result = new Dictionary<string, object>();
            if (string.IsNullOrEmpty(_personRoleSource))
            {
                return raw;
            }
            var parts = referenceField.Split('.');

            if (!raw.TryGetValue(_personRoleSource, out var propValue) ||
                !(raw[_personRoleSource] is IList<KeyValuePair<string, Dictionary<string, string>>> parents))
                return raw;

            List<int> idsArray = new List<int>();
            List<string> errorArray = new List<string>();
            foreach (var par in parents)
            {
                var value = par.Value[parts.Last()];
                var entity = await getEntityAction(value);
                if (entity != null)
                {
                    idsArray.Add(entity.Id);
                }
                else
                {
                    errorArray.Add(value);
                }
            }
            if (errorArray.Any()) throw new Exception($"Reference for {string.Join(",", errorArray)} was not found");
            foreach (var keyValuePair in raw)
            {
                if (keyValuePair.Value is string)
                {
                    result[keyValuePair.Key] = keyValuePair.Value;
                }
                else
                {
                    var val = keyValuePair.Value as IList<KeyValuePair<string, Dictionary<string, string>>>;
                    for (int i = 0; i < val.Count; i++)
                    {
                        var elem = new List<KeyValuePair<string, Dictionary<string, string>>>();
                        if (keyValuePair.Value is IList<KeyValuePair<string, Dictionary<string, string>>> arrayKeyValuePair)
                            foreach (var kv in arrayKeyValuePair)
                            {
                                var dic = new Dictionary<string, string>();
                                foreach (var kvValue in kv.Value)
                                {
                                    dic.Add(kvValue.Key, kvValue.Key == parts.Last() ? idsArray[i].ToString() : kvValue.Value);
                                }
                                elem.Add(new KeyValuePair<string, Dictionary<string, string>>(kv.Key, dic));
                            }

                        result[keyValuePair.Key] = elem;
                    }
                }
            }
            return result;
        }
    }
}
