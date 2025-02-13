using Application.Abstractions;
using Domain.Abstraction;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ModelCooperationRepository : IModelCooperationRepository
    {
        private readonly IApplicationDbContext _dbContext;

        public ModelCooperationRepository(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddModelCooperation(int modelId, ICollection<int> cooperatedDevicesIds)
        {
            var cooperations = new List<ModelCooperation>();
            foreach(var cooperatedDeviceId in cooperatedDevicesIds)
            {
                var cooperation = new ModelCooperation() 
                {
                    ModelFromId = modelId,
                    ModelToId = cooperatedDeviceId,
                };
                cooperations.Add(cooperation);
            };
            await _dbContext.ModelCooperation.AddRangeAsync(cooperations);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<ICollection<ModelCooperation>> GetCooperationsForModelByModelId(int modelId, CancellationToken cancellationToken)
        {
            var cooperations = await _dbContext.ModelCooperation.Where(x => x.ModelFromId == modelId || x.ModelToId == modelId).Include(x => x.ModelFrom).Include(x => x.ModelTo).ToListAsync(cancellationToken);
            return cooperations;
        }

        public async Task RemoveModelCooperations(ICollection<int> cooperationsIdsToBeRemoved, CancellationToken cancellationToken)
        {
            var cooperationsToRemove = await _dbContext.ModelCooperation.Where(x => cooperationsIdsToBeRemoved.Contains(x.ModelFromId) || cooperationsIdsToBeRemoved.Contains(x.ModelToId)).ToListAsync(cancellationToken);
            _dbContext.ModelCooperation.RemoveRange(cooperationsToRemove);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
