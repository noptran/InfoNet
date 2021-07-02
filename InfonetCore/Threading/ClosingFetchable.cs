namespace Infonet.Core.Threading {
	public class ClosingFetchable<TElement> : IFetchable<TElement> {
		private readonly IFetchable<TElement> _inner;
		private volatile bool _isClosed = false;
		private volatile bool _closeOnEmpty = false;

		public ClosingFetchable(IFetchable<TElement> inner) {
			_inner = inner;
		}

		public void Close() {
			_isClosed = true;
		}

		public void CloseOnEmpty() {
			_closeOnEmpty = true;
		}

		public int Fetch(TElement[] buffer) {
			return Fetch(buffer, 0, buffer?.Length ?? 0);
		}

		public int Fetch(TElement[] buffer, int offset, int count) {
			if (_isClosed)
				return -1;

			int result = _inner.Fetch(buffer, offset, count);
			if (result < count && _closeOnEmpty) {
				(_inner as LoggingFetchable<TElement>)?.LogDebug("closed; no work remaining");
				_isClosed = true;
				if (result == 0)
					return -1;
			}
			return result;
		}

		public override string ToString() {
			return $"{GetType().Name}{{{_inner}}}";
		}
	}
}