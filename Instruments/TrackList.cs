using CashApp.UIElements;
using System.Collections.Generic;
using System.Linq;

namespace CashApp.Instruments
{
    public class TrackList<T> : List<T>
    {
        // EVENTS

        public delegate void ChangedHandler(object sender, T[] items, int startIndex);

        public event ChangedHandler PreviewAddingItems;
        public event ChangedHandler PreviewRemovedItems;

        public event ChangedHandler AddingItems;
        public event ChangedHandler RemovedItems;


        // METHODS

        public new void Add(T item)
        {
            T[] items = { item };

            PreviewAddingItems?.Invoke(this, items, Count);
            base.Add(item);
            AddingItems?.Invoke(this, items, Count - 1);
        }

        public new void AddRange(IEnumerable<T> items)
        {
            if (items.Count() == 0) return;

            PreviewAddingItems?.Invoke(this, items.ToArray(), Count);
            base.AddRange(items);
            AddingItems?.Invoke(this, items.ToArray(), Count - items.Count());
        }

        public void Add(params T[] items) => AddRange(items);


        public new void Insert(int index, T item)
        {
            T[] items = { item };

            PreviewAddingItems?.Invoke(this, items, index);
            base.Insert(index, item);
            AddingItems?.Invoke(this, items, index);
        }

        public new void InsertRange(int index, IEnumerable<T> items)
        {
            if (items.Count() == 0) return;

            PreviewAddingItems?.Invoke(this, items.ToArray(), index);
            base.AddRange(items);
            AddingItems?.Invoke(this, items.ToArray(), index);
        }

        public void Insert(int index, params T[] items) => InsertRange(index, items);


        public new bool Remove(T item)
        {
            T[] items = { item };
            int index = IndexOf(item);

            PreviewRemovedItems?.Invoke(this, items, index);
            bool isRemove = base.Remove(item);
            RemovedItems?.Invoke(this, items, index);

            return isRemove;
        }

        public new void RemoveRange(int index, int count)
        {
            T[] items = GetRange(index, count).ToArray();

            PreviewRemovedItems?.Invoke(this, items, index);
            base.RemoveRange(index, count);
            RemovedItems?.Invoke(this, items, index);
        }

        public new void RemoveAt(int index)
        {
            T[] items = { this[index] };

            PreviewRemovedItems?.Invoke(this, items, index);
            base.RemoveAt(index);
            RemovedItems?.Invoke(this, items, index);
        }


        public new void Clear()
        {
            T[] items = GetRange(0, Count).ToArray();

            PreviewRemovedItems?.Invoke(this, items, 0);
            base.Clear();
            RemovedItems?.Invoke(this, items, 0);
        }
    }
}
