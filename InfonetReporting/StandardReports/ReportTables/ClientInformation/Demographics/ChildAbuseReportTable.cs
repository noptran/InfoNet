using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.ClientInformation;

namespace Infonet.Reporting.StandardReports.ReportTables.ClientInformation.Demographics {
	public class ChildAbuseReportTable : ReportTable<ClientInformationDemographicsLineItem> {
		public ChildAbuseReportTable(string title, int displayOrder) : base(title, displayOrder) { }

		public override void CheckAndApply(ClientInformationDemographicsLineItem item) {
			if (item.ClientTypeID == (int)ReportTableSubHeaderEnum.Child)
				foreach (var row in Rows) {
					bool childAbuseApplys = false;
					if (item.DCFSInvestigation == 1 && row.Code == 1)
						childAbuseApplys = true;
					else if (item.DCFSOpen == 1 && row.Code == 2)
						childAbuseApplys = true;

					if (childAbuseApplys)
						foreach (var currentHeader in Headers) // Check New vs. Ongoing - allow Total
							if (item.ClientStatus == currentHeader.Code || currentHeader.Code == ReportTableHeaderEnum.Total)
								row.Counts[currentHeader.Code.ToString()][ReportTableSubHeaderEnum.Child.ToString()] += 1;
				}
		}
	}
}