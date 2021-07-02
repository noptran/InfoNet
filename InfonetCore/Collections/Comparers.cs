using System.Collections;
using System.Collections.Generic;

namespace Infonet.Core.Collections {
	public static class Comparers {
		public static IComparer Adapt<T>(IComparer<T> comparer) {
			return new ComparerAdapter<T>(comparer);
		}

		public static IComparer NullsLast(IComparer comparer) {
			return new NullsLastComparer<object>(comparer);
		}

		public static IComparer NullsLast<T>(IComparer<T> comparer) {
			return new NullsLastComparer<T>(comparer);
		}

		public static IComparer Reverse(IComparer comparer) {
			return new ReverseComparer<object>(comparer);
		}

		public static IComparer Reverse<T>(IComparer<T> comparer) {
			return new ReverseComparer<T>(comparer);
		}
	}

	public static class Comparers<T> {
		public static IComparer<T> Adapt(IComparer comparer) {
			return new ComparerAdapter<T>(comparer);
		}

		public static IComparer<T> NullsLast(IComparer comparer) {
			return new NullsLastComparer<T>(comparer);
		}

		public static IComparer<T> NullsLast(IComparer<T> comparer) {
			return new NullsLastComparer<T>(comparer);
		}

		public static IComparer<T> Reverse(IComparer comparer) {
			return new ReverseComparer<T>(comparer);
		}

		public static IComparer<T> Reverse(IComparer<T> comparer) {
			return new ReverseComparer<T>(comparer);
		}
	}

	internal class ComparerAdapter<T> : IComparer<T>, IComparer {
		private readonly IComparer _inner;
		private readonly IComparer<T> _innerT;

		internal ComparerAdapter(IComparer inner) {
			_inner = inner;
			_innerT = null;
		}

		internal ComparerAdapter(IComparer<T> inner) {
			_inner = null;
			_innerT = inner;
		}

		public virtual int Compare(object a, object b) {
			return _inner?.Compare(a, b) ?? _innerT.Compare((T)a, (T)b);
		}

		public virtual int Compare(T a, T b) {
			return _innerT?.Compare(a, b) ?? _inner.Compare(a, b);
		}
	}

	internal class NullsLastComparer<T> : ComparerAdapter<T> {
		internal NullsLastComparer(IComparer inner) : base(inner) { }

		internal NullsLastComparer(IComparer<T> inner) : base(inner) { }

		public override int Compare(object a, object b) {
			if (ReferenceEquals(a, b))
				return 0;
			if (a == null)
				return 1;
			if (b == null)
				return -1;
			return base.Compare(a, b);
		}

		public override int Compare(T a, T b) {
			if (ReferenceEquals(a, b))
				return 0;
			if (a == null)
				return 1;
			if (b == null)
				return -1;
			return base.Compare(a, b);
		}
	}

	internal class ReverseComparer<T> : ComparerAdapter<T> {
		internal ReverseComparer(IComparer inner) : base(inner) { }

		internal ReverseComparer(IComparer<T> inner) : base(inner) { }

		public override int Compare(object a, object b) {
			return -1 * base.Compare(a, b);
		}

		public override int Compare(T a, T b) {
			return -1 * base.Compare(a, b);
		}
	}
}