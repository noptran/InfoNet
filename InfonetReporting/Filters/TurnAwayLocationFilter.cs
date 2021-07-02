using System.Linq;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class TurnAwayLocationFilter : LocationFilter {
		public TurnAwayLocationFilter(int?[] locationIds) : base(locationIds) {
			Label = "Turn Away Location";
		}

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.TurnAwayService.Predicates.Add(tas => LocationIds.Contains(tas.LocationId));
		}
	}
}