using System.Linq;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class CancellationStaffFilter : StaffFilter {
		public CancellationStaffFilter(int?[] svIds) : base(svIds) { }

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.Cancellation.Predicates.Add(q => SvIds.Contains(q.SVID));
		}
	}
}