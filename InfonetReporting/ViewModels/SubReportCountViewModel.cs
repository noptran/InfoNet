using System.Collections.Generic;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;

namespace Infonet.Reporting.ViewModels {
	public class SubReportCountViewModel {
		public SubReportSelection SubReportType { get; set; }
		public int DisplayOrder { get; set; }
		public ReportContainer ReportContainer { get; set; }
		public IReportQuery Query { get; set; }
		public IEnumerable<ReportOrderDisplay> AppliedOrdering { get; set; }
	}
}