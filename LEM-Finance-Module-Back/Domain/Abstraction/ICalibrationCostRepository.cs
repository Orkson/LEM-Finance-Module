using Domain.Entities;

namespace Domain.Abstraction
{
    public interface ICalibrationCostRepository
    {
        Task<List<CalibrationCost>> GetCostsByYearAsync(int year);
        Task<CalibrationCost> AddCostAsync(CalibrationCost cost);
        Task<bool> UpdateCostAsync(CalibrationCost cost);
        Task<bool> DeleteCostAsync(Guid costId);
    }
}
