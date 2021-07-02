using System.Linq;
using Infonet.Data.Looking;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class OffenderRelationshipFilter : LookupFilter {
		public OffenderRelationshipFilter(int?[] codeIds = null) : base(Lookups.RelationshipToClient, codeIds) {
			Label = "Offender Relationship to Victim";
		}

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.Offender.Predicates.Add(t => CodeIds.Contains(t.RelationshipToClientId));
		}
	}
}