namespace TestLEM.Models
{
    public class MeasuredValueDto
    {
        public string PhysicalMagnitudeName { get; set; }
        public string? PhysicalMagnitudeUnit { get; set; }
        public ICollection<MeasuredRangesDto> MeasuredRanges { get; set; }
    }
}