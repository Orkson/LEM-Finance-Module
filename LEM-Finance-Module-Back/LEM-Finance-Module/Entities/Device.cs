﻿using System.Reflection;

namespace TestLEM.Entities
{
    public class Device
    {
        public int Id { get; set; }
        public string IdentifiactionNumber { get; set; }
        public DateTime? ProductionDate { get; set; }
        public int? CalibrationPeriodInYears { get; set; }
        public DateTime? LastCalibrationDate { get; set; }
        public bool? IsCalibrated { get; set; }
        public bool? IsCalibrationCloseToExpire { get; set; }
        public string? StorageLocation { get; set; }
        public int ModelId { get; set; }

        public virtual Model Model { get; set; }
    }
}
