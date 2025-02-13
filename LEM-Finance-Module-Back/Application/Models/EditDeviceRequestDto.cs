namespace Application.Models
{
    public class EditDeviceRequestDto
    {
        public AddDeviceDto OldDevice { get; set; }
        public EditDeviceDto NewDevice { get; set; }
        public ICollection<int>? ModelCooperationsToBeRemoved { get; set; }
    }
}
