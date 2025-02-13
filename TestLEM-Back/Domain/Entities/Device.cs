namespace Domain.Entities
{
    public class Device
    {
        public int Id { get; set; }
        public string IdentificationNumber { get; set; }
        public DateTime? ProductionDate { get; set; }
        public int? CalibrationPeriodInYears { get; set; }
        public DateTime? LastCalibrationDate { get; set; }
        public DateTime? NextCalibrationDate { get; set; }
        public bool? IsCalibrated { get; set; }
        public bool? IsCalibrationCloseToExpire { get; set; }
        public string? StorageLocation { get; set; }
        public int ModelId { get; set; }

        public virtual Model Model { get; set; }
        public virtual ICollection<Document> Documents { get; set; }
    }
}
