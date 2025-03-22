using AutoMapper;
using Domain.Abstraction;
using Domain.Entities;
using MediatR;

namespace Application.Services.Queries
{
    public class GetServiceHandler : IRequestHandler<GetServiceQuery, List<Service>>
    {
        private readonly IExpensePlannerRepository _repository;
        private readonly IMapper _mapper;
        public GetServiceHandler(IExpensePlannerRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<Service>> Handle(GetServiceQuery request, CancellationToken cancellationToken)
        {
            var service = await _repository.GetAllServices();
            
            return _mapper.Map<List<Service>>(service);
        }
    }
}
