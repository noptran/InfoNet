using System.Collections.Generic;
using System.Linq;
using Infonet.Core.Collections;
using Infonet.Core.IO;
using Infonet.Data.Looking;
using Infonet.Data.Models.Clients;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.ManagementReports.ReportTables.Client;

namespace Infonet.Reporting.ManagementReports.Builders {
	public class ClientChildBehavioralSubReport : SubReportCountBuilder<ChildBehavioralIssues, ClientChildBehavioralIssuesLineItem> {
		public ClientChildBehavioralSubReport(SubReportSelection subReportType) : base(subReportType) { }

		protected override IEnumerable<ClientChildBehavioralIssuesLineItem> PerformSelect(IQueryable<ChildBehavioralIssues> query) {
			query = query.Where(q => q.ClientCase.Client.ClientTypeId == (int)ClientTypeEnum.DVChild);
			return query.Select(q => new ClientChildBehavioralIssuesLineItem {
				CaseId = q.CaseId,
				ClientId = q.ClientId,
				ClientCode = q.ClientCase.Client.ClientCode,
				ClientStatus = q.ClientCase.Client.ClientCases.GroupBy(cc => cc.ClientId).Select(cc => cc.Min(c => c.FirstContactDate)).FirstOrDefault().Value >= ReportContainer.StartDate && q.ClientCase.Client.ClientCases.GroupBy(cc => cc.ClientId).Select(cc => cc.Min(c => c.FirstContactDate)).FirstOrDefault().Value <= ReportContainer.EndDate ? ReportTableHeaderEnum.New : ReportTableHeaderEnum.Ongoing,
				AbuseAlcohol = q.AbuseAlcohol,
				Accepts = q.Accepts,
				AbuseDrugs = q.AbuseDrugs,
				Afraid = q.Afraid,
				BedWet = q.BedWet,
				BehavesYoung = q.BehavesYoung,
				BehaviorProblems = q.BehaviorProblems,
				CantLeave = q.CantLeave,
				Cries = q.Cries,
				DropOut = q.DropOut,
				Fire = q.Fire,
				HarmsAnimals = q.HarmsAnimals,
				HitsKicksBites = q.HitsKicksBites,
				HurtsSelf = q.HurtsSelf,
				Illnesses = q.Illnesses,
				LearningProblems = q.LearningProblems,
				MissSchool = q.MissSchool,
				Mood = q.Mood,
				MoreActive = q.MoreActive,
				Nightmares = q.Nightmares,
				NoInteract = q.NoInteract,
				Possessive = q.Possessive,
				Protective = q.Protective,
				Resists = q.Resists,
				RoleReversal = q.RoleReversal,
				SchoolRules = q.SchoolRules,
				SpecClassBeh = q.SpecialClassBehavioral,
				SpecClassLearn = q.SpecialClassLearning,
				SpecialClassActive = q.SpecialClassActive,
				Suicidal = q.Suicidal,
				Weight = q.Weight
			});
		}

		protected override string[] CsvHeaders {
			get { return new[] { "Client ID", "Case ID", "Client Status", "Abuses Alcohol", "Abuses Drugs", "Accepts w/o Question", "Is Often afraid", "Bed Wets", "Behaves Young", "Behavior Problems", "Can't Leave Parent", "Cries Often", "Drop Out", "Plays with Fire", "Harms Animals", "Hits Kicks Bites", "Hurts Self", "Illness Often", "Learning Problems", "Misses School", "Mood Swings", "More Active", "Nightmares", "Little Interaction", "Possessive", "Protective", "Resists", "Role Reversal", "Special Class Behavioral Problems", "Special Class Learning Problems", "Special Class Active", "Suicidal", "Weight Problem" }; }
		}

		protected override void WriteCsvRecord(CsvWriter csv, ClientChildBehavioralIssuesLineItem record) {
			csv.WriteField(record.ClientCode);
			csv.WriteField(record.CaseId);
			csv.WriteField(record.ClientStatus);
			csv.WriteField(record.AbuseAlcohol);
			csv.WriteField(record.AbuseDrugs);
			csv.WriteField(record.Accepts);
			csv.WriteField(record.Afraid);
			csv.WriteField(record.BedWet);
			csv.WriteField(record.BehavesYoung);
			csv.WriteField(record.BehaviorProblems);
			csv.WriteField(record.CantLeave);
			csv.WriteField(record.Cries);
			csv.WriteField(record.DropOut);
			csv.WriteField(record.Fire);
			csv.WriteField(record.HarmsAnimals);
			csv.WriteField(record.HitsKicksBites);
			csv.WriteField(record.HurtsSelf);
			csv.WriteField(record.Illnesses);
			csv.WriteField(record.LearningProblems);
			csv.WriteField(record.MissSchool);
			csv.WriteField(record.Mood);
			csv.WriteField(record.MoreActive);
			csv.WriteField(record.Nightmares);
			csv.WriteField(record.NoInteract);
			csv.WriteField(record.Possessive);
			csv.WriteField(record.Protective);
			csv.WriteField(record.Resists);
			csv.WriteField(record.RoleReversal);
			csv.WriteField(record.SpecClassBeh);
			csv.WriteField(record.SpecClassLearn);
			csv.WriteField(record.SpecialClassActive);
			csv.WriteField(record.Suicidal);
			csv.WriteField(record.Weight);
		}

		protected override void CreateReportTables() {
			var headers = GetHeaders();

			var behaviorIssuesCount = new ClientChildrenWithBehavioralIssuesCountReportTable("", 1) {
				Headers = headers,
				HideSubheaders = true,
				HideSubtotal = true
			};
			behaviorIssuesCount.Rows.Add(new ReportRow { Title = "Total Number Of Children With a Behavioral Issue", Code = null });
			ReportTableList.Add(behaviorIssuesCount);

			var childBehavioralIssueCount = new ClientChildBehavioralIssuesReportTable("Behavioral Issue", 2) {
				Headers = headers,
				HideSubheaders = true
			};
			foreach (var item in Lookups.ChildBehavioralIssues[ReportContainer.Provider])
				childBehavioralIssueCount.Rows.Add(new ReportRow { Title = ((ChildBehaviorsEnum)item.CodeId).GetDisplayName(), Code = item.CodeId });
			ReportTableList.Add(childBehavioralIssueCount);
		}

		private List<ReportTableHeader> GetHeaders() {
			return new List<ReportTableHeader> {
				new ReportTableHeader { Code = ReportTableHeaderEnum.New, Title = "New", SubHeaders = new List<ReportTableSubHeader> { new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.Total, Title = string.Empty } } },
				new ReportTableHeader { Code = ReportTableHeaderEnum.Ongoing, Title = "Ongoing", SubHeaders = new List<ReportTableSubHeader> { new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.Total, Title = string.Empty } } },
				new ReportTableHeader { Code = ReportTableHeaderEnum.Total, Title = "Total", SubHeaders = new List<ReportTableSubHeader> { new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.Total, Title = string.Empty } } }
			};
		}
	}

	public class ClientChildBehavioralIssuesLineItem {
		public int? ClientId { get; set; }
		public int? CaseId { get; set; }
		public ReportTableHeaderEnum ClientStatus { get; set; }
		public bool Afraid { get; set; }
		public bool CantLeave { get; set; }
		public bool Accepts { get; set; }
		public bool Cries { get; set; }
		public bool Mood { get; set; }
		public bool NoInteract { get; set; }
		public bool Nightmares { get; set; }
		public bool HurtsSelf { get; set; }
		public bool Suicidal { get; set; }
		public bool BedWet { get; set; }
		public bool Illnesses { get; set; }
		public bool Weight { get; set; }
		public bool MoreActive { get; set; }
		public bool SpecialClassActive { get; set; }
		public bool AbuseDrugs { get; set; }
		public bool AbuseAlcohol { get; set; }
		public bool Fire { get; set; }
		public bool RoleReversal { get; set; }
		public bool Protective { get; set; }
		public bool Resists { get; set; }
		public bool Possessive { get; set; }
		public bool HitsKicksBites { get; set; }
		public bool BehavesYoung { get; set; }
		public bool HarmsAnimals { get; set; }
		public bool MissSchool { get; set; }
		public bool DropOut { get; set; }
		public bool SchoolRules { get; set; }
		public bool BehaviorProblems { get; set; }
		public bool SpecClassBeh { get; set; }
		public bool LearningProblems { get; set; }
		public bool SpecClassLearn { get; set; }
		public string ClientCode { get; set; }
	}
}