using System.IO;
using System.Linq;
using Infonet.Core.IO;
using Infonet.Reporting.Core;

namespace Infonet.Reporting.Filters {
	public abstract class StaffFilter : ReportFilter {
		protected StaffFilter(int?[] svIds) {
			Label = "Staff Name";
			SvIds = svIds;
		}

		public int?[] SvIds { get; set; }

		//KMS DO ignores nulls
		public override void WriteCriteriaOn(TextWriter w, ReportContainer container) {
			w.WriteConjoined("or", null, container.InfonetContext.T_StaffVolunteer.Where(s => SvIds.Contains(s.SvId)).Select(s => s.FirstName + " " + s.LastName));
		}
	}
}