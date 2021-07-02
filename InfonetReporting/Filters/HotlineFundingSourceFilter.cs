using System.Linq;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class HotlineFundingSourceFilter : FundingSourceFilter {
		public HotlineFundingSourceFilter(int?[] fundingSourceIds) : base(fundingSourceIds) { }

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.PhoneHotline.FundServiceProgramOfStaff.Predicates.Add(fs => FundingSourceIds.Contains(fs.FundingSourceID));
		}
	}
}