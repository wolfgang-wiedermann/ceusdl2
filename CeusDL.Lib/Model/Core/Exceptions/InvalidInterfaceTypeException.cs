using System;

namespace KDV.CeusDL.Model.Core
{
    internal class InvalidInterfaceTypeException : Exception
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