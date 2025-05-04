using MediatR;

namespace Application.ExpensesPlanner.Commands
{
    public class RemoveExpensePlannerCommand: IRequest
    {
        public int Id { get; set; }
    }
}
