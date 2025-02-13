namespace Domain.Entities
{
    public class PhysicalMagnitude
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Unit { get; set; }

        public virtual ICollection<MeasuredValue> MeasuredValues { get; set; }
    }
}