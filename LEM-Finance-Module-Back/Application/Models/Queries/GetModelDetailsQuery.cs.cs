using MediatR;

namespace Application.Models.Queries
{
    public record GetModelDetailsQuery(int modelId, string modelName) : IRequest<ModelDetailsDto>;
}
