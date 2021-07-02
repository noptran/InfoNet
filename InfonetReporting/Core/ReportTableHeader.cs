using System.Collections.Generic;
using Infonet.Reporting.Enumerations;

namespace Infonet.Reporting.Core {
	public class ReportTableHeader {
		public ReportTableHeader() { }

		public ReportTableHeader(string title, ReportTableHeaderEnum code, List<ReportTableSubHeader> subheaders) {
			Title = title;
			Code = code;
			SubHeaders = subheaders;
		}

		public string Title { get; set; }
		public ReportTableHeaderEnum Code { get; set; }
		public List<ReportTableSubHeader> SubHeaders { get; set; }
	}
}