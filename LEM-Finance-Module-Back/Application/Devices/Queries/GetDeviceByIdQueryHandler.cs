using Application.Abstractions;
using Application.Helpers;
using Application.Models;
using Domain.Abstraction;
using Domain.Entities;
using Domain.Exceptions.Devices;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Devices.Queries
{
    internal class GetDeviceByIdQueryHandler : IRequestHandler<GetDeviceByIdQuery, DeviceDetailsDto>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IModelCooperationRepository _modelCooperationRepository;
        private readonly IDeviceRepository _deviceRepository;

        public GetDeviceByIdQueryHandler(IApplicationDbContext dbContext, IModelCooperationRepository modelCooperationRepository, IDeviceRepository deviceRepository)
        {
            _dbContext = dbContext;
            _modelCooperationRepository = modelCooperationRepository;
            _deviceRepository = deviceRepository;
        }

        public async Task<DeviceDetailsDto> Handle(GetDeviceByIdQuery request, CancellationToken cancellationToken)
        {
            var device = await _deviceRepository.GetDeviceById(request.deviceId, cancellationToken);

            if (device == null)
            {
                throw new DeviceNotFoundException(request.deviceId);
            }

            var deviceDetailsDto = new DeviceDetailsDto
            {
                DeviceId = device.Id,
                DeviceIdentificationNumber = device.IdentificationNumber,
                MeasuredValues = GetMeasuredValues(device),
                SerialNumber = device.SerialNumber,
                Model = device.Model,
                StorageLocation = device.StorageLocation,
                ProductionDate = device.ProductionDate,
                LastCalibrationDate = device.LastCalibrationDate,
                Producer = device.Company != null ? device.Company.Name : null,
                CalibrationPeriodInYears = device.CalibrationPeriodInYears,
                IsCalibrated = CheckIfDeviceIsCalibrated(device.LastCalibrationDate, device.CalibrationPeriodInYears),
                DeviceDocuments = GetDocumentsForDevice(device.Id),
                EstimatedCalibrationDate = device.LastCalibrationDate.HasValue
                    ? device.LastCalibrationDate.Value.AddYears(device.CalibrationPeriodInYears ?? 0)
                    : (DateTime?)null,
                DeviceRelations = device.RelatedDevices,
                //RelatedDevices = device.RelatedDevices
                //    .Where(d => d.DeviceId != device.Id)
                //
                //    .Select(d => new DeviceRelations
                //    {
                //        DeviceId = d.DeviceId,
                //        Device = d.Device,
                //        RelatedDeviceId = d.RelatedDeviceId,
                //        RelatedDevice = d.RelatedDevice
                //    })
                    
            };

            return deviceDetailsDto;
        }

        private bool? CheckIfDeviceIsCalibrated(DateTime? lasCalibrationDate, int? calibrationPeriodInYears) // do oddzielnego helpera
        {
            if (!lasCalibrationDate.HasValue || !calibrationPeriodInYears.HasValue)
            {
                return null;
            }

            return lasCalibrationDate.Value.AddYears(calibrationPeriodInYears.Value) > DateTime.Now;
        }

        private ICollection<DocumentDto>? GetDocumentsForDevice(int deviceId)
        {
            var deviceDocuments = _dbContext.Documents.Where(x => x.DeviceId == deviceId).ToList();
            if (!deviceDocuments.Any())
            {
                return null;
            }

            return GetDocumentsDto(deviceDocuments);

        }
        private ICollection<DocumentDto>? GetDocumentsForModel(int deviceId)
        {
            var modelDocuments = _dbContext.Documents.Where(x => x.DeviceId == deviceId).ToList();
            if (!modelDocuments.Any())
            {
                return null;
            }

            return GetDocumentsDto(modelDocuments);
        }

        private List<DocumentDto> GetDocumentsDto(ICollection<Document> documents)
        {
            var result = new List<DocumentDto>();
            foreach (var document in documents)
            {
                var documentDto = new DocumentDto
                {
                    Id = document.Id,
                    Name = document.Name,
                    Format = document.Format,
                };
                result.Add(documentDto);
            }

            return result;
        }

        private async Task<List<ModelDto>> GetRelatedModelsAsync(int modelId, CancellationToken cancellationToken)
        {
            var cooperatedModels = await _modelCooperationRepository.GetCooperationsForModelByModelId(modelId, cancellationToken);

            var relatedModelsFrom = cooperatedModels.Where(x => x.ModelFromId == modelId)
                .Select(y => new ModelDto
                {
                    Id = y.ModelToId,
                    Name = y.ModelTo.Name,
                    SerialNumber = y.ModelTo.SerialNumber,
                }).ToList();

            var relatedModelsTo = cooperatedModels.Where(x => x.ModelToId == modelId)
                .Select(y => new ModelDto
                {
                    Id = y.ModelFromId,
                    Name = y.ModelFrom.Name,
                    SerialNumber = y.ModelFrom.SerialNumber,
                }).ToList();

            var relatedModels = new List<ModelDto>();

            if (relatedModelsFrom != null)
            {
                relatedModels.AddRange(relatedModelsFrom);
            }

            if (relatedModelsTo != null)
            {
                relatedModels.AddRange(relatedModelsTo);
            }

            return relatedModels;
        }

        private List<MeasuredValueDto> GetMeasuredValues(Device device)
        {
            return device.MeasuredValues?
            .Select(mv => new MeasuredValueDto
            {
                Id = mv.Id,
                PhysicalMagnitudeName = mv.PhysicalMagnitude?.Name,
                PhysicalMagnitudeUnit = mv.PhysicalMagnitude?.Unit,
                MeasuredRanges = mv.MeasuredRanges?
                    .Select(r => new MeasuredRangesDto
                    {
                        Id = r.Id,
                        Range = r.Range,
                        AccuracyInPercent = r.AccuracyInPercet
                    })
                    .ToList()
            })
            .ToList();
        }
    }
}
