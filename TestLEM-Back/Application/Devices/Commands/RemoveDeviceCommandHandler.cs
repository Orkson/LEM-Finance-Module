using Application.Abstractions.Messaging;
using Domain.Abstraction;

namespace Application.Devices.Commands
{
    internal class RemoveDeviceCommandHandler : ICommandHandler<RemoveDeviceCommand, bool>
    {
        private IDeviceRepository _deviceRepository;
        private IModelRepository _modelRepository;
        private ICompanyRepository _companyRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveDeviceCommandHandler(IDeviceRepository deviceRepository, IUnitOfWork unitOfWork, IModelRepository modelRepository, ICompanyRepository companyRepository)
        {
            _deviceRepository = deviceRepository;
            _unitOfWork = unitOfWork;
            _modelRepository = modelRepository;
            _companyRepository = companyRepository;
        }

        public async Task<bool> Handle(RemoveDeviceCommand request, CancellationToken cancellationToken)
        {
            var deviceToBeRemoved = await _deviceRepository.GetDeviceById(request.deviceId, cancellationToken);
            var deviceModel = deviceToBeRemoved?.Model;
            var modelWithRelatedDevices = await _modelRepository.GetModelWithRelatedDevices(deviceModel.Id, cancellationToken);

            using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                if (modelWithRelatedDevices.Devices.Count == 1)
                {
                    if (modelWithRelatedDevices.Company?.Models.Count == 1)
                    {
                        await _companyRepository.RemoveCompanyById(modelWithRelatedDevices.Company.Id, cancellationToken);
                    }
                    await _modelRepository.RemoveModelById(deviceModel.Id, cancellationToken);
                } else
                {
                    await _deviceRepository.RemoveDeviceById(request.deviceId, cancellationToken);
                }
                
                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception(ex.Message);
            }
        }
    }
}
