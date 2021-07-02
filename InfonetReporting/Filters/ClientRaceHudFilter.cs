using System.Linq;
using Infonet.Data.Looking;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class ClientRaceHudFilter : LookupFilter {
		public ClientRaceHudFilter(int?[] codeIds = null) : base(Lookups.RaceHud, codeIds) { }

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.ClientRace.Predicates.Add(cr => CodeIds.Contains(cr.RaceHudId));
		}
	}
}