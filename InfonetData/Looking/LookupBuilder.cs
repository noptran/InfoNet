using System.Collections.Generic;

namespace Infonet.Data.Looking {
	public class LookupBuilder {
		public LookupBuilder() {
			Codes = new List<Code>();
		}

		public int TableId { get; set; }

		public string TableName { get; set; }

		public string DisplayName { get; set; }

		public string Description { get; set; }

		public IList<Code> Codes { get; }

		public Lookup ToLookup(double minLoadFactor = Lookup.DEFAULT_MIN_LOAD_FACTOR) {
			return new Lookup(this, minLoadFactor);
		}

		public class Code {
			public Code() {
				Entries = new List<Entry>();
			}

			public int CodeId { get; set; }

			public string Description { get; set; }

			public IList<Entry> Entries { get; }

			public LookupCode ToLookupCode(Lookup owner) {
				return new LookupCode(owner, this);
			}
		}

		public class Entry {
			public Provider Provider { get; set; }

			public int DisplayOrder { get; set; }

			public bool IsActive { get; set; }

			public LookupCode.Entry ToEntry(LookupCode owner) {
				return new LookupCode.Entry(owner, this);
			}
		}
	}
}