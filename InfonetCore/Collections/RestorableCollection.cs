using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Infonet.Core.Collections {
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public class RestorableCollection<TElement> : CollectionDecorator<TElement> {
		private readonly ICollection<TElement> _inner;
		private readonly List<TElement> _restorable;

		public RestorableCollection(ICollection<TElement> collection, bool enableRestore = true) {
			_inner = collection;
			if (enableRestore)
				_restorable = new List<TElement>();
		}

		protected override ICollection<TElement> Inner {
			get { return _inner; }
		}

		public bool IsRestorable {
			get { return _restorable != null; }
		}

		public IList<TElement> Restorable {
			get {
				if (!IsRestorable)
					throw new InvalidOperationException("Collection restore not enabled");
				return _restorable;
			}
		}

		/** Returns a concatenation of this collection and it's Restorable items. **/
		public IEnumerable<TElement> IncludingRestorable {
			get { return this.Concat(Restorable); }
		}

		/** In addition to normal IColleciton.Clear() behavior, adds all cleared items to Restorable. **/
		public override void Clear() {
			var clearedItems = this.ToArray();
			base.Clear();
			if (IsRestorable)
				_restorable.AddRange(clearedItems);
		}

		/** In addition to normal IColleciton.Remove(T) behavior, adds item (if removed) to Restorable. **/
		public override bool Remove(TElement item) {
			bool found = base.Remove(item);
			if (found && IsRestorable)
				_restorable.Add(item);
			return found;
		}

		/** Restores all restorable items and clears Restorable. **/
		public void RestoreAll() {
			if (!IsRestorable)
				throw new InvalidOperationException("Restore not enabled");

			foreach (var each in _restorable)
				Inner.Add(each);
			_restorable.Clear();
		}

		/** Restores (and removes) first occurrence of item from Restorable. **/
		public void Restore(TElement item) {
			if (!IsRestorable)
				throw new InvalidOperationException("Restore not enabled");

			if (!_restorable.Contains(item))
				throw new Exception("Item not restorable: " + item);

			Inner.Add(item);
			_restorable.Remove(item);
		}

		/** Restores (and removes) item at Restorable index. **/
		public void RestoreAt(int index) {
			if (!IsRestorable)
				throw new InvalidOperationException("Restore not enabled");

			var item = _restorable[index];
			Inner.Add(item);
			_restorable.RemoveAt(index);
		}

		/** Restores (and removes) 'count' items starting at Restorable index. **/
		public void RestoreRange(int index, int count) {
			for (int i = 0; i < count; i++)
				RestoreAt(index);
		}

		/* Wraps inner enumerator in case it exposes ability to remove items. */
		public override IEnumerator<TElement> GetEnumerator() {
			return new EnumeratorWrapper<TElement>(Inner.GetEnumerator());
		}
	}
}