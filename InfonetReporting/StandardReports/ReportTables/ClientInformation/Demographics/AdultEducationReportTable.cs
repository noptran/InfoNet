using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.ClientInformation;

namespace Infonet.Reporting.StandardReports.ReportTables.ClientInformation.Demographics {
	public class AdultEducationReportTable : ReportTable<ClientInformationDemographicsLineItem> {
		public AdultEducationReportTable(string title, int displayOrder) : base(title, displayOrder) {

		}
		public override void CheckAndApply(ClientInformationDemographicsLineItem item) {
			foreach (ReportRow row in Rows) {
				if (row.Code == item.EducationID && item.ClientTypeID == (int)ReportTableSubHeaderEnum.Adult) {
					foreach (ReportTableHeader currentHeader in Headers) {
						// Check New vs. Ongoing - allow Total
						if (item.ClientStatus == currentHeader.Code || currentHeader.Code == ReportTableHeaderEnum.Total) {
									row.Counts[currentHeader.Code.ToString()][ReportTableSubHeaderEnum.Adult.ToString()] += 1;
							}
						}
					}
				}
			}
		}
	}
