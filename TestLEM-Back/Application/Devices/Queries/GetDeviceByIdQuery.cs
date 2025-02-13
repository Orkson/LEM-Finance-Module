using Application.Models;
using MediatR;

namespace Application.Devices.Queries
{
    public record GetDeviceByIdQuery(int deviceId) : IRequest<DeviceDetailsDto>;
}
