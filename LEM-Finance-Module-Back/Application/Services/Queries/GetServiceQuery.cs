using Domain.Entities;
using MediatR;

namespace Application.Services.Queries
{
    public class GetServiceQuery : IRequest<List<Service>>
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
