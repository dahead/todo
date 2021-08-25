using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace todo
{
    public static class Ensure
    {
        public static void NotNull<T>(T item, [NotNull]string name)
            where T : class
        {
            if (item == null)
            {
                throw new ArgumentNullException(name);
            }
        }
    }
    
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            if (enumerable != null && action != null)
            {
                foreach (var item in enumerable)
                {
                    action(item);
                }
            }
        }
    }
}