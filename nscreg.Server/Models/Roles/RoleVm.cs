﻿using System;
using nscreg.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using nscreg.Data.Constants;
using nscreg.Server.Models.DataAccess;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace nscreg.Server.Models.Roles
{
    public class RoleVm
    {
        public static RoleVm Create(Role role) => new RoleVm
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            AccessToSystemFunctions = role.AccessToSystemFunctionsArray,
            StandardDataAccess = DataAccessModel.FromString(role.StandardDataAccess),
        };

        public string Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public IEnumerable<int> AccessToSystemFunctions { get; private set; }
        public DataAccessModel StandardDataAccess { get; private set; }
    }
}
