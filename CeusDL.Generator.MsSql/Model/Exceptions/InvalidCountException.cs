using System;
using System.Runtime.Serialization;

namespace KDV.CeusDL.Model.Exceptions
{
    [Serializable]
    internal class InvalidCountException : Exception
    {
        public InvalidCountException()
        {
        }

        public InvalidCountException(string message) : base(message)
        {
        }

        public InvalidCountException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidCountException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}