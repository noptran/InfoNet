using System;
using System.Collections.Generic;
using System.Linq;

namespace Infonet.Core.Collections {
	public static class EnumerableExtensions {
		public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> self, Func<TSource, TKey> keySelector, bool nullsLast) {
			return nullsLast
				? self.OrderByDescending(s => keySelector(s) != null).ThenBy(keySelector)
				: self.OrderBy(keySelector);
		}

		public static void CopyTo<TSource>(this IEnumerable<TSource> source, TSource[] array, int arrayIndex) {
			foreach (var each in source)
				array[arrayIndex++] = each;
		}
	}
}