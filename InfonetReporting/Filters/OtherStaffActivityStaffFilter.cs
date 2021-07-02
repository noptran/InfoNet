using System.Linq;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class OtherStaffActivityStaffFilter : StaffFilter {
		public OtherStaffActivityStaffFilter(int?[] svIds) : base(svIds) { }

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.OtherStaffActivity.Predicates.Add(q => SvIds.Contains(q.SVID));
		}
	}
}