using Domain.Entities;
using MediatR;

namespace Application.ExpensesPlanner.Commands
{
    public class CreateServiceCommand : IRequest<ExpensePlanner>
    {
        public DateTime PlannedDate { get; set; }
        public string? StorageLocationName { get; set; }
        public decimal NetPrice { get; set; }
        public decimal GrossPrice { get; set; }
        public decimal Tax { get; set; }
        public string? Currency { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public virtual int ServiceId { get; set; }
        public virtual int DeviceId { get; set; }
        public decimal? NetPricePLN { get; set; }
        public decimal? GrossPricePLN { get; set; }
        public decimal? TaxPLN { get; set; }
    }
}
