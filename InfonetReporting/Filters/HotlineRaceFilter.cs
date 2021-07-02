using System.Linq;
using Infonet.Data.Looking;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	/* keep until we're sure we can drop requirement for this */
	public class HotlineRaceFilter : LookupFilter {
		public HotlineRaceFilter(int?[] codeIds = null) : base(Lookups.Race, codeIds) { }

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.PhoneHotline.Predicates.Add(ph => CodeIds.Contains(ph.RaceID));
		}
	}
}