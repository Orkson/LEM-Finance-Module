using Application.Models;
using MediatR;

namespace Application.Devices.Queries
{
    public record GetAllDevicesQuery(PagedAndSortedDevicesListQueryDto pagedAndSortedDevicesQueryDto) : IRequest<PagedList<DeviceDto>>;
}
