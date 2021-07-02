using System.Collections.Generic;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.ClientInformation;

namespace Infonet.Reporting.StandardReports.ReportTables.ClientInformation.Demographics {
	public class NonCashBenefitsReportTable : ReportTable<ClientInformationDemographicsLineItem> {
		public NonCashBenefitsReportTable(string title, int displayOrder) : base(title, displayOrder) {
			UniqueCases = new HashSet<string>();
		}

		private HashSet<string> UniqueCases { get; set; }

		public override void CheckAndApply(ClientInformationDemographicsLineItem item) {
			string caseIdentifier = $"{item.ClientID}:{item.CaseID}";
			if (item.ClientTypeID == (int)ReportTableSubHeaderEnum.Adult)
				foreach (var row in Rows) {
					bool itemHasThisBenefit = false;
					switch (row.Code) {
						case (int)NonCashBenefitsEnum.FoodBenefit:
							if (item.NCB_FoodBenefit ?? false)
								itemHasThisBenefit = true;
							break;
						case (int)NonCashBenefitsEnum.SpecSuppNutr:
							if (item.NCB_SpecSuppNutr ?? false)
								itemHasThisBenefit = true;
							break;
						case (int)NonCashBenefitsEnum.TANFChildCare:
							if (item.NCB_TANFChildCare ?? false)
								itemHasThisBenefit = true;
							break;
						case (int)NonCashBenefitsEnum.TANFTransportation:
							if (item.NCB_TANFTrans ?? false)
								itemHasThisBenefit = true;
							break;
						case (int)NonCashBenefitsEnum.OtherTANF:
							if (item.NCB_OtherTANF ?? false)
								itemHasThisBenefit = true;
							break;
						case (int)NonCashBenefitsEnum.PublicHousing:
							if (item.NCB_PublicHousing ?? false)
								itemHasThisBenefit = true;
							break;
						case (int)NonCashBenefitsEnum.OtherSource:
							if (item.NCB_OtherSource ?? false)
								itemHasThisBenefit = true;
							break;
						case (int)NonCashBenefitsEnum.NoBenefit:
							if (item.NCB_NoBenefit ?? false)
								itemHasThisBenefit = true;
							break;
						case (int)NonCashBenefitsEnum.Unknown:
							if (item.NCB_Unknown ?? false)
								itemHasThisBenefit = true;
							break;
						default: {
							if (row.Code == null)
								itemHasThisBenefit =
									!((item.NCB_FoodBenefit ?? false) || (item.NCB_SpecSuppNutr ?? false) ||
									  (item.NCB_TANFChildCare ?? false) || (item.NCB_TANFTrans ?? false) ||
									  (item.NCB_OtherTANF ?? false) || (item.NCB_PublicHousing ?? false) ||
									  (item.NCB_OtherSource ?? false) || (item.NCB_NoBenefit ?? false) ||
									  (item.NCB_Unknown ?? false));
							break;
						}
					}
					if (itemHasThisBenefit) {
						foreach (var header in Headers)
							if (item.ClientStatus == header.Code || header.Code == ReportTableHeaderEnum.Total) {
								row.Counts[header.Code.ToString()][ReportTableSubHeaderEnum.Adult.ToString()] += 1;
								if (!UniqueCases.Contains(caseIdentifier))
									NonDuplicatedSubtotalRow.Counts[header.Code.ToString()][ReportTableSubHeaderEnum.Adult.ToString()] += 1;
							}
						UniqueCases.Add(caseIdentifier);
					}
				}
		}
	}
}