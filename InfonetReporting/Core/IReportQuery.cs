using System.Collections.Generic;
using System.IO;

namespace Infonet.Reporting.Core {
	public interface IReportQuery {
		IList<ReportFilter> Filters { get; }
		IEnumerable<ReportFilter> VisibleFilters { get; }
		ReportContainer ReportContainer { get; set; }
		void Write(List<OrderedTextOutput> html, TextWriter csv);
	}
}