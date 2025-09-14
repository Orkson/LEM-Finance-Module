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
            using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                await _deviceRepository.RemoveDeviceById(request.deviceId, cancellationToken);

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
