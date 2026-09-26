using System;
using System.Runtime.Serialization;

namespace ToggleableBindings.Exceptions
{
    public class TypeArgumentAbstractException : TypeArgumentException
    {
        private const string DefaultMessage = "The specified type parameter should not reference an abstract type.";

        public TypeArgumentAbstractException() : this(DefaultMessage) { }

        public TypeArgumentAbstractException(string message) : base(message) { }

        public TypeArgumentAbstractException(string message, string typeParamName) : base(message, typeParamName) { }

        public TypeArgumentAbstractException(string message, Exception innerException) : base(message, innerException) { }

        protected TypeArgumentAbstractException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}