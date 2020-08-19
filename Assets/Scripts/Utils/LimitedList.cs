using System.Collections.Generic;
using System.Linq;

namespace Utils
{
    public class LimitedList<T>
    {
        public int Count => _list.Count;
        public bool IsFull => _list.Count == _capacity;

        private readonly List<T> _list = new List<T>();
        private readonly int _capacity;
        private readonly bool _autoRemove;

        public LimitedList(int capacity, bool autoRemove = true)
        {
            _capacity = capacity;
            _autoRemove = autoRemove;
        }

        public T this[int index] => _list[index];

        public bool Add(T element)
        {
            if (_list.Count == _capacity)
            {
                if (!_autoRemove)
                    return false;

                _list.RemoveAt(0);
            }

            _list.Add(element);
            return true;
        }

        public dynamic Average()
        {
            T genericDefault = default;
            dynamic avg = _list.Aggregate<T, dynamic>(genericDefault, (current, unknown) => current + unknown);

            avg /= _capacity;
            return avg;
        }

        public dynamic Max() => _list.Max();

        public bool AllGreaterThan(dynamic value) => _list.All(element => (dynamic) element > value);

        public bool AllSmallerThan(dynamic value) => _list.All(element => (dynamic) element < value);

        public bool Equals(List<T> otherList)
        {
            return !_list.Where((t, i) => !t.Equals(otherList[i])).Any();
        }

        public void Clear() => _list.Clear();
    }
}