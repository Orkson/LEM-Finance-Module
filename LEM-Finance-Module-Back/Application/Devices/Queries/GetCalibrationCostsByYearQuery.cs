using Application.Models;
using MediatR;

namespace Application.Devices.Queries
{
    public class GetCalibrationCostsByYearQuery : IRequest<List<CalibrationCostDto>>
    {
        public int Year { get; set; }
    }

}
