using Application.Abstractions;
using Domain.Abstraction;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ExpensePlannerRepository : IExpensePlannerRepository
    {
        private readonly IApplicationDbContext _context;
        public ExpensePlannerRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ExpensePlanner> AddPlannerAsync(ExpensePlanner planner)
        {
            _context.ExpensePlanner.Add(planner);
            await _context.SaveChangesAsync();

            return planner;
        }

        public async Task<List<ExpensePlanner>> GetPlannerByYearAsync(int year)
        {
            var x =  await _context.ExpensePlanner
                .Include(e => e.Service)
                .Include(e => e.Device)
                .Where(c => EF.Functions.Like(c.PlannedDate.ToString(), $"{year}-%"))
                .ToListAsync();

            return x;
        }

        public async Task<ExpensePlanner> GetPlannerByIdAsync(int id)
        {
            return await _context.ExpensePlanner
                .Where(c => c.Id == id)
                .Include(e => e.Device)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdatePlannerAsync(ExpensePlanner planner)
        {
            _context.ExpensePlanner.Update(planner);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeletePlannerAsync(int plannerId)
        {
            var planner = await _context.ExpensePlanner.FindAsync(plannerId);
            if (planner == null)
            {
                return false;
            }
            _context.ExpensePlanner.Remove(planner);

            return await _context.SaveChangesAsync() > 0;
        }

        public Task<Service> GetServiceByIdAsync(int serviceId)
        {
            return _context.Service
                .Where(c => c.Id == serviceId)
                .FirstOrDefaultAsync();
        }

        public Task<List<Service>> GetAllServices()
        {
            return _context.Service
                .ToListAsync();
        }

        public Task<Device> GetDeviceByIdAsync(int deviceId)
        {
            return _context.Devices
                .Where(c => c.Id == deviceId)
                .FirstOrDefaultAsync();
        }
    }
}
