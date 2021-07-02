using System.Collections.Generic;
using System.Linq;
using Infonet.Core.IO;
using Infonet.Data.Looking;
using Infonet.Data.Models.Clients;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.ReportTables.Medical.ProsecutionInvolvement;

namespace Infonet.Reporting.StandardReports.Builders.MedicalCJ {
	public class MedicalCJProsecutionInvolvementPoliceProsecutionSubReport : SubReportCountBuilder<ClientPoliceProsecution, MedicalCJPoliceProsecutionLineItem> {
		public MedicalCJProsecutionInvolvementPoliceProsecutionSubReport(SubReportSelection subReportSelectionType) : base(subReportSelectionType) {
			IsInGroup = true;
		}

		protected override string[] CsvHeaders {
			get { return new[] { "ID", "Client ID", "Case ID", "Client Status", "State's Attourney Interview", "Trial Scheduled", "Trial Type", "Court Activities", "Victim/Witness Participation" }; }
		}

		protected override void WriteCsvRecord(CsvWriter csv, MedicalCJPoliceProsecutionLineItem record) {
			csv.WriteField(record.ClientID);
			csv.WriteField(record.ClientCode);
			csv.WriteField(record.CaseID);
			csv.WriteField(record.ClientStatus);
			csv.WriteField(record.SAInterview);
			csv.WriteField(record.TrialScheduled);
			csv.WriteField(Lookups.TrialType[record.TrialType]?.Description);
			csv.WriteField(string.Join("|", record.CourtActivities.Select(ca => Lookups.CourtContinuance[ca]?.Description ?? string.Empty)));
			csv.WriteField(Lookups.VictimWitnessParticipation[record.VWParticipation]?.Description);
		}

		protected override void CreateReportTables() {
			var newAndOngoingTotalOnly = GetNewAndOngoingHeaders(ReportTableSubHeaderEnum.Total, false);

			if (ReportContainer.Provider != Provider.CAC) {
				var saInterviewTable = new ProsecutionStateAttorneyInterviewReportTable("State's Attorney Interview with Victim", 1);
				saInterviewTable.Headers = newAndOngoingTotalOnly;
				saInterviewTable.HideSubheaders = true;
				saInterviewTable.HideSubtotal = true;
				saInterviewTable.Rows.Add(new ReportRow { Title = "Yes", Code = (int)ShortAnswerEnum.Yes });
				ReportTableList.Add(saInterviewTable);

				var trialScheduledtable = new ProsecutionTrialScheduledReportTable("Trial Scheduled", 3);
				trialScheduledtable.Headers = newAndOngoingTotalOnly;
				trialScheduledtable.HideSubheaders = true;
				trialScheduledtable.HideSubtotal = true;
				trialScheduledtable.Rows.Add(new ReportRow { Title = "Yes", Code = (int)ShortAnswerEnum.Yes });
				ReportTableList.Add(trialScheduledtable);

				var trialTypetable = new ProsecutionTrialTypesReportTable("Trial Type", 4);
				trialTypetable.Headers = newAndOngoingTotalOnly;
				trialTypetable.HideSubheaders = true;
				foreach (var item in Lookups.TrialType[ReportContainer.Provider])
					trialTypetable.Rows.Add(GetReportRowFromLookup(item));
				ReportTableList.Add(trialTypetable);

				var courtActivityTable = new ProsecutionCourtActivityReportTable("Court Activity", 5);
				courtActivityTable.Headers = newAndOngoingTotalOnly;
				courtActivityTable.HideSubheaders = true;
				foreach (var item in Lookups.CourtContinuance[ReportContainer.Provider])
					courtActivityTable.Rows.Add(GetReportRowFromLookup(item));
				courtActivityTable.Rows.Add(new ReportRow { Title = "Unassigned", Code = null });
				ReportTableList.Add(courtActivityTable);

				var vwPartipication = new ProsecutionVWParticipationReportTable("Victim/Witness Participation", 6);
				vwPartipication.Headers = newAndOngoingTotalOnly;
				vwPartipication.HideSubheaders = true;
				foreach (var item in Lookups.VictimWitnessParticipation[ReportContainer.Provider])
					vwPartipication.Rows.Add(GetReportRowFromLookup(item));
				ReportTableList.Add(vwPartipication);
			}
		}

		protected override IEnumerable<MedicalCJPoliceProsecutionLineItem> PerformSelect(IQueryable<ClientPoliceProsecution> query) {
			if (ReportContainer.Provider == Provider.DV)
				query = query.Where(q => q.ClientCase.Client.ClientTypeId == (int)ClientTypeEnum.DVAdult);
			else if (ReportContainer.Provider == Provider.SA)
				query = query.Where(q => q.ClientCase.Client.ClientTypeId == (int)ClientTypeEnum.SAVictim);

			return query.Select(q => new MedicalCJPoliceProsecutionLineItem {
				ClientID = q.ClientId,
				ClientCode = q.ClientCase.Client.ClientCode,
				CaseID = q.CaseId,
				ClientStatus = q.ClientCase.Client.ClientCases.GroupBy(c => c.ClientId).Select(c => c.Min(c2 => c2.FirstContactDate)).FirstOrDefault().Value >= ReportContainer.StartDate && q.ClientCase.Client.ClientCases.GroupBy(c => q.ClientId).Select(c => c.Min(c2 => c2.FirstContactDate)).FirstOrDefault().Value <= ReportContainer.EndDate ? ReportTableHeaderEnum.New : ReportTableHeaderEnum.Ongoing,
				SAInterview = q.SAInterview ?? false,
				TrialScheduled = q.TrialScheduled ?? false,
				TrialType = q.TrialTypeId,
				VWParticipation = q.VWParticipateID,
				CourtActivities = q.ClientCase.ClientCourtAppearances.Select(ap => ap.CourtContinuanceID)
			});
		}
	}

	public class MedicalCJPoliceProsecutionLineItem {
		public int? ClientID { get; set; }
		public string ClientCode { get; set; }
		public int? CaseID { get; set; }
		public ReportTableHeaderEnum ClientStatus { get; set; }
		public bool SAInterview { get; set; }
		public bool TrialScheduled { get; set; }
		public int? TrialType { get; set; }
		public int? VWParticipation { get; set; }
		public IEnumerable<int?> CourtActivities { get; internal set; }
	}
}