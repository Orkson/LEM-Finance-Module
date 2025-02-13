using System;
namespace Domain.Exceptions.Base
{
    public abstract class AlreadyExistsException : Exception
    {
        protected AlreadyExistsException(string message)
            : base(message)
        {
        }
    }

}