using System;
using System.Collections;
using System.Collections.Generic;

namespace ToggleableBindings.Collections
{
    public class ReadOnlyList<T> : IReadOnlyList<T>
    {
        private readonly IList<T> _list;

        public int Count => _list.Count;

        public T this[int index] => _list[index];

        public ReadOnlyList(IList<T> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            _list = list;
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}