using System;
using System.Runtime.Serialization;

namespace GeneticRoute
{
    [Serializable]
    public class FillingDictionaryException : ApplicationException
    {
        public FillingDictionaryException() { }

        public FillingDictionaryException(string message) : base(message) { }

        public FillingDictionaryException(string message, Exception inner) : base(message, inner) { }

        protected FillingDictionaryException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}