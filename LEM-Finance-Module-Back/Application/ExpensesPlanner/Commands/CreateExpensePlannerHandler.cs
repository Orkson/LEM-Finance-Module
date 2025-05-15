using Domain.Abstraction;
using Domain.Entities;
using MediatR;

namespace Application.ExpensesPlanner.Commands
{
    public class CreateServiceHandler : IRequestHandler<CreateServiceCommand, ExpensePlanner>
    {
        private readonly IExpensePlannerRepository _repository;

        public CreateServiceHandler(IExpensePlannerRepository repository)
        {
            _repository = repository;
        }
        public async Task<ExpensePlanner> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
        {
            var planner = new ExpensePlanner
            {
                Currency = request.Currency,
                Description = request.Description,
                GrossPrice = request.GrossPrice,
                NetPrice = request.NetPrice,
                PlannedDate = request.PlannedDate,
                Status = request.Status,
                Service = await _repository.GetServiceByIdAsync(request.ServiceId),
                Device = await _repository.GetDeviceByIdAsync(request.DeviceId),
                StorageLocationName = request.StorageLocationName,
                Tax = request.Tax,
                NetPricePLN = request.NetPricePLN,
                GrossPricePLN = request.GrossPricePLN,
                TaxPLN = request.TaxPLN,
            };

            return await _repository.AddPlannerAsync(planner);
        }
    }
}
