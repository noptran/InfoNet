using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.MedicalCJ;

namespace Infonet.Reporting.StandardReports.ReportTables.Medical.ProsecutionInvolvement {
	public class ProsecutionCourtActivityReportTable : ReportTable<MedicalCJPoliceProsecutionLineItem> {
		public ProsecutionCourtActivityReportTable(string title, int displayOrder) : base(title, displayOrder) {

		}

		public override void CheckAndApply(MedicalCJPoliceProsecutionLineItem item) {
			foreach (var appearance in item.CourtActivities) {
				foreach (ReportRow row in Rows) {
					if (appearance == row.Code) {
						foreach (ReportTableHeader header in Headers) {
							if (header.Code == item.ClientStatus || header.Code == ReportTableHeaderEnum.Total) {
								row.Counts[header.Code.ToString()][ReportTableSubHeaderEnum.Total.ToString()] += 1;
							}
						}
					}
				}
			}
		}
	}
}