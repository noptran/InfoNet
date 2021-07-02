using System.Collections.Generic;
using System.Linq;
using Infonet.Core.IO;
using Infonet.Data.Looking;
using Infonet.Data.Models.Clients;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.ReportTables.Medical.PoliceInvolvement;

namespace Infonet.Reporting.StandardReports.Builders.MedicalCJ {
	public class MedicalCJPoliceInvolvementMedicalCJSubReport : SubReportCountBuilder<ClientPoliceProsecution, MedicalCJPoliceInvolvementCJLineItem> {
		public MedicalCJPoliceInvolvementMedicalCJSubReport(SubReportSelection subReportSelectionType) : base(subReportSelectionType) {
			IsInGroup = true;
		}

		protected override string[] CsvHeaders {
			get { return new[] { "ID", "Client ID", "Case ID", "Client Status", "Reported to Police", "Patrol Interview", "Detective Interview" }; }
		}

		protected override void WriteCsvRecord(CsvWriter csv, MedicalCJPoliceInvolvementCJLineItem record) {
			csv.WriteField(record.ClientID);
			csv.WriteField(record.ClientCode);
			csv.WriteField(record.CaseID);
			csv.WriteField(record.ClientStatus);
			csv.WriteField(record.ReportedToPolice);
			csv.WriteField(record.PatrolInterview);
			csv.WriteField(record.DetectiveInterview);
		}

		protected override void CreateReportTables() {
			var newAndOngoingTotal = GetNewAndOngoingHeaders(ReportTableSubHeaderEnum.Total, false);

			if (ReportContainer.Provider != Provider.CAC) {
				var reportedPoliceGroup = new ReportedToPoliceReportTable("Reported To Police", 1);
				reportedPoliceGroup.HideSubheaders = true;
				reportedPoliceGroup.HideSubtotal = true;
				reportedPoliceGroup.Headers = newAndOngoingTotal;
				reportedPoliceGroup.Rows.Add(new ReportRow { Code = 1, Title = "Yes" });
				ReportTableList.Add(reportedPoliceGroup);

				var patrolGroup = new PatrolInterviewReportTable("Patrol Interview with Victim", 2);
				patrolGroup.HideSubheaders = true;
				patrolGroup.HideSubtotal = true;
				patrolGroup.Headers = newAndOngoingTotal;
				patrolGroup.Rows.Add(new ReportRow { Code = 1, Title = "Yes" });
				ReportTableList.Add(patrolGroup);

				var detectiveGroup = new DetectiveInterviewReportTable("Detective Interview with Victim", 3);
				detectiveGroup.HideSubheaders = true;
				detectiveGroup.HideSubtotal = true;
				detectiveGroup.Headers = newAndOngoingTotal;
				detectiveGroup.Rows.Add(new ReportRow { Code = 1, Title = "Yes" });
				ReportTableList.Add(detectiveGroup);
			}
		}

		protected override IEnumerable<MedicalCJPoliceInvolvementCJLineItem> PerformSelect(IQueryable<ClientPoliceProsecution> query) {
			if (ReportContainer.Provider == Provider.DV)
				query = query.Where(q => q.ClientCase.Client.ClientTypeId == (int)ClientTypeEnum.DVAdult);
			else if (ReportContainer.Provider == Provider.SA)
				query = query.Where(q => q.ClientCase.Client.ClientTypeId == (int)ClientTypeEnum.SAVictim);

			return query.Select(q => new MedicalCJPoliceInvolvementCJLineItem {
				ClientID = q.ClientId,
				ClientCode = q.ClientCase.Client.ClientCode,
				CaseID = q.CaseId,
				ClientStatus = q.ClientCase.Client.ClientCases.GroupBy(c => c.ClientId).Select(c => c.Min(c2 => c2.FirstContactDate)).FirstOrDefault().Value >= ReportContainer.StartDate && q.ClientCase.Client.ClientCases.GroupBy(c => q.ClientId).Select(c => c.Min(c2 => c2.FirstContactDate)).FirstOrDefault().Value <= ReportContainer.EndDate ? ReportTableHeaderEnum.New : ReportTableHeaderEnum.Ongoing,
				ReportedToPolice = q.DateReportPolice.HasValue,
				PatrolInterview = q.PatrolInterview ?? false,
				DetectiveInterview = q.DetectiveInterview ?? false
			});
		}
	}

	public class MedicalCJPoliceInvolvementCJLineItem {
		public int? ClientID { get; set; }
		public string ClientCode { get; set; }
		public int? CaseID { get; set; }
		public ReportTableHeaderEnum ClientStatus { get; set; }
		public bool ReportedToPolice { get; set; }
		public bool DetectiveInterview { get; set; }
		public bool PatrolInterview { get; set; }
	}
}