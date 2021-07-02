using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.ManagementReports.Builders;

namespace Infonet.Reporting.ManagementReports.ReportTables.Client {
	public class ClientChildBehavioralIssuesReportTable : ReportTable<ClientChildBehavioralIssuesLineItem> {
		public ClientChildBehavioralIssuesReportTable(string title, int displayOrder) : base(title, displayOrder) { }

		public override void CheckAndApply(ClientChildBehavioralIssuesLineItem item) {
			foreach (var row in Rows) {
				bool fitsThisCategory = false;
				switch ((ChildBehaviorsEnum)row.Code) {
					case ChildBehaviorsEnum.AbuseAlcohol:
						fitsThisCategory = item.AbuseAlcohol;
						break;
					case ChildBehaviorsEnum.AbuseDrugs:
						fitsThisCategory = item.AbuseDrugs;
						break;
					case ChildBehaviorsEnum.Accepts:
						fitsThisCategory = item.Accepts;
						break;
					case ChildBehaviorsEnum.Afraid:
						fitsThisCategory = item.Afraid;
						break;
					case ChildBehaviorsEnum.BedWet:
						fitsThisCategory = item.BedWet;
						break;
					case ChildBehaviorsEnum.BehavesYoung:
						fitsThisCategory = item.BehavesYoung;
						break;
					case ChildBehaviorsEnum.BehaviorProblems:
						fitsThisCategory = item.BehaviorProblems;
						break;
					case ChildBehaviorsEnum.CantLeave:
						fitsThisCategory = item.CantLeave;
						break;
					case ChildBehaviorsEnum.Cries:
						fitsThisCategory = item.Cries;
						break;
					case ChildBehaviorsEnum.DropOut:
						fitsThisCategory = item.DropOut;
						break;
					case ChildBehaviorsEnum.Fire:
						fitsThisCategory = item.Fire;
						break;
					case ChildBehaviorsEnum.HarmsAnimals:
						fitsThisCategory = item.HarmsAnimals;
						break;
					case ChildBehaviorsEnum.HitsKicksBites:
						fitsThisCategory = item.HitsKicksBites;
						break;
					case ChildBehaviorsEnum.HurtsSelf:
						fitsThisCategory = item.HurtsSelf;
						break;
					case ChildBehaviorsEnum.Illnesses:
						fitsThisCategory = item.Illnesses;
						break;
					case ChildBehaviorsEnum.LearningProblems:
						fitsThisCategory = item.LearningProblems;
						break;
					case ChildBehaviorsEnum.MissSchool:
						fitsThisCategory = item.MissSchool;
						break;
					case ChildBehaviorsEnum.Mood:
						fitsThisCategory = item.Mood;
						break;
					case ChildBehaviorsEnum.MoreActive:
						fitsThisCategory = item.MoreActive;
						break;
					case ChildBehaviorsEnum.Nightmares:
						fitsThisCategory = item.Nightmares;
						break;
					case ChildBehaviorsEnum.NoInteract:
						fitsThisCategory = item.NoInteract;
						break;
					case ChildBehaviorsEnum.Possessive:
						fitsThisCategory = item.Possessive;
						break;
					case ChildBehaviorsEnum.Protective:
						fitsThisCategory = item.Protective;
						break;
					case ChildBehaviorsEnum.Resists:
						fitsThisCategory = item.Resists;
						break;
					case ChildBehaviorsEnum.RoleReversal:
						fitsThisCategory = item.RoleReversal;
						break;
					case ChildBehaviorsEnum.SchoolRules:
						fitsThisCategory = item.SchoolRules;
						break;
					case ChildBehaviorsEnum.SpecClassBeh:
						fitsThisCategory = item.SpecClassBeh;
						break;
					case ChildBehaviorsEnum.SpecClassLearn:
						fitsThisCategory = item.SpecClassLearn;
						break;
					case ChildBehaviorsEnum.SpecialClassActive:
						fitsThisCategory = item.SpecialClassActive;
						break;
					case ChildBehaviorsEnum.Suicidal:
						fitsThisCategory = item.Suicidal;
						break;
					case ChildBehaviorsEnum.Weight:
						fitsThisCategory = item.Weight;
						break;
				}
				if (fitsThisCategory)
					foreach (var header in Headers)
						if (item.ClientStatus == header.Code || header.Code == ReportTableHeaderEnum.Total)
							foreach (var subheader in header.SubHeaders)
								row.Counts[header.Code.ToString()][subheader.Code.ToString()] += 1;
			}
		}
	}
}