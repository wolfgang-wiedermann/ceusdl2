using System;

namespace KDV.CeusDL.Model.Core
{
    public class InvalidDataTypeException : Exception
    {
        public InvalidDataTypeException()
        {
        }

        public InvalidDataTypeException(string message) : base(message)
        {
        }

        public InvalidDataTypeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}