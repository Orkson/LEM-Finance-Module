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
        Task<Device> GetDeviceByIdAsync(int deviceId, bool includeRelated, CancellationToken cancellationToken);
        Task<List<Device>> GetDevicesByIdsAsync(IEnumerable<int> deviceIds, CancellationToken cancellationToken);
        Task<List<DeviceRelations>> GetDeviceRelationsAsync(int deviceId, CancellationToken ct);
        Task AddDeviceRelationsAsync(List<DeviceRelations> toInsert, CancellationToken cancellationToken);
        Task RemoveDeviceRelationsAsync(IEnumerable<DeviceRelations> enumerable, CancellationToken cancellationToken);
    }
}
