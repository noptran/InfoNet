using System.Collections.Generic;
using System.Linq;
using Infonet.Data.Looking;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.ClientInformation;

namespace Infonet.Reporting.StandardReports.ReportTables.ClientInformation.Demographics {
	public class PrimaryIncomeReportTable : ReportTable<ClientInformationDemographicsLineItem> {
		public PrimaryIncomeReportTable(string title, int displayOrder) : base(title, displayOrder) {
			UniqueCases = new HashSet<string>();
		}

		private HashSet<string> UniqueCases { get; }

		public override void CheckAndApply(ClientInformationDemographicsLineItem item) {
			string caseIdentifier = $"{item.ClientID}:{item.CaseID}";
			bool addToHashset = false;
			if (item.ClientTypeID == (int)ReportTableSubHeaderEnum.Adult)
				foreach (var row in Rows) {
					if (item.PrimaryIncomeSources.Any(a => a.IncomeID == row.Code) || !item.PrimaryIncomeSources.Any() && row.Code == null) {
						addToHashset = true;
						foreach (var header in Headers)
							if (item.ClientStatus == header.Code || header.Code == ReportTableHeaderEnum.Total) {
								row.Counts[header.Code.ToString()][ReportTableSubHeaderEnum.Adult.ToString()] += 1;
								if (!UniqueCases.Contains(caseIdentifier))
									NonDuplicatedSubtotalRow.Counts[header.Code.ToString()][ReportTableSubHeaderEnum.Adult.ToString()] += 1;
							}
					}
					if (addToHashset)
						UniqueCases.Add(caseIdentifier);
				}
			else if (Provider == Provider.SA)
				foreach (var row in Rows)
					if (row.Code == item.SAPrimaryIncomeSource)
						foreach (var currentHeader in Headers) // Check New vs. Ongoing - allow Total
							if (item.ClientStatus == currentHeader.Code || currentHeader.Code == ReportTableHeaderEnum.Total)
								foreach (var currentSubHeader in currentHeader.SubHeaders) // Check if Client Type matches
									if (item.ClientTypeID == (int)currentSubHeader.Code || currentSubHeader.Code == ReportTableSubHeaderEnum.Total)
										row.Counts[currentHeader.Code.ToString()][currentSubHeader.Code.ToString()] += 1;
		}
	}
}