using Application.Abstractions;
using Application.Models;
using Domain.Abstraction;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ModelRepository : IModelRepository
    {
        private readonly IApplicationDbContext _dbContext;

        public ModelRepository(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> ChcekIfModelExists(string name, string serialNumber) => await _dbContext.Models.AnyAsync(x => x.SerialNumber == serialNumber || x.Name == name); //trzeba jakoś ograć co w przypadku gdy użtkownik poda nieistenijący serial, a istenijącą nazwę
        public int GetModelId(string name, string serialNumber) => _dbContext.Models.First(x => x.SerialNumber == serialNumber || x.Name == name).Id;


        public async Task AddModel(Model model, CancellationToken cancellationToken)
        {
            await _dbContext.Models.AddAsync(model);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<Model> GetModelById(int modelId, CancellationToken cancellationToken)
        {
            return await _dbContext.Models
                                    .Include(x => x.Company)
                                    .Include(x => x.MeasuredValues).ThenInclude(x => x.PhysicalMagnitude).ThenInclude(x => x.MeasuredValues).ThenInclude(x => x.MeasuredRanges)
                                    .Include(x => x.CooperateFrom)
                                    .Include(x => x.CooperateTo)
                                    .FirstAsync(x => x.Id == modelId, cancellationToken);
        }

        public async Task<ICollection<Model>> GetModelsByName(string name, CancellationToken cancellationToken)
        {
            var models = await _dbContext.Models.Where(x => x.Name.ToLower() == name.ToLower()).ToListAsync(cancellationToken);
            return models;
        }

        public async Task UpdateModelAsync(int modelId, ModelDto newModel, CancellationToken cancellationToken)
        {
            var model = await _dbContext.Models.FirstAsync(x => x.Id == modelId, cancellationToken);
        }

        public async Task<bool> CheckIfModelExistsByIdAsync(int id) => await _dbContext.Models.AnyAsync(x => x.Id == id);

        public async Task UpdateModelValuesAsync(int modelId, Model newModel, CancellationToken cancellationToken)
        {
            var model = await _dbContext.Models.FirstAsync(x => x.Id == modelId, cancellationToken);
            model.Name = newModel.Name;
            model.SerialNumber = newModel.SerialNumber;
            model.Company = newModel.Company;
            model.Devices = newModel.Devices;
            model.MeasuredValues = newModel.MeasuredValues;
            model.Documents = newModel.Documents;
            model.CooperateTo = newModel.CooperateTo;
            model.CooperateFrom = newModel.CooperateFrom;

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<Model> GetModelByName(string name)
        {
            var result = await _dbContext.Models
                                .Include(x => x.MeasuredValues).ThenInclude(x => x.PhysicalMagnitude)
                                .Include(x => x.MeasuredValues).ThenInclude(x => x.MeasuredRanges)
                                .FirstOrDefaultAsync(x => x.Name == name);               ;
            return result;
        }

        public async Task<Model> GetModelWithRelatedDevices(int modelId, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Models
                .Include(x => x.Devices)
                .Include(x => x.Company).ThenInclude(x => x.Models)
                .FirstAsync(x => x.Id == modelId);

            return result;
        }

        public async Task RemoveModelById(int modelId, CancellationToken cancellationToken)
        {
            var modelToBeRemoved = await _dbContext.Models.Include(x => x.Documents).FirstAsync(x => x.Id == modelId);
            _dbContext.Models.Remove(modelToBeRemoved);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
