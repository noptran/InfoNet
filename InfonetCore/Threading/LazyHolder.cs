using System;

namespace Infonet.Core.Threading {
	/** Threadsafe lazy-initialized value holder. **/
	public class LazyHolder<T> {
		private readonly Func<T> _initializer;
		private volatile bool _initialized = false;
		private readonly object _lock = new object();
		private T _value = default(T);

		public LazyHolder(Func<T> initializer) {
			_initializer = initializer;
		}

		// ReSharper disable once UnusedMember.Global
		public bool IsInitialized {
			get { return _initialized; }
		}

		public T Value {
			get {
				if (_initialized)
					return _value;

				lock (_lock) {
					if (_initialized)
						return _value;

					_value = _initializer.Invoke();
					_initialized = true;
					return _value;
				}
			}
		}

		public void Reset() {
			lock (_lock) {
				if (!_initialized)
					return;

				_initialized = false;
				_value = default(T);
			}
		}
	}
}