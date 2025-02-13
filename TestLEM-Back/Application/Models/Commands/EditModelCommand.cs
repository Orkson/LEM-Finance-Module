using Application.Abstractions.Messaging;
using System.Windows.Input;

namespace Application.Models.Commands
{
    public record EditModelCommand(int modelId, ModelDto newModelDto, ICollection<int>? cooperationsIdsToBeRemoved) : ICommand<int>
    {
    }
}
