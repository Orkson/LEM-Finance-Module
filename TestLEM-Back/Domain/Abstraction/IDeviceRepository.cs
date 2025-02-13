using Domain.Entities;

namespace Domain.Abstraction
{
    public interface IDeviceRepository
    {
        Task<int> AddDevice(Device device, CancellationToken cancellationToken);
        Task<bool> CheckIfDeviceExists(string identificationNumber, CancellationToken cancellationToken = default);
        Task<Device> GetDeviceById(int id, CancellationToken cancellationToken);
        Task UpdateDeviceAsync(int deviceId, Device newDevice, CancellationToken cancellationToken);
        Task RemoveDeviceById(int deviceId, CancellationToken cancellationToken);
        int TotalDevicesByModelCount(string modelName);
    }
}
