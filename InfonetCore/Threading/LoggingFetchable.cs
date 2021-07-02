using System;
using Infonet.Core.Logging;

namespace Infonet.Core.Threading {
	public class LoggingFetchable<TElement> : IFetchable<TElement> {
		private readonly IFetchable<TElement> _inner;

		public LoggingFetchable(IFetchable<TElement> inner) {
			_inner = inner;
		}

		public void LogDebug(string message, params object[] args) {
			Log.Debug(_inner + " " + message, args);
		}

		public int Fetch(TElement[] buffer) {
			return Fetch(buffer, 0, buffer?.Length ?? 0);
		}

		public int Fetch(TElement[] buffer, int offset, int count) {
			LogDebug($"attempting to fetch {count} {typeof(TElement).Name}s");
			try {
				int result = _inner.Fetch(buffer, offset, count);
				LogDebug($"fetched {result} {typeof(TElement).Name}s");
				return result;
			} catch (Exception e) {
				LogDebug(e.ToString());
				LogDebug($"failed unexpectedly while fetching {typeof(TElement).Name}s: returning zero results");
				return 0;
			}
		}

		public override string ToString() {
			return $"{GetType().Name}{{{_inner}}}";
		}
	}
}