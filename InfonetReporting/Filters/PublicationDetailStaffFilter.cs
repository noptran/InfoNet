using System.Linq;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class PublicationDetailStaffFilter : StaffFilter {
		public PublicationDetailStaffFilter(int?[] svIds) : base(svIds) { }

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.PublicationDetailStaff.Predicates.Add(t => SvIds.Contains(t.SVID));
		}
	}
}