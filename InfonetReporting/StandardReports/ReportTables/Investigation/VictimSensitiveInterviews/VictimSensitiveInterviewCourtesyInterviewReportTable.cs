using Infonet.Core.Collections;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.Investigation;

namespace Infonet.Reporting.StandardReports.ReportTables.Investigation.VictimSensitiveInterviews {
	public class VictimSensitiveInterviewCourtesyInterviewReportTable : ReportTable<VictimSensitiveInterviewLineItem> {
		public VictimSensitiveInterviewCourtesyInterviewReportTable(string title, int displayOrder) : base(title, displayOrder) { }

		public override void CheckAndApply(VictimSensitiveInterviewLineItem item) {
			foreach (var row in Rows)
				if (item.CourtesyInterview.HasValue && item.CourtesyInterview.Value)
					foreach (var header in Headers)
						if (header.Code == item.ClientStatus || header.Code == ReportTableHeaderEnum.Total)
							foreach (var subheader in header.SubHeaders)
								if (subheader.Code.ToInt32() == item.ClientTypeID || subheader.Code == ReportTableSubHeaderEnum.Total)
									row.Counts[header.Code.ToString()][subheader.Code.ToString()] += 1;
		}
	}
}