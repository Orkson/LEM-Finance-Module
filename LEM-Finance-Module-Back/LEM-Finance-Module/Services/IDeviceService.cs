using TestLEM.Models;

namespace TestLEM.Services
{
    public interface IDeviceService
    {
        void AddDeviceToDatabase(AddDeviceDto addDeviceDto);
    }
}
