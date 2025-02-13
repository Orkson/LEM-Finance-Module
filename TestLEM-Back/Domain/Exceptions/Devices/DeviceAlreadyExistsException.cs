using Domain.Exceptions.Base;
namespace Domain.Exceptions.Devices
{
    public sealed class DeviceAlreadyExistsException : AlreadyExistsException
    {
        public DeviceAlreadyExistsException(string identificationNumber)
            : base($"Device with identification number: {identificationNumber} already exists in database")
        {
        }
    }
}
