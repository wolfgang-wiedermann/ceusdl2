using System;

namespace KDV.CeusDL.Model.Core
{
    public class InvalidInterfaceTypeException : Exception
    {
        public InvalidInterfaceTypeException()
        {
        }

        public InvalidInterfaceTypeException(string message) : base(message)
        {
        }

        public InvalidInterfaceTypeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}