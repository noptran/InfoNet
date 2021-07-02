using System;
using System.Collections.Generic;

namespace Infonet.Core.Collections {
	public class ShadowCollection<TElement> : CollectionDecorator<TElement> {
		private readonly Func<ICollection<TElement>> _collectionAccessor;

		public ShadowCollection(Func<ICollection<TElement>> collectionAccessor) {
			_collectionAccessor = collectionAccessor;
		}

		protected override ICollection<TElement> Inner {
			get { return _collectionAccessor.Invoke(); }
		}
	}
}