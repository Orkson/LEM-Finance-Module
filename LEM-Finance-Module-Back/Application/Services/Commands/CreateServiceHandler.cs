using Domain.Abstraction;
using Domain.Entities;
using MediatR;

namespace Application.Services.Commands
{
    public class CreateServiceHandler : IRequestHandler<CreateServiceCommand, Domain.Entities.Service>
    {
        private readonly IExpensePlannerRepository _repository;

        public CreateServiceHandler(IExpensePlannerRepository repository)
        {
            _repository = repository;
        }
        public async Task<Domain.Entities.Service> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
        {
            var service = new Domain.Entities.Service
            {
                Id = request.Id,
                ServiceName = request.ServiceName,
            };

            return null;
        }
    }
}
