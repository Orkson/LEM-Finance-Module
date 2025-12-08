using Application.Models;
using MediatR;

namespace Application.ExpensesPlanner.Queries
{
    public class GetExpensePlannerByIdQuery : IRequest<ExpensePlannerDto>
    {
        public int Id { get; set; }

        public GetExpensePlannerByIdQuery(int id)
        {
            Id = id;
        }
    }
}
