using System.IO;
using System.Linq;
using Infonet.Core.IO;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class ReferralDetailLocationFilter : ReportFilter {
		public ReferralDetailLocationFilter(int?[] locationIds) {
			Label = "Referral Location";
			LocationIds = locationIds;
		}

		public int?[] LocationIds { get; set; }

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.ClientReferralDetail.Predicates.Add(t => LocationIds.Contains(t.LocationID));
		}

		//KMS DO ignores nulls
		public override void WriteCriteriaOn(TextWriter w, ReportContainer container) {
			w.WriteConjoined("or", null, container.InfonetContext.T_Center.Where(c => LocationIds.Contains(c.CenterID)).Select(c => c.CenterName));
		}
	}
}