namespace Application.Models
{
    public class CreatedDeviceResponseDto
    {
        public string IdentificationNumber { get; set; }
        public int? DeviceId { get; set; }
        public int? ModelId { get; set; }

        public CreatedDeviceResponseDto(string identificationNumber, int? modelId, int? deviceId)
        {
            IdentificationNumber = identificationNumber;
            DeviceId = deviceId;
            ModelId = modelId;
        }
    }
}
