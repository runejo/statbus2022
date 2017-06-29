﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using nscreg.Data.Entities;

namespace nscreg.Business.DataSources
{
    public static class StatUnitKeyValueParser
    {
        public static void ParseAndMutateStatUnit(
            IReadOnlyDictionary<string, string> mappings,
            IReadOnlyDictionary<string, string> nextProps,
            IStatisticalUnit unit)
        {
            foreach (var kv in nextProps)
                if (mappings.TryGetValue(kv.Key, out string tmpKey))
                    UpdateObject(tmpKey, kv.Value, unit);
        }

        private static void UpdateObject(string key, string value, IStatisticalUnit unit)
        {
            var propInfo = unit.GetType().GetProperty(key);
            var type = propInfo.PropertyType;
            var res = !string.IsNullOrEmpty(value) || Nullable.GetUnderlyingType(type) == null
                ? Type.GetTypeCode(type) == TypeCode.String
                    ? value
                    : Convert.ChangeType(value, type, CultureInfo.InvariantCulture)
                : null;
            propInfo.SetValue(unit, res);
        }
    }
}
