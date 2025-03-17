namespace Application.Models
{
    public class CalibrationCostDto
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public DateTime CalibrationDate { get; set; }
    }
}
