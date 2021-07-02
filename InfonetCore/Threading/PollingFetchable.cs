using System;
using System.Threading;
using Infonet.Core.IO;

namespace Infonet.Core.Threading {
	public class PollingFetchable<TElement> : IFetchable<TElement>, IDisposable {
		// ReSharper disable once MemberCanBePrivate.Global
		public const int DEFAULT_RETRY_AFTER_MILLISECONDS = 30000;

		private readonly IFetchable<TElement> _inner;
		private readonly object _lock = new object();
		private volatile int _retryAfterMilliseconds;
		private readonly ManualResetEvent _retryWaitHandle = new ManualResetEvent(false);
		private bool _disposed = false;

		private volatile int _need = 0;

		private DateTime _lastEmpty = DateTime.MinValue;
		private TElement[] _buffer = new TElement[0];
		private int _offset = 0;
		private int _available = 0;

		public PollingFetchable(IFetchable<TElement> fetchable, int? retryMilliseconds) {
			_inner = fetchable;
			_retryAfterMilliseconds = retryMilliseconds ?? DEFAULT_RETRY_AFTER_MILLISECONDS;
		}

		// ReSharper disable once UnusedMember.Global
		public int RetryAfterMilliseconds {
			get { return _retryAfterMilliseconds; }
			set { _retryAfterMilliseconds = value; }
		}

		public void WaitNoMore() {
			_retryAfterMilliseconds = 0;
			_retryWaitHandle.Set();
		}

		/**
		 * Polls inner IFetchable until non-zero TElements are returned.
		**/
		public int Fetch(TElement[] buffer) {
			return Fetch(buffer, 0, buffer?.Length ?? 0);
		}

		/**
		 * Polls inner IFetchable until non-zero TElements are returned.
		**/
		public int Fetch(TElement[] buffer, int offset, int count) {
			BufferHelper.AssertBufferIsValid(buffer, offset, count);

			_need += count;
			lock (_lock) {
				while (_available == 0) {
					var now = DateTime.Now;
					var untilNextPoll = TimeSpan.FromMilliseconds(_retryAfterMilliseconds) - (now - _lastEmpty);
					if (untilNextPoll.Ticks > 0) {
						_retryWaitHandle.WaitOne(untilNextPoll);
						now = DateTime.Now;
					}

					int toFetch = _need; //take a snapshot in case _need increases during inner fetch
					if (_buffer.Length < toFetch)
						_buffer = new TElement[toFetch];
					_available = _inner.Fetch(_buffer, _offset = 0, toFetch);
					if (_available < toFetch)
						_lastEmpty = now;
				}
				if (_available < 0)
					return _available;

				int toCopy = Math.Min(_available, count);
				Array.Copy(_buffer, _offset, buffer, offset, toCopy);
				_offset += toCopy;
				_available -= toCopy;
				_need -= count;
				return toCopy;
			}
		}

		protected virtual void Dispose(bool disposing) {
			if (!_disposed) {
				if (disposing)
					_retryWaitHandle.Dispose();
				_disposed = true;
			}
		}

		public void Dispose() {
			Dispose(true);
		}
	}
}