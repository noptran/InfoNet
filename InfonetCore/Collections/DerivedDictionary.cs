using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Infonet.Core.Collections {
	/**
	 * A dictionary derived from a shadowed ICollection.  As such, the Values
	 * of this dictionary always reflect those of the shadowed ICollection.
	 * The Keys of this dictionary are generated as needed to provide unique
	 * identification of the Values (i.e. TElements).  Furthermore, the Keys
	 * are meant to be string-serializable (i.e. ToString()) such that they
	 * can be easily stored, transported, and used with other dictionaries with
	 * similarly content.  To accomplish this, the Keys are composed of values
	 * derived from their TElements via a sequence of expressions.  The Keys
	 * are then suffixed with an auto-generated integer used to ensure the
	 * uniqueness of Keys with the same component values identifying different
	 * TElement instances.
	 * 
	 * Each Key is immutable so their component values cannot change when their
	 * corresponding TElements change.  However, this dictionary detects such
	 * changes and will issue fresh Keys to reflect the changes as needed.
	 * Note: this does not invalidate the original, stale Keys.  Those stale
	 * Keys may still be used to retrieve their corresponding TElements from
	 * the dictionary.  In other words, once a Key is associated with a
	 * TElement, it is also associated with that TElement.  That said, this
	 * dictionary prefers fresh Keys to stale ones so it will always use
	 * fresh Keys when returning them to method/property callers.
	 * 
	 * //KMS DO restore and templating
	**/
	public class DerivedDictionary<TElement> : IDictionary<Key, TElement>, IKeyAccess where TElement : class {
		private readonly RestorableCollection<TElement> _values;
		private readonly KeyProvider<TElement> _keyProvider;
		private readonly KeyCollection _keys;
		private readonly KeyCollection _restorableKeys;
		private Func<TElement> _template = null;

		public DerivedDictionary(Func<ICollection<TElement>> collectionAccessor, bool enableRestore, params Func<TElement, string>[] keyAccessors) {
			if (collectionAccessor == null)
				throw new ArgumentNullException(nameof(collectionAccessor));

			_keyProvider = new KeyProvider<TElement>(keyAccessors);
			_values = new RestorableCollection<TElement>(new ShadowCollection<TElement>(collectionAccessor), enableRestore);
			_keys = new KeyCollection(_values, _keyProvider);
			_restorableKeys = enableRestore ? new KeyCollection(_values.Restorable, _keyProvider) : null;
		}

		/**
		 * This indexer provides a wider range of access than a typical
		 * dictionary in that it allows TElement retrieval by stale Keys (Keys
		 * whose components no longer match TElement but once did) and allows
		 * retrieval of TElements that are no longer contained in this
		 * Dictionary but are still restorable via Values' Restore methods.  //KMS DO
		 * 
		 * Searches for the TElement uniquely identified by Key.  As Keys are
		 * generated from mutable TElement state, the current Key for TElement
		 * may differ from the specified Key, but this is irrelevant because once
		 * generated or imported, Keys continue to uniqurely identify their
		 * TElement for all time.
		 * 
		 * Restorables?  //KMS DO  Only the indexer considers restorable TElements and their Keys.  All other methods behave as if those restorable TElements no longer exist.
		 * 
		 * Templates?  //KMS DO
		**/
		public TElement this[Key key] {
			get {
				TElement result;
				if (TryGetValue(key, out result))
					return result;

				if (key.IsTemplate)
					throw new KeyNotFoundException("No template found");

				throw new KeyNotFoundException("No element found for Key '" + key + "'");
			}
			set { throw new NotSupportedException(); }
		}

		object IKeyAccess.this[Key key] {
			get { return this[key]; }
		}

		/**
		 * Equivalent to calling Values.Count.
		**/
		public int Count {
			get { return _values.Count; }
		}

		/**
		 * Equivalent to calling Values.IsReadOnly.
		**/
		public bool IsReadOnly {
			get { return _values.IsReadOnly; }
		}

		/**
		 * This collection contains only the freshest Keys for each TElement in
		 * the Values collection.  Removing Keys from this colletion will only
		 * succeed if the Key specified for removal is the freshest Key for
		 * its corresponding TElement.  Contains method work in a very similar
		 * fashion.
		**/
		public ICollection<Key> Keys {
			get { return _keys; }
		}

		public ICollection<Key> RestorableKeys {
			get { return _restorableKeys; }
		}

		public IEnumerable<Key> KeysFor(IEnumerable<TElement> elements) {
			return new KeyEnumerable(elements, _keyProvider);
		}

		public Key KeyFor(TElement element) {
			return _keyProvider.KeyFor(element);
		}

		/**
		 * This collection of Values should behave exactly as the underlying
		 * shadowed collection does.  The only exception to this is that this
		 * collection is Restorable.  This means that, when enabled, removed
		 * elements are captured for subsequent restoration upon request.
		**/
		public RestorableCollection<TElement> Values {
			get { return _values; }
		}

		ICollection<TElement> IDictionary<Key, TElement>.Values {
			get { return Values; }
		}

		/**
		 * See Add(Key, TElement).
		**/
		public void Add(KeyValuePair<Key, TElement> pair) {
			Add(pair.Key, pair.Value);
		}

		/**
		 * See Add(Key, TElement).
		**/
		public void Add(Key key, object value) {
			Add(key, (TElement)value);
		}

		/**
		 * If Key already idenitifies Value (i.e. TElement), this method is
		 * equivalent to Values.Add(TElement).  If not, dictionary will attempt
		 * to assign Key for use with TElement.  See KeyProvider.Import for
		 * more information before proceeding as Values.Add(TElement).
		 * Note: The components of Key need not be in sync with TElement for
		 * this to succeed.
		**/
		public void Add(Key key, TElement value) {
			_keyProvider.ImportFor(key, value);
			_values.Add(value);
		}

		/**
		 * Equivalent to calling Values.Clear().
		**/
		public void Clear() {
			_values.Clear();
		}

		/**
		 * If Key idenitifies Value (i.e. TElement) regardless of freshness,
		 * this method will be equivalent to ContainsKey(Key).  If not, returns
		 * false.
		**/
		public bool Contains(KeyValuePair<Key, TElement> pair) {
			return _keyProvider.HasKeyFor(pair.Key, pair.Value) && ContainsKey(pair.Key);
		}

		/**
		 * Copies KeyValuePairs into supplied array using freshest Keys.
		**/
		public void CopyTo(KeyValuePair<Key, TElement>[] array, int arrayIndex) {
			this.CopyTo<KeyValuePair<Key, TElement>>(array, arrayIndex);
		}

		/**
		 * Returns true if Values contains the instance of TElement identified
		 * by Key regardless of freshness.  Note: Neither Contains nor
		 * ContainsKey consider Restorables when searching for TElements.  The
		 * same goes for the Remove and methods.
		**/
		public bool ContainsKey(Key key) {
			return _keys.Contains(key);
		}

		/**
		 * Returns an Enumerator providing iteration over Values paired with
		 * their freshest Keys.
		**/
		public IEnumerator<KeyValuePair<Key, TElement>> GetEnumerator() {
			return new Enumerator(this);
		}

		/**
		 * Returns an Enumerator providing iteration over Values paired with
		 * their freshest Keys.
		**/
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		/**
		 * Behaves just like ContainsKey(Key) above except that it calls
		 * Values.Remove(TElement) if found.  Note: Because ICollection does
		 * not support removal by ReferenceEquals, this method must fail if
		 * the TElement to be removed is not the same instance as indicated by
		 * they Key.
		**/
		public bool Remove(Key key) {
			return _keys.Remove(key);
		}

		/**
		 * If Key idenitifies Value (i.e. TElement) regardless of freshness,
		 * this method will be equivalent to Remove(Key).  If not, returns
		 * false.
		**/
		public bool Remove(KeyValuePair<Key, TElement> pair) {
			return _keyProvider.HasKeyFor(pair.Key, pair.Value) && Remove(pair.Key);
		}

		/**
		 * Performs same operations as Indexer without throwing exceptions.
		**/
		public bool TryGetValue(Key key, out TElement value) {
			return TryGetValue(key, true, true, true, out value);
		}

		/**
		 * Per parameters, perform same operations as Indexer without throwing exceptions.
		**/
		// ReSharper disable once MemberCanBePrivate.Global
		public bool TryGetValue(Key key, bool checkForTemplate, bool searchValues, bool searchRestorable, out TElement value) {
			value = null;
			if (checkForTemplate && key.IsTemplate && Template != null) {
				value = Template.Invoke();
				// KMS DO - Should we apply the Template Key's componets?
				return true;
			}

			if (searchValues)
				foreach (var each in _values)
					if (_keyProvider.HasKeyFor(key, each)) {
						value = each;
						return true;
					}

			if (searchRestorable && _values.IsRestorable)
				foreach (var each in _values.Restorable)
					if (_keyProvider.HasKeyFor(key, each)) {
						value = each;
						return true;
					}

			return false;
		}

		public Func<TElement> Template {
			// ReSharper disable once MemberCanBePrivate.Global
			get { return _template; }
			set { _template = value; }
		}

		public int NextOccurrenceFor(params string[] components) {
			if (components == null)
				components = new string[] { null };

			/* make sure all current elements have keys assigned */
			foreach (var each in _values)
				_keyProvider.KeyFor(each);

			/* do the same for restorables */
			if (_values.IsRestorable)
				foreach (var each in _values.Restorable)
					_keyProvider.KeyFor(each);

			return _keyProvider.PeekFor(components);
		}

		#region enumerator
		private class Enumerator : EnumeratorDecorator<TElement, KeyValuePair<Key, TElement>> {
			private readonly DerivedDictionary<TElement> _owner;
			private readonly IEnumerator<TElement> _inner;

			internal Enumerator(DerivedDictionary<TElement> owner) {
				_owner = owner;
				_inner = owner._values.GetEnumerator();
			}

			protected override IEnumerator<TElement> Inner {
				get { return _inner; }
			}

			public override KeyValuePair<Key, TElement> Current {
				get {
					var current = Inner.Current;
					return new KeyValuePair<Key, TElement>(_owner._keyProvider.KeyFor(current), current);
				}
			}
		}
		#endregion

		#region keys
		private class KeyCollection : ICollection<Key> {
			private readonly ICollection<TElement> _inner;
			private readonly KeyProvider<TElement> _provider;

			internal KeyCollection(ICollection<TElement> inner, KeyProvider<TElement> provider) {
				_inner = inner;
				_provider = provider;
			}

			public int Count {
				get { return _inner.Count; }
			}

			public bool IsReadOnly {
				get { return _inner.IsReadOnly; }
			}

			public void Add(Key item) {
				throw new NotSupportedException();
			}

			public void Clear() {
				_inner.Clear();
			}

			public bool Contains(Key key) {
				return _inner.Any(each => _provider.HasKeyFor(key, each));
			}

			public void CopyTo(Key[] array, int arrayIndex) {
				this.CopyTo<Key>(array, arrayIndex);
			}

			public IEnumerator<Key> GetEnumerator() {
				return new KeyEnumerator(_inner.GetEnumerator(), _provider);
			}

			IEnumerator IEnumerable.GetEnumerator() {
				return GetEnumerator();
			}

			public bool Remove(Key key) {
				var elements = _inner.Where(each => _provider.HasKeyFor(key, each)).ToArray();
				if (elements.Length == 0)
					return false;

				//KMS DO check if shadowed collection is a list...if so, go beneath Values and use it...else fail as below
				if (!ReferenceEquals(elements[0], _inner.First(each => each.SafeEquals(elements[0]))))
					throw new InvalidOperationException("Collection has multiple equal TElement instances.  Unable to remove instance identified by Key with certainty.");

				_inner.Remove(elements[0]);
				return true;
			}
		}

		private class KeyEnumerable : IEnumerable<Key> {
			private readonly IEnumerable<TElement> _inner;
			private readonly KeyProvider<TElement> _provider;

			internal KeyEnumerable(IEnumerable<TElement> inner, KeyProvider<TElement> provider) {
				_inner = inner;
				_provider = provider;
			}

			public IEnumerator<Key> GetEnumerator() {
				return new KeyEnumerator(_inner.GetEnumerator(), _provider);
			}

			IEnumerator IEnumerable.GetEnumerator() {
				return GetEnumerator();
			}
		}

		private class KeyEnumerator : EnumeratorDecorator<TElement, Key> {
			private readonly IEnumerator<TElement> _inner;
			private readonly KeyProvider<TElement> _provider;

			internal KeyEnumerator(IEnumerator<TElement> inner, KeyProvider<TElement> provider) {
				_inner = inner;
				_provider = provider;
			}

			protected override IEnumerator<TElement> Inner {
				get { return _inner; }
			}

			public override Key Current {
				get { return _provider.KeyFor(Inner.Current); }
			}
		}
		#endregion
	}
}