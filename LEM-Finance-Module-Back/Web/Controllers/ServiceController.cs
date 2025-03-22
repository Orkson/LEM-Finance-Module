using Application.ExpensesPlanner.Queries;
using Application.Services.Queries;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Route("api/services")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ServiceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<Service>>> GetServices()
        {
            var result = await _mediator.Send(new GetServiceQuery { });
            if (result is null || !result.Any())
                return NoContent();

            return Ok(result);
        }
    }
}
