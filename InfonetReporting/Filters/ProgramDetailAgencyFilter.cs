using System.IO;
using System.Linq;
using Infonet.Core.IO;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class ProgramDetailAgencyFilter : ReportFilter {
		public ProgramDetailAgencyFilter(int?[] agencyIds) {
			Label = "Agency Name";
			AgencyIds = agencyIds;
		}

		public int?[] AgencyIds { get; set; }

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.ProgramDetail.Predicates.Add(q => AgencyIds.Contains(q.AgencyID));
		}

		//KMS DO ignores nulls
		public override void WriteCriteriaOn(TextWriter w, ReportContainer container) {
			w.WriteConjoined("or", null, container.InfonetContext.T_Agency.Where(a => AgencyIds.Contains(a.AgencyID)).Select(a => a.AgencyName));
		}
	}
}