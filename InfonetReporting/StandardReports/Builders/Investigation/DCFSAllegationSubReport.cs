using System.Collections.Generic;
using System.Linq;
using Infonet.Core.IO;
using Infonet.Data.Looking;
using Infonet.Data.Models.Clients;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.ReportTables.Investigation.DCFSAllegations;

namespace Infonet.Reporting.StandardReports.Builders.Investigation {
	public class DCFSAllegationSubReportBuilder : SubReportCountBuilder<DCFSAllegation, InvestigationDCFSAllegationLineItem> {
		public DCFSAllegationSubReportBuilder(SubReportSelection subReportSelectionType) : base(subReportSelectionType) { }

		protected override IEnumerable<InvestigationDCFSAllegationLineItem> PerformSelect(IQueryable<DCFSAllegation> query) {
			query = query.Where(q => q.ClientCase.Client.ClientTypeId == (int)ClientTypeEnum.CACChildVictim);
			return query.Select(q => new InvestigationDCFSAllegationLineItem {
				Id = q.Id,
				ClientId = q.ClientId,
				ClientCode = q.ClientCase.Client.ClientCode,
				CaseId = q.CaseId,
				ClientStatus = q.ClientCase.Client.ClientCases.GroupBy(cc => cc.ClientId).Select(cc => cc.Min(c => c.FirstContactDate)).FirstOrDefault().Value >= ReportContainer.StartDate && q.ClientCase.Client.ClientCases.GroupBy(cc => cc.ClientId).Select(cc => cc.Min(c => c.FirstContactDate)).FirstOrDefault().Value <= ReportContainer.EndDate ? ReportTableHeaderEnum.New : ReportTableHeaderEnum.Ongoing,
				AbuseAllegationId = q.AbuseAllegationId,
				FindingId = q.FindingId
			});
		}

		protected override string[] CsvHeaders {
			get { return new[] { "ID", "Client ID", "Case ID", "Client Status", "Abuse Allegation", "Finding" }; }
		}

		protected override void WriteCsvRecord(CsvWriter csv, InvestigationDCFSAllegationLineItem record) {
			csv.WriteField(record.Id);
			csv.WriteField(record.ClientCode);
			csv.WriteField(record.CaseId);
			csv.WriteField(record.ClientStatus);
			csv.WriteField(Lookups.AbuseAllegation[record.AbuseAllegationId]?.Description);
			csv.WriteField(Lookups.AbuseAllegationFinding[record.FindingId]?.Description);
		}

		protected override void CreateReportTables() {
			var totalVictimsGroup = new InvestigationDCFSAllegationTotalVictimsReportTable("Total Number of Victims", 1) {
				HideTitle = true,
				HideSubtotal = true,
				HideSubheaders = true,
				Headers = GetNewAndOngoingHeaders(ReportTableSubHeaderEnum.ChildVictim, false)
			};
			totalVictimsGroup.Rows.Add(new ReportRow { Title = "Total Number of Victims" });
			ReportTableList.Add(totalVictimsGroup);

			var totalVictimCasesGroup = new InvestigationDCFSAllegationTotalVictimCasesReportTable("Total Number of Victim Cases", 2) {
				HideTitle = true,
				HideSubtotal = true,
				HideSubheaders = true,
				Headers = GetNewAndOngoingHeaders(ReportTableSubHeaderEnum.ChildVictim, false)
			};
			totalVictimCasesGroup.Rows.Add(new ReportRow { Title = "Total Number of Victim Cases" });
			ReportTableList.Add(totalVictimCasesGroup);

			var abuseAllegationsGroup = new InvestigationDCFSAllegationAbuseAllegationReportTable("DCFS Allegations", 3) {
				HideSubheaders = true,
				Headers = GetNewAndOngoingHeaders(ReportTableSubHeaderEnum.ChildVictim, false)
			};
			foreach (var item in Lookups.AbuseAllegation)
				abuseAllegationsGroup.Rows.Add(GetReportRowFromLookup(item));
			ReportTableList.Add(abuseAllegationsGroup);

			var findingsGroup = new InvestigationDCFSAllegationFindingReportTable("DCFS Findings", 4) {
				HideSubheaders = true,
				Headers = GetNewAndOngoingHeaders(ReportTableSubHeaderEnum.ChildVictim, false)
			};
			foreach (var item in Lookups.AbuseAllegationFinding)
				findingsGroup.Rows.Add(GetReportRowFromLookup(item));
			ReportTableList.Add(findingsGroup);
		}
	}

	public class InvestigationDCFSAllegationLineItem {
		public int? Id { get; set; }
		public int? ClientId { get; set; }
		public string ClientCode { get; set; }
		public int? CaseId { get; set; }
		public ReportTableHeaderEnum ClientStatus { get; set; }
		public int? AbuseAllegationId { get; set; }
		public int? FindingId { get; set; }
	}
}