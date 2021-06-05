using System;
using System.Collections.Generic;
using System.Linq;

namespace Kaede.Lib.Extensions {
    public static class IEnumerableExtensions {
        public static IEnumerable<T> OrEmptyIfNull<T>(this IEnumerable<T> collection) {
            return collection ?? Enumerable.Empty<T>();
        }
    }
}
