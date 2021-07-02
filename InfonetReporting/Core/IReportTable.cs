using System.Collections.Generic;
using Infonet.Reporting.Enumerations;

namespace Infonet.Reporting.Core {
	public interface IReportTable {
		string Title { get; }
		string PreHeader { get; }
		string Footer { get; }
		int DisplayOrder { get; }
		bool HideHeaders { get; }
		bool HideSubheaders { get; }
		bool HideSubtotal { get; }
		bool HideTitle { get; }
		bool HideZeroValues { get; }
		bool UseNonDuplicatedSubtotal { get; }
        bool UseNonDuplicatedSubtotalLabel { get; }
		List<ReportTableHeader> Headers { get; }
		List<IReportTable> ReportTables { get; }
		List<ReportRow> Rows { get; }
		ReportRow NonDuplicatedSubtotalRow { get; }
		double GrandTotalFor(ReportTableHeaderEnum header, ReportTableSubHeaderEnum subheader);
		void PreCheckAndApply(ReportContainer container);
		void PostCheckAndApply(ReportContainer container);
	}
}