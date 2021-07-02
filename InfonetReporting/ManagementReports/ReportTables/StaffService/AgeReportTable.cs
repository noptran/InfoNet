using System.Linq;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.ManagementReports.Builders;

namespace Infonet.Reporting.ManagementReports.ReportTables.StaffService {
	public class AgeReportTable : ReportTable<ManagementClientInformationDemographicsLineItem> {
		public AgeReportTable(string title, int displayOrder) : base(title, displayOrder) {

		}

		public override void CheckAndApply(ManagementClientInformationDemographicsLineItem item) {
			if (item.ClientStatus == ReportTableHeaderEnum.New) {
				var targetRow = Rows.Single(r => r.Code == -1);

				// DT - Using the Code as the "minimum Age" for each row
				if (item.AgeAtFirstContact.HasValue && item.AgeAtFirstContact >= 0)
					targetRow = Rows.OrderByDescending(r => r.Code).First(r => r.Code <= item.AgeAtFirstContact);

				foreach(ReportTableHeader header in Headers) {
					// Check Male vs. Female - allow Total
					if (item.Gender == header.Code || header.Code == ReportTableHeaderEnum.Total) {
						foreach (ReportTableSubHeader subheader in header.SubHeaders) {
							targetRow.Counts[header.Code.ToString()][subheader.Code.ToString()] += 1;
						}
					}
				}
			}
		}
	}
}