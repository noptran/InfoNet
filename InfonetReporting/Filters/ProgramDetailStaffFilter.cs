using System.Linq;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class ProgramDetailStaffFilter : StaffFilter {
		public ProgramDetailStaffFilter(int?[] svIds) : base(svIds) { }

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.ProgramDetailStaff.Predicates.Add(pds => SvIds.Contains(pds.SVID));
		}
	}
}