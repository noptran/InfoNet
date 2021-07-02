using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Infonet.Core.Collections {
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public class KeyedCollection<TKey, TItem> : System.Collections.ObjectModel.KeyedCollection<TKey, TItem> {
		private readonly Func<TItem, TKey> _keySelector;

		public KeyedCollection(Func<TItem, TKey> keySelector) {
			_keySelector = keySelector;
		}

		public KeyedCollection(Func<TItem, TKey> keySelector, IEqualityComparer<TKey> comparer) : base(comparer) {
			_keySelector = keySelector;
		}

		public KeyedCollection(Func<TItem, TKey> keySelector, IEqualityComparer<TKey> comparer, int dictionaryCreationThreshold) : base(comparer, dictionaryCreationThreshold) {
			_keySelector = keySelector;
		}

		protected override TKey GetKeyForItem(TItem item) {
			return _keySelector(item);
		}

		public bool TryGetValue(TKey key, out TItem item) {
			item = default(TItem);
			return Dictionary != null && Dictionary.TryGetValue(key, out item);
		}
	}
}