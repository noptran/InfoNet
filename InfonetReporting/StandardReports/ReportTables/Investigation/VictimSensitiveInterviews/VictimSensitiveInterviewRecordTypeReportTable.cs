using Infonet.Core.Collections;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.Investigation;

namespace Infonet.Reporting.StandardReports.ReportTables.Investigation.VictimSensitiveInterviews {
	public class VictimSensitiveInterviewRecordTypeReportTable : ReportTable<VictimSensitiveInterviewLineItem> {
		public VictimSensitiveInterviewRecordTypeReportTable(string title, int displayOrder) : base(title, displayOrder) {

		}

		public override void CheckAndApply(VictimSensitiveInterviewLineItem item) {
			foreach (ReportRow row in Rows) {
				if (row.Code == item.RecordTypeID) {
					foreach (ReportTableHeader header in Headers) {
						if (header.Code == item.ClientStatus || header.Code == ReportTableHeaderEnum.Total) {
							foreach (ReportTableSubHeader subheader in header.SubHeaders) {
								if (subheader.Code.ToInt32() == item.ClientTypeID || subheader.Code == ReportTableSubHeaderEnum.Total) {
									row.Counts[header.Code.ToString()][subheader.Code.ToString()] += 1;
								}
							}
						}
					}
				}
			}
		}
	}
}