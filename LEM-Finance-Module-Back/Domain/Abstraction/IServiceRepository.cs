using Domain.Entities;

namespace Domain.Abstraction
{
    public interface IServiceRepository
    {
        Task<Service> AddService(Service service);
    }
}
