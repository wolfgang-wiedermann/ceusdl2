using System;
using System.Runtime.Serialization;

namespace KDV.CeusDL.Model.Core
{
    [Serializable]
    internal class InvalidAttributeNameException : Exception
    {
        public InvalidAttributeNameException()
        {
        }

        public InvalidAttributeNameException(string message) : base(message)
        {
        }

        public InvalidAttributeNameException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidAttributeNameException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}