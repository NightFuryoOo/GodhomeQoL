using System.Collections.Generic;

namespace ToggleableBindings.Collections
{
    public interface IReadOnlyCollection<T> : IEnumerable<T>
    {
        int Count { get; }

        bool Contains(T item);
    }
}