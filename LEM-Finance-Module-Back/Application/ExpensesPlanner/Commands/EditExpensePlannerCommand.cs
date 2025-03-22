using MediatR;

namespace Application.ExpensesPlanner.Commands
{
    public class EditExpensePlannerCommand : IRequest
    {
        public int Id { get; set; }
        public DateTime PlannedDate { get; set; }
        public string? StorageLocationName { get; set; }
        public decimal NetPrice { get; set; }
        public decimal GrossPrice { get; set; }
        public decimal Tax { get; set; }
        public string? Currency { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public int ServiceId { get; set; }
        public int DeviceId { get; set; }
    }
}
