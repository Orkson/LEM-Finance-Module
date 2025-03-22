using Application.ExpensesPlanner.Commands;
using Application.ExpensesPlanner.Queries;
using Application.Models;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        //[HttpGet("{year}")]
        //public async Task<ActionResult<ExpensePlanner>> GetExpensePlanner(int year)
        //{
        //    var result = await _mediator
        //        .Send(new GetExpensePlannerByYearQuery { Year = year });
        //    return Ok(result);
        //}

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



        //[HttpPost]
        //public async Task<IActionResult> CreateExpensePlanner([FromBody] CreateExpensePlannerCommand command)
        //{
        //    var result = await _mediator.Send(command);
        //
        //    return Ok(result);
        //}

        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateExpensePlanner(int id, ExpensePlanner expensePlanner)
        //{
        //    if (id != expensePlanner.Id)
        //    {
        //        return BadRequest();
        //    }
        //
        //    _context.Entry(expensePlanner).State = EntityState.Modified;
        //
        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!_context.ExpensePlanner.Any(e => e.Id == id))
        //        {
        //            return NotFound();
        //        }
        //        throw;
        //    }
        //
        //    return NoContent();
        //}

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteExpensePlanner(int id)
        //{
        //    var expensePlanner = await _context.ExpensePlanner.FindAsync(id);
        //    if (expensePlanner == null)
        //    {
        //        return NotFound();
        //    }
        //
        //    _context.ExpensePlanner.Remove(expensePlanner);
        //    await _context.SaveChangesAsync();
        //
        //    return NoContent();
        //}
    }
}
