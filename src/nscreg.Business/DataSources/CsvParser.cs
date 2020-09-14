using ServiceStack;
using System.Collections.Generic;
using System.Linq;
using nscreg.Utilities.Extensions;
using ServiceStack.Text;
using Unit = nscreg.Data.Entities.StatisticalUnit;
using System.Collections;

namespace nscreg.Business.DataSources
{
    public static class CsvParser
    {
        // Todo: to reflection implementation
        private readonly static string[] StatisticalUnitArrayPropertyNames = new[] { nameof(Unit.Activities), nameof(Unit.Persons), nameof(Unit.ForeignParticipationCountriesUnits) };
        private readonly static KeyValueTupleComparer CsvColumnValueComparer = new KeyValueTupleComparer();
        public static IEnumerable<IReadOnlyDictionary<string, object>> GetParsedEntities(string rawLines, string delimiter, (string source, string target)[] variableMappingsArray)
        {
            if (rawLines.Length == 0) return new List<Dictionary<string, object>>();

            CsvConfig.ItemSeperatorString = delimiter;
            var csvHeaders = rawLines.Split(new []{'\r', '\n'}, 2).First().Split(delimiter);
            var rowsFromCsv = rawLines.FromCsv<List<Dictionary<string, string>>>();
            var resultDictionary = new List<Dictionary<string, object>>();

            // Transform to array of (target, value) according to mapping for the first row
            var unitPartCsv = GetUnitPartCsvAfterMapping(variableMappingsArray, rowsFromCsv[0]);
            // fill units
            for (var j = 0; j < rowsFromCsv.Count;)
            {


                var unitResult = new Dictionary<string, object>();
                // fill primitive properties and initialize arrays
                foreach (var keyValue in unitPartCsv)
                {
                    // keySplitted[0] = one of the value arrayConst
                    // keySplitted[1] = Activity, Person or ForeignParticipationCountry
                    // keySplitted[2] = one of the value in keySplitted[1]

                    var keySplitted = keyValue.targetKeySplitted;
                    /// If <see cref="Unit"></see>'s property is not an array
                    if (!StatisticalUnitArrayPropertyNames.Contains(keySplitted[0]))
                    {
                        unitResult.Add(keyValue.targetKey, keyValue.value);
                        continue;
                    }


                    List<KeyValuePair<string, Dictionary<string, string>>> arrayProperty = null;
                    if (!unitResult.TryGetValue(keySplitted[0], out object obj))
                    {
                        obj = new List<KeyValuePair<string, Dictionary<string, string>>>()
                        {
                            new KeyValuePair<string, Dictionary<string, string>>(keySplitted[1], new Dictionary<string, string>())
                        };
                        unitResult.Add(keySplitted[0], obj);
                    }

                    arrayProperty = obj as List<KeyValuePair<string, Dictionary<string, string>>>;

                    if (arrayProperty != null)
                        arrayProperty[0].Value[keySplitted[2]] = keyValue.value.ToString();



                }

                // next rows can be a continuation of a unit. If so, they contain the same values for primitive fields and new values for arrays. So we have to collect new values to the arrays
                j++;

                var firstPartOfUnit = unitPartCsv;
                while (j < rowsFromCsv.Count)
                {
                    //// Transform to array of (target, value) according to mapping
                    unitPartCsv = GetUnitPartCsvAfterMapping(variableMappingsArray, rowsFromCsv[j]);
                    // If new unit started, then break the while cycle
                    if (!IsTheSameUnit(firstPartOfUnit, unitPartCsv)) break;

                    // Get columns which values are parts of array items
                    var columnValuesForArrays = unitPartCsv.Where(x => StatisticalUnitArrayPropertyNames.Contains(x.targetKeySplitted[0])).GroupBy(x=>x.targetKeySplitted[0]);
                    // If it is an array, we are sure that keySplitted.Length == 3 (Like Activities.Activity.SomeOtherPartWithPossibleDots)

                    foreach(var csvUnitArrayProperty in columnValuesForArrays)
                    {
                        //So, csvUnitArrayProperty.Key is a value from statisticalUnitArrayPropertyNames variable
                        var arrayProperty = unitResult[csvUnitArrayProperty.Key] as List<KeyValuePair<string, Dictionary<string, string>>>;
                        var arrayItem = new Dictionary<string, string>();
                        foreach (var csvArrayItemProperty in csvUnitArrayProperty)
                        {
                            arrayItem[csvArrayItemProperty.targetKeySplitted[2]] = csvArrayItemProperty.value;
                        }

                        arrayProperty.Add(new KeyValuePair<string, Dictionary<string, string>>(csvUnitArrayProperty.First().targetKeySplitted[1], arrayItem));
                    }

                    j++;
                }

                resultDictionary.Add(unitResult);
            }
            return resultDictionary;
        }

        private static bool IsTheSameUnit(IEnumerable<(string targetKey, string value, string[] targetKeySplitted)> originalCsvKeyValues, IEnumerable<(string targetKey, string value, string[] targetKeySplitted)> targetCsvKeyValues)
        {
            var columnValuesForPrimitivePropsOriginal = originalCsvKeyValues.Where(x => !StatisticalUnitArrayPropertyNames.Contains(x.targetKeySplitted[0])).ToList();
            var columnValuesForPrimitivePropsTarget = targetCsvKeyValues.Where(x => !StatisticalUnitArrayPropertyNames.Contains(x.targetKeySplitted[0])).ToList();
            var intersectedCount = columnValuesForPrimitivePropsOriginal.Intersect(columnValuesForPrimitivePropsTarget, CsvColumnValueComparer).Count();
            return intersectedCount == columnValuesForPrimitivePropsOriginal.Count() && intersectedCount == columnValuesForPrimitivePropsTarget.Count();
        }

        private static IEnumerable<(string targetKey, string value, string[] targetKeySplitted)> GetUnitPartCsvAfterMapping((string source, string target)[] variableMappingsArray, Dictionary<string, string> rowsFromCsv)
        {
            return rowsFromCsv.Where(z => z.Value.HasValue()).Join(variableMappingsArray, r => r.Key, m => m.source,
                                (r, m) => (targetKey: m.target, value: r.Value, targetKeySplitted: m.target.Split('.', 3)));
        }


        private class KeyValueTupleComparer : IEqualityComparer<(string targetKey, string value, string[] targetKeySplitted)>
        {
            public bool Equals((string targetKey, string value, string[] targetKeySplitted) x, (string targetKey, string value, string[] targetKeySplitted) y)
            {
                return x.targetKey == y.targetKey && x.value == y.value;
            }

            public int GetHashCode((string targetKey, string value, string[] targetKeySplitted) obj)
            {
                return obj.targetKey.GetHashCode() ^ obj.value.GetHashCode();
            }
        }
    }

    
}
