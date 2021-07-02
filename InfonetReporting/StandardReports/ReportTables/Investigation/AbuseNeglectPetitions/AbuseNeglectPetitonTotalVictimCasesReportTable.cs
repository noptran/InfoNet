using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.Investigation;

namespace Infonet.Reporting.StandardReports.ReportTables.Investigation.AbuseNeglectPetitions {
	public class AbuseNeglectPetitonTotalVictimCasesReportTable : ReportTable<AbuseNeglectPetitionLineItem> {
		public AbuseNeglectPetitonTotalVictimCasesReportTable(string title, int displayOrder) : base(title, displayOrder) {

		}
		public override void CheckAndApply(AbuseNeglectPetitionLineItem item) {
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