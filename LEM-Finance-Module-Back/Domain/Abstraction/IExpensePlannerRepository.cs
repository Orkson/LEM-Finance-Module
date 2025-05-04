using Domain.Entities;

namespace Domain.Abstraction
{
    public interface IExpensePlannerRepository
    {
        Task<List<ExpensePlanner>> GetPlannerByYearAsync(int year);
        Task<ExpensePlanner> GetPlannerByIdAsync(int id);
        Task<ExpensePlanner> AddPlannerAsync(ExpensePlanner planner);
        Task<bool> UpdatePlannerAsync(ExpensePlanner planner);
        Task<bool> DeletePlannerAsync(int plannerId);
        Task<Service> GetServiceByIdAsync(int serviceId);
        Task<List<Service>> GetAllServices();
        Task<Device> GetDeviceByIdAsync(int deviceId);
    }
}
