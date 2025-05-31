using Domain.Entities;

namespace Application.Models
{
    public class ExpensePlannerDto
    {
        public int Id { get; set; }
        public DateTime PlannedDate { get; set; }
        public string? StorageLocationName { get; set; }
        public decimal NetPrice { get; set; }
        public decimal GrossPrice { get; set; }
        public decimal NetPricePLN { get; set; }
        public decimal GrossPricePLN { get; set; }
        public decimal Tax { get; set; }
        public string? Currency { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public virtual int ServiceId { get; set; }
        public virtual Service? Service { get; set; }
        public virtual Device? Device { get; set; }
    }
}
