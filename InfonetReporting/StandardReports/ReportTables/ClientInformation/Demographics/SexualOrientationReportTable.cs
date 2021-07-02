using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.ClientInformation;
using Infonet.Data.Looking;

namespace Infonet.Reporting.StandardReports.ReportTables.ClientInformation.Demographics {
    public class SexualOrientationReportTable : ReportTable<ClientInformationDemographicsLineItem> {
        public SexualOrientationReportTable(string title, int displayOrder) : base(title, displayOrder) { }
        public override void CheckAndApply(ClientInformationDemographicsLineItem item) {
            foreach (ReportRow row in Rows) {
                if (Provider == Provider.DV && row.Code == item.SexualOrientationID && item.ClientTypeID == (int)ReportTableSubHeaderEnum.Adult) {
                    foreach (ReportTableHeader currentHeader in Headers) {
                        // Check New vs. Ongoing - allow Total
                        if (item.ClientStatus == currentHeader.Code || currentHeader.Code == ReportTableHeaderEnum.Total) {
                            row.Counts[currentHeader.Code.ToString()][ReportTableSubHeaderEnum.Adult.ToString()] += 1;
                        }
                    }
                }
                else if (row.Code == item.SexualOrientationID && Provider == Provider.SA) {
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
