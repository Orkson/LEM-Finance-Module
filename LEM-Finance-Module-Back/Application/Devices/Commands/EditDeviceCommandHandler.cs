using Application.Abstractions.Messaging;
using Application.Models;
using Application.Models.Commands;
using AutoMapper;
using Domain.Abstraction;
using Domain.Entities;
using MediatR;

namespace Application.Devices.Commands
{
    internal class EditDeviceCommandHandler : ICommandHandler<EditDeviceCommand, EditedDeviceResponseDto>
    {
        private readonly IDeviceRepository _deviceRepository;
        private readonly IMapper _mapper;
        private readonly ISender _sender;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IModelRepository _modelRepository;

        public EditDeviceCommandHandler(IDeviceRepository deviceRepository, IMapper mapper, ISender sender, IUnitOfWork unitOfWork, IModelRepository modelRepository)
        {
            _deviceRepository = deviceRepository;
            _mapper = mapper;
            _sender = sender;
            _unitOfWork = unitOfWork;
            _modelRepository = modelRepository;
        }

        public async Task<EditedDeviceResponseDto> Handle(EditDeviceCommand request, CancellationToken cancellationToken)
        {
            var device = await _deviceRepository.GetDeviceById(request.deviceId, cancellationToken);

            var newValues = request.newDeviceDto;

            if (device?.IdentificationNumber != newValues.IdentificationNumber)
            {
                device.IdentificationNumber = newValues.IdentificationNumber;
            }
            if (device?.ProductionDate != newValues.ProductionDate)
            {
                device.ProductionDate = newValues.ProductionDate;
            }
            if (device?.CalibrationPeriodInYears != newValues.CalibrationPeriodInYears)
            {
                device.CalibrationPeriodInYears = newValues.CalibrationPeriodInYears;
            }
            if (device?.LastCalibrationDate != newValues.LastCalibrationDate)
            {
                device.LastCalibrationDate = newValues.LastCalibrationDate;
            }
            if (device?.NextCalibrationDate != newValues.NextCalibrationDate)
            {
                device.NextCalibrationDate = newValues.NextCalibrationDate;
            }
            if (device?.IsCalibrated != newValues.IsCalibrated)
            {
                device.IsCalibrated = newValues.IsCalibrated;
            }
            if (device?.IsCalibrationCloseToExpire != newValues.IsCalibrationCloseToExpire)
            {
                device.IsCalibrationCloseToExpire = newValues.IsCalibrationCloseToExpire;
            }
            if (device?.StorageLocation != newValues.StorageLocation)
            {
                device.StorageLocation = newValues.StorageLocation;
            }

            var requested = (newValues.RelatedDeviceIds ?? Enumerable.Empty<int>())
                .Distinct()
                .Where(id => id != request.deviceId)
                .ToHashSet();

            var current = (await _deviceRepository
                .GetDeviceRelationsAsync(request.deviceId, cancellationToken))
                .ToHashSet();

            var toAdd = requested.Except(current.Select(a => a.DeviceId)).ToList();
            var toRemove = current.Select(a => a.DeviceId).Except(requested).ToList();

            //if (device?.RelatedDevices != newValues.RelatedDeviceIds)
            //{
            //    var relatedDevices = await _deviceRepository.GetDevicesByIdsAsync(request.newDeviceDto.RelatedDeviceIds, cancellationToken);
            //
            //    device.RelatedDevices = relatedDevices.Select(s => s.Id).ToList();
            //}

            using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                if (toAdd.Count > 0)
                {
                    var existingRelated = await _deviceRepository.GetDevicesByIdsAsync(toAdd, cancellationToken);
                    var existingIds = existingRelated.Select(d => d.Id).ToHashSet();

                    var links = new List<DeviceRelations>();
                    foreach (var rid in existingIds)
                    {
                        links.Add(new DeviceRelations { DeviceId = request.deviceId, RelatedDeviceId = rid });
                        links.Add(new DeviceRelations { DeviceId = rid, RelatedDeviceId = request.deviceId });
                    }
                    if (links.Count > 0)
                        await _deviceRepository.AddDeviceRelationsAsync(links, cancellationToken);
                }

                if (toRemove.Count > 0)
                {
                    await _deviceRepository.RemoveDeviceRelationsAsync(
                        toRemove.Select(rid => new DeviceRelations { DeviceId = request.deviceId, RelatedDeviceId = rid }),
                        cancellationToken);

                    // drugi kierunek B -> A (jeśli utrzymujesz symetrię)
                    await _deviceRepository.RemoveDeviceRelationsAsync(
                        toRemove.Select(rid => new DeviceRelations { DeviceId = rid, RelatedDeviceId = request.deviceId }),
                        cancellationToken);
                }

                // 5) Zapisz zmiany samego urządzenia
                await _deviceRepository.UpdateDeviceAsync(request.deviceId, device, cancellationToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);

                return new EditedDeviceResponseDto(device.IdentificationNumber, device.Id);

                var newMappedDevice = _mapper.Map<Device>(device);

                if (device.NextCalibrationDate == null && device.CalibrationPeriodInYears != null && device.LastCalibrationDate != null)
                {
                    device.NextCalibrationDate = SetNextCalibrationDate(request.newDeviceDto);
                }

                await _deviceRepository.UpdateDeviceAsync(request.deviceId, newMappedDevice, cancellationToken);
                transaction.Commit();
                var result = new EditedDeviceResponseDto(newMappedDevice.IdentificationNumber, newMappedDevice.Id);
                return result;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception(ex.Message);
            }
        }


        private async Task<int> HandleModelEditionAsync(Model currentModel, ModelDto newModel, ICollection<int>? cooperationsToBeRemoved)
        {
            var modelExists = await _modelRepository.ChcekIfModelExists(newModel.Name, newModel.SerialNumber);

            if (modelExists)
            {
                return await _sender.Send(new EditModelCommand(currentModel.Id, newModel, cooperationsToBeRemoved));
            }
            else
            {
                return await _sender.Send(new CreateModelCommand(newModel));
            }
        }

        private DateTime SetNextCalibrationDate(EditDeviceDto editDeviceDto) => editDeviceDto.LastCalibrationDate.Value.AddYears(editDeviceDto.CalibrationPeriodInYears.Value);
    }
}
