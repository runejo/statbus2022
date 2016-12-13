﻿using Newtonsoft.Json;
using nscreg.Data;
using nscreg.ReadStack;
using nscreg.Server.Models.StatUnits;
using System;
using System.Collections.Generic;
using System.Linq;

namespace nscreg.Server.Services
{
    public class StatUnitSearchService
    {
        private readonly ReadContext _readCtx;

        public StatUnitSearchService(NSCRegDbContext dbContext)
        {
            _readCtx = new ReadContext(dbContext);
        }

        public SearchVm Search(SearchQueryM query, IEnumerable<string> propNames)
        {
            var filtered = _readCtx.StatUnits
                .Where(x =>
                    (query.IncludeLiquidated || x.LiqDate == null)
                        && x.Name.Contains(query.Wildcard)
                        || x.Address.AddressPart1.Contains(query.Wildcard)
                        || x.Address.AddressPart2.Contains(query.Wildcard)
                        || x.Address.AddressPart3.Contains(query.Wildcard)
                        || x.Address.AddressPart4.Contains(query.Wildcard)
                        || x.Address.AddressPart5.Contains(query.Wildcard)
                        || x.Address.GeographicalCodes.Contains(query.Wildcard));
            var resultGroup = filtered
                .Skip(query.PageSize * query.Page)
                .Take(query.PageSize)
                .GroupBy(p => new { Total = filtered.Count() })
                .FirstOrDefault();
            return SearchVm.Create(
                resultGroup?.Select(x => JsonConvert.SerializeObject(x)) ?? Array.Empty<string>(),
                resultGroup?.Key.Total ?? 0,
                (int)Math.Ceiling((double)(resultGroup?.Key.Total ?? 0) / query.PageSize));
        }
    }
}
