using Application.Abstractions;
using Domain.Abstraction;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    internal class ServiceRepository : IServiceRepository
    {
        private readonly IApplicationDbContext _context;
        public ServiceRepository(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Service> AddService(Service service)
        {
            //_context.Service.Add(service);
            //
            //await _context.SaveChangesAsync();

            return service;
        }
    }
}
