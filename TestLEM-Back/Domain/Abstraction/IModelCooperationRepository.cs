using Domain.Entities;

namespace Domain.Abstraction
{
    public interface IModelCooperationRepository
    {
        Task AddModelCooperation(int deviceId, ICollection<int> cooperatedDevicesIds);
        Task<ICollection<ModelCooperation>> GetCooperationsForModelByModelId(int modelId, CancellationToken cancellationToken);
        Task RemoveModelCooperations(ICollection<int> cooperationsIdsToBeRemoved, CancellationToken cancellationToken);
    }
}
