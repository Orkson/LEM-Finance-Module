namespace Domain.Entities
{
    public class MeasuredValue
    {
        public int Id { get; set; }
        public int? ModelId { get; set; }
        public int? PhysicalMagnitudeId { get; set; }

        public virtual PhysicalMagnitude PhysicalMagnitude { get; set; }
        public virtual ICollection<MeasuredRange>? MeasuredRanges { get; set; }
        public virtual Model Model { get; set; }
    }
}
