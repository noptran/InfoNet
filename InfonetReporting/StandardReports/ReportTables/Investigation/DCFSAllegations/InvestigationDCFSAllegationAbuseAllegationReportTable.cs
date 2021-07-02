using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.Investigation;

namespace Infonet.Reporting.StandardReports.ReportTables.Investigation.DCFSAllegations {
	public class InvestigationDCFSAllegationAbuseAllegationReportTable : ReportTable<InvestigationDCFSAllegationLineItem> {
		public InvestigationDCFSAllegationAbuseAllegationReportTable(string title, int displayOrder) : base(title, displayOrder) {

		}
		public override void CheckAndApply(InvestigationDCFSAllegationLineItem item) {
			foreach (ReportRow row in Rows) {
				if (row.Code == item.AbuseAllegationId) {
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