using System.Linq;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class OtherStaffActivityLocationFilter : LocationFilter {
		public OtherStaffActivityLocationFilter(int?[] locationIds) : base(locationIds) {
			Label = "Activity Location";
		}

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.OtherStaffActivity.Predicates.Add(osa => LocationIds.Contains(osa.StaffVolunteer.CenterId));
		}
	}
}