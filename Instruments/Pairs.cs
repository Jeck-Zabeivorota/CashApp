using System.Collections.Generic;
using System.Linq;

namespace CashApp.Instruments
{
    public class Pairs<T1, T2>
    {
        // FIELDS

        readonly List<T1> _items1 = new List<T1>();
        readonly List<T2> _items2 = new List<T2>();

        public T1[] Items1 => _items1.ToArray();
        public T2[] Items2 => _items2.ToArray();

        public int Count => _items1.Count();


        public T2 this[T1 key]
        {
            get => _items2[FindIndex(key)];
            set
            {
                int index = FindIndex(key);

                if (index == -1)
                    Add(key, value);
                else
                    _items2[index] = value;
            }
        }

        public T1 this[T2 key]
        {
            get => _items1[FindIndex(key)];
            set
            {
                int index = FindIndex(key);

                if (index == -1)
                    Add(value, key);
                else
                    _items1[index] = value;
            }
        }


        // METHODS

        int FindIndex(T1 key) => _items1.FindIndex(item => item.Equals(key));
        int FindIndex(T2 key) => _items2.FindIndex(item => item.Equals(key));


        public void Add(T1 item1, T2 item2)
        {
            _items1.Add(item1);
            _items2.Add(item2);
        }

        public void Insert(int index, T1 item1, T2 item2)
        {
            _items1.Insert(index, item1);
            _items2.Insert(index, item2);
        }

        public bool Contains(T1 item) => _items1.Contains(item);
        public bool Contains(T2 item) => _items2.Contains(item);

        public void RemoveAt(int index)
        {
            _items1.RemoveAt(index);
            _items2.RemoveAt(index);
        }
        public void Remove(T2 item) => RemoveAt(FindIndex(item));
        public void Remove(T1 item) => RemoveAt(FindIndex(item));

        public void RemoveRange(int index, int count)
        {
            _items1.RemoveRange(index, count);
            _items2.RemoveRange(index, count);
        }

        public void Clear()
        {
            _items1.Clear();
            _items2.Clear();
        }
    }
}
