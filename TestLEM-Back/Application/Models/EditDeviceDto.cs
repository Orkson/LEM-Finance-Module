namespace Application.Models
{
    public class EditDeviceDto : AddDeviceDto
    {
        public ICollection<int>? CooperationsIdsToBeRemoved { get; set; }
    }
}
