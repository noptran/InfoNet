using System.Linq;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class ServiceDetailStaffFundingSourceFilter : FundingSourceFilter {
		public ServiceDetailStaffFundingSourceFilter(int?[] fundingSourceIds) : base(fundingSourceIds) { }

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.ServiceDetailOfClient.StaffFunding.Predicates.Add(t => FundingSourceIds.Contains(t.FundingSourceId));
		}
	}
}