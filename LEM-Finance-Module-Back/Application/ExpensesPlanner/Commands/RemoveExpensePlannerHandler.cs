using Domain.Abstraction;
using MediatR;

namespace Application.ExpensesPlanner.Commands
{
    public class RemoveExpensePlannerHandler : IRequestHandler<RemoveExpensePlannerCommand>
    {
        private readonly IExpensePlannerRepository _repository;

        public RemoveExpensePlannerHandler(IExpensePlannerRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(RemoveExpensePlannerCommand request, CancellationToken cancellationToken)
        {
            if (!await _repository.DeletePlannerAsync(request.Id))
            {
                throw new Exception($"Expense Planner with ID {request.Id} not found.");
            }
        }
    }
}
