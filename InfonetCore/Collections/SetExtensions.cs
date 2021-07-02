using System;
using System.Collections.Generic;

namespace Infonet.Core.Collections {
	public static class SetExtensions {
		public static bool AddRange<T>(this ISet<T> self, IEnumerable<T> items) {
			if (items == null)
				throw new ArgumentNullException(nameof(items));

			bool result = false;
			foreach (var each in items)
				result |= self.Add(each);
			return result;
		}
	}
}