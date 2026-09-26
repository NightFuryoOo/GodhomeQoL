// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable enable

namespace System
{
    internal readonly struct Index : IEquatable<Index>
    {
        private readonly int _value;

        public static Index Start => new Index(0);

        public static Index End => new Index(~0);

        public int Value => (_value < 0) ? ~_value : _value;

        public bool IsFromEnd => _value < 0;

        public Index(int value, bool fromEnd = false)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value));

            if (fromEnd)
                _value = ~value;
            else
                _value = value;
        }

        private Index(int value)
        {
            _value = value;
        }

        public static Index FromStart(int value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value));

            return new Index(value);
        }

        public static Index FromEnd(int value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value));

            return new Index(~value);
        }

        public static implicit operator Index(int value) => FromStart(value);

        public int GetOffset(int length)
        {
            int offset = _value;
            if (IsFromEnd)
            {
                offset += length + 1;
            }
            return offset;
        }

        public override bool Equals(object? value) => value is Index index && _value == index._value;

        public bool Equals(Index other) => _value == other._value;

        public override int GetHashCode() => _value;

        public override string ToString()
        {
            if (IsFromEnd)
                return '^' + Value.ToString();
            return ((uint)Value).ToString();
        }
    }
}