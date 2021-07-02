using System.IO;
using System.Linq;
using Infonet.Core.IO;
using Infonet.Reporting.Core;

namespace Infonet.Reporting.Filters {
	public abstract class LocationFilter : ReportFilter {
		protected LocationFilter(int?[] locationIds) {
			Label = "Location";
			LocationIds = locationIds;
		}

		public int?[] LocationIds { get; set; }

		//KMS DO ignores nulls
		public override void WriteCriteriaOn(TextWriter w, ReportContainer container) {
			w.WriteConjoined("or", null, container.InfonetContext.T_Center.Where(c => LocationIds.Contains(c.CenterID)).Select(c => c.CenterName));
		}
	}
}