using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.ManagementReports.Builders;

namespace Infonet.Reporting.ManagementReports.ReportTables.StaffService {

	public class CaseCountReportTable : ReportTable<ManagementClientInformationDemographicsLineItem> {
		public CaseCountReportTable(string title, int displayOrder) : base(title, displayOrder) {

		}
		public override void CheckAndApply(ManagementClientInformationDemographicsLineItem item) {
			foreach (ReportRow row in Rows) {
				foreach (ReportTableHeader header in Headers) {
					if (item.ClientStatus == header.Code || header.Code == ReportTableHeaderEnum.Total) {
						foreach (ReportTableSubHeader subheader in header.SubHeaders) {
							row.Counts[header.Code.ToString()][subheader.Code.ToString()] += 1;
						}
					}
				}
			}
		}
	}
}