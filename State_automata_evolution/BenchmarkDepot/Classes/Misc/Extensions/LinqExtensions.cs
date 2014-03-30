using System;
using System.Collections.Generic;

namespace BenchmarkDepot.Classes.Misc
{

    /// <summary>
    /// Extension methods for linq queries
    /// </summary>
    public static class LinqExtensions
    {

        /// <summary>
        /// Returns the object with the highest value of a given comparable property of an enumerable sequence
        /// </summary>
        public static T MaxBy<T, K>(this IEnumerable<T> source, Func<T, K> selector)
            where K : IComparable<K>
        {
            if (source == null) throw new ArgumentNullException("source");

            using (var iter = source.GetEnumerator())
            {
                if (!iter.MoveNext()) throw new InvalidOperationException("Sequence contains no elements!");

                var maxVal = iter.Current;
                var maxKey = selector(maxVal);
                while (iter.MoveNext())
                {
                    var currentKey = selector(iter.Current);
                    if (currentKey.CompareTo(maxKey) <= 0) continue;
                    maxKey = currentKey;
                    maxVal = iter.Current;
                }

                return maxVal;
            }
        }

        /// <summary>
        /// Returns the object with the lowest value of a given comparable property of an enumerable sequence
        /// </summary>
        public static T MinBy<T, K>(this IEnumerable<T> source, Func<T, K> selector)
            where K : IComparable<K>
        {
            if (source == null) throw new ArgumentNullException("source");

            using (var iter = source.GetEnumerator())
            {
                if (!iter.MoveNext()) throw new InvalidOperationException("Sequence contains no elements!");

                var minVal = iter.Current;
                var minKey = selector(minVal);
                while (iter.MoveNext())
                {
                    var currentKey = selector(iter.Current);
                    if (currentKey.CompareTo(minKey) >= 0) continue;
                    minKey = currentKey;
                    minVal = iter.Current;
                }

                return minVal;
            }
        }

    }

}
