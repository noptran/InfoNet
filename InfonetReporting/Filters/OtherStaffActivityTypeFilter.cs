using System.IO;
using System.Linq;
using Infonet.Core.IO;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;

namespace Infonet.Reporting.Filters {
	public class OtherStaffActivityTypeFilter : ReportFilter {
		public OtherStaffActivityTypeFilter(int?[] activityIds) {
			Label = "Activity Name";
			ActivityIds = activityIds;
		}

		public int?[] ActivityIds { get; set; }

		public override void ApplyTo(FilterContext context, ReportContainer container) {
			context.OtherStaffActivity.Predicates.Add(q => ActivityIds.Contains(q.OtherStaffActivityID));
		}

		//KMS DO ignores nulls
		public override void WriteCriteriaOn(TextWriter w, ReportContainer container) {
			w.WriteConjoined("or", null, container.InfonetContext.TLU_Codes_OtherStaffActivity.Where(a => ActivityIds.Contains(a.CodeID)).Select(a => a.Description));
		}
	}
}