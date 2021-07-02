using System.Linq;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class HotlineStaffFilter : StaffFilter {
		public HotlineStaffFilter(int?[] svIds) : base(svIds) { }

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.PhoneHotline.Predicates.Add(ph => SvIds.Contains(ph.SVID));
		}
	}
}