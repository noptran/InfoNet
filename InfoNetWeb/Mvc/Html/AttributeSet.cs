using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Infonet.Core.Collections;

namespace Infonet.Web.Mvc.Html {
	public class AttributeSet : IDictionary<string, object> {
		#region constants
		public static readonly AttributeSet Empty = new AttributeSet(new RouteValueDictionary());
		#endregion

		#region members
		private readonly RouteValueDictionary _inner;
		#endregion

		#region constructing
		private AttributeSet(RouteValueDictionary inner) {
			_inner = inner;
		}

		public static AttributeSet From(object htmlAttributes) {
			return From(AsEnumerable(htmlAttributes));
		}

		public static AttributeSet From(IEnumerable<KeyValuePair<string, object>> htmlAttributes) {
			if (htmlAttributes == null)
				return Empty;

			var attributeSet = htmlAttributes as AttributeSet;
			if (attributeSet != null)
				return attributeSet;

			var collection = htmlAttributes as ICollection<KeyValuePair<string, object>>;
			if (collection != null && collection.Count == 0)
				return Empty;

			var dictionary = htmlAttributes as IDictionary<string, object>;
			if (dictionary != null)
				return new AttributeSet(new RouteValueDictionary(dictionary));

			RouteValueDictionary rvd = null;
			foreach (var each in htmlAttributes) {
				if (rvd == null)
					rvd = new RouteValueDictionary();
				rvd.Add(each.Key, each.Value);
			}
			return rvd == null ? Empty : new AttributeSet(rvd);
		}
		#endregion

		#region utils
		private static IEnumerable<KeyValuePair<string, object>> AsEnumerable(object htmlAttributes) {
			if (htmlAttributes == null)
				return Empty;

			var enumerable = htmlAttributes as IEnumerable<KeyValuePair<string, object>>;
			return enumerable ?? HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
		}
		#endregion

		#region operations
		public bool IsEmpty {
			get { return !this.Any(); }
		}

		public AttributeSet And(object moreAttributes) {
			if (IsEmpty)
				return From(moreAttributes);

			var enumerable = AsEnumerable(moreAttributes);

			var collection = enumerable as ICollection<KeyValuePair<string, object>>;
			if (collection != null && collection.Count == 0)
				return this;

			//if AsEnumerable resulted in a new RouteValueDictionary, avoid creating another
			var rvd = enumerable as RouteValueDictionary;
			if (rvd != null && !ReferenceEquals(moreAttributes, enumerable)) {
				rvd.AddRange(this);
				return new AttributeSet(rvd);
			}

			rvd = null;
			foreach (var each in enumerable) {
				if (rvd == null)
					rvd = new RouteValueDictionary(_inner);
				rvd.Add(each.Key, each.Value);
			}
			return rvd == null ? this : new AttributeSet(rvd);
		}

		public AttributeSet AndIf(bool condition, object thenMoreAttributes) {
			return condition ? And(thenMoreAttributes) : this;
		}

		[SuppressMessage("ReSharper", "UnusedMember.Global")]
		public AttributeSet AndIfElse(bool condition, object thenMoreAttributes, object elseMoreAttibutes) {
			return And(condition ? thenMoreAttributes : elseMoreAttibutes);
		}

		[SuppressMessage("ReSharper", "UnusedMember.Global")]
		public AttributeSet IfEmpty(object otherAttibutes) {
			return IsEmpty ? From(otherAttibutes) : this;
		}
		#endregion

		#region dictionary
		public ICollection<string> Keys {
			get { return _inner.Keys; }
		}

		public ICollection<object> Values {
			get { throw new NotSupportedException(); }
		}

		public int Count {
			get { return _inner.Count; }
		}

		public bool IsReadOnly {
			get { return true; }
		}

		public object this[string key] {
			get { return _inner[key]; }
			set { throw new NotSupportedException(); }
		}

		public bool ContainsKey(string key) {
			return _inner.ContainsKey(key);
		}

		public void Add(string key, object value) {
			throw new NotSupportedException();
		}

		public bool Remove(string key) {
			throw new NotSupportedException();
		}

		public bool TryGetValue(string key, out object value) {
			return _inner.TryGetValue(key, out value);
		}

		public void Add(KeyValuePair<string, object> item) {
			throw new NotSupportedException();
		}

		public void Clear() {
			throw new NotSupportedException();
		}

		public bool Contains(KeyValuePair<string, object> item) {
			return _inner.Contains(item);
		}

		public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) {
			foreach (var each in this)
				array[arrayIndex++] = each;
		}

		public bool Remove(KeyValuePair<string, object> item) {
			throw new NotSupportedException();
		}

		public IEnumerator<KeyValuePair<string, object>> GetEnumerator() {
			return _inner.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		#endregion
	}
}