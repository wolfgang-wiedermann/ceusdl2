using System;

namespace KDV.CeusDL.Parser.Exceptions
{
    internal class InvalidCharException : Exception
    {
        
        public InvalidCharException()
        {
        }

        public InvalidCharException(string message) : base(message)
        {
        }

        public InvalidCharException(string message, ParsableData data) : base($"{message} : {data.GetPosTextForException()}")
        {
        }        

        public InvalidCharException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}