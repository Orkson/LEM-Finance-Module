namespace Domain.Entities
{
    public class Device
    {
        public int Id { get; set; }
        public string? IdentificationNumber { get; set; }
        public DateTime? ProductionDate { get; set; }
        public int? CalibrationPeriodInYears { get; set; }
        public DateTime? LastCalibrationDate { get; set; }
        public DateTime? NextCalibrationDate { get; set; }
        public bool? IsCalibrated { get; set; }
        public bool? IsCalibrationCloseToExpire { get; set; }
        public string? StorageLocation { get; set; }
        public string? SerialNumber { get; set; }
        public string? Model { get; set; }
        public virtual Company? Company { get; set; }
        public virtual ICollection<Document>? Documents { get; set; }
        public virtual ICollection<MeasuredValue>? MeasuredValues { get; set; }
        public virtual ICollection<DeviceRelations> RelatedDevices { get; set; } = new List<DeviceRelations>();
        public virtual ICollection<DeviceRelations> RelatedByDevices { get; set; } = new List<DeviceRelations>();
    }
}
