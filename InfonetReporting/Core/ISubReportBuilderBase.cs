using System.Collections.Generic;
using Infonet.Reporting.Enumerations;

namespace Infonet.Reporting.Core {
	public interface ISubReportBuilderBase {
		int DisplayOrder { get; set; }
		ReportContainer ReportContainer { get; set; }
		List<IReportTable> ReportTableList { get; set; }
		SubReportSelection SubReportType { get; set; }
		string ReportName { get; set; }
	}
}