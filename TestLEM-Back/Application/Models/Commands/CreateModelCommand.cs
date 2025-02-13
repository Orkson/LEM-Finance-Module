using Application.Abstractions.Messaging;

namespace Application.Models.Commands
{
    public class CreateModelCommand(ModelDto modelDto) : ICommand<int>
    {
        public ModelDto ModelDto { get; set; } = modelDto;
    }
}
