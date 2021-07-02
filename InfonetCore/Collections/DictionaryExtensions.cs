using System.Collections.Generic;

namespace Infonet.Core.Collections {
	public static class DictionaryExtensions {
		public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> self, IEnumerable<KeyValuePair<TKey, TValue>> keyValuePairs) {
			foreach (var each in keyValuePairs)
				self.Add(each);
		}

		// ReSharper disable once UnusedMember.Global
		public static IDictionary<string, object> CopyWith(this IDictionary<string, object> self, IEnumerable<KeyValuePair<string, object>> keyValuePairs) {
			var result = new Dictionary<string, object>(self);
			result.AddRange(keyValuePairs);
			return result;
		}

		// ReSharper disable once UnusedMember.Global
		public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> self, TKey key, TValue defaultValue) {
			TValue value;
			return self.TryGetValue(key, out value) ? value : defaultValue;
		}
	}
}