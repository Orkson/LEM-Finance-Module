using Application.Models;
using AutoMapper;
using Domain.Abstraction;
using MediatR;

namespace Application.ExpensesPlanner.Queries
{
    internal class GetExpensePlannerByIdHandler : IRequestHandler<GetExpensePlannerByIdQuery, ExpensePlannerDto>
    {
        private readonly IExpensePlannerRepository _repository;
        private readonly IMapper _mapper;

        public GetExpensePlannerByIdHandler(IExpensePlannerRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ExpensePlannerDto> Handle(GetExpensePlannerByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetPlannerByIdAsync(request.Id);

            if (entity == null)
                return null;

            return _mapper.Map<ExpensePlannerDto>(entity);
        }
    }
}
