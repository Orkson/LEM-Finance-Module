using Application.Abstractions.Messaging;

namespace Application.Devices.Commands
{
    public record RemoveDeviceCommand(int deviceId, CancellationToken cancellationToken) : ICommand<bool>
    {
    }
}
