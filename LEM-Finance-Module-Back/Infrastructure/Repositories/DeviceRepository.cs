using Application.Abstractions;
using Domain.Abstraction;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class DeviceRepository : IDeviceRepository
    {
        private readonly IApplicationDbContext _dbContext;

        public DeviceRepository(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> AddDevice(Device device, CancellationToken cancellationToken)
        {
            _dbContext.Devices.Add(device);

            await _dbContext.SaveChangesAsync(cancellationToken);
            return device.Id;
        }

        public async Task<bool> CheckIfDeviceExists(string identificationNumber, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Devices.AnyAsync(x => x.IdentificationNumber == identificationNumber, cancellationToken);
            return result;
        }

        public async Task<Device> GetDeviceById(int id, CancellationToken cancellationToken) => 
            await _dbContext.Devices
            .Include(x => x.Company)
            .Include(x => x.RelatedDevices)
                .ThenInclude(r => r.RelatedDevice)
            .FirstAsync(x => x.Id == id, cancellationToken);

        public async Task RemoveDeviceById(int deviceId, CancellationToken cancellationToken)
        {
            var deviceToBeRemoved = await GetDeviceById(deviceId, cancellationToken);

            var costPlannerToBeRemoved = await GetExpensePlannersToRemove(deviceId, cancellationToken);
            var relationsToBeRemoved = await GetRelationsToRemove(deviceId, cancellationToken);

            _dbContext.DeviceRelations.RemoveRange(relationsToBeRemoved);
            _dbContext.ExpensePlanner.RemoveRange(costPlannerToBeRemoved);
            _dbContext.Devices.Remove(deviceToBeRemoved);
            
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private async Task<List<DeviceRelations>> GetRelationsToRemove(int deviceId, CancellationToken cancellationToken)
        {
            return await _dbContext.DeviceRelations
                .Where(x => x.RelatedDeviceId == deviceId)
                .ToListAsync();
        }

        private async Task<List<ExpensePlanner>> GetExpensePlannersToRemove(int deviceId, CancellationToken cancellationToken)
        {
            return await _dbContext.ExpensePlanner
                .Where(x => x.DeviceId == deviceId)
                .ToListAsync(cancellationToken);
        }

        public async Task UpdateDeviceAsync(int deviceId, Device newDevice, CancellationToken cancellationToken)
        {
            var device = await GetDeviceById(deviceId, cancellationToken);
            device.IdentificationNumber = newDevice.IdentificationNumber;
            device.ProductionDate = newDevice.ProductionDate;
            device.CalibrationPeriodInYears = newDevice.CalibrationPeriodInYears;
            device.LastCalibrationDate = newDevice.LastCalibrationDate;
            device.NextCalibrationDate = newDevice.NextCalibrationDate;
            device.IsCalibrated = newDevice.IsCalibrated;
            device.IsCalibrationCloseToExpire = newDevice.IsCalibrationCloseToExpire;
            device.StorageLocation = newDevice.StorageLocation;

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<Device> GetDeviceByIdAsync(int deviceId, bool includeRelated, CancellationToken ct)
        {
            var q = _dbContext.Devices.AsQueryable();
            if (includeRelated)
            {
                q = q.Include(d => d.RelatedDevices).Include(d => d.RelatedByDevices);
            }
            
            return await q.FirstAsync(d => d.Id == deviceId, ct);
        }

        public Task<List<Device>> GetDevicesByIdsAsync(IEnumerable<int> ids, CancellationToken ct)
            => _dbContext.Devices.Where(d => ids.Contains(d.Id)).ToListAsync(ct);

        public Task<List<DeviceRelations>> GetDeviceRelationsAsync(int deviceId, CancellationToken ct)
        {
            return _dbContext.DeviceRelations
                .Where(r => (r.DeviceId == deviceId) || (r.RelatedDeviceId == deviceId))
                .ToListAsync(ct);
        }

        public async Task AddDeviceRelationsAsync(List<DeviceRelations> toInsert, CancellationToken cancellationToken)
        {
            var pairs = (toInsert ?? new List<DeviceRelations>())
                .Select(l => (A: l.DeviceId, B: l.RelatedDeviceId))
                .Where(p => p.A != p.B)
                .Distinct()
                .ToList();

            if (pairs.Count == 0)
            {
                return;
            }

            var deviceIds = pairs.Select(p => p.A).ToHashSet();
            var relatedIds = pairs.Select(p => p.B).ToHashSet();

            IQueryable<DeviceRelations> q = _dbContext.DeviceRelations.AsNoTracking()
                .Where(r => deviceIds.Contains(r.DeviceId) && relatedIds.Contains(r.RelatedDeviceId));

            var existing = await q.ToListAsync(cancellationToken);
            var existingSet = new HashSet<(int, int)>(existing.Select(e => (e.DeviceId, e.RelatedDeviceId)));

            var toAdd = new List<DeviceRelations>();
            foreach (var (a, b) in pairs)
            {
                if (!existingSet.Contains((a, b)))
                    toAdd.Add(new DeviceRelations { DeviceId = a, RelatedDeviceId = b });
            }

            if (toAdd.Count == 0)
            {
                return;
            }

            await _dbContext.DeviceRelations.AddRangeAsync(toAdd, cancellationToken);
            
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task RemoveDeviceRelationsAsync(IEnumerable<DeviceRelations> enumerable, CancellationToken cancellationToken)
        {
            var pairs = enumerable
                .Select(l => new { A = l.DeviceId, B = l.RelatedDeviceId })
                .Where(p => p.A != p.B)
                .Distinct()
                .ToList();

            if (pairs.Count == 0)
            {
                return;
            }

            var deviceIds = pairs.Select(p => p.A).ToHashSet();
            var relatedIds = pairs.Select(p => p.B).ToHashSet();

            var candidates = await _dbContext.DeviceRelations
                .Where(r => deviceIds.Contains(r.DeviceId) && relatedIds.Contains(r.RelatedDeviceId))
                .ToListAsync(cancellationToken);

            var set = pairs.Select(p => (p.A, p.B)).ToHashSet();
            
            var toDelete = candidates
                .Where(r =>
                    set.Contains((r.DeviceId, r.RelatedDeviceId)))
                .ToList();

            if (toDelete.Count == 0)
                return;

            _dbContext.DeviceRelations.RemoveRange(toDelete);
            
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
