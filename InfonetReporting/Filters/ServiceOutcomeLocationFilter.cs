using System.Linq;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class ServiceOutcomeLocationFilter : LocationFilter {
		public ServiceOutcomeLocationFilter(int?[] locationIds) : base(locationIds) {
			Label = "Outcome Location";
		}

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.ServiceOutcome.Predicates.Add(t => LocationIds.Contains(t.LocationID));
		}
	}
}