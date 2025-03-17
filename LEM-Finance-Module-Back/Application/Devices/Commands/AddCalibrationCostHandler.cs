using Domain.Abstraction;
using Domain.Entities;
using MediatR;

namespace Application.Devices.Commands
{
    public class AddCalibrationCostHandler : IRequestHandler<AddCalibrationCostCommand, CalibrationCost>
    {
        private readonly ICalibrationCostRepository _repository;

        public AddCalibrationCostHandler(ICalibrationCostRepository repository)
        {
            _repository = repository;
        }

        public async Task<CalibrationCost> Handle(AddCalibrationCostCommand request, CancellationToken cancellationToken)
        {
            var cost = new CalibrationCost
            {
                DeviceId = request.DeviceId,
                Amount = request.Amount,
                Currency = request.Currency,
                CalibrationDate = request.CalibrationDate,
                Year = request.Year
            };

            return await _repository.AddCostAsync(cost);
        }
    }
}
