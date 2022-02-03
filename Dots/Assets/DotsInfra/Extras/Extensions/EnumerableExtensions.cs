using System;
using System.Collections.Generic;

namespace Dots.Extras
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (T tItem in items)
            {
                action(tItem);
            }
        }

        public static void ForEach<TItem>(this IEnumerable<TItem> items, Action<TItem, int> action)
        {
            var i = 0;
            foreach (var item in items)
            {
                action(item, i++);
            }
        }
    }
}