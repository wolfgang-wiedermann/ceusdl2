using System;

namespace KDV.CeusDL.Parser.Exceptions
{
    internal class InvalidTokenException : Exception
    {
        public InvalidTokenException()
        {
        }

        public InvalidTokenException(string message) : base(message)
        {
        }

        public InvalidTokenException(string message, ParsableData data) : base($"{message} in Line {data.Line} Column {data.Column}")
        {
        }

        public InvalidTokenException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}