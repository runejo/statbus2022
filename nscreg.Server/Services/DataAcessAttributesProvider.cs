﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using nscreg.Data.Entities;
using nscreg.Server.Models.DataAccess;
using nscreg.Utilities;
using nscreg.Utilities.Attributes;

namespace nscreg.Server.Services
{
    public class DataAcessAttributesProvider<T> where T : IStatisticalUnit
    {
        private static readonly List<PropertyInfo> Properties = typeof(T).GetProperties()
            .Where(v => v.GetCustomAttribute<NotMappedForAttribute>() == null)
            .ToList();

        private static readonly List<DataAccessAttributeM> Attributes = Properties.Select(v =>
        {
            var displayAttribute = v.GetCustomAttribute<DisplayAttribute>();
            return Tuple.Create(
                displayAttribute?.GetOrder() ?? int.MaxValue,
                new DataAccessAttributeM()
                {
                    Name = DataAccessAttributesHelper.GetName(typeof(T), v.Name),
                    GroupName = displayAttribute?.GroupName,
                    LocalizeKey = v.Name,
                });
        }).OrderBy(v => v.Item1).Select(v => v.Item2).ToList();

        //TODO: COMMON ATTRIBUTES

        public static IReadOnlyCollection<DataAccessAttributeM> List()
        {
            return Attributes;
        }
    }

}
