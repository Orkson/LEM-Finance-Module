using Application.Models;
using MediatR;

namespace Application.ExpensesPlanner.Queries
{
    public class GetExpensePlannerByYearQuery : IRequest<List<ExpensePlannerDto>>
    {
        public int Year { get; set; }
    }
}
