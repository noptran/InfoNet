using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.ClientInformation;

namespace Infonet.Reporting.StandardReports.ReportTables.ClientInformation.Demographics {
	public class LivesWithReportTable : ReportTable<ClientInformationDemographicsLineItem> {
        public LivesWithReportTable(string title, int displayOrder) : base(title, displayOrder) {
            
        }
        public override void CheckAndApply(ClientInformationDemographicsLineItem item) {
            foreach (ReportRow row in Rows) {
                if (row.Code == item.LivesWithID && item.ClientTypeID == (int)ReportTableSubHeaderEnum.Child) {
                    foreach (ReportTableHeader currentHeader in Headers) {
                        // Check New vs. Ongoing - allow Total
                        if (item.ClientStatus == currentHeader.Code || currentHeader.Code == ReportTableHeaderEnum.Total) {
                            row.Counts[currentHeader.Code.ToString()][ReportTableSubHeaderEnum.Child.ToString()] += 1;
                        }
                    }
                }
            }
        }
    }
}