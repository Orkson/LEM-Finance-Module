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
            IQueryable<Device> devicesQuery = _dbContext.Devices.Include(x => x.RelatedDevices);
            
            var searchTerm = request.pagedAndSortedDevicesQueryDto.SearchTerm?.Trim().ToLower();
            var sortColumn = request.pagedAndSortedDevicesQueryDto.SortColumn?.Trim().ToLower();
            var sortOrder = request.pagedAndSortedDevicesQueryDto.SortOrder?.Trim().ToLower();
            var desc = sortOrder == "desc";

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                devicesQuery = devicesQuery.Where(x =>
                    x.MeasuredValues.Any(mv =>
                        mv.PhysicalMagnitude != null &&
                        mv.PhysicalMagnitude.Name != null &&
                        mv.PhysicalMagnitude.Name.ToLower().Contains(searchTerm)
                    )
                    || (x.IdentificationNumber != null && x.IdentificationNumber.ToLower().Contains(searchTerm))
                );
            }

            sortColumn = sortColumn switch
            {
                "modelname" => "model",
                "calibrationdate" => "nextcalibrationdate",
                "identificationnumber" => "identificationnumber",
                "deviceidentificationnumber" => "identificationnumber",
                _ => sortColumn
            };

            devicesQuery = sortColumn switch
            {
                "model" => desc
                    ? devicesQuery.OrderByDescending(x => x.Model)
                    : devicesQuery.OrderBy(x => x.Model),

                "producer" => desc
                    ? devicesQuery.OrderByDescending(x => x.Company != null ? x.Company.Name : null)
                    : devicesQuery.OrderBy(x => x.Company != null ? x.Company.Name : null),

                "identificationnumber" => desc
                    ? devicesQuery.OrderByDescending(x => x.IdentificationNumber)
                    : devicesQuery.OrderBy(x => x.IdentificationNumber),

                // sortowanie jak wcześniej: najpierw te z datą, potem null-e na końcu
                "nextcalibrationdate" => desc
                    ? devicesQuery.OrderByDescending(x => x.NextCalibrationDate == null)
                                 .ThenByDescending(x => x.NextCalibrationDate)
                    : devicesQuery.OrderBy(x => x.NextCalibrationDate == null)
                                 .ThenBy(x => x.NextCalibrationDate),

                // DEFAULT: jak wcześniej po dacie kolejnej kalibracji
                _ => desc
                    ? devicesQuery.OrderByDescending(x => x.NextCalibrationDate == null)
                                 .ThenByDescending(x => x.NextCalibrationDate)
                    : devicesQuery.OrderBy(x => x.NextCalibrationDate == null)
                                 .ThenBy(x => x.NextCalibrationDate),
            };

            var deviceQueryResponse = devicesQuery
            .Select(x => new DeviceDto
            {
                DeviceId = x.Id,
                DeviceIdentificationNumber = x.IdentificationNumber,
                SerialNumber = x.SerialNumber,
                StorageLocation = x.StorageLocation,
                LastCalibrationDate = x.LastCalibrationDate,
                NextCalibrationDate = x.NextCalibrationDate,
                ProductionDate = x.ProductionDate,
                CalibrationPeriodInYears = x.CalibrationPeriodInYears,
                Producer = x.Company != null ? x.Company.Name : null,
                Model = x.Model,
                DeviceRelations = x.RelatedDevices,
            });

            var deviceList = await deviceQueryResponse.ToListAsync();

            foreach (var device in deviceList)
            {
                device.EstimatedCalibrationDate = device.LastCalibrationDate.HasValue
                    ? device.LastCalibrationDate.Value.AddYears(device.CalibrationPeriodInYears ?? 0)
                    : (DateTime?)null;

                device.MeasuredValues = devicesQuery
                    .Where(d => d.Id == device.DeviceId)
                    .SelectMany(d => d.MeasuredValues)
                    .Select(y => new MeasuredValueDto
                    {
                        PhysicalMagnitudeName = y.PhysicalMagnitude != null ? y.PhysicalMagnitude.Name : null,
                        PhysicalMagnitudeUnit = y.PhysicalMagnitude != null ? y.PhysicalMagnitude.Unit : null
                    })
                    .ToList();
            }

            var devices = await PagedList<DeviceDto>.CreateAsync(deviceQueryResponse, request.pagedAndSortedDevicesQueryDto.Page, request.pagedAndSortedDevicesQueryDto.PageSize);

            return devices;
        }
    }
}
