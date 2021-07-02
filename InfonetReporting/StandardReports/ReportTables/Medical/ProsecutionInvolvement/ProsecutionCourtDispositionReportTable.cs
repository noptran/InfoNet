using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.MedicalCJ;

namespace Infonet.Reporting.StandardReports.ReportTables.Medical.ProsecutionInvolvement {
	public class ProsecutionCourtDispositionReportTable : ReportTable<MedicalCJProsecutionInvolvementTrialChargeLineItem> {
		public ProsecutionCourtDispositionReportTable(string title, int displayOrder) : base(title, displayOrder) {

		}

		public override void CheckAndApply(MedicalCJProsecutionInvolvementTrialChargeLineItem item) {
			if (item.CourtDisposition.HasValue) {
				foreach (ReportRow row in Rows) {
					if (item.CourtDisposition == row.Code) {
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