using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.ClientInformation;

namespace Infonet.Reporting.StandardReports.ReportTables.ClientInformation.PresentingIssues {
	public class PresentingIssueLocationReportTable : ReportTable<ClientInformationPresentingIssuesLineItem> {
        public PresentingIssueLocationReportTable(string title, int displayOrder) : base(title, displayOrder) {
        }
        public override void CheckAndApply(ClientInformationPresentingIssuesLineItem item) {
            foreach (ReportRow row in Rows) {
               if(row.Code == item.LocOfPrimOffenseID) { 
                    foreach (ReportTableHeader currentHeader in Headers) {
                        // Check New vs. Ongoing - allow Total
                        if (item.ClientStatus == currentHeader.Code || currentHeader.Code == ReportTableHeaderEnum.Total) {
                            row.Counts[currentHeader.Code.ToString()][((ReportTableSubHeaderEnum)item.ClientTypeID).ToString()] += 1;
                        }
                    }
                }
            }
        }
    }
}