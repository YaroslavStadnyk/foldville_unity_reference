using System;
using System.Collections.Generic;
using System.Linq;
using Core.Interfaces;

namespace Core.Extensions
{
    public static class LinqExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> values)
        {
            return values is not ICollection<T> collection || collection.Count == 0;
        }

        public static bool IsNullOrEmpty<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> pairs)
        {
            return pairs is not IDictionary<TKey, TValue> dictionary || dictionary.Count == 0;
        }

        public static TValue FirstOrDefault<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> pairs, TKey key)
        {
            if (pairs is IDictionary<TKey, TValue> dictionary && dictionary.TryGetValue(key, out var value))
            {
                return value;
            }

            return default;
        }

        public static TValue FirstOrDefault<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> pairs, IEnumerable<TKey> keys)
        {
            foreach (var key in keys)
            {
                if (pairs is IDictionary<TKey, TValue> dictionary && dictionary.TryGetValue(key, out var value))
                {
                    return value;
                }
            }

            return default;
        }

        public static T FirstOrDefault<T>(this IEnumerable<T> identities, string id) where T : IIdentity
        {
            foreach (var identity in identities)
            {
                if (identity?.ID == id)
                {
                    return identity;
                }
            }

            return default;
        }

        public static bool Contains<T>(this IEnumerable<T> identities, string id) where T : IIdentity
        {
            foreach (var identity in identities)
            {
                if (identity?.ID == id)
                {
                    return true;
                }
            }

            return false;
        }

        public static void SetupKeys<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IReadOnlyCollection<TKey> keys, TValue value = default)
        {
            foreach (var key in dictionary.Keys.Except(keys).ToArray())
            {
                dictionary.Remove(key);
            }

            foreach (var key in keys.Except(dictionary.Keys))
            {
                dictionary.Add(key, value);
            }
        }

        public static void SetupKeys<TKey>(this ISet<TKey> set, IReadOnlyCollection<TKey> keys)
        {
            foreach (var key in set.Except(keys).ToArray())
            {
                set.Remove(key);
            }

            foreach (var key in keys.Except(set))
            {
                set.Add(key);
            }
        }

        public static T[,] Resize<T>(this T[,] array, int sizeX, int sizeY)
        {
            var boundX = array.GetUpperBound(0);
            var boundY = array.GetUpperBound(0);

            var resizedArray = new T[sizeX, sizeY];
            for (var y = 0; y < sizeY; y++)
            {
                for (var x = 0; x < sizeX; x++)
                {
                    if (x > boundX || y > boundY)
                    {
                        continue;
                    }

                    resizedArray[x, y] = array[x, y];
                }
            }

            return resizedArray;
        }

        public static Dictionary<T, int> CountDuplicates<T>(this IEnumerable<T> values)
        {
            var sum = new Dictionary<T, int>();
            foreach (var value in values)
            {
                if (value != null)
                {
                    sum[value] = sum.FirstOrDefault(value) + 1;
                }
            }

            return sum;
        }

        public static bool AddUniqueValue<T>(this ICollection<T> values) where T : Enum
        {
            foreach (var obj in Enum.GetValues(typeof(T)))
            {
                if (obj is not T value || values.Contains(value))
                {
                    continue;
                }

                values.Add(value);
                return true;
            }

            return false;
        }
    }
}