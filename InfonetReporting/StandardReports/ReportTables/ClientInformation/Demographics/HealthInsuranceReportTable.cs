using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.ClientInformation;

namespace Infonet.Reporting.StandardReports.ReportTables.ClientInformation.Demographics {
	public class HealthInsuranceReportTable : ReportTable<ClientInformationDemographicsLineItem> {
		public HealthInsuranceReportTable(string title, int displayOrder) : base(title, displayOrder) { }

		public override void CheckAndApply(ClientInformationDemographicsLineItem item) {
			foreach (var row in Rows)
				if (row.Code == item.HealthInsuranceID)
					foreach (var currentHeader in Headers)
						if (item.ClientStatus == currentHeader.Code || currentHeader.Code == ReportTableHeaderEnum.Total) // Check New vs. Ongoing - allow Total
							foreach (var currentSubHeader in currentHeader.SubHeaders)
								if (item.ClientTypeID == (int)currentSubHeader.Code || currentSubHeader.Code == ReportTableSubHeaderEnum.Total) // Check if Client Type matches
									row.Counts[currentHeader.Code.ToString()][currentSubHeader.Code.ToString()] += 1;
		}
	}
}