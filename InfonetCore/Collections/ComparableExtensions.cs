using System;
using System.Collections.Generic;

namespace Infonet.Core.Collections {
	public static class ComparableExtensions {
		/** Equivalent to Equals() but succeeds even when 'this' is null. **/
		public static bool SafeEquals<T>(this T self, T other) {
			return EqualityComparer<T>.Default.Equals(self, other);
		}

		/** Equivalent to CompareTo(T) but handles nulls and sorts them last. **/
		public static int SafeCompareTo<T>(this T self, T other) where T : IComparable<T> {
			if (self == null && other == null)
				return 0;
			if (self == null)
				return 1;
			if (other == null)
				return -1;
			return self.CompareTo(other);
		}

		/** Equivalent to CompareTo(object) but handles nulls and sorts them last. **/
		public static int SafeCompareTo(this IComparable self, object other) {
			if (self == null && other == null)
				return 0;
			if (self == null)
				return 1;
			if (other == null)
				return -1;
			return self.CompareTo(other);
		}
	}
}