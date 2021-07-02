using System;
using System.Collections;
using System.Collections.Generic;

namespace Infonet.Core.Collections {
	public static class SublistExtensions {
		/* startIndex may be equal to list.Count just as string.Substring allows. */
		public static IList<TElement> AsSublist<TElement>(this IList<TElement> list, int startIndex, int? count = null) {
			return new Sublist<TElement>(list, startIndex, count);
		}

		/* Entirely index based, changes to underlying indexes will be reflected in Sublist.
		   However, changes to underlying IList size will not be reflected in Sublist startIndex or Count.
		   This may lead to ArgumentOutOfRangeExceptions in Sublist if underlying IList range no longer covers
		   Sublist range. */
		private class Sublist<TElement> : IList<TElement> {
			private readonly IList<TElement> _list;
			private readonly int _offset;
			private int _count;

			internal Sublist(IList<TElement> list, int startIndex, int? count) {
				if (list == null)
					throw new ArgumentNullException(nameof(list));
				if (startIndex < 0 || startIndex > list.Count)
					throw new ArgumentOutOfRangeException(nameof(startIndex));
				if (count == null)
					count = list.Count - startIndex;
				else if (count > list.Count - startIndex)
					throw new ArgumentOutOfRangeException(nameof(count));

				var sublist = list as Sublist<TElement>;
				if (sublist != null) {
					list = sublist._list;
					startIndex = sublist._offset + startIndex;
				}

				_list = list;
				_offset = startIndex;
				_count = (int)count;
			}

			public int Count {
				get { return _count; }
			}

			public bool IsReadOnly {
				get { return _list.IsReadOnly; }
			}

			private void AssertInRange(int index, bool forInsert = false) {
				int insertAllowance = forInsert ? 1 : 0;
				if (index < 0 || index >= _count + insertAllowance)
					throw new ArgumentOutOfRangeException(nameof(index));
				if (_offset + index >= _list.Count + insertAllowance)
					throw new ArgumentOutOfRangeException(nameof(index), "Index is in Sublist range but Sublist is out of underlying IList range");
			}

			private TElement ElementAt(int index) {
				return _list[_offset + index];
			}

			public TElement this[int index] {
				get {
					AssertInRange(index);
					return ElementAt(index);
				}
				set {
					AssertInRange(index);
					_list[_offset + index] = value;
				}
			}

			public int IndexOf(TElement item) {
				for (var en = new Enumerator(this); en.MoveNext();)
					if (en.Current.SafeEquals(item))
						return en.Index;
				return -1;
			}

			public bool Contains(TElement item) {
				return IndexOf(item) >= 0;
			}

			public IEnumerator<TElement> GetEnumerator() {
				return new Enumerator(this);
			}

			IEnumerator IEnumerable.GetEnumerator() {
				return GetEnumerator();
			}

			public void Insert(int index, TElement item) {
				AssertInRange(index, true);
				_list.Insert(_offset + index, item);
				_count++;
			}

			public void Add(TElement item) {
				Insert(_count, item);
			}

			private void RemoveElementAt(int index) {
				_list.RemoveAt(_offset + index);
				_count--;
			}

			public void Clear() {
				if (_count == 0)
					return;

				var list = _list as List<TElement>;
				if (list != null) {
					list.RemoveRange(_offset, _count);
					_count = 0;
				} else {
					while (_count > 0)
						RemoveElementAt(0);
				}
			}

			public void RemoveAt(int index) {
				AssertInRange(index);
				RemoveElementAt(index);
			}

			public bool Remove(TElement item) {
				int index = IndexOf(item);
				if (index < 0)
					return false;

				RemoveElementAt(index);
				return false;
			}

			public void CopyTo(TElement[] array, int arrayIndex) {
				if (_count == 0)
					return;

				var list = _list as List<TElement>;
				if (list != null)
					list.CopyTo(_offset, array, arrayIndex, _count);
				else
					for (var en = new Enumerator(this); en.MoveNext();)
						array[arrayIndex + en.Index] = en.Current;
			}

			private class Enumerator : IEnumerator<TElement> {
				private readonly Sublist<TElement> _owner;
				private int _index = -1;
				private TElement _current = default(TElement);

				internal Enumerator(Sublist<TElement> owner) {
					_owner = owner;
				}

				internal int Index {
					get { return _index; }
				}

				public bool MoveNext() {
					_current = default(TElement);
					if (_index + 1 >= _owner.Count)
						return false;

					_index++;
					_current = _owner.ElementAt(_index);
					return true;
				}

				public TElement Current {
					get { return _current; }
				}

				object IEnumerator.Current {
					get { return Current; }
				}

				public void Reset() {
					_index = -1;
					_current = default(TElement);
				}

				public void Dispose() { }
			}
		}
	}
}