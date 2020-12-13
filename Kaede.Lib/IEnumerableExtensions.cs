﻿using System;
using System.Collections.Generic;

namespace Kaede.Lib {
    public static class IEnumerableExtensions {
        public static void ForEach<T>(this IEnumerable<T> sequence, Action<T> action) {
            foreach(T item in sequence)
                action(item);
        }
    }
}
