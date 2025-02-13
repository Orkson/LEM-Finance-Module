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
            var deviceExists = await _deviceRepository.CheckIfDeviceExists(identificationNumber, cancellationToken);

            if (deviceExists)
            {
                throw new DeviceAlreadyExistsException(identificationNumber);
            }
            var device = _mapper.Map<Device>(request.AddDeviceDto);

            if (device.NextCalibrationDate == null && device.CalibrationPeriodInYears != null && device.LastCalibrationDate != null)
            {
                device.NextCalibrationDate = SetNextCalibrationDate(request.AddDeviceDto);
            }

            var model = request.AddDeviceDto.Model;
            var createdDeviceResponse = new CreatedDeviceResponseDto(identificationNumber, null, null);

            using (var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken))
            {
                try
                {
                    var modelId = await _sender.Send(
                    new CreateModelCommand(model),
                    cancellationToken);

                    device.ModelId = modelId;

                    var deviceId = await _deviceRepository.AddDevice(device, cancellationToken);

                    createdDeviceResponse.ModelId = modelId;
                    createdDeviceResponse.DeviceId = deviceId;

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
