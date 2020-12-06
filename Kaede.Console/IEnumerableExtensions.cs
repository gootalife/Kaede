using System;
using System.Collections.Generic;

namespace KaedeConsole {
    public static class IEnumerableExtensions {
        public static void ForEach<T>(this IEnumerable<T> sequence, Action<T> action) {
            foreach(T item in sequence)
                action(item);
        }
    }
}
