﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using nscreg.Data;
using nscreg.Data.Constants;
using nscreg.Resources.Languages;
using nscreg.Server.Common.Models.DataSources;
using nscreg.Server.Common.Services;
using nscreg.Server.Core;
using nscreg.Server.Core.Authorize;
using SearchQueryM = nscreg.Server.Common.Models.DataSourceQueues.SearchQueryM;

namespace nscreg.Server.Controllers
{
    [Route("api/[controller]")]
    public class DataSourceQueuesController: Controller
    {
        private readonly DataSourceQueuesService _svc;

        public DataSourceQueuesController(NSCRegDbContext ctx)
        {
            _svc = new DataSourceQueuesService(ctx);
        }

        [HttpGet]
        [SystemFunction(SystemFunctions.DataSourceQueuesView)]
        public async Task<IActionResult> GetAllDataSourceQueues([FromQuery] SearchQueryM query)
            => Ok(await _svc.GetAllDataSourceQueues(query).ConfigureAwait(false));

        [HttpPost]
        [SystemFunction(SystemFunctions.DataSourcesUpload)]
        public async Task<IActionResult> Create([FromForm] UploadDataSourceVm data)
        {
            var files = Request.Form.Files;
            if (files.Count < 1) return BadRequest(new {message = nameof(Resource.NoFilesAttached)});
            await _svc.CreateAsync(files, data, User.GetUserId());
            return Ok();
        }
    }
}