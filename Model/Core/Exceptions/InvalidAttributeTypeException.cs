using System;

namespace KDV.CeusDL.Model.Core
{
    internal class InvalidAttributeTypeException : Exception
    {
        public InvalidAttributeTypeException()
        {
        }

        public InvalidAttributeTypeException(string message) : base(message)
        {
        }

        public InvalidAttributeTypeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}