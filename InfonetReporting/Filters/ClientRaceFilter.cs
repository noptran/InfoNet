using System.Linq;
using Infonet.Data.Looking;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class ClientRaceFilter : LookupFilter {
		public ClientRaceFilter(int?[] codeIds = null) : base(Lookups.Race, codeIds) { }

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.Client.Predicates.Add(c => CodeIds.Contains(c.RaceId));
		}
	}
}