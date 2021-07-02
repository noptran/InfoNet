using System.Linq;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class EventDetailFundingSourceFilter : FundingSourceFilter {
		public EventDetailFundingSourceFilter(int?[] fundingSourceIds) : base(fundingSourceIds) { }

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.EventDetailStaff.FundServiceProgramOfStaff.Predicates.Add(fs => FundingSourceIds.Contains(fs.FundingSourceID));
		}
	}
}