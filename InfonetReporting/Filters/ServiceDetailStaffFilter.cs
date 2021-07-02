using System.Linq;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class ServiceDetailStaffFilter : StaffFilter {
		public ServiceDetailStaffFilter(int?[] svIds) : base(svIds) { }

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.ServiceDetailOfClient.StaffFunding.Predicates.Add(t => SvIds.Contains(t.SvId));
		}
	}
}