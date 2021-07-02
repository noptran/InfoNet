using System;
using System.Collections;
using System.Collections.Generic;

namespace Infonet.Core.Collections {
	public sealed class EmptyEnumerator<TElement> : IEnumerator<TElement> {
		public static readonly IEnumerator<TElement> Instance = new EmptyEnumerator<TElement>();

		private EmptyEnumerator() { }

		public bool MoveNext() {
			return false;
		}

		public TElement Current {
			get { throw new InvalidOperationException("No " + nameof(Current) + " " + nameof(TElement)); }
		}

		object IEnumerator.Current {
			get { return Current; }
		}

		public void Reset() { }

		public void Dispose() { }
	}
}