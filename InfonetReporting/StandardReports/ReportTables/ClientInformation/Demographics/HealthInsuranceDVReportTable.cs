using System.Collections.Generic;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.ClientInformation;

namespace Infonet.Reporting.StandardReports.ReportTables.ClientInformation.Demographics {
	public class HealthInsuranceDVReportTable : ReportTable<ClientInformationDemographicsLineItem> {
		public HealthInsuranceDVReportTable(string title, int displayOrder) : base(title, displayOrder) {
			UniqueCases = new HashSet<string>();
		}

		private HashSet<string> UniqueCases { get; }

		public override void CheckAndApply(ClientInformationDemographicsLineItem item) {
			string caseIdentifier = $"{item.ClientID}:{item.CaseID}";
			if (item.ClientTypeID == (int)ReportTableSubHeaderEnum.Adult)
				foreach (var row in Rows) {
					bool isInGroup = false;
					switch (row.Code) {
						case 1:
							isInGroup = item.Ins_Medicaid.HasValue && item.Ins_Medicaid.Value;
							break;
						case 2:
							isInGroup = item.Ins_Medicare.HasValue && item.Ins_Medicare.Value;
							break;
						case 3:
							isInGroup = item.Ins_StateChildHealth.HasValue && item.Ins_StateChildHealth.Value;
							break;
						case 4:
							isInGroup = item.Ins_VetAdminMed.HasValue && item.Ins_VetAdminMed.Value;
							break;
						case 5:
							isInGroup = item.Ins_Private.HasValue && item.Ins_Private.Value;
							break;
						case 6:
							isInGroup = item.Ins_NoHealthIns.HasValue && item.Ins_NoHealthIns.Value;
							break;
						case 7:
							isInGroup = item.Ins_Unknown.HasValue && item.Ins_Unknown.Value;
							break;
						case null:
							isInGroup = (!item.Ins_Medicaid.HasValue || !item.Ins_Medicaid.Value) &&
										(!item.Ins_Medicare.HasValue || !item.Ins_Medicare.Value) &&
										(!item.Ins_StateChildHealth.HasValue || !item.Ins_StateChildHealth.Value) &&
										(!item.Ins_VetAdminMed.HasValue || !item.Ins_VetAdminMed.Value) &&
										(!item.Ins_Private.HasValue || !item.Ins_Private.Value) &&
										(!item.Ins_NoHealthIns.HasValue || !item.Ins_NoHealthIns.Value) &&
										(!item.Ins_Unknown.HasValue || !item.Ins_Unknown.Value);
							break;
					}
					if (isInGroup) {
						foreach (var currentHeader in Headers) // Check New vs. Ongoing - allow Total
							if (item.ClientStatus == currentHeader.Code || currentHeader.Code == ReportTableHeaderEnum.Total) {
								row.Counts[currentHeader.Code.ToString()][((ReportTableSubHeaderEnum)item.ClientTypeID).ToString()] += 1;
								if (!UniqueCases.Contains(caseIdentifier))
									NonDuplicatedSubtotalRow.Counts[currentHeader.Code.ToString()][ReportTableSubHeaderEnum.Adult.ToString()] += 1;
							}
						UniqueCases.Add(caseIdentifier);
					}
				}
		}
	}
}