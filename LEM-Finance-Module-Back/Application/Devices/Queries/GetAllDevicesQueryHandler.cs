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
            IQueryable<Device> devicesQuery = _dbContext.Devices;//.Include(x => x.Model);
            var searchTerm = request.pagedAndSortedDevicesQueryDto.SearchTerm?.ToLower();
            const string descWord = "desc";

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                devicesQuery = devicesQuery
                        .Include(x => x.MeasuredValues)
                            //.Include(x => x.PhysicalMagnitude)
                    .Where(x => x.MeasuredValues.Where(x => x.PhysicalMagnitude.Name.ToLower().Contains(searchTerm)).Any()
                             || x.IdentificationNumber == searchTerm);
            }
            if (request.pagedAndSortedDevicesQueryDto.SortOrder?.ToLower() == descWord)
            {
                //if (request.pagedAndSortedDevicesQueryDto.SortColumn == "modelName")
                //{
                //    devicesQuery = devicesQuery.OrderByDescending(x => x.Model.Name.ToLower());
                //}
                //else
                //{
                devicesQuery = devicesQuery.OrderByDescending(x => x.NextCalibrationDate)
                                               .ThenByDescending(x => x.NextCalibrationDate == null);
                //}
            }
            else
            {
                //if (request.pagedAndSortedDevicesQueryDto.SortColumn == "modelName")
                //{
                //    devicesQuery = devicesQuery.OrderBy(x => x.Model.Name.ToLower());
                //}
                //else
                //{
                devicesQuery = devicesQuery.OrderBy(x => x.NextCalibrationDate == null)
                                               .ThenBy(x => x.NextCalibrationDate);
                //}
            }

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
                // EstimatedCalibrationDate i MeasuredValues dodamy później
            });

            // 2. Pobierasz dane z bazy (np. po paginacji jeśli chcesz) i dopiero liczysz w pamięci:
            var deviceList = await deviceQueryResponse.ToListAsync();

            foreach (var device in deviceList)
            {
                // EstimatedCalibrationDate obliczamy w pamięci
                device.EstimatedCalibrationDate = device.LastCalibrationDate.HasValue
                    ? device.LastCalibrationDate.Value.AddYears(device.CalibrationPeriodInYears ?? 0)
                    : (DateTime?)null;

                // MeasuredValues też pobierz osobno albo jeśli były w eager loadingu to mapuj
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
