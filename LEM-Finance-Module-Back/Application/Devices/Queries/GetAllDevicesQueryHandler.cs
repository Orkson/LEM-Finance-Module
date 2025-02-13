using Application.Abstractions;
using Application.Models;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Devices.Queries
{
    internal class GetAllDevicesQueryHandler :
        IRequestHandler<GetAllDevicesQuery, PagedList<DeviceDto>>
    {
        private readonly IApplicationDbContext _dbContext;

        public GetAllDevicesQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedList<DeviceDto>> Handle(GetAllDevicesQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Device> devicesQuery = _dbContext.Devices.Include(x => x.Model);
            var searchTerm = request.pagedAndSortedDevicesQueryDto.SearchTerm?.ToLower();
            const string descWord = "desc";

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                devicesQuery = devicesQuery
                        .Include(x => x.Model.MeasuredValues)
                            .ThenInclude(x => x.PhysicalMagnitude)
                    .Where(x => x.Model.Name.ToLower().Contains(searchTerm)
                             || x.Model.MeasuredValues.Where(x => x.PhysicalMagnitude.Name.ToLower().Contains(searchTerm)).Any()
                             || x.IdentificationNumber == searchTerm);

            }
            if (request.pagedAndSortedDevicesQueryDto.SortOrder?.ToLower() == descWord)
            {
                if (request.pagedAndSortedDevicesQueryDto.SortColumn == "modelName")
                {
                    devicesQuery = devicesQuery.OrderByDescending(x => x.Model.Name.ToLower());
                }
                else
                {
                    devicesQuery = devicesQuery.OrderByDescending(x => x.NextCalibrationDate)
                                               .ThenByDescending(x => x.NextCalibrationDate == null);
                }
            }
            else
            {
                if (request.pagedAndSortedDevicesQueryDto.SortColumn == "modelName")
                {
                    devicesQuery = devicesQuery.OrderBy(x => x.Model.Name.ToLower());
                }
                else
                {
                    devicesQuery = devicesQuery.OrderBy(x => x.NextCalibrationDate == null)
                                               .ThenBy(x => x.NextCalibrationDate);
                }
            }

            var deviceQueryResponse = devicesQuery
                .Select(x => new DeviceDto
                {
                    DeviceId = x.Id,
                    DeviceIdentificationNumber = x.IdentificationNumber,
                    ModelName = x.Model.Name,
                    ModelSerialNumber = x.Model.SerialNumber,
                    ModelId = x.ModelId,
                    StorageLocation = x.StorageLocation,
                    LastCalibrationDate = x.LastCalibrationDate,
                    NextCalibrationDate = x.NextCalibrationDate,
                    ProductionDate = x.ProductionDate,
                    CalibrationPeriodInYears = x.CalibrationPeriodInYears,
                    Producer = x.Model.Company.Name,
                    MeasuredValues = (ICollection<MeasuredValueDto>)x.Model.MeasuredValues.Select(y =>
                        new MeasuredValueDto
                        {
                            PhysicalMagnitudeName = y.PhysicalMagnitude.Name,
                            PhysicalMagnitudeUnit = y.PhysicalMagnitude.Unit
                        })
                });

            var devices = await PagedList<DeviceDto>.CreateAsync(deviceQueryResponse, request.pagedAndSortedDevicesQueryDto.Page, request.pagedAndSortedDevicesQueryDto.PageSize);

            return devices;
        }
    }
}
