using TestLEM.Entities;

namespace TestLEM.Repositories
{
    public interface IDeviceRepository
    {
        void AddDeviceToDatabase(Device device);
        bool ChcekIfDeviceAlreadyExistsInDatabase(string identificationNumber);
    }
}
