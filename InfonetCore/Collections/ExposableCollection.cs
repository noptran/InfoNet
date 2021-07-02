using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Infonet.Core.Collections {
	/**
	 * A collection that may be exposed in a public interface without violating encapsulation.
	 * This is due to Actions defined at instantiation that allow the owner to control and react
	 * to all changes made to the collection.  Similar to ObservableCollection but simpler and
	 * exposes less.
	**/
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public class ExposableCollection<TElement> : CollectionDecorator<TElement> {
		#region fields
		private readonly Action<TElement> _onAdding;
		private readonly Action<TElement> _onAdded;
		private readonly Action<TElement> _onRemoving;
		private readonly Action<TElement> _onRemoved;
		private readonly ICollection<TElement> _inner;
		#endregion

		#region contstructing
		internal ExposableCollection(Action<TElement> onAdding, Action<TElement> onAdded, Action<TElement> onRemoving, Action<TElement> onRemoved, ICollection<TElement> inner) {
			_inner = inner;
			_onAdding = onAdding;
			_onAdded = onAdded;
			_onRemoving = onRemoving;
			_onRemoved = onRemoved;
		}

		public ExposableCollection(Action<TElement> onAdding = null, Action<TElement> onAdded = null, Action<TElement> onRemoving = null, Action<TElement> onRemoved = null) : this(onAdding, onAdded, onRemoving, onRemoved, new List<TElement>()) { }

		public ExposableCollection(IEnumerable<TElement> initialItems, Action<TElement> onAdding = null, Action<TElement> onAdded = null, Action<TElement> onRemoving = null, Action<TElement> onRemoved = null) : this(onAdding, onAdded, onRemoving, onRemoved, new List<TElement>(initialItems)) {
			if (initialItems == null)
				throw new ArgumentNullException(nameof(initialItems));
		}
		#endregion

		#region private
		protected override ICollection<TElement> Inner {
			get { return _inner; }
		}

		private static void Trigger(Action<TElement> action, params TElement[] items) {
			Trigger(action, (IEnumerable<TElement>)items);
		}

		private static void Trigger(Action<TElement> action, IEnumerable<TElement> items) {
			if (action != null)
				foreach (var each in items)
					action(each);
		}
		#endregion

		/** In addition to normal IColleciton.Add(T) behavior, executes _onAdding and _onAdded for the item. **/
		public override void Add(TElement item) {
			Trigger(_onAdding, item);
			base.Add(item);
			Trigger(_onAdded, item);
		}

		/** In addition to normal IColleciton.Clear() behavior, executes _onRemoving and _onRemoved foreach item. **/
		public override void Clear() {
			Trigger(_onRemoving, this);
			var clearedItems = this.ToArray();
			base.Clear();
			Trigger(_onRemoved, clearedItems);
		}

		/** In addition to normal IColleciton.Remove(T) behavior, executes _onRemoving and _onRemoved for the item. **/
		public override bool Remove(TElement item) {
			var foundItems = Inner.Where(i => i.SafeEquals(item)).Take(1).ToArray();
			if (!foundItems.Any())
				return false;

			var removedItem = foundItems.First();
			Trigger(_onRemoving, removedItem);
			base.Remove(removedItem);
			Trigger(_onRemoved, removedItem);
			return true;
		}

		/* Wraps inner enumerator in case it exposes ability to add or remove items. */
		public override IEnumerator<TElement> GetEnumerator() {
			return new EnumeratorWrapper<TElement>(Inner.GetEnumerator());
		}
	}

	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public static class ExposableCollectionExtensions {
		public static ExposableCollection<TElement> AsExposable<TElement>(this ICollection<TElement> self, Action<TElement> onAdding = null, Action<TElement> onAdded = null, Action<TElement> onRemoving = null, Action<TElement> onRemoved = null) {
			return new ExposableCollection<TElement>(onAdding, onAdded, onRemoving, onRemoved, self);
		}
	}
}