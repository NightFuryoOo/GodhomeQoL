using System;
using System.Collections;
using System.Collections.Generic;

namespace ToggleableBindings.Collections
{
    public class ReadOnlyCollection<T> : IReadOnlyCollection<T>
    {
        private readonly ICollection<T> _collection;

        public int Count => _collection.Count;

        public ReadOnlyCollection(ICollection<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            _collection = collection;
        }

        public bool Contains(T item)
        {
            return _collection.Contains(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}