using System.Collections.Generic;
using System.Linq;
using Infonet.Core.IO;
using Infonet.Data.Looking;
using Infonet.Data.Models.Clients;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.ReportTables.ClientInformation.Referral;

namespace Infonet.Reporting.StandardReports.Builders.ClientInformation {
	public class ClientInformationReferralSourcesSubReport : SubReportCountBuilder<ClientCase, ClientInformationReferralSourcesLineItem> {
		public ClientInformationReferralSourcesSubReport(SubReportSelection subReportSelectionType) : base(subReportSelectionType) { }

		protected override string[] CsvHeaders {
			get { return new[] { "ID", "Client ID", "Case ID", "Client Status", "Law Enforcement", "Hospital", "Medical", "Medical Advocacy Program", "Legal System", "Clergy", "Social Service Program", "Education System", "Friend", "Relative", "Self", "Other", "Private Attorney", "Public Health", "Media", "State Attorney", "Circuit Clerk", "DCFS", "Child Advocacy Center", "Other Rape Crisis Center", "Statewide HelpLine", "National Hotline", "Other Local Hotline", "Housing Program", "Sexual Assault Program", "Other DV Program" }; }
		}

		protected override void WriteCsvRecord(CsvWriter csv, ClientInformationReferralSourcesLineItem record) {
			csv.WriteField(record.ClientId);
			csv.WriteField(record.ClientCode);
			csv.WriteField(record.CaseId);
			csv.WriteField(record.ClientStatus);
			csv.WriteField(record.Police);
			csv.WriteField(record.Hospital);
			csv.WriteField(record.Medical);
			csv.WriteField(record.MedicalAdvocacyProgram);
			csv.WriteField(record.LegalSystem);
			csv.WriteField(record.Clergy);
			csv.WriteField(record.SocialServiceProgram);
			csv.WriteField(record.EducationSystem);
			csv.WriteField(record.Friend);
			csv.WriteField(record.Relative);
			csv.WriteField(record.Self);
			csv.WriteField(record.Other);
			csv.WriteField(record.PrivateAttorney);
			csv.WriteField(record.PublicHealth);
			csv.WriteField(record.Media);
			csv.WriteField(record.StateAttorney);
			csv.WriteField(record.CircuitClerk);
			csv.WriteField(record.DCFS);
			csv.WriteField(record.ChildAdvocacyCenter);
			csv.WriteField(record.OtherRapeCrisisCenter);
			csv.WriteField(record.StatewideHelpLine);
			csv.WriteField(record.NationalHotline);
			csv.WriteField(record.OtherLocalHotline);
			csv.WriteField(record.HousingProgram);
			csv.WriteField(record.SexualAssualtProgram);
			csv.WriteField(record.OtherDVProgram);
		}

		protected override IEnumerable<ClientInformationReferralSourcesLineItem> PerformSelect(IQueryable<ClientCase> query) {
			// If Provider == DV
			switch (ReportContainer.Provider) {
				case Provider.DV:
					query = query.Where(m => m.Client.ClientTypeId == 1);
					break;
				case Provider.CAC:
					query = query.Where(m => m.Client.ClientTypeId == 7);
					break;
			}

			return query.Select(m => new ClientInformationReferralSourcesLineItem {
				ClientId = m.ClientId,
				ClientCode = m.Client.ClientCode,
				CaseId = m.CaseId,
				ClientTypeId = m.Client.ClientTypeId,
				ClientStatus = m.Client.ClientCases.GroupBy(cc => cc.ClientId).Select(cc => cc.Min(c => c.FirstContactDate)).FirstOrDefault().Value >= ReportContainer.StartDate && m.Client.ClientCases.GroupBy(cc => cc.ClientId).Select(cc => cc.Min(c => c.FirstContactDate)).FirstOrDefault().Value <= ReportContainer.EndDate ? ReportTableHeaderEnum.New : ReportTableHeaderEnum.Ongoing,
				Police = m.ClientReferralSource.Police,
				Hospital = m.ClientReferralSource.Hospital,
				Medical = m.ClientReferralSource.Medical,
				MedicalAdvocacyProgram = m.ClientReferralSource.MedicalAdvocacyProgram,
				LegalSystem = m.ClientReferralSource.LegalSystem,
				Clergy = m.ClientReferralSource.Clergy,
				SocialServiceProgram = m.ClientReferralSource.SocialServiceProgram,
				EducationSystem = m.ClientReferralSource.EducationSystem,
				Friend = m.ClientReferralSource.Friend,
				Relative = m.ClientReferralSource.Relative,
				Self = m.ClientReferralSource.Self,
				Other = m.ClientReferralSource.Other,
				PrivateAttorney = m.ClientReferralSource.PrivateAttorney,
				PublicHealth = m.ClientReferralSource.PublicHealth,
				Media = m.ClientReferralSource.Media,
				StateAttorney = m.ClientReferralSource.StateAttorney,
				CircuitClerk = m.ClientReferralSource.CircuitClerk,
				DCFS = m.ClientReferralSource.DCFS,
				ChildAdvocacyCenter = m.ClientReferralSource.ChildAdvocacyCenter,
				OtherRapeCrisisCenter = m.ClientReferralSource.OtherRapeCrisisCenter,
				StatewideHelpLine = m.ClientReferralSource.StatewideHelpLine,
				NationalHotline = m.ClientReferralSource.NationalHotline,
				OtherLocalHotline = m.ClientReferralSource.OtherLocalHotline,
				CenterHotline = m.ClientReferralSource.Hotline,
				HousingProgram = m.ClientReferralSource.HousingProgram,
				SexualAssualtProgram = m.ClientReferralSource.SexualAssaultProgram,
				OtherDVProgram = m.ClientReferralSource.OtherDVProgram
			});
		}

		protected override void CreateReportTables() {
			//All
			var newAndOnGoingAllClientTypes = GetNewAndOngoingHeaders();
			//DV
			var newAndOnGoingAdult = GetNewAndOngoingHeaders(ReportTableSubHeaderEnum.Adult, false);
			//CAC
			var newAndOnGoingChildVictim = GetNewAndOngoingHeaders(ReportTableSubHeaderEnum.ChildVictim, false);

			if (ReportContainer.Provider == Provider.DV) {
				var referralSourceGroup = new ReferralSourceReportTable("Referral Source", 1);
				referralSourceGroup.Headers = newAndOnGoingAdult;
				referralSourceGroup.HideSubheaders = true;
				referralSourceGroup.Rows.Add(new ReportRow { Title = "Child Advocacy Center", Code = 0 });
				referralSourceGroup.Rows.Add(new ReportRow { Title = "Circuit Clerk", Code = 1 });
				referralSourceGroup.Rows.Add(new ReportRow { Title = "Clergy", Code = 2 });
				referralSourceGroup.Rows.Add(new ReportRow { Title = "DCFS", Code = 4 });
				referralSourceGroup.Rows.Add(new ReportRow { Title = "Education System", Code = 5 });
				referralSourceGroup.Rows.Add(new ReportRow { Title = "Friend", Code = 6 });
				referralSourceGroup.Rows.Add(new ReportRow { Title = "Hospital", Code = 7 });
				referralSourceGroup.Rows.Add(new ReportRow { Title = "Housing Program", Code = 8 });
				referralSourceGroup.Rows.Add(new ReportRow { Title = "Illinois DV Helpline", Code = 9 });
				referralSourceGroup.Rows.Add(new ReportRow { Title = "Legal System", Code = 11 });
				referralSourceGroup.Rows.Add(new ReportRow { Title = "Media", Code = 12 });
				referralSourceGroup.Rows.Add(new ReportRow { Title = "Medical", Code = 13 });
				referralSourceGroup.Rows.Add(new ReportRow { Title = "Medical Advocacy Program", Code = 14 });
				referralSourceGroup.Rows.Add(new ReportRow { Title = "National DV Hotline", Code = 16 });
				referralSourceGroup.Rows.Add(new ReportRow { Title = "Police", Code = 17 });
				referralSourceGroup.Rows.Add(new ReportRow { Title = "Private Attorney", Code = 18 });
				referralSourceGroup.Rows.Add(new ReportRow { Title = "Public Health", Code = 19 });
				referralSourceGroup.Rows.Add(new ReportRow { Title = "Relative", Code = 20 });
				referralSourceGroup.Rows.Add(new ReportRow { Title = "Self", Code = 21 });
				referralSourceGroup.Rows.Add(new ReportRow { Title = "Sexual Assault Program", Code = 22 });
				referralSourceGroup.Rows.Add(new ReportRow { Title = "Social Services Program", Code = 23 });
				referralSourceGroup.Rows.Add(new ReportRow { Title = "State's Attorney", Code = 24 });
				referralSourceGroup.Rows.Add(new ReportRow { Title = "Other", Code = 25 });
				referralSourceGroup.Rows.Add(new ReportRow { Title = "Other DV Program", Code = 26 });
				referralSourceGroup.Rows.Add(new ReportRow { Title = "Other Local Hotline", Code = 27 });
				referralSourceGroup.Rows.Add(new ReportRow { Title = "Unassigned", Code = null });
				ReportTableList.Add(referralSourceGroup);
			} else if (ReportContainer.Provider == Provider.SA) {
				var referralSourceGroup = new ReferralSourceReportTable("Referral Source", 1);
				referralSourceGroup.Headers = newAndOnGoingAllClientTypes;
				referralSourceGroup.Rows.Add(new ReportRow { Title = "Child Advocacy Center", Code = 0 });
				referralSourceGroup.Rows.Add(new ReportRow { Title = "Center Hotline", Code = 3 });
				referralSourceGroup.Rows.Add(new ReportRow { Title = "Clergy", Code = 2 });
				referralSourceGroup.Rows.Add(new ReportRow { Title = "DCFS", Code = 4 });
				referralSourceGroup.Rows.Add(new ReportRow { Title = "Education System", Code = 5 });
				referralSourceGroup.Rows.Add(new ReportRow { Title = "Friend", Code = 6 });
				referralSourceGroup.Rows.Add(new ReportRow { Title = "Hospital", Code = 7 });
				referralSourceGroup.Rows.Add(new ReportRow { Title = "Housing Program", Code = 8 });
				referralSourceGroup.Rows.Add(new ReportRow { Title = "Legal System, State's Attorney", Code = 24 });
				referralSourceGroup.Rows.Add(new ReportRow { Title = "Media", Code = 12 });
				referralSourceGroup.Rows.Add(new ReportRow { Title = "Police", Code = 17 });
				referralSourceGroup.Rows.Add(new ReportRow { Title = "Private Attorney", Code = 18 });
				referralSourceGroup.Rows.Add(new ReportRow { Title = "Public Health", Code = 19 });
				referralSourceGroup.Rows.Add(new ReportRow { Title = "Relative", Code = 20 });
				referralSourceGroup.Rows.Add(new ReportRow { Title = "Self", Code = 21 });
				referralSourceGroup.Rows.Add(new ReportRow { Title = "Social Services Program", Code = 23 });
				referralSourceGroup.Rows.Add(new ReportRow { Title = "Other", Code = 25 });
				referralSourceGroup.Rows.Add(new ReportRow { Title = "Other Medical", Code = 13 });
				referralSourceGroup.Rows.Add(new ReportRow { Title = "Other Rape Crisis Center", Code = 29 });
				referralSourceGroup.Rows.Add(new ReportRow { Title = "Unassigned", Code = null });
				ReportTableList.Add(referralSourceGroup);
			} else if (ReportContainer.Provider == Provider.CAC) {
				var referralSourceGroup = new ReferralSourceReportTable("Referral Source", 1);
				referralSourceGroup.Headers = newAndOnGoingChildVictim;
				referralSourceGroup.HideSubheaders = true;
				referralSourceGroup.Rows.Add(new ReportRow { Title = "DCFS", Code = 4 });
				referralSourceGroup.Rows.Add(new ReportRow { Title = "Law Enforcement", Code = 17 });
				referralSourceGroup.Rows.Add(new ReportRow { Title = "Medical Provider", Code = 13 });
				referralSourceGroup.Rows.Add(new ReportRow { Title = "State's Attorney", Code = 24 });
				referralSourceGroup.Rows.Add(new ReportRow { Title = "Other", Code = 25 });
				referralSourceGroup.Rows.Add(new ReportRow { Title = "Unassigned", Code = null });
				ReportTableList.Add(referralSourceGroup);
			}
		}
	}

	public class ClientInformationReferralSourcesLineItem {
		public int? ClientId { get; set; }
		public string ClientCode { get; set; }
		public int? CaseId { get; set; }
		public ReportTableHeaderEnum ClientStatus { get; set; }
		public bool? Police { get; set; }
		public bool? Hospital { get; set; }
		public bool? Medical { get; set; }
		public bool? MedicalAdvocacyProgram { get; set; }
		public bool? LegalSystem { get; set; }
		public bool? Clergy { get; set; }
		public bool? SocialServiceProgram { get; set; }
		public bool? EducationSystem { get; set; }
		public bool? Friend { get; set; }
		public bool? Relative { get; set; }
		public bool? Self { get; set; }
		public bool? Other { get; set; }
		public bool? PrivateAttorney { get; set; }
		public bool? PublicHealth { get; set; }
		public bool? Media { get; set; }
		public bool? StateAttorney { get; set; }
		public bool? CircuitClerk { get; set; }
		public bool? DCFS { get; set; }
		public bool? ChildAdvocacyCenter { get; set; }
		public bool? OtherRapeCrisisCenter { get; set; }
		public bool? StatewideHelpLine { get; set; }
		public bool? NationalHotline { get; set; }
		public bool? OtherLocalHotline { get; set; }
		public bool? HousingProgram { get; set; }
		public bool? SexualAssualtProgram { get; set; }
		public bool? OtherDVProgram { get; set; }
		public int? ClientTypeId { get; set; }
		public bool? CenterHotline { get; set; }
	}
}