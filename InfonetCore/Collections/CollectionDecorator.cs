using System.Collections;
using System.Collections.Generic;

namespace Infonet.Core.Collections {
	public abstract class CollectionDecorator<TElement> : ICollection<TElement> {
		protected abstract ICollection<TElement> Inner { get; }

		public virtual int Count {
			get { return Inner.Count; }
		}

		public virtual bool IsReadOnly {
			get { return Inner.IsReadOnly; }
		}

		public virtual void Add(TElement item) {
			Inner.Add(item);
		}

		public virtual void Clear() {
			Inner.Clear();
		}

		public virtual bool Contains(TElement item) {
			return Inner.Contains(item);
		}

		public virtual void CopyTo(TElement[] array, int arrayIndex) {
			Inner.CopyTo(array, arrayIndex);
		}

		public virtual IEnumerator<TElement> GetEnumerator() {
			return Inner.GetEnumerator();
		}

		public virtual bool Remove(TElement item) {
			return Inner.Remove(item);
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		public override bool Equals(object obj) {
			return Inner.Equals(obj);
		}

		public override int GetHashCode() {
			return Inner.GetHashCode();
		}

		public override string ToString() {
			return $"{GetType().Name}({Inner})";
		}
	}
}