#nullable enable

using System;
using System.Diagnostics.CodeAnalysis;

namespace ToggleableBindings.Utility
{
    public partial class TryResult<T> : TryResult
    {
        [AllowNull]
        private readonly T _value;

        public T Value => (IsSuccess) ? _value : throw new InvalidOperationException($"Cannot get the value of an unsuccessful {nameof(TryResult<T>)}.");

        protected TryResult([AllowNull] T value, Exception? exception = null) : base(exception)
        {
            _value = value;
        }

        public T GetValueOrThrow()
        {
            ThrowIfUnsuccessful();
            return _value;
        }

        public bool TryGetValue(out T? result, T? defaultValue = default)
        {
            result = (IsSuccess) ? _value : defaultValue;
            return IsSuccess;
        }
    }

    public partial class TryResult<T>
    {
        public static implicit operator TryResult<T>([AllowNull] T value) => new(value);

        public static implicit operator TryResult<T>(Exception value) => new(default, value);
    }
}