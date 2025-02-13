using Application.Documents;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Route("api/files")]
    public class FilesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FilesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddFiles([FromForm]List<IFormFile>files, int? modelId, int? deviceId)
        {
            var addDocumentsCommand = new AddDocumentsCommand(files, modelId, deviceId);

            var documentNames = await _mediator.Send(addDocumentsCommand);
            return Ok(documentNames);
        }

        [HttpGet]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DownloadFile(string documentName, int? modelId, int? deviceId)
        {
            var downloadDocumentsQuery = new DownloadDocumentQuery(documentName, deviceId, modelId);

            var result = await _mediator.Send(downloadDocumentsQuery);

            return File(result.Data, result.ContentType, result.FileName);
        }

        [HttpDelete]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteFiles([FromBody]ICollection<int> documentsId)
        {
            var deleteDocumentsCommand = new DeleteDocumentsCommand(documentsId);

            var result = await _mediator.Send(deleteDocumentsCommand);
            return Ok(result);
        }
    }
}
