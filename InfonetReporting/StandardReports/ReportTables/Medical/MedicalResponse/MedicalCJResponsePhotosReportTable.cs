using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.MedicalCJ;

namespace Infonet.Reporting.StandardReports.ReportTables.Medical.MedicalResponse {
	public class MedicalCJResponsePhotosReportTable : ReportTable<MedicalSystemInvolvementLineItem> {
		public MedicalCJResponsePhotosReportTable(string title, int displayOrder) : base(title, displayOrder) {

		}

		public override void CheckAndApply(MedicalSystemInvolvementLineItem item) {
			foreach (ReportRow row in Rows) {
				if (row.Code == item.PhotosTakenId) {
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