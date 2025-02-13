namespace Application.Models
{
    public class MeasuredRangesDto
    {
        public int Id { get; set; }
        public string? Range { get; set; }
        public decimal? AccuracyInPercent { get; set; }
    }
}