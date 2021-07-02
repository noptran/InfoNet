using System.IO;
using System.Linq;
using Infonet.Core.IO;
using Infonet.Data.Looking;
using Infonet.Reporting.Core;

namespace Infonet.Reporting.Filters {
	public abstract class LookupFilter : ReportFilter {
		private readonly Lookup _lookup;

		protected LookupFilter(Lookup lookup, int?[] codeIds) {
			_lookup = lookup;
			Label = lookup.DisplayName;
			CodeIds = codeIds ?? new int?[] { };
		}

		public int?[] CodeIds { get; set; }

		public override void WriteCriteriaOn(TextWriter w, ReportContainer container) {
			w.WriteConjoined(';', "or", null, CodeIds.Select(id => _lookup[id].Description ?? "<unassigned>"));
		}
	}
}