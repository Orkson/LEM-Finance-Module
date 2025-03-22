using MediatR;

namespace Application.Services.Commands
{
    public class CreateServiceCommand : IRequest<Domain.Entities.Service>
    {
        public int Id { get; set; }
        public string ServiceName { get; set; }
    }
}
