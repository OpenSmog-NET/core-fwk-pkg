using System;
using System.Collections.Generic;

namespace OS.Core.Queries
{
    internal static class InternalExtensions
    {
        public static void ForEach<T>(this IList<T> collection, Action<T> action)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            foreach (var item in collection)
            {
                action(item);
            }
        }

        public static bool IsNullable(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}