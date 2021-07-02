using System.Collections.Generic;

namespace Infonet.Core.Collections {
	public class EnumeratorWrapper<TElement> : EnumeratorDecorator<TElement, TElement> {
		private readonly IEnumerator<TElement> _inner;

		public EnumeratorWrapper(IEnumerator<TElement> inner) {
			_inner = inner;
		}

		protected override IEnumerator<TElement> Inner {
			get { return _inner; }
		}

		public override TElement Current {
			get { return Inner.Current; }
		}
	}
}