using System;
using System.Collections.Generic;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;

namespace Infonet.Reporting.ViewModels {
	public class SubReportDataViewModel {
		public SubReportDataViewModel() {
			AppliedOrdering = new ReportOrderDisplay[0];
		}
		public int DisplayOrder { get; set; }
		public DateTime ReportRanTimestamp { get; set; }
		public ReportContainer ReportContainer { get; set; }
		public string Output { get; set; }
		public List<SubReportColumnSelection> ColumnSelections { get; set; }
		public List<SubReportTableGroupingSelection> GroupingSelections { get; set; }
		public IReportQuery Query { get; set; }
		public IEnumerable<ReportOrderDisplay> AppliedOrdering { get; set; }
		public SubReportSelection SubReportType { get; set; }
	}
}