using Application.Devices.Commands;
using Application.Devices.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Route("api/calibration-cost")]
    [ApiController]
    public class CalibrationCostController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CalibrationCostController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> AddCost([FromBody] AddCalibrationCostCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("{year}")]
        public async Task<IActionResult> GetCostsByYear(int year)
        {
            var result = await _mediator.Send(new GetCalibrationCostsByYearQuery { Year = year });
            return Ok(result);
        }
    }
}
