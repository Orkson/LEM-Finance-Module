namespace Application.Models
{
    public class MeasuredValueDto
    {
        public int Id { get; set; }
        public int PhysicalMagnitudeId { get; set; }
        public string PhysicalMagnitudeName { get; set; }
        public string? PhysicalMagnitudeUnit { get; set; }
        public ICollection<MeasuredRangesDto>? MeasuredRanges { get; set; }
    }
}