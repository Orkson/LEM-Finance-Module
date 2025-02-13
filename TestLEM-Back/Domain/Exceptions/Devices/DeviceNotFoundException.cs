using Domain.Exceptions.Base;

namespace Domain.Exceptions.Devices
{
    public class DeviceNotFoundException : NotFoundException
    {
        public DeviceNotFoundException(int id) :
            base($"Device with id: {id} has not been found.")
        {
        }
    }
}
