using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.Models
{
    public class AddDeviceDto
    {
        public string IdentificationNumber { get; set; }
        public DateTime? ProductionDate { get; set; }
        public int? CalibrationPeriodInYears { get; set; }
        public DateTime? LastCalibrationDate { get; set; }
        public DateTime? NextCalibrationDate { get; set; }
        public bool? IsCalibrated { get; set; }
        public bool? IsCalibrationCloseToExpire { get; set; }
        public string? StorageLocation { get; set; }
        public string Model { get; set; }
        public virtual Company Company { get; set; }
        public string SerialNumber { get; set; }
    }
}
