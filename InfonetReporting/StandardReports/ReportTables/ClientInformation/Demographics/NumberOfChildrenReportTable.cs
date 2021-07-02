using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.ClientInformation;

namespace Infonet.Reporting.StandardReports.ReportTables.ClientInformation.Demographics {
	public class NumberOfChildrenReportTable : ReportTable<ClientInformationDemographicsLineItem> {
		public NumberOfChildrenReportTable(string title, int displayOrder) : base(title, displayOrder) { }

		public override void CheckAndApply(ClientInformationDemographicsLineItem item) {
			if (item.ClientTypeID == (int)ReportTableSubHeaderEnum.Adult || item.ClientTypeID == (int)ReportTableSubHeaderEnum.NonOffendingCaretaker)
				foreach (var row in Rows) {
					int? currentNumChildrenRowCode;
					if (item.NumberOfChildren <= 0)
						currentNumChildrenRowCode = 0;
					else if (item.NumberOfChildren >= 1 && item.NumberOfChildren < 8 || item.NumberOfChildren == null)
						currentNumChildrenRowCode = item.NumberOfChildren;
					else
						currentNumChildrenRowCode = 8;

					if (row.Code == currentNumChildrenRowCode)
						foreach (var currentHeader in Headers) // Check New vs. Ongoing - allow Total
							if (item.ClientStatus == currentHeader.Code || currentHeader.Code == ReportTableHeaderEnum.Total)
								row.Counts[currentHeader.Code.ToString()][((ReportTableSubHeaderEnum)item.ClientTypeID).ToString()] += 1;
				}
		}
	}
}