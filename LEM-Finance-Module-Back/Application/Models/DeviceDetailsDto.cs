namespace Application.Models
{
    public class DeviceDetailsDto : DeviceDto
    {
        public bool? IsCalibrated { get; set; }
        public int TotalDevicesOfModelCount { get; set; }

        public ICollection<DocumentDto>? DeviceDocuments { get; set; }
    }
}
