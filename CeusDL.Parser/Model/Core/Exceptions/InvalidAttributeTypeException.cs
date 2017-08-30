using System;

namespace KDV.CeusDL.Model.Core
{
    public class InvalidAttributeTypeException : Exception
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