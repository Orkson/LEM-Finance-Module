namespace Domain.Entities
{
    public class CalibrationCost
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public DateTime CalibrationDate { get; set; }
        public int Year { get; set; }
        public virtual Device Device { get; set; }
    }
}
