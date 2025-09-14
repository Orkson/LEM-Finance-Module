namespace Domain.Entities
{
    public class MeasuredValue
    {
        public int Id { get; set; }
        public int? DeviceId { get; set; }
        public int? PhysicalMagnitudeId { get; set; }

        public virtual PhysicalMagnitude? PhysicalMagnitude { get; set; }
        public virtual ICollection<MeasuredRange>? MeasuredRanges { get; set; }
        public virtual Device? Device { get; set; }
    }
}
