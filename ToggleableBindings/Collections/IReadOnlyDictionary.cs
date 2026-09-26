using System.Collections.Generic;

namespace ToggleableBindings.Collections
{
    public interface IReadOnlyDictionary<TKey, TValue> : IReadOnlyCollection<KeyValuePair<TKey, TValue>>
    {
        TValue this[TKey key] { get; }

        IReadOnlyCollection<TKey> Keys { get; }

        IReadOnlyCollection<TValue> Values { get; }

        bool ContainsKey(TKey key);

        bool TryGetValue(TKey key, out TValue value);
    }
}