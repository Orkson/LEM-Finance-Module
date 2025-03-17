using Domain.Entities;
using MediatR;

namespace Application.Devices.Commands
{
    public class AddCalibrationCostCommand : IRequest<CalibrationCost>
    {
        public int DeviceId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public DateTime CalibrationDate { get; set; }
        public int Year { get; set; }
    }
}
