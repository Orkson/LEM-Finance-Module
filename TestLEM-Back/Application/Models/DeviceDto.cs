namespace Application.Models
{
    public class DeviceDto
    {
        public int DeviceId { get; set; }
        public string DeviceIdentificationNumber { get; set; }
        public string ModelName { get; set; }
        public string ModelSerialNumber { get; set; }
        public int ModelId { get; set; }
        public ICollection<MeasuredValueDto> MeasuredValues { get; set; }
        public string? StorageLocation { get; set; }
        public DateTime? ProductionDate { get; set; }
        public DateTime? LastCalibrationDate { get; set; }
        public DateTime? NextCalibrationDate { get; set; }
        public int? CalibrationPeriodInYears { get; set; }
        public bool? IsCloseToExpire { get; set; }
        public string? Producer { get; set; }
    }
}
