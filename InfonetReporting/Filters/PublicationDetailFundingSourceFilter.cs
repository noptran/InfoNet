using System.Linq;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class PublicationDetailFundingSourceFilter : FundingSourceFilter {
		public PublicationDetailFundingSourceFilter(int?[] fundingSourceIds) : base(fundingSourceIds) { }

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.PublicationDetailStaff.FundServiceProgramOfStaff.Predicates.Add(t => FundingSourceIds.Contains(t.FundingSourceID));
		}
	}
}