using System;

namespace KDV.CeusDL.Parser
{
    internal class InvalidCharException : Exception
    {
        public InvalidCharException()
        {
        }

        public InvalidCharException(string message) : base(message)
        {
        }

        public InvalidCharException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}