using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using nscreg.Data.Constants;
using nscreg.Data.Core;
using nscreg.Data.Entities;
using nscreg.Data.Entities.ComplexTypes;
using nscreg.ModelGeneration;
using nscreg.Utilities;
using nscreg.Utilities.Attributes;

namespace nscreg.Server.Common.Models.StatUnits
{
    public static class StatUnitViewModelCreator
    {
        public static StatUnitViewModel Create(
            IStatisticalUnit domainEntity,
            DataAccessPermissions dataAccess,
            StatUnitTypes unitType,
            IReadOnlyDictionary<string, bool> mandatoryFields)
        {
            return new StatUnitViewModel
            {
                StatUnitType = StatisticalUnitsTypeHelper.GetStatUnitMappingType(domainEntity.GetType()),
                Properties = GetFilteredProperties(domainEntity.GetType())
                    .Select(x => PropertyMetadataFactory.Create(
                        x.PropInfo, domainEntity, x.Writable,
                        mandatoryFields.TryGetValue(x.PropInfo.Name, out var mandatory) ? mandatory : (bool?) null))
                    .ToArray(),
                Permissions = dataAccess.Permissions //TODO: Filter By Type (Optimization)
            };

            IEnumerable<(PropertyInfo PropInfo, bool Writable)> GetFilteredProperties(Type type)
                => type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(x =>
                        dataAccess.HasWriteOrReadPermission(DataAccessAttributesHelper.GetName(type, x.Name))
                        && x.CanRead
                        && x.CanWrite
                        && x.GetCustomAttribute<NotMappedForAttribute>(true) == null)
                    .OrderBy(x =>
                        ((DisplayAttribute) x.GetCustomAttribute(typeof(DisplayAttribute)))?.GetOrder() ?? int.MaxValue)
                    .Select(x => (x, dataAccess.HasWritePermission(DataAccessAttributesHelper.GetName(type, x.Name))));
        }
    }
}
