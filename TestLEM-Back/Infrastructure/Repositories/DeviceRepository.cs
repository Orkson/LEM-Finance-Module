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

        public async Task<Device> GetDeviceById(int id, CancellationToken cancellationToken) => await _dbContext.Devices.Include(x => x.Model).ThenInclude(x => x.Company).FirstAsync(x => x.Id == id, cancellationToken);

        public async Task RemoveDeviceById(int deviceId, CancellationToken cancellationToken)
        {
            var deviceToBeRemoved = await GetDeviceById(deviceId, cancellationToken);
            _dbContext.Devices.Remove(deviceToBeRemoved);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public int TotalDevicesByModelCount(string modelName) => _dbContext.Devices.Include(x => x.Model).Where(x => x.Model.Name.ToLower() == modelName.ToLower()).Count();
        

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
            device.Model = newDevice.Model;
            device.ModelId = newDevice.ModelId;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
