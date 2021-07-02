using System;
using System.Collections;
using System.Collections.Generic;

namespace Infonet.Core.Collections {
	public class OrdinalEnumMap<TEnum, TValue> : IEnumerable<KeyValuePair<TEnum, TValue>> where TEnum : struct, IConvertible {
		private readonly TValue[] _values = new TValue[OrdinalEnum<TEnum>.Length];

		public OrdinalEnumMap(IDictionary<TEnum, TValue> values) {
			foreach (var each in values)
				_values[OrdinalEnum<TEnum>.OrdinalOf(each.Key)] = each.Value;
		}

		public TValue this[TEnum key] {
			get { return _values[OrdinalEnum<TEnum>.OrdinalOf(key)]; }
		}

		public IDictionary<TEnum, TValue> ToDictionary() {
			var result = new Dictionary<TEnum, TValue>(OrdinalEnum<TEnum>.Length);
			foreach (var each in this)
				result.Add(each.Key, each.Value);
			return result;
		}

		public IEnumerator<KeyValuePair<TEnum, TValue>> GetEnumerator() {
			return new Enumerator(_values);
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		#region inner Enumerator
		public class Enumerator : EnumeratorDecorator<TEnum, KeyValuePair<TEnum, TValue>> {
			private readonly IEnumerator<TEnum> _inner = OrdinalEnum<TEnum>.Values.GetEnumerator();
			private readonly TValue[] _values;

			internal Enumerator(TValue[] values) {
				_values = values;
			}

			protected override IEnumerator<TEnum> Inner {
				get { return _inner; }
			}

			public override KeyValuePair<TEnum, TValue> Current {
				get {
					var current = Inner.Current;
					return new KeyValuePair<TEnum, TValue>(current, _values[OrdinalEnum<TEnum>.OrdinalOf(current)]);
				}
			}
		}
		#endregion
	}
}