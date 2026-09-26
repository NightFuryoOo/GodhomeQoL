#nullable enable

using System;
using System.Diagnostics.CodeAnalysis;

namespace ToggleableBindings.Utility
{
    public partial class TryResult
    {
        [MemberNotNullWhen(false, nameof(Exception))]
        public bool IsSuccess => Exception == null;

        public Exception? Exception { get; }

        protected TryResult(Exception? exception = null)
        {
            Exception = exception;
        }

        public void ThrowIfUnsuccessful()
        {
            if (!IsSuccess)
                throw Exception;
        }

        public override string ToString()
        {
            string output = IsSuccess.ToString();
            if (!IsSuccess)
                output += ": " + Exception.ToString();
            return output;
        }
    }

    public partial class TryResult
    {
        public static TryResult Success { get; } = new TryResult();

        public static implicit operator TryResult(Exception value) => new(value);

        public static implicit operator bool(TryResult value) => value.IsSuccess;
    }
}