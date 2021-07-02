using System.Linq;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class HivMentalSubstanceLocationFilter : LocationFilter {
		public HivMentalSubstanceLocationFilter(int?[] locationIds) : base(locationIds) {
			Label = "Aggregate Client Information Location";
		}

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.HivMentalSubstance.Predicates.Add(t => LocationIds.Contains(t.LocationID));
		}
	}
}