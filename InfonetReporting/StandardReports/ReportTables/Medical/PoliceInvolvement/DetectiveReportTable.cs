using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.MedicalCJ;

namespace Infonet.Reporting.StandardReports.ReportTables.Medical.PoliceInvolvement {
	public class DetectiveInterviewReportTable : ReportTable<MedicalCJPoliceInvolvementCJLineItem> {
		public DetectiveInterviewReportTable(string title, int displayOrder) : base(title, displayOrder) {

		}
		public override void CheckAndApply(MedicalCJPoliceInvolvementCJLineItem item) {
			if (item.DetectiveInterview) {
				foreach (ReportRow row in Rows) {
					foreach (ReportTableHeader header in Headers) {
						if (header.Code == item.ClientStatus || header.Code == ReportTableHeaderEnum.Total) {
							foreach (ReportTableSubHeader subheader in header.SubHeaders) {
								row.Counts[header.Code.ToString()][subheader.Code.ToString()] += 1;
							}
						}
					}
				}
			}
		}
	}
}