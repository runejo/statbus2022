using System.Text;
using Microsoft.AspNetCore.Mvc;
using nscreg.Data;
using nscreg.Data.Constants;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using nscreg.Server.Common.Models.SampleFrames;
using nscreg.Server.Common.Services.SampleFrames;
using nscreg.Server.Core;
using nscreg.Server.Core.Authorize;
using nscreg.Utilities;

namespace nscreg.Server.Controllers
{
    [Route("api/[controller]")]
    public class SampleFramesController : Controller
    {
        private readonly SampleFramesService _sampleFramesService;
        private readonly CsvHelper _csvHelper;

        public SampleFramesController(NSCRegDbContext context, IConfiguration configuration)
        {
            _sampleFramesService = new SampleFramesService(context, configuration);
            _csvHelper = new CsvHelper();
        }

        [HttpGet]
        [SystemFunction(SystemFunctions.SampleFramesView)]
        public async Task<IActionResult> GetAll([FromQuery] SearchQueryM model) =>
            Ok(await _sampleFramesService.GetAll(model,User.GetUserId()));

        [HttpGet("{id:int}")]
        [SystemFunction(SystemFunctions.SampleFramesView)]
        public async Task<IActionResult> GetById(int id) =>
            Ok(await _sampleFramesService.GetById(id, User.GetUserId()));

        [HttpGet("{id:int}/preview")]
        [SystemFunction(SystemFunctions.SampleFramesPreview)]
        public async Task<IActionResult> Preview(int id) =>
            Ok(await _sampleFramesService.Preview(id, User.GetUserId(), 10));

        [HttpGet("{id:int}/preview/download")]
        [SystemFunction(SystemFunctions.SampleFramesPreview)]
        public async Task<IActionResult> DownloadPreview(int id)
        {
            var preview = await _sampleFramesService.Preview(id, User.GetUserId());
            var csvString = _csvHelper.ConvertToCsv(preview);
            var nameOfFile = _sampleFramesService.GetById(id, User.GetUserId()).Result.Name + ".csv";
            return File(Encoding.UTF8.GetBytes(csvString), "text/csv", nameOfFile);
        }

        [HttpPost]
        [SystemFunction(SystemFunctions.SampleFramesCreate)]
        public async Task<IActionResult> Create([FromBody] SampleFrameM data)
        {
            var model = await _sampleFramesService.Create(data, User.GetUserId());
            return Created($"api/sampleframes/{model.Id}", model);
        }

        [HttpPut("{id:int}")]
        [SystemFunction(SystemFunctions.SampleFramesEdit)]
        public async Task<IActionResult> Edit(int id, [FromBody] SampleFrameM data)
        {
            await _sampleFramesService.Edit(id, data, User.GetUserId());
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [SystemFunction(SystemFunctions.SampleFramesDelete)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await _sampleFramesService.Delete(id, User.GetUserId());
            return NoContent();
        }

    }
}
