using Infonet.Data.Looking;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.ClientInformation;

namespace Infonet.Reporting.StandardReports.ReportTables.ClientInformation.Demographics {
	public class MaritalStatusReportTable : ReportTable<ClientInformationDemographicsLineItem> {
		public MaritalStatusReportTable(string title, int displayOrder) : base(title, displayOrder) {

		}
		public override void CheckAndApply(ClientInformationDemographicsLineItem item) {
            foreach (ReportRow row in Rows) {
                if (row.Code == item.MaritalStatusID) {
                    if (item.ClientTypeID == (int)ReportTableSubHeaderEnum.Adult || item.ClientTypeID == (int)ReportTableSubHeaderEnum.NonOffendingCaretaker) {
                        foreach (ReportTableHeader currentHeader in Headers) {
                            // Check New vs. Ongoing - allow Total
                            if (item.ClientStatus == currentHeader.Code || currentHeader.Code == ReportTableHeaderEnum.Total) {
                                row.Counts[currentHeader.Code.ToString()][((ReportTableSubHeaderEnum)item.ClientTypeID).ToString()] += 1;
                            }
                        }
                    } else if (Provider == Provider.SA) {
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
}
