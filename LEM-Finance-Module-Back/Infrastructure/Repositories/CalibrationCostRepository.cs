using Application.Abstractions;
using Domain.Abstraction;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CalibrationCostRepository : ICalibrationCostRepository
    {
        private readonly IApplicationDbContext _context;

        public CalibrationCostRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CalibrationCost>> GetCostsByYearAsync(int year)
        {
            return await _context.CalibrationCosts
                .Where(c => c.Year == year)
                .ToListAsync();

            var xyz = _context.CalibrationCosts.ToListAsync();
            var device = _context.Devices.FirstOrDefault();

            var cal = new CalibrationCost
            {
                Id = 2,
                DeviceId = 41,
                Amount = 25,
                Currency = "USD",
                CalibrationDate = new DateTime(),
                Year = 2025,
            };

            _context.CalibrationCosts.Add(cal);
            var trying = _context.SaveChangesAsync();
        }

        public async Task<CalibrationCost> AddCostAsync(CalibrationCost cost)
        {
            _context.CalibrationCosts.Add(cost);
            await _context.SaveChangesAsync();
            return cost;
        }

        public async Task<bool> UpdateCostAsync(CalibrationCost cost)
        {
            _context.CalibrationCosts.Update(cost);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteCostAsync(Guid costId)
        {
            var cost = await _context.CalibrationCosts.FindAsync(costId);
            if (cost == null) return false;
            _context.CalibrationCosts.Remove(cost);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
