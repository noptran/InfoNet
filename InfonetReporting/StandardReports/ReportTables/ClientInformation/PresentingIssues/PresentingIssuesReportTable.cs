using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.ClientInformation;

namespace Infonet.Reporting.StandardReports.ReportTables.ClientInformation.PresentingIssues {
	public class PresentingIssuesReportTable : ReportTable<ClientInformationPresentingIssuesLineItem> {
        public PresentingIssuesReportTable (string title, int displayOrder) : base(title, displayOrder) {
        }
        public override void CheckAndApply(ClientInformationPresentingIssuesLineItem item) {
            foreach (ReportRow row in Rows) {
               if(row.Code == item.PrimaryPresentingIssueID) { 
                    foreach (ReportTableHeader currentHeader in Headers) {
                        // Check New vs. Ongoing - allow Total
                        if (item.ClientStatus == currentHeader.Code || currentHeader.Code == ReportTableHeaderEnum.Total) {
                            foreach (ReportTableSubHeader currentSubHeader in currentHeader.SubHeaders) {
                                // Check if Client Type matches
                                if (item.ClientTypeID == (int)currentSubHeader.Code || currentSubHeader.Code == ReportTableSubHeaderEnum.Total) {
                                    row.Counts[currentHeader.Code.ToString()][currentSubHeader.Code.ToString()] += 1;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}