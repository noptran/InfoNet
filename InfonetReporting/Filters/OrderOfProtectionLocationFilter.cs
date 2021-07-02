using System.Linq;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class OrderOfProtectionLocationFilter : LocationFilter {
		public OrderOfProtectionLocationFilter(int?[] locationIds) : base(locationIds) {
			Label = "Order of Protection Location";
		}

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.OrderOfProtection.Predicates.Add(oop => LocationIds.Contains(oop.LocationID));
		}
	}
}