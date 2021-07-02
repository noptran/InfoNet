using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Infonet.Core.Collections;

namespace Infonet.Core.Data {
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	[SuppressMessage("ReSharper", "ArgumentsStyleNamedExpression")]
	public class DelimitedField<TItem> : ICollection<TItem> where TItem : struct, IConvertible {
		private readonly Action<TItem> _clearValue;
		private string _value = null;
		private ExposableCollection<TItem> _items = null;

		/**
		 * No delimiter escaping is performed so be sure delimiter, prefix, and suffix can
		 * never appear in TItems converted to Strings.  ALSO, TItems must not convert to
		 * empty strings.
		 */
		public DelimitedField(string delimiter, string prefix, string suffix) {
			_clearValue = item => { _value = null; };
			Delimiter = delimiter;
			Prefix = prefix;
			Suffix = suffix;
		}

		public string Delimiter { get; }

		public string Prefix { get; }

		public string Suffix { get; }

		public string Value {
			get {
				if (_value == null && _items != null)
					_value = $"{Prefix}{string.Join(Delimiter, _items.Select(i => Convert.ToString(i)))}{Suffix}";
				return _value;
			}
			set {
				if (value == _value)
					return;

				if (value == null) {
					_items = null;
					_value = null;
					return;
				}
				if (!value.StartsWith(Prefix))
					throw new ArgumentException($"{nameof(Value)} missing {nameof(Prefix).ToLower()} \"{Prefix}\": {value}");

				if (value.Length < Prefix.Length + Suffix.Length || !value.EndsWith(Suffix))
					throw new ArgumentException($"{nameof(Value)} missing {nameof(Suffix).ToLower()} \"{Suffix}\": {value}");

				string innerValue = value.Substring(Prefix.Length, value.Length - (Prefix.Length + Suffix.Length));
				var innerItems = innerValue == ""
					? new TItem[0]
					: innerValue.Split(new[] { Delimiter }, StringSplitOptions.None).Select(i => (TItem)Convert.ChangeType(i == "" ? null : i, typeof(TItem)));
				_items = new ExposableCollection<TItem>(innerItems, onAdding: _clearValue, onRemoving: _clearValue);
				_value = value;
			}
		}

		public IEnumerable<TItem> Items {
			get { return _items; }
			set {
				_value = null;
				_items = value == null ? null : new ExposableCollection<TItem>(value, onAdding: _clearValue, onRemoving: _clearValue);
			}
		}

		#region ICollection
		public IEnumerator<TItem> GetEnumerator() {
			return _items?.GetEnumerator() ?? EmptyEnumerator<TItem>.Instance;
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		public void Add(TItem item) {
			if (_items == null)
				_items = new ExposableCollection<TItem>(onAdding: _clearValue, onRemoving: _clearValue);
			_items.Add(item);
		}

		public void Clear() {
			_items?.Clear();
		}

		public bool Contains(TItem item) {
			return _items?.Contains(item) ?? false;
		}

		public void CopyTo(TItem[] array, int arrayIndex) {
			_items?.CopyTo(array, arrayIndex);
		}

		public bool Remove(TItem item) {
			return _items?.Remove(item) ?? false;
		}

		public int Count {
			get { return _items?.Count ?? 0; }
		}

		public bool IsReadOnly {
			get { return false; }
		}
		#endregion
	}
}