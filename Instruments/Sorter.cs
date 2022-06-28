using System;
using System.Collections.Generic;
using System.Linq;

namespace CashApp.Instruments
{
    public abstract class Sorter
    {
        public static IEnumerable<T> Sort<T>(IEnumerable<T> collection, Func<T, T, T> priority)
        {
            List<T> sortCollection = new List<T>(),
                    cloneCollection = new List<T>();

            cloneCollection.AddRange(collection);


            while (cloneCollection.Count > 0)
            {
                T priorityValue = cloneCollection[0];

                foreach (T item in cloneCollection)
                {
                    T value = priority(priorityValue, item);

                    if (!value.Equals(priorityValue) && !value.Equals(item)) throw new InvalidCastException($"\"priority\" returned wrong value");

                    priorityValue = value;
                }

                sortCollection.Add(priorityValue);
                cloneCollection.Remove(priorityValue);
            }

            return sortCollection;
        }

        public static Dictionary<TKey, TValue> Sort<TKey, TValue>(Dictionary<TKey, TValue> dictionary, Func<TKey, TKey, TKey> priority)
        {
            Dictionary<TKey, TValue> sortDictionary = new Dictionary<TKey, TValue>(),
                                     cloneDictionary = new Dictionary<TKey, TValue>();

            foreach (TKey key in dictionary.Keys)
                cloneDictionary[key] = dictionary[key];

            List<TKey> keys = cloneDictionary.Keys.ToList();


            while (cloneDictionary.Count > 0)
            {
                TKey priorityKey = keys[0];

                foreach (TKey key in cloneDictionary.Keys)
                {
                    TKey value = priority(priorityKey, key);

                    if (!value.Equals(priorityKey) && !value.Equals(key)) throw new InvalidCastException($"\"priority\" returned wrong value");

                    priorityKey = value;
                }

                sortDictionary[priorityKey] = cloneDictionary[priorityKey];
                keys.Remove(priorityKey);
                cloneDictionary.Remove(priorityKey);
            }

            return sortDictionary;
        }
    }
}
