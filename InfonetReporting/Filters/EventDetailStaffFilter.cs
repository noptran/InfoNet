using System.Linq;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class EventDetailStaffFilter : StaffFilter {
		public EventDetailStaffFilter(int?[] svIds) : base(svIds) { }

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.EventDetailStaff.Predicates.Add(eds => SvIds.Contains(eds.SVID));
		}
	}
}