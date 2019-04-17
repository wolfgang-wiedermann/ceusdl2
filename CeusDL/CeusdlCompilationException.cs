using System;
using System.Runtime.Serialization;
using KDV.CeusDL.Validator;

namespace CeusDL2
{
    [Serializable]
    internal class CeusdlCompilationException : Exception
    {
        private ValidationResultRepository validationResult;

        public CeusdlCompilationException()
        {
        }

        public CeusdlCompilationException(string message) : base(message)
        {
        }

        public CeusdlCompilationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CeusdlCompilationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public CeusdlCompilationException(ValidationResultRepository validationResult) : base(validationResult.ToString())
        {
            this.validationResult = validationResult;
        }

        public ValidationResultRepository ValidationResult => ValidationResult;
    }
}