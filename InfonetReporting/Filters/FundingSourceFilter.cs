using System.IO;
using System.Linq;
using Infonet.Core.IO;
using Infonet.Reporting.Core;

namespace Infonet.Reporting.Filters {
	public abstract class FundingSourceFilter : ReportFilter {
		protected FundingSourceFilter(int?[] fundingSourceIds) {
			Label = "Funding Source";
			FundingSourceIds = fundingSourceIds;
		}

		public int?[] FundingSourceIds { get; set; }

		//KMS DO ignores nulls
		public override void WriteCriteriaOn(TextWriter w, ReportContainer container) {
			w.WriteConjoined("or", null, container.InfonetContext.TLU_Codes_FundingSource.Where(fs => FundingSourceIds.Contains(fs.CodeID)).Select(fs => fs.Description));
		}
	}
}