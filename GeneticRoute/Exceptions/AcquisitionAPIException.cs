using System;
using System.Runtime.Serialization;

namespace GeneticRoute
{
    [System.Serializable]
    public class AcquisitionApiException : System.ApplicationException
    {
        public AcquisitionApiException() { }

        public AcquisitionApiException(string message) : base(message) { }

        public AcquisitionApiException(string message, Exception inner) : base(message, inner) { }

        protected AcquisitionApiException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}