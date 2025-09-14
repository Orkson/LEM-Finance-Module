using Domain.Entities;

namespace Application.Models
{
    public class DeviceDto
    {
        public int? DeviceId { get; set; }
        public string? DeviceIdentificationNumber { get; set; }
        public string? SerialNumber { get; set; }
        public ICollection<MeasuredValueDto>? MeasuredValues { get; set; }
        public string? StorageLocation { get; set; }
        public DateTime? ProductionDate { get; set; }
        public DateTime? LastCalibrationDate { get; set; }
        public DateTime? NextCalibrationDate { get; set; }
        public int? CalibrationPeriodInYears { get; set; }
        public bool? IsCloseToExpire { get; set; }
        public string? Producer { get; set; }
        public string? Model { get; set; }
        public ICollection<DeviceRelations>? DeviceRelations { get; set; }
        public ICollection<Device>? RelatedDevices { get; set; }
        public DateTime? EstimatedCalibrationDate { get; set; }
    }
}
