using Application.Abstractions.Messaging;
using Application.Models;
using System.Windows.Input;

namespace Application.Devices.Commands
{
    public record EditDeviceCommand(int deviceId, AddDeviceDto oldDeviceDto, EditDeviceDto newDeviceDto, ICollection<int>? modelCooperationsToBeRemoved, CancellationToken cancellationToken) : ICommand<EditedDeviceResponseDto>;
}
