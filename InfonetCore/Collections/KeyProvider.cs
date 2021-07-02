using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Infonet.Core.Collections {
	public class KeyProvider<TElement> where TElement : class {
		private readonly Func<TElement, string>[] _keyAccessors;
		private readonly ConditionalWeakTable<TElement, List<Key>> _keys = new ConditionalWeakTable<TElement, List<Key>>();
		private readonly Dictionary<string, int> _nextOccurrences = new Dictionary<string, int>();
		private readonly string[] _componentBuffer;

		public KeyProvider(params Func<TElement, string>[] keyAccessors) {
			if (keyAccessors == null)
				throw new ArgumentNullException(nameof(keyAccessors));

			_keyAccessors = new Func<TElement, string>[keyAccessors.Length];
			_componentBuffer = new string[_keyAccessors.Length];

			Array.Copy(keyAccessors, _keyAccessors, _keyAccessors.Length);
		}

		/**
		 * Returns a Key that uniquely identifies the specified instance of
		 * TElement.  The returned Key is composed of values determined by
		 * sequential evaluation of Func&lt;TElement,string>[].  A generated,
		 * ever-increasing "occurrence" integer is appended to the resulting
		 * string[] of key components to ensure uniqueness (in the event that
		 * multiple TElements produce identical component string[]'s).
		 *  
		 * Subsequent calls to KeyFor(TElement) will return that same instance
		 * of Key so long as TElement has not changed in ways that change its
		 * component string[].  If its components have changed, a new Key is
		 * generated as described above.  Previously generated Keys will are
		 * retained and reused (for the life of the TElement instance) should
		 * its components change again to match those of retained Keys.
		 * 
		 * KMS DO what about imports that include duplicate components with different occurrence numbers?
		**/
		public Key KeyFor(TElement element) {
			string components = ComponentsFor(element);

			List<Key> availableKeys;
			if (_keys.TryGetValue(element, out availableKeys)) {
				foreach (var each in availableKeys)
					if (each._components == components)
						return each;
			} else {
				availableKeys = new List<Key>();
			}

			Key newKey = new Key(components, NextFor(components));
			availableKeys.Insert(0, newKey);
			if (availableKeys.Count == 1)
				_keys.Add(element, availableKeys);
			return newKey;
		}

		/**
		 * Returns true if the specified TElement has a Key equal to that
		 * specified.  Returns false otherwise.
		**/
		public bool HasKeyFor(Key key, TElement element) {
			if (TestKeyFor(key, element))
				return true;

			/* Before giving up, refresh current Key and try again. */
			KeyFor(element);

			return TestKeyFor(key, element);
		}

		/**
		 * Imports an externally created Key to the list of Keys generated, as
		 * described, above for the specified instance of TElement.  Returns
		 * true if the Key is imported successfully.  Returns false if the
		 * specified Key was already present for this TElement.  Except when
		 * already present, this action will fail with an exception if the
		 * occurrence integer appended to the Key's component string[] is not
		 * greater than the last integer assigned to any TElement's Key for
		 * that same component string[].  Also throws an exception if the
		 * specified TElement already has a Key with components matching those
		 * of the specified Key while possessing a different occurrence number. 
		**/
		// ReSharper disable once UnusedMethodReturnValue.Global
		public bool ImportFor(Key key, TElement element) {
			List<Key> availableKeys;
			if (_keys.TryGetValue(element, out availableKeys)) {
				foreach (var each in availableKeys)
					if (each == key)
						return false;
				//KMS DO uncomment or delete this!   udate comment above as well
				//foreach (var each in availableKeys)
				//	if (each._components == key._components)
				//		throw new ArgumentException("Key not imported because a key with same components (" + key._components + ") is already available for TElement.");
			} else {
				availableKeys = new List<Key>();
			}

			/* allowing use of any unused occurrence would be better but not needed at this time */
			int next = PeekFor(key._components);
			if (key.Occurrence < next)
				throw new ArgumentException("Key not imported because its Occurrence (" + key.Occurrence + ") is less than next available (" + next + ").");

			_nextOccurrences[key._components] = key.Occurrence + 1;
			availableKeys.Insert(0, key);
			if (availableKeys.Count == 1)
				_keys.Add(element, availableKeys);
			return true;
		}

		public int PeekFor(IEnumerable<string> components) {
			int next;
			return _nextOccurrences.TryGetValue(Key.Compose(components), out next) ? next : 0;
		}

		#region private
		private bool TestKeyFor(Key key, TElement element) {
			List<Key> availableKeys;
			if (!_keys.TryGetValue(element, out availableKeys))
				return false;

			foreach (var each in availableKeys)
				if (each == key)
					return true;

			return false;
		}

		private string ComponentsFor(TElement element) {
			InvokeKeyAccessors(element, _componentBuffer);
			return Key.Compose(_componentBuffer);
		}

		private void InvokeKeyAccessors(TElement element, string[] buffer) {
			for (int i = 0; i < _keyAccessors.Length; i++)
				buffer[i] = _keyAccessors[i].Invoke(element);
		}

		private int NextFor(string components) {
			int next = PeekFor(components);
			_nextOccurrences[components] = next + 1;
			return next;
		}

		private int PeekFor(string components) {
			int next;
			return _nextOccurrences.TryGetValue(components, out next) ? next : 0;
		}
		#endregion
	}
}