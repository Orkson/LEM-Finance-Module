namespace Domain.Entities
{
    public class DeviceRelations
    {
        public int DeviceId { get; set; }
        public Device? Device { get; set; }
        public int RelatedDeviceId { get; set; }
        public Device? RelatedDevice { get; set; }
    }
}
