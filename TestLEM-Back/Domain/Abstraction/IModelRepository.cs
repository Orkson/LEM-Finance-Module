using Domain.Entities;

namespace Domain.Abstraction
{
    public interface IModelRepository
    {
        Task<bool> ChcekIfModelExists(string name, string serialNumber);
        Task<bool> CheckIfModelExistsByIdAsync(int id);
        int GetModelId(string name, string serialNumber);
        Task AddModel(Model model, CancellationToken cancellationToken = default);
        Task<Model> GetModelById(int modelId, CancellationToken cancellationToken);
        Task<ICollection<Model>> GetModelsByName(string name, CancellationToken cancellationToken);
        Task UpdateModelValuesAsync(int modelId, Model newModelValues, CancellationToken cancellationToken);
        Task<Model> GetModelByName(string name);
        Task<Model> GetModelWithRelatedDevices(int modelId, CancellationToken cancellationToken);
        Task RemoveModelById(int modelId, CancellationToken cancellationToken);
    }
}
