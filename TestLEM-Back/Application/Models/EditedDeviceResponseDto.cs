namespace Application.Models
{
    public class EditedDeviceResponseDto
    {
        public string IdentificationNumber { get; set; }
        public int? DeviceId { get; set; }
        public int? ModelId { get; set; }

        public EditedDeviceResponseDto(string identificationNumber, int? modelId, int? deviceId)
        {
            IdentificationNumber = identificationNumber;
            DeviceId = deviceId;
            ModelId = modelId;
        }
    }
}
