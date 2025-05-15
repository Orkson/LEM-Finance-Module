using AutoMapper;
using Domain.Abstraction;
using MediatR;

namespace Application.ExpensesPlanner.Commands
{
    public record EditExpensePlannerHandler : IRequestHandler<EditExpensePlannerCommand>
    {
        private readonly IExpensePlannerRepository _repository;
        private readonly IMapper _mapper;

        public EditExpensePlannerHandler(IExpensePlannerRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task Handle(EditExpensePlannerCommand request, CancellationToken cancellationToken)
        {
            var expensePlanner = await _repository.GetPlannerByIdAsync(request.Id);

            if (expensePlanner == null)
                throw new Exception($"Expense Planner with ID {request.Id} not found.");

            expensePlanner.PlannedDate = request.PlannedDate;
            expensePlanner.StorageLocationName = request.StorageLocationName;
            expensePlanner.NetPrice = request.NetPrice;
            expensePlanner.GrossPrice = request.GrossPrice;
            expensePlanner.Tax = request.Tax;
            expensePlanner.Currency = request.Currency;
            expensePlanner.Description = request.Description;
            expensePlanner.Status = request.Status;
            expensePlanner.Service = await _repository.GetServiceByIdAsync(request.ServiceId);
            expensePlanner.Device = await _repository.GetDeviceByIdAsync(request.DeviceId);
            expensePlanner.NetPricePLN = request.NetPricePLN;
            expensePlanner.GrossPricePLN = request.GrossPricePLN;
            expensePlanner.TaxPLN = request.TaxPLN;

            await _repository.UpdatePlannerAsync(expensePlanner);
        }
    }
}
