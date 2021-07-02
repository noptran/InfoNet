using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.MedicalCJ;

namespace Infonet.Reporting.StandardReports.ReportTables.Medical.ProsecutionInvolvement {
	public class ProsecutionVWParticipationReportTable : ReportTable<MedicalCJPoliceProsecutionLineItem> {
		public ProsecutionVWParticipationReportTable(string title, int displayOrder) : base(title, displayOrder) {

		}

		public override void CheckAndApply(MedicalCJPoliceProsecutionLineItem item) {
			if (item.VWParticipation.HasValue) {
				foreach (ReportRow row in Rows) {
					if (item.VWParticipation == row.Code) { 
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