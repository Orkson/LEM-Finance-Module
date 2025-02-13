using AutoMapper;
using TestLEM.Entities;

namespace TestLEM.Repositories
{
    public class DeviceRepository : IDeviceRepository
    {
        private readonly IMapper mapper;
        private readonly LemDbContext dbContext;

        public DeviceRepository(IMapper mapper, LemDbContext dbContext)
        {
            this.mapper = mapper;
            this.dbContext = dbContext;
        }

        public void AddDeviceToDatabase(Device device)
        {
            dbContext.Devices.Add(device);
            dbContext.SaveChanges();
        }

        public bool ChcekIfDeviceAlreadyExistsInDatabase(string identificationNumber) => dbContext.Devices.Any(x => x.IdentifiactionNumber == identificationNumber);

    }
}
