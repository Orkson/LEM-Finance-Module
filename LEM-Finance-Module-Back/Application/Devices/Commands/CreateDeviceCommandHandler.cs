using Application.Abstractions.Messaging;
using Application.Models;
using Application.Models.Commands;
using AutoMapper;
using Domain.Abstraction;
using Domain.Entities;
using Domain.Exceptions.Devices;
using MediatR;

namespace Application.Devices.Commands
{
    internal class CreateDeviceCommandHandler : ICommandHandler<CreateDeviceCommand, CreatedDeviceResponseDto>
    {
        private readonly IDeviceRepository _deviceRepository;
        private readonly IMapper _mapper;
        private readonly ISender _sender;
        private readonly IUnitOfWork _unitOfWork;

        public CreateDeviceCommandHandler(IDeviceRepository deviceRepository, IMapper mapper, ISender sender, IUnitOfWork unitOfWork)
        {
            _deviceRepository = deviceRepository;
            _mapper = mapper;
            _sender = sender;
            _unitOfWork = unitOfWork;
        }

        public async Task<CreatedDeviceResponseDto> Handle(CreateDeviceCommand request, CancellationToken cancellationToken)
        {
            var identificationNumber = request.AddDeviceDto.IdentificationNumber;
            
            var device = _mapper.Map<Device>(request.AddDeviceDto);

            if (device.NextCalibrationDate == null && device.CalibrationPeriodInYears != null && device.LastCalibrationDate != null)
            {
                device.NextCalibrationDate = SetNextCalibrationDate(request.AddDeviceDto);
            }

            var createdDeviceResponse = new CreatedDeviceResponseDto(identificationNumber, null);

            using (var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken))
            {
                try
                {
                    var deviceId = await _deviceRepository.AddDevice(device, cancellationToken);

                    createdDeviceResponse.DeviceId = deviceId;

                    var relatedIds = request.AddDeviceDto.RelatedDeviceIds?
                        .Distinct()
                        .Where(id => id != deviceId)
                        .ToList() ?? new List<int>();

                    if (relatedIds.Count > 0)
                    {
                        var existingRelated = await _deviceRepository.GetDevicesByIdsAsync(relatedIds, cancellationToken);
                        var existingRelatedIds = existingRelated.Select(d => d.Id).ToHashSet();

                        var alreadyLinked = await _deviceRepository.GetDeviceRelationsAsync(deviceId, cancellationToken);

                        var already = alreadyLinked.Select(r => (r.DeviceId, r.RelatedDeviceId)).ToHashSet();

                        var toInsert = new List<DeviceRelations>();

                        foreach (var rdId in existingRelatedIds)
                        {
                            if (!already.Contains((deviceId, rdId)))
                                toInsert.Add(new DeviceRelations { DeviceId = deviceId, RelatedDeviceId = rdId });

                            if (!already.Contains((rdId, deviceId)))
                                toInsert.Add(new DeviceRelations { DeviceId = rdId, RelatedDeviceId = deviceId });
                        }

                        if (toInsert.Count > 0)
                        {
                            await _deviceRepository.AddDeviceRelationsAsync(toInsert, cancellationToken);
                            await _unitOfWork.SaveChangesAsync(cancellationToken);
                        }
                    }

                    await _unitOfWork.CommitAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    _unitOfWork.Rollback();
                    throw new Exception(ex.Message);
                }
            }

            return createdDeviceResponse;
        }

        private DateTime SetNextCalibrationDate(AddDeviceDto addDeviceDto) => addDeviceDto.LastCalibrationDate.Value.AddYears(addDeviceDto.CalibrationPeriodInYears.Value);
    }
}
