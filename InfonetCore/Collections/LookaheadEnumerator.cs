using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Infonet.Core.Collections {
	[SuppressMessage("ReSharper", "AssignmentInConditionalExpression")]
	public class LookaheadEnumerator<TElement> : IEnumerator<TElement> {
		private readonly IEnumerator<TElement> _inner;
		private bool _hasLooked = false;
		private bool _hasNext = false;
		private TElement _next;
		private bool _hasCurrent = false;
		private TElement _current;
		private int _index = -1;

		public LookaheadEnumerator(IEnumerable<TElement> elements) {
			_inner = elements.GetEnumerator();
		}

		public bool HasCurrent {
			get { return _hasCurrent; }
		}

		public TElement Current {
			get {
				if (!_hasCurrent)
					throw new InvalidOperationException("No " + nameof(Current) + " " + nameof(TElement));
				return _current;
			}
		}

		object IEnumerator.Current {
			get { return Current; }
		}

		public bool HasNext {
			get {
				if (!_hasLooked) {
					if (_hasNext = _inner.MoveNext())
						_next = _inner.Current;
					_hasLooked = true;
				}
				return _hasNext;
			}
		}

		public TElement Next {
			get {
				if (!HasNext)
					throw new InvalidOperationException("No " + nameof(Next) + " " + nameof(TElement));
				return _next;
			}
		}

		public bool MoveNext() {
			_hasCurrent = HasNext;
			_current = _next;
			if (_hasCurrent) {
				_index++;
				_hasNext = _inner.MoveNext();
				_next = _hasNext ? _inner.Current : default(TElement);
			}
			return _hasCurrent;
		}

		public bool IsFirst {
			get {
				if (!_hasCurrent)
					throw new InvalidOperationException(nameof(IsFirst) + " is undefined when there is no " + nameof(Current) + " " + nameof(TElement));
				return _index == 0;
			}
		}

		public bool IsLast {
			get {
				if (!_hasCurrent)
					throw new InvalidOperationException(nameof(IsLast) + " is undefined when there is no " + nameof(Current) + " " + nameof(TElement));
				return !_hasNext;
			}
		}

		public int Index {
			get {
				if (!_hasCurrent)
					throw new InvalidOperationException(nameof(Index) + " is undefined when there is no " + nameof(Current) + " " + nameof(TElement));
				return _index;
			}
		}

		public int Count {
			get {
				if (HasNext)
					throw new InvalidOperationException(nameof(Count) + " is undefined while " + nameof(TElement) + "s remain");
				return _index + 1;
			}
		}

		public void Reset() {
			_inner.Reset();
			_hasLooked = _hasNext = _hasCurrent = false;
			_next = _current = default(TElement);
			_index = -1;
		}

		public void Dispose() {
			_inner.Dispose();
		}
	}
}