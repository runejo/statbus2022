using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using nscreg.Data.Entities;
using nscreg.Utilities.Configuration;

namespace nscreg.Data.DbDataProviders
{
    public class PostgreSqlDbDataProvider : IDbDataProvider
    {
        public async Task<List<ReportTree>> GetReportsTree(NSCRegDbContext context, string sqlWalletUser, IConfiguration config)
        {
            var sqlWalletProvider = new SqlWalletDataProvider();
            return await sqlWalletProvider.GetReportsTree(context, sqlWalletUser, config);
        }

        public int[] GetActivityChildren(NSCRegDbContext context, object fieldValue)
        {
            return context.ActivityCategories.FromSql(@"SELECT * FROM ""GetActivityChildren""({0})", Convert.ToInt32(fieldValue)).Select(x => x.Id)
                .ToArray();
        }

        public int[] GetRegionChildren(NSCRegDbContext context, object fieldValue)
        {
            return context.Regions.FromSql(@"SELECT * FROM ""GetRegionChildren""({0})", Convert.ToInt32(fieldValue)).Select(x => x.Id)
                .ToArray();
        }
    }
}
