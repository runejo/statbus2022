using Newtonsoft.Json;
using nscreg.Data.Entities;
using nscreg.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using nscreg.Data;
using nscreg.Data.Constants;
using static nscreg.Utilities.JsonPathHelper;

namespace nscreg.Business.DataSources
{
    public static class StatUnitKeyValueParser
    {
        public static readonly string[] StatisticalUnitArrayPropertyNames = { nameof(StatisticalUnit.Activities), nameof(StatisticalUnit.Persons), nameof(StatisticalUnit.ForeignParticipationCountriesUnits) };

        public static string GetStatIdMapping(IEnumerable<(string source, string target)> mapping)
            => mapping.FirstOrDefault(vm => vm.target == nameof(StatisticalUnit.StatId)).target;

        public static void ParseAndMutateStatUnit(
            IReadOnlyDictionary<string, object> nextProps,
            StatisticalUnit unit, NSCRegDbContext context)
        {
            foreach (var kv in nextProps)
            {

                if (kv.Value is string)
                {
                    try
                    {
                        UpdateObject(kv.Key, kv.Value);
                    }
                    catch (Exception ex)
                    {
                        ex.Data.Add("target property", kv.Key);
                        ex.Data.Add("value", kv.Value);
                        ex.Data.Add("unit", unit);
                        throw;
                    }
                }
                else if (kv.Value is List<KeyValuePair<string, Dictionary<string, string>>> arrayProperty)
                {
                    var targetArrKeys = arrayProperty.SelectMany(x=>x.Value.Select(d=>d.Key)).Distinct();
                    var mapping = targetArrKeys.ToDictionary(x => x, x => new string[] { x });
                    try
                    {
                        UpdateObject(kv.Key, kv.Value, mapping);
                    }
                    catch (Exception ex)
                    {
                        ex.Data.Add("source property", kv.Key);
                        ex.Data.Add("target property", targetArrKeys);
                        ex.Data.Add("value", kv.Value);
                        ex.Data.Add("unit", unit);
                        throw;
                    }
                }
                else
                {
                    System.Diagnostics.Debug.Fail("Bad sector of code. NextProps: " + JsonConvert.SerializeObject(nextProps));
                }
            }

            void UpdateObject(string propPath, object inputValue,
                Dictionary<string, string[]> mappingsArr = null)
            {
                var propHead = PathHead(propPath);
                var propTail = PathTail(propPath);
                var unitType = unit.GetType();
                var propInfo = unitType.GetProperty(propHead);
                if (propInfo == null)
                {
                    throw new Exception(
                        $"Property `{propHead}` not found in type `{unitType}`,"
                        + $" property path: `{propPath}`, value: `{inputValue}`");
                }
                object propValue;
                string value = "";
                List<KeyValuePair<string, Dictionary<string, string>>> valueArr = null;
                if (inputValue is string s)
                {
                    value = s;
                }
                else
                {
                    valueArr = inputValue as List<KeyValuePair<string, Dictionary<string, string>>>;
                }
                switch (propHead)
                {
                    case nameof(StatisticalUnit.Activities):
                        propInfo = unit.GetType().GetProperty(nameof(StatisticalUnit.ActivitiesUnits));
                        var unitActivities = unit.ActivitiesUnits ?? new List<ActivityStatisticalUnit>();
                        if (valueArr != null)
                            UpdateActivities(unitActivities, valueArr, mappingsArr);
                        propValue = unitActivities;
                        break;
                    case nameof(StatisticalUnit.Persons):
                        propInfo = unit.GetType().GetProperty(nameof(StatisticalUnit.PersonsUnits));
                        var persons = unit.PersonsUnits ?? new List<PersonStatisticalUnit>();
                        unit.PersonsUnits?.ForEach(x => x.Person.Role = x.PersonTypeId);
                        if (valueArr != null)
                            UpdatePersons(persons, valueArr, mappingsArr, context);
                        propValue = persons;
                        break;
                    case nameof(StatisticalUnit.ForeignParticipationCountriesUnits):
                        var fpcPropValue = new List<CountryStatisticalUnit>();
                        var foreignParticipationCountries = unit.ForeignParticipationCountriesUnits ?? new List<CountryStatisticalUnit>();
                        if (valueArr != null)
                            foreach (var countryFromArray in valueArr)
                            {
                                Country prev = new Country();
                                foreach (var countryValue in countryFromArray.Value)
                                {
                                    if (!mappingsArr.TryGetValue(countryValue.Key, out string[] targetKeys)) continue;
                                    foreach (var targetKey in targetKeys)
                                    {
                                        PropertyParser.ParseCountry(targetKey, countryValue.Value, prev);
                                    }
                                }
                                foreignParticipationCountries.Add(new CountryStatisticalUnit()
                                {
                                    CountryId = prev.Id,
                                    Country = prev
                                });
                                fpcPropValue.AddRange(foreignParticipationCountries);
                            }
                        propValue = fpcPropValue;
                        break;
                    case nameof(StatisticalUnit.Address):
                        propValue = PropertyParser.ParseAddress(propTail, value, unit.Address);
                        break;
                    case nameof(StatisticalUnit.ActualAddress):
                        propValue = PropertyParser.ParseAddress(propTail, value, unit.ActualAddress);
                        break;
                    case nameof(StatisticalUnit.PostalAddress):
                        propValue = PropertyParser.ParseAddress(propTail, value, unit.PostalAddress);
                        break;
                    case nameof(StatisticalUnit.LegalForm):
                        propValue = PropertyParser.ParseLegalForm(propTail, value, unit.LegalForm);
                        break;
                    case nameof(StatisticalUnit.InstSectorCode):
                        propValue = PropertyParser.ParseSectorCode(propTail, value, unit.InstSectorCode);
                        break;
                    case nameof(StatisticalUnit.DataSourceClassification):
                        propValue = PropertyParser.ParseDataSourceClassification(propTail, value, unit.DataSourceClassification);
                        break;
                    case nameof(StatisticalUnit.Size):
                        propValue = PropertyParser.ParseSize(propTail, value, unit.Size);
                        break;
                    case nameof(StatisticalUnit.UnitStatus):
                        propValue = PropertyParser.ParseUnitStatus(propTail, value, unit.UnitStatus);
                        break;
                    case nameof(StatisticalUnit.ReorgType):
                        propValue = PropertyParser.ParseReorgType(propTail, value, unit.ReorgType);
                        break;
                    case nameof(StatisticalUnit.RegistrationReason):
                        propValue = PropertyParser.ParseRegistrationReason(propTail, value, unit.RegistrationReason);
                        break;
                    case nameof(StatisticalUnit.ForeignParticipation):
                        propValue = PropertyParser.ParseForeignParticipation(propTail, value, unit.ForeignParticipation);
                        break;
                    default:
                        var type = propInfo.PropertyType;
                        var underlyingType = Nullable.GetUnderlyingType(type);
                        propValue = value.HasValue() || underlyingType == null
                            ? Type.GetTypeCode(type) == TypeCode.String
                                ? value
                                : PropertyParser.ConvertOrDefault(underlyingType ?? type, value)
                            : null;
                        break;
                }

                propInfo?.SetValue(unit, propValue);
            }
        }
        private static void UpdateActivities(ICollection<ActivityStatisticalUnit> dbActivities, List<KeyValuePair<string,Dictionary<string, string>>> importActivities, Dictionary<string, string[]> mappingsArr)
        {
            var defaultYear = DateTime.Now.Year - 1;
            var propPathActivityCategoryCode = string.Join(".", nameof(ActivityCategory), nameof(ActivityCategory.Code));
            var dbActivitiesGroups = dbActivities.GroupBy(x => x.Activity.ActivityYear).ToList();
            var importActivitiesGroups =
                importActivities.GroupBy(x => int.TryParse(x.Value.GetValueOrDefault(nameof(Activity.ActivityYear)), out int val) ? (int?)val : defaultYear).ToList();

            foreach (var importActivitiesGroup in importActivitiesGroups)
            {
                var dbGroup = dbActivitiesGroups.FirstOrDefault(x => x.Key == importActivitiesGroup.Key);
                if (dbGroup == null)
                {
                   var parsedActivities =  importActivitiesGroup.Select((x,i) => new ActivityStatisticalUnit
                   {
                       Activity = ParseActivity(null, x.Value, mappingsArr, i == 0 ? ActivityTypes.Primary : ActivityTypes.Secondary)
                   });
                   dbActivities.AddRange(parsedActivities);
                   continue;
                }

                importActivitiesGroup.GroupJoin(dbGroup,
                    import => import.Value.GetValueOrDefault(propPathActivityCategoryCode),
                    db => db.Activity.ActivityCategory.Code, (importRow, dbRows) => (importRow: importRow, dbRows: dbRows))
                    .ForEach(x =>
                    {
                        var dbRow = x.dbRows.FirstOrDefault();
                        if (dbRow != null)
                        {
                            dbRow.Activity = ParseActivity(dbRow.Activity, x.importRow.Value, mappingsArr, ActivityTypes.Secondary);
                        }
                        else
                        {
                            dbRow = new ActivityStatisticalUnit
                            {
                                Activity = ParseActivity(null, x.importRow.Value, mappingsArr, ActivityTypes.Secondary)
                            };
                            dbActivities.Add(dbRow);
                        }
                    });
            }
        }

        private static Activity ParseActivity(Activity activity, Dictionary<string, string> targetKeys,
            Dictionary<string, string[]> mappingsArr, ActivityTypes defaultType)
        {
            var activityTypeWasSet = activity != null;
            activity = activity ?? new Activity();
            foreach (var (key, val) in targetKeys)
            {
                if (!mappingsArr.TryGetValue(key, out var targetValues)) continue;
                foreach (var targetKey in targetValues)
                {
                    if (targetKey == nameof(Activity.ActivityType))
                    {
                        activityTypeWasSet = true;
                    }
                    activity = PropertyParser.ParseActivity(targetKey, val, activity);
                }
            }
            if (!activityTypeWasSet)
            {
                activity.ActivityType = defaultType;
            }
            return activity;
        }

        private static Person ParsePerson(Dictionary<string, string> targetKeys,
            Dictionary<string, string[]> mappingsArr, NSCRegDbContext context)
        {
            Person person = new Person();
            foreach (var (key,value) in targetKeys)
            {
                if(!mappingsArr.TryGetValue(key, out var targetValues)) continue;
                foreach (var targetKey in targetValues)
                {
                    if (targetKey == nameof(Person.Role))
                    {
                        var roleValue = value.ToLower();

                        var roleType = context.PersonTypes.Local.FirstOrDefault(x =>
                            x.Name.ToLower() == roleValue || x.NameLanguage1.HasValue() && x.NameLanguage1.ToLower() == roleValue ||
                            x.NameLanguage2.HasValue() && x.NameLanguage2.ToLower() == roleValue);

                        person = PropertyParser.ParsePerson(targetKey, roleType?.Id.ToString(), person);
                        continue;
                    }
                    person = PropertyParser.ParsePerson(targetKey, value, person);

                }
            }
            return person;
        }

        private static void UpdatePersons(ICollection<PersonStatisticalUnit> persons,
            List<KeyValuePair<string, Dictionary<string, string>>> importPersons,
            Dictionary<string, string[]> mappingsArr, NSCRegDbContext context)
        {
            var newPersonStatUnits = new List<PersonStatisticalUnit>();
            foreach (var person in importPersons)
            {
                var newPerson = ParsePerson(person.Value, mappingsArr, context);
                if (persons.Any())
                {
                    foreach (var existPerson in persons)
                    {
                        if (existPerson.PersonTypeId == newPerson.Role)
                        {
                            existPerson.Person = newPerson;
                            continue;
                        }
                        newPersonStatUnits.Add(new PersonStatisticalUnit() { Person = newPerson, PersonTypeId = newPerson.Role });
                    }
                }
                else
                {
                    newPersonStatUnits.Add(new PersonStatisticalUnit() { Person = newPerson, PersonTypeId = newPerson.Role });
                }
                
            }
            persons.AddRange(newPersonStatUnits);
        }

    }
}
