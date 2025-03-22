using Application.Models;
using AutoMapper;
using Domain.Abstraction;
using MediatR;

namespace Application.ExpensesPlanner.Queries
{
    public class GetExpensePlannerByYearHandler : IRequestHandler<GetExpensePlannerByYearQuery, List<ExpensePlannerDto>>
    {
        private readonly IExpensePlannerRepository _repository;
        private readonly IMapper _mapper;
        public GetExpensePlannerByYearHandler(IExpensePlannerRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<ExpensePlannerDto>> Handle(GetExpensePlannerByYearQuery request, CancellationToken cancellationToken)
        {
            var planner = await _repository.GetPlannerByYearAsync(request.Year);
            var x = _mapper.Map<List<ExpensePlannerDto>>(planner);

            return x;
        }
    }
}
