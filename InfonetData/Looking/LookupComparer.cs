using System;
using System.Collections;
using System.Collections.Generic;

namespace Infonet.Data.Looking {
	public class LookupComparer : IComparer, IComparer<LookupCode> {
		private readonly Provider _provider;

		public LookupComparer(Provider provider) {
			_provider = provider;
		}

		public int Compare(object a, object b) {
			return Compare((LookupCode)a, (LookupCode)b);
		}

		public int Compare(LookupCode a, LookupCode b) {
			if (a == b)
				return 0;
			if (a == null)
				return 1;
			if (b == null)
				return -1;
			var entryA = a.Entries[_provider];
			var entryB = b.Entries[_provider];
			if (entryA == null && entryB == null)
				return string.Compare(a.Description, b.Description, StringComparison.CurrentCultureIgnoreCase);
			if (entryA == null)
				return 1;
			if (entryB == null)
				return -1;
			return entryA.CompareTo(entryB);
		}
	}
}