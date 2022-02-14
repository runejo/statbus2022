using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using nscreg.Data;
using nscreg.Data.DbDataProviders;
using nscreg.Data.Entities;
using nscreg.Resources.Languages;
using nscreg.Utilities.Configuration;
using nscreg.Utilities.Enums;
using Newtonsoft.Json;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace nscreg.Server.Common.Services
{
    public class ReportService
    {
        private readonly NSCRegDbContext _ctx;
        private readonly ReportingSettings _settings;
        private readonly IConfiguration _configuration;

        public ReportService(NSCRegDbContext context, ReportingSettings settings, IConfiguration configuration)
        {
            _ctx = context;
            _settings = settings;
            _configuration = configuration;
        }

        public async Task<List<ReportTree>> GetReportsTree(string userName)
        {
            var role =
                from u in _ctx.Users
                join ur in _ctx.UserRoles on u.Id equals ur.UserId
                join r in _ctx.Roles on ur.RoleId equals r.Id
                where u.UserName == userName
                select r;

            var sqlWalletUser = role.FirstOrDefault()?.SqlWalletUser;

            if (string.IsNullOrEmpty(sqlWalletUser))
                throw new Exception("Please specify sqlWalletUser in Administrator or Employee roles");

            List<ReportTree> queryResult = await GetReportsTreeByProvider(_ctx, sqlWalletUser);

            var resultNodes = new List<ReportTree>(queryResult);
            RemoveEmptyFolders(queryResult, resultNodes);

            var result = GetAccessToken(_settings, sqlWalletUser);

            if (string.IsNullOrEmpty(result))
                throw new Exception("Can not get access token from SqlWallet.");

            var hostName = !string.IsNullOrEmpty(_settings.ExternalHostName)
                ? _settings.ExternalHostName
                : _settings.HostName;

            foreach (var node in resultNodes)
            {
                if (node.Type == "Report")
                    node.ReportUrl = $"http://{hostName}/embed?access_token={result}#{node.ReportId}";
            }

            return resultNodes;
        }

        private static void RemoveEmptyFolders(ICollection<ReportTree> nodes, ICollection<ReportTree> resultNodes)
        {
            if (nodes == null || nodes.Count == 0)
                return;
            foreach (var reportTreeNode in nodes)
            {
                var childNodes = resultNodes.Where(x => x.ParentNodeId == reportTreeNode.Id).Select(x => x).ToList();
                RemoveEmptyFolders(childNodes, resultNodes);
                if (resultNodes.All(x => x.ParentNodeId != reportTreeNode.Id) && (reportTreeNode.ReportId == null && reportTreeNode.ParentNodeId != null))
                    resultNodes.Remove(reportTreeNode);
            }
        }

        private string GetAccessToken(ReportingSettings settings, string sqlWalletUserName)
        {
            var authResponse = new SqlWalletResponse();

            var client = new HttpClient();
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, $"http://{settings.HostName}/connect/token")
                {
                    Content = new StringContent(
                        $"client_secret={settings.SecretKey}&grant_type=client_credentials&client_id=sqlwallet&scope=sqlwallet",
                        Encoding.UTF8,
                        "application/x-www-form-urlencoded")
                };

                request.Headers.ExpectContinue = true;

                client.SendAsync(request).ContinueWith(responseTask =>
                {
                    var content = responseTask.Result.Content.ReadAsStringAsync().Result;

                    authResponse = JsonConvert.DeserializeObject<SqlWalletResponse>(content);
                }).Wait();


                var userRequest =
                    new HttpRequestMessage(HttpMethod.Post,
                        $"http://{settings.HostName}/auth/accesstoken/{sqlWalletUserName}")
                    {
                        Content = new StringContent("", Encoding.UTF8, "application/json")
                    };

                userRequest.Headers.Authorization =
                    new AuthenticationHeaderValue(authResponse.Token_Type, authResponse.Access_Token);
                userRequest.Headers.CacheControl = new CacheControlHeaderValue { NoCache = true };
                userRequest.Headers.Host = settings.HostName;

                var accessToken = "";
                client.SendAsync(userRequest).ContinueWith(respTask =>
                {
                    accessToken = respTask.Result.Content.ReadAsStringAsync().Result;
                }).Wait();

                return accessToken;
            }
            catch (Exception e)
            {
                throw new Exception($"An error occured while trying getting access token. Message: {e.Message}");
            }
            finally
            {
                client.CancelPendingRequests();
            }


        }

        private async Task<List<ReportTree>> GetReportsTreeByProvider(NSCRegDbContext context, string sqlWalletUser)
        {
            List<ReportTree> queryResult;
            var provider = _configuration
                .GetSection(nameof(ConnectionSettings))
                .Get<ConnectionSettings>()
                .ParseProvider();
            try
            {
                IDbDataProvider dbDataProvider;
                switch (provider)
                {
                    case ConnectionProvider.SqlServer:
                        dbDataProvider = new MsSqlDbDataProvider();
                        break;
                    case ConnectionProvider.PostgreSql:
                        dbDataProvider = new PostgreSqlDbDataProvider();
                        break;
                    case ConnectionProvider.MySql:
                        dbDataProvider = new MySqlDataProvider();
                        break;
                    default: throw new Exception(Resource.ProviderIsNotSet);
                }

                queryResult = await dbDataProvider.GetReportsTree(_ctx, sqlWalletUser, _configuration);
                return queryResult;
            }
            catch (Exception e)
            {
                throw new Exception($"An error occured while trying get data of reports from database. Message: {e.Message}");
            }
        }

        internal class SqlWalletResponse
        {
            public string Access_Token { get; set; }
            public string Expires_In { get; set; }
            public string Token_Type { get; set; }
        }
            
        public async Task<byte[]> DownloadStatUnitEnterprise()
        {
            var records = await _ctx.StatUnitEnterprise_2021.ToListAsync();
            using var mem = new MemoryStream();
            using var writer = new StreamWriter(mem);

            writer.Write("StatUnitEnterprise_2021");
            writer.Write("StatId");
            writer.Write("Oblast");
            writer.Write("Rayon");
            writer.Write("ActCat_section_code");
            writer.Write("ActCat_section_desc");
            writer.Write("ActCat_2dig_code");
            writer.Write("ActCat_2dig_desc");
            writer.Write("ActCat_3dig_code");
            writer.Write("ActCat_3dig_desc");
            writer.Write("LegalForm_code");
            writer.Write("LegalForm_desc");
            writer.Write("InstSectorCode_level1");
            writer.Write("InstSectorCode_level1_desc");
            writer.Write("InstSectorCode_level2");
            writer.Write("InstSectorCode_level2_desc");
            writer.Write("SizeCode");
            writer.Write("SizeDesc");
            writer.Write("Turnover");
            writer.Write("Employees");
            writer.Write("NumOfPeopleEmp");
            writer.Write("RegistrationDate");
            writer.Write("LiqDate");
            writer.Write("StatusCode");
            writer.Write("StatusDesc");
            writer.Write("Sex");

            foreach (var record in records)
            {
                writer.Write(record.StatId);
                writer.Write(record.Oblast);
                writer.Write(record.Rayon);
                writer.Write(record.ActCat_section_code);
                writer.Write(record.ActCat_section_desc);
                writer.Write(record.ActCat_2dig_code);
                writer.Write(record.ActCat_2dig_desc);
                writer.Write(record.ActCat_3dig_code);
                writer.Write(record.ActCat_3dig_desc);
                writer.Write(record.LegalForm_code);
                writer.Write(record.LegalForm_desc);
                writer.Write(record.InstSectorCode_level1);
                writer.Write(record.InstSectorCode_level1_desc);
                writer.Write(record.InstSectorCode_level2);
                writer.Write(record.InstSectorCode_level2_desc);
                writer.Write(record.SizeCode);
                writer.Write(record.SizeDesc);
                writer.Write(record.Turnover);
                writer.Write(record.Employees);
                writer.Write(record.NumOfPeopleEmp);
                writer.Write(record.RegistrationDate);
                writer.Write(record.LiqDate);
                writer.Write(record.StatusCode);
                writer.Write(record.StatusDesc);
                writer.Write(record.Sex);
            }

            writer.Flush();
            return mem.ToArray();
        }

        public async Task<byte[]> DownloadStatUnitLocal()
        {
            var records = await _ctx.StatUnitEnterprise_2021.ToListAsync();
            using var mem = new MemoryStream();
            using var writer = new StreamWriter(mem);

            writer.Write("StatUnitLocal_2021");
            writer.Write("StatId");
            writer.Write("Oblast");
            writer.Write("Rayon");
            writer.Write("ActCat_section_code");
            writer.Write("ActCat_section_desc");
            writer.Write("ActCat_2dig_code");
            writer.Write("ActCat_2dig_desc");
            writer.Write("ActCat_3dig_code");
            writer.Write("ActCat_3dig_desc");
            writer.Write("LegalForm_code");
            writer.Write("LegalForm_desc");
            writer.Write("InstSectorCode_level1");
            writer.Write("InstSectorCode_level1_desc");
            writer.Write("InstSectorCode_level2");
            writer.Write("InstSectorCode_level2_desc");
            writer.Write("SizeCode");
            writer.Write("SizeDesc");
            writer.Write("Turnover");
            writer.Write("Employees");
            writer.Write("NumOfPeopleEmp");
            writer.Write("RegistrationDate");
            writer.Write("LiqDate");
            writer.Write("StatusCode");
            writer.Write("StatusDesc");
            writer.Write("Sex");

            foreach (var record in records)
            {
                writer.Write(record.StatId);
                writer.Write(record.Oblast);
                writer.Write(record.Rayon);
                writer.Write(record.ActCat_section_code);
                writer.Write(record.ActCat_section_desc);
                writer.Write(record.ActCat_2dig_code);
                writer.Write(record.ActCat_2dig_desc);
                writer.Write(record.ActCat_3dig_code);
                writer.Write(record.ActCat_3dig_desc);
                writer.Write(record.LegalForm_code);
                writer.Write(record.LegalForm_desc);
                writer.Write(record.InstSectorCode_level1);
                writer.Write(record.InstSectorCode_level1_desc);
                writer.Write(record.InstSectorCode_level2);
                writer.Write(record.InstSectorCode_level2_desc);
                writer.Write(record.SizeCode);
                writer.Write(record.SizeDesc);
                writer.Write(record.Turnover);
                writer.Write(record.Employees);
                writer.Write(record.NumOfPeopleEmp);
                writer.Write(record.RegistrationDate);
                writer.Write(record.LiqDate);
                writer.Write(record.StatusCode);
                writer.Write(record.StatusDesc);
                writer.Write(record.Sex);
            }

            writer.Flush();
            return mem.ToArray();
        }
    }
}
