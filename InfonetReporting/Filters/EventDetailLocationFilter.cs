using System.Linq;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class EventDetailLocationFilter : LocationFilter {
		public EventDetailLocationFilter(int?[] locationIds) : base(locationIds) {
			Label = "Event Location";
		}

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.EventDetail.Predicates.Add(q => LocationIds.Contains(q.CenterID));
		}
	}
}