using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Infonet.Core.Collections;

namespace Infonet.Data.Looking {
	public class LookupSelection : ILookupSelection {
		private readonly LookupGroup _owner;
		private readonly IEnumerable<LookupCode> _selected;

		internal LookupSelection(LookupGroup owner, IEnumerable<LookupCode> selected) : this(owner, selected, true) { }

		private LookupSelection(LookupGroup owner, IEnumerable<LookupCode> selected, bool sortAndFlatten) {
			_owner = owner;
			_selected = !sortAndFlatten ? selected : selected.OrderBy(c => c.Entries[_owner.Provider], true).ToArray();
		}

		public IEnumerator<LookupCode> GetEnumerator() {
			return _selected.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		public bool IsEmpty {
			get { return ((LookupCode[])_selected).Length == 0; }
		}

		public ILookupSelection Exclude(params int[] codeIds) {
			return new LookupSelection(_owner, this.Where(c => !codeIds.Contains(c.CodeId)), false);
		}

		public ILookupSelection Include(params int[] codeIds) {
			var missingCodeIds = codeIds.Except(this.Select(c => c.CodeId)).ToArray();
			var missingCodes = new LookupCode[missingCodeIds.Length];
			for (int i = 0; i < missingCodeIds.Length; i++)
				missingCodes[i] = _owner[missingCodeIds[i]];
			return new LookupSelection(_owner, this.Concat(missingCodes), true);
		}

		public ILookupSelection ExcludeFor(Provider provider, params int[] codeIds) {
			return _owner.Provider != provider ? this : Exclude(codeIds);
		}

		public ILookupSelection IncludeFor(Provider provider, params int[] codeIds) {
			return _owner.Provider != provider ? this : Include(codeIds);
		}
	}
}