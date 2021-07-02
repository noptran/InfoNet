using Infonet.Data.Looking;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.Builders.ClientInformation;

namespace Infonet.Reporting.StandardReports.ReportTables.ClientInformation.Referral {
	public class ReferralSourceReportTable : ReportTable<ClientInformationReferralSourcesLineItem> {
		public ReferralSourceReportTable(string title, int displayOrder) : base(title, displayOrder) { }

		public override void CheckAndApply(ClientInformationReferralSourcesLineItem item) {
			bool itemHasThisReferralSource;
			if (item.ClientTypeId == (int)ReportTableSubHeaderEnum.Adult || item.ClientTypeId == (int)ReportTableSubHeaderEnum.ChildVictim)
				foreach (var row in Rows) {
					itemHasThisReferralSource = false;
					switch (row.Code) {
						case (int)ClientReferralSourceEnum.ChildAdvocacyCenter:
							if (item.ChildAdvocacyCenter ?? false)
								itemHasThisReferralSource = true;
							break;
						case (int)ClientReferralSourceEnum.CircuitClerk:
							if (item.CircuitClerk ?? false)
								itemHasThisReferralSource = true;
							break;
						case (int)ClientReferralSourceEnum.Clergy:
							if (item.Clergy ?? false)
								itemHasThisReferralSource = true;
							break;
						case (int)ClientReferralSourceEnum.DCFS:
							if (item.DCFS ?? false)
								itemHasThisReferralSource = true;
							break;
						case (int)ClientReferralSourceEnum.EducationSystem:
							if (item.EducationSystem ?? false)
								itemHasThisReferralSource = true;
							break;
						case (int)ClientReferralSourceEnum.Friend:
							if (item.Friend ?? false)
								itemHasThisReferralSource = true;
							break;
						case (int)ClientReferralSourceEnum.Hospital:
							if (item.Hospital ?? false)
								itemHasThisReferralSource = true;
							break;
						case (int)ClientReferralSourceEnum.HousingProgram:
							if (item.HousingProgram ?? false)
								itemHasThisReferralSource = true;
							break;
						case (int)ClientReferralSourceEnum.IlLDVHelpline:
							if (item.StatewideHelpLine ?? false)
								itemHasThisReferralSource = true;
							break;
						case (int)ClientReferralSourceEnum.LegalSystem:
							if (item.LegalSystem ?? false)
								itemHasThisReferralSource = true;
							break;
						case (int)ClientReferralSourceEnum.Media:
							if (item.Media ?? false)
								itemHasThisReferralSource = true;
							break;
						case (int)ClientReferralSourceEnum.Medical:
							if (item.Medical ?? false)
								itemHasThisReferralSource = true;
							break;
						case (int)ClientReferralSourceEnum.MedicalAdvocacyProgram:
							if (item.MedicalAdvocacyProgram ?? false)
								itemHasThisReferralSource = true;
							break;
						case (int)ClientReferralSourceEnum.NationalDVHotline:
							if (item.NationalHotline ?? false)
								itemHasThisReferralSource = true;
							break;
						case (int)ClientReferralSourceEnum.Other:
							if (item.Other ?? false)
								itemHasThisReferralSource = true;
							break;
						case (int)ClientReferralSourceEnum.OtherDVProgram:
							if (item.OtherDVProgram ?? false)
								itemHasThisReferralSource = true;
							break;
						case (int)ClientReferralSourceEnum.OtherLocalhotline:
							if (item.OtherLocalHotline ?? false)
								itemHasThisReferralSource = true;
							break;
						case (int)ClientReferralSourceEnum.Police:
							if (item.Police ?? false)
								itemHasThisReferralSource = true;
							break;
						case (int)ClientReferralSourceEnum.PrivateAttorney:
							if (item.PrivateAttorney ?? false)
								itemHasThisReferralSource = true;
							break;
						case (int)ClientReferralSourceEnum.PublicHealth:
							if (item.PublicHealth ?? false)
								itemHasThisReferralSource = true;
							break;
						case (int)ClientReferralSourceEnum.Relative:
							if (item.Relative ?? false)
								itemHasThisReferralSource = true;
							break;
						case (int)ClientReferralSourceEnum.Self:
							if (item.Self ?? false)
								itemHasThisReferralSource = true;
							break;
						case (int)ClientReferralSourceEnum.SexualAssaultProgram:
							if (item.SexualAssualtProgram ?? false)
								itemHasThisReferralSource = true;
							break;
						case (int)ClientReferralSourceEnum.SocialServicesProgram:
							if (item.SocialServiceProgram ?? false)
								itemHasThisReferralSource = true;
							break;
						case (int)ClientReferralSourceEnum.StateAttorney:
							if (item.StateAttorney ?? false)
								itemHasThisReferralSource = true;
							break;
						default: {
							if (row.Code == null)
								itemHasThisReferralSource =
									!((item.ChildAdvocacyCenter ?? false) ||
									(item.CircuitClerk ?? false) ||
									(item.Clergy ?? false) ||
									(item.DCFS ?? false) ||
									(item.EducationSystem ?? false) ||
									(item.Friend ?? false) ||
									(item.Hospital ?? false) ||
									(item.HousingProgram ?? false) ||
									(item.LegalSystem ?? false) ||
									(item.Media ?? false) ||
									(item.Medical ?? false) ||
									(item.MedicalAdvocacyProgram ?? false) ||
									(item.NationalHotline ?? false) ||
									(item.Other ?? false) ||
									(item.OtherDVProgram ?? false) ||
									(item.OtherLocalHotline ?? false) ||
									(item.Police ?? false) ||
									(item.PrivateAttorney ?? false) ||
									(item.PublicHealth ?? false) ||
									(item.Relative ?? false) ||
									(item.Self ?? false) ||
									(item.SexualAssualtProgram ?? false) ||
									(item.SocialServiceProgram ?? false) ||
									(item.StateAttorney ?? false) ||
									(item.StatewideHelpLine ?? false));
							break;
						}
					}
					if (itemHasThisReferralSource)
						foreach (ReportTableHeader currentHeader in Headers) // Check New vs. Ongoing - allow Total
							if (item.ClientStatus == currentHeader.Code || currentHeader.Code == ReportTableHeaderEnum.Total)
								row.Counts[currentHeader.Code.ToString()][((ReportTableSubHeaderEnum)item.ClientTypeId).ToString()] += 1;
				}
			else if (Provider == Provider.SA)
				foreach (ReportRow row in Rows) {
					itemHasThisReferralSource = false;
					switch (row.Code) {
						case (int)ClientReferralSourceEnum.ChildAdvocacyCenter:
							if (item.ChildAdvocacyCenter ?? false)
								itemHasThisReferralSource = true;
							break;
						case (int)ClientReferralSourceEnum.CenterHotline:
							if (item.CenterHotline ?? false)
								itemHasThisReferralSource = true;
							break;
						case (int)ClientReferralSourceEnum.Clergy:
							if (item.Clergy ?? false)
								itemHasThisReferralSource = true;
							break;
						case (int)ClientReferralSourceEnum.DCFS:
							if (item.DCFS ?? false)
								itemHasThisReferralSource = true;
							break;
						case (int)ClientReferralSourceEnum.EducationSystem:
							if (item.EducationSystem ?? false)
								itemHasThisReferralSource = true;
							break;
						case (int)ClientReferralSourceEnum.Friend:
							if (item.Friend ?? false)
								itemHasThisReferralSource = true;
							break;
						case (int)ClientReferralSourceEnum.Hospital:
							if (item.Hospital ?? false)
								itemHasThisReferralSource = true;
							break;
						case (int)ClientReferralSourceEnum.StateAttorney:
							if (item.StateAttorney ?? false)
								itemHasThisReferralSource = true;
							break;
						case (int)ClientReferralSourceEnum.Media:
							if (item.Media ?? false)
								itemHasThisReferralSource = true;
							break;
						case (int)ClientReferralSourceEnum.Other:
							if (item.Other ?? false)
								itemHasThisReferralSource = true;
							break;
						case (int)ClientReferralSourceEnum.Medical:
							if (item.Medical ?? false)
								itemHasThisReferralSource = true;
							break;
						case (int)ClientReferralSourceEnum.Police:
							if (item.Police ?? false)
								itemHasThisReferralSource = true;
							break;
						case (int)ClientReferralSourceEnum.PrivateAttorney:
							if (item.PrivateAttorney ?? false)
								itemHasThisReferralSource = true;
							break;
						case (int)ClientReferralSourceEnum.PublicHealth:
							if (item.PublicHealth ?? false)
								itemHasThisReferralSource = true;
							break;
						case (int)ClientReferralSourceEnum.Relative:
							if (item.Relative ?? false)
								itemHasThisReferralSource = true;
							break;
						case (int)ClientReferralSourceEnum.Self:
							if (item.Self ?? false)
								itemHasThisReferralSource = true;
							break;
						case (int)ClientReferralSourceEnum.SocialServicesProgram:
							if (item.SocialServiceProgram ?? false)
								itemHasThisReferralSource = true;
							break;
						case (int)ClientReferralSourceEnum.OtherRapeCrisisCenter:
							if (item.OtherRapeCrisisCenter ?? false)
								itemHasThisReferralSource = true;
							break;
						default: {
							if (row.Code == null)
								itemHasThisReferralSource =
									!((item.ChildAdvocacyCenter ?? false) ||
									(item.CenterHotline ?? false) ||
									(item.Clergy ?? false) ||
									(item.DCFS ?? false) ||
									(item.EducationSystem ?? false) ||
									(item.Friend ?? false) ||
									(item.Hospital ?? false) ||
									(item.Media ?? false) ||
									(item.Medical ?? false) ||
									(item.Other ?? false) ||
									(item.Police ?? false) ||
									(item.PrivateAttorney ?? false) ||
									(item.PublicHealth ?? false) ||
									(item.Relative ?? false) ||
									(item.Self ?? false) ||
									(item.SocialServiceProgram ?? false) ||
									(item.StateAttorney ?? false) ||
									(item.StatewideHelpLine ?? false));
							break;
						}
					}
					if (itemHasThisReferralSource)
						foreach (var currentHeader in Headers) // Check New vs. Ongoing - allow Total
							if (item.ClientStatus == currentHeader.Code || currentHeader.Code == ReportTableHeaderEnum.Total)
								foreach (var currentSubHeader in currentHeader.SubHeaders) // Check if Client Type matches
									if (item.ClientTypeId == (int)currentSubHeader.Code || currentSubHeader.Code == ReportTableSubHeaderEnum.Total)
										row.Counts[currentHeader.Code.ToString()][currentSubHeader.Code.ToString()] += 1;
				}
		}
	}
}