using System.Linq;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class ProgramDetailFundingSourceFilter : FundingSourceFilter {
		public ProgramDetailFundingSourceFilter(int?[] fundingSourceIds) : base(fundingSourceIds) { }

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.ProgramDetailStaff.FundServiceProgramOfStaff.Predicates.Add(fs => FundingSourceIds.Contains(fs.FundingSourceID));
		}
	}
}