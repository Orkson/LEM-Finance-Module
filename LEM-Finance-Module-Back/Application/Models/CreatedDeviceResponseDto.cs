namespace Application.Models
{
    public class CreatedDeviceResponseDto
    {
        public string IdentificationNumber { get; set; }
        public int? DeviceId { get; set; }

        public CreatedDeviceResponseDto(string identificationNumber, int? deviceId)
        {
            IdentificationNumber = identificationNumber;
            DeviceId = deviceId;
        }
    }
}
