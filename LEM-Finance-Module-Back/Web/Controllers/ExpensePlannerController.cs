using Application.ExpensesPlanner.Commands;
using Application.ExpensesPlanner.Queries;
using Application.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Route("api/expenses-planner")]
    [ApiController]
    public class ExpensePlannerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ExpensePlannerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{year:int}")]
        public async Task<ActionResult<List<ExpensePlannerDto>>> GetExpensePlannerByYear(int year)
        {
            var result = await _mediator.Send(new GetExpensePlannerByYearQuery { Year = year });
            if (result is null || !result.Any())
                return NotFound($"Nie znaleziono wydatków na rok {year}.");

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ExpensePlannerDto>> CreateExpensePlanner([FromBody] CreateServiceCommand command)
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetExpensePlannerByYear), new { year = result.PlannedDate.Year }, result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateExpensePlanner(int id, [FromBody] EditExpensePlannerCommand command)
        {
            command.Id = id;

            await _mediator.Send(command);
            
            return NoContent();
        }
        
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteExpensePlanner(int id)
        {
            await _mediator.Send(new RemoveExpensePlannerCommand { Id = id });
            
            return NoContent();
        }
    }
}
