using System;

namespace KDV.CeusDL.Model.Core
{
    internal class InvalidParameterException : Exception
    {
        public InvalidParameterException()
        {
        }

        public InvalidParameterException(string message) : base(message)
        {
        }

        public InvalidParameterException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}