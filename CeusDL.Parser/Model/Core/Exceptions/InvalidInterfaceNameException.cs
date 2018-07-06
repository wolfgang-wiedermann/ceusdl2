using System;
using System.Runtime.Serialization;

namespace KDV.CeusDL.Model.Core
{
    [Serializable]
    internal class InvalidInterfaceNameException : Exception
    {
        public InvalidInterfaceNameException()
        {
        }

        public InvalidInterfaceNameException(string message) : base(message)
        {
        }

        public InvalidInterfaceNameException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidInterfaceNameException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}