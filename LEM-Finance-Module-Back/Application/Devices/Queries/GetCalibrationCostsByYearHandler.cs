using Application.Models;
using AutoMapper;
using Domain.Abstraction;
using MediatR;

namespace Application.Devices.Queries
{
    public class GetCalibrationCostsByYearHandler : IRequestHandler<GetCalibrationCostsByYearQuery, List<CalibrationCostDto>>
    {
        private readonly ICalibrationCostRepository _repository;
        private readonly IMapper _mapper;

        public GetCalibrationCostsByYearHandler(ICalibrationCostRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<CalibrationCostDto>> Handle(GetCalibrationCostsByYearQuery request, CancellationToken cancellationToken)
        {
            var costs = await _repository.GetCostsByYearAsync(request.Year);
            return _mapper.Map<List<CalibrationCostDto>>(costs);
        }
    }
}
