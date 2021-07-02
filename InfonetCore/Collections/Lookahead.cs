using System.Collections.Generic;

namespace Infonet.Core.Collections {
	public static class Lookahead {
		public static LookaheadEnumerator<TElement> New<TElement>(IEnumerable<TElement> enumerable) {
			return new LookaheadEnumerator<TElement>(enumerable);
		}
	}
}