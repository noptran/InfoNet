using System.Collections;
using System.Collections.Generic;

namespace Infonet.Core.Collections {
	public abstract class EnumeratorDecorator<TInner, TElement> : IEnumerator<TElement> {
		protected abstract IEnumerator<TInner> Inner { get; }

		//KMS DO implement with template method?
		//KMS DO if so, could hang on to last current value
		public abstract TElement Current { get; }

		object IEnumerator.Current {
			get { return Current; }
		}

		public bool MoveNext() {
			return Inner.MoveNext();
		}

		public void Reset() {
			Inner.Reset();
		}

		public void Dispose() {
			Inner.Dispose();
		}
	}
}