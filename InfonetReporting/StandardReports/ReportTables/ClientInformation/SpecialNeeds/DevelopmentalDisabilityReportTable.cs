using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.ClientInformation;

namespace Infonet.Reporting.StandardReports.ReportTables.ClientInformation.SpecialNeeds {
	public class DevelopmentalDisabilityReportTable : ReportTable<ClientInformationSpecialNeedsLineItem> {
		public DevelopmentalDisabilityReportTable(string title, int displayOrder) : base(title, displayOrder) { }

		public override void CheckAndApply(ClientInformationSpecialNeedsLineItem item) {
			foreach (var row in Rows) {
				bool thisAnswerApplies = false;
				switch (row.Code) {
					case (int)ShortAnswerEnum.Yes:
						if (item.DevelopmentalDisabled ?? false)
							thisAnswerApplies = true;
						break;
					default:
						if (item.DevelopmentalDisabled != true)
							thisAnswerApplies = true;
						break;
				}

				if (thisAnswerApplies)
					foreach (var currentHeader in Headers) // Check New vs. Ongoing - allow Total
						if (item.ClientStatus == currentHeader.Code || currentHeader.Code == ReportTableHeaderEnum.Total)
							foreach (var currentSubHeader in currentHeader.SubHeaders) // Check if Client Type matches
								if (item.ClientTypeID == (int)currentSubHeader.Code || currentSubHeader.Code == ReportTableSubHeaderEnum.Total)
									row.Counts[currentHeader.Code.ToString()][currentSubHeader.Code.ToString()] += 1;
			}
		}
	}
}