#nullable enable

using System;
using System.Runtime.Serialization;

namespace ToggleableBindings.Exceptions
{
    public class TypeArgumentException : Exception
    {
        private const string DefaultMessage = "An invalid type argument was specified.";
        private const string TypeParamName_Name = "(Type parameter '{0}')";

        public override string Message
        {
            get
            {
                string output = base.Message;
                if (!string.IsNullOrEmpty(TypeParamName))
                    output += " " + string.Format(TypeParamName_Name, TypeParamName);

                return output;
            }
        }

        public string? TypeParamName { get; }

        public TypeArgumentException() : this(DefaultMessage) { }

        public TypeArgumentException(string? message) : base(message) { }

        public TypeArgumentException(string? message, string? typeParamName) : base(message)
        {
            TypeParamName = typeParamName;
        }

        public TypeArgumentException(string? message, Exception innerException) : base(message, innerException) { }

        protected TypeArgumentException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}