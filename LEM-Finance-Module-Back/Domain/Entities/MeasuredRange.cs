namespace Domain.Entities
{
    public class MeasuredRange
    {
        public int Id { get; set; }
        public string? Range { get; set; }
        public decimal? AccuracyInPercet { get; set; }
        public int? MeasuredValueId { get; set; }

        public virtual MeasuredValue MeasuredValue { get; set; }
    }
}