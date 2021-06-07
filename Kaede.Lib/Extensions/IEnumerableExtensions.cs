using System;
using System.Collections.Generic;
using System.Linq;

namespace Kaede.Lib.Extensions {
    public static class IEnumerableExtensions {
        /// <summary>
        /// 引数が空コレクションのときは長さ0のコレクションを返す
        /// </summary>
        /// <typeparam name="T">コレクションの型</typeparam>
        /// <param name="collection">コレクション</param>
        /// <returns>コレクション</returns>
        public static IEnumerable<T> OrEmptyIfNull<T>(this IEnumerable<T> collection) {
            return collection ?? Enumerable.Empty<T>();
        }

        public static void ForEach<T>(this IEnumerable<T> sequence, Action<T> action) {
            foreach(T item in sequence)
                action(item);
            }
        }
}
