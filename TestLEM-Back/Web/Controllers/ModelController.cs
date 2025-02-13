using Application.Models.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Route("api/models")]
    public class ModelController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ModelController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetModelDetails(int modelId, string modelName)
        {
            var modelDetilsQuery = new GetModelDetailsQuery(modelId, modelName);

            var modelDetails = await _mediator.Send(modelDetilsQuery);
            return Ok(modelDetails);
        }
    }
}
