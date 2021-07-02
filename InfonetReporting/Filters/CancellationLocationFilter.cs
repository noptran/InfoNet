using System.Linq;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class CancellationLocationFilter : LocationFilter {
		public CancellationLocationFilter(int?[] locationIds) : base(locationIds) {
			Label = "Cancellation Location";
		}

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.Cancellation.Predicates.Add(c => LocationIds.Contains(c.LocationID));
		}
	}
}