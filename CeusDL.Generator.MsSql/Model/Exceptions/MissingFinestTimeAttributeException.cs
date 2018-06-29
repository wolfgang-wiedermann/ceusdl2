using System;
using System.Runtime.Serialization;

namespace KDV.CeusDL.Model.Exceptions
{
    [Serializable]
    public class MissingFinestTimeAttributeException : Exception
    {
        public MissingFinestTimeAttributeException()
        {
        }

        public MissingFinestTimeAttributeException(string message) : base(message)
        {
        }

        public MissingFinestTimeAttributeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MissingFinestTimeAttributeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}