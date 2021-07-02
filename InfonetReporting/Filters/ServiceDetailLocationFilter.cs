using System.Linq;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class ServiceDetailLocationFilter : LocationFilter {
		public ServiceDetailLocationFilter(int?[] locationIds) : base(locationIds) {
			Label = "Service Location";
		}

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.ServiceDetailOfClient.Predicates.Add(t => LocationIds.Contains(t.LocationID));
		}
	}
}