namespace Application.Models
{
    public class DeviceDetailsDto : DeviceDto
    {
        public bool? IsCalibrated { get; set; }
        public int TotalDevicesOfModelCount { get; set; }

        public ICollection<DocumentDto>? DeviceDocuments { get; set; }
        public ICollection<DocumentDto>? ModelDocuments { get; set; }
        public ICollection<ModelDto>? RelatedModels { get; set; }
    }
}
