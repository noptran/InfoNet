using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.ClientInformation;

namespace Infonet.Reporting.StandardReports.ReportTables.ClientInformation.Demographics {
	public class CollegeStudentReportTable : ReportTable<ClientInformationDemographicsLineItem> {
        public CollegeStudentReportTable (string title, int displayOrder) : base(title, displayOrder) {

        }
        public override void CheckAndApply(ClientInformationDemographicsLineItem item) {
            foreach (ReportRow row in Rows)
                if (item.IsCollegeStudent == 1) { 
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
