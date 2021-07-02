using System.Collections.Generic;
using System.Linq;
using Infonet.Core.IO;
using Infonet.Data.Looking;
using Infonet.Data.Models.Clients;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.ReportTables.Investigation.AbuseNeglectPetitions;

namespace Infonet.Reporting.StandardReports.Builders.Investigation {
	public class AbuseNeglectPetitonSubReportBuilder : SubReportCountBuilder<AbuseNeglectPetition, AbuseNeglectPetitionLineItem> {
		public AbuseNeglectPetitonSubReportBuilder(SubReportSelection subReportSelectionType) : base(subReportSelectionType) { }

		protected override IEnumerable<AbuseNeglectPetitionLineItem> PerformSelect(IQueryable<AbuseNeglectPetition> query) {
			int? clientType = (int)ClientTypeEnum.CACChildVictim;
			query = query.Where(q => q.ClientCase.Client.ClientTypeId == clientType);
			return query.Select(q => new AbuseNeglectPetitionLineItem {
				Id = q.Id,
				ClientId = q.ClientId,
				ClientCode = q.ClientCase.Client.ClientCode,
				CaseId = q.CaseId,
				ClientStatus = q.ClientCase.Client.ClientCases.GroupBy(cc => cc.ClientId).Select(cc => cc.Min(c => c.FirstContactDate)).FirstOrDefault().Value >= ReportContainer.StartDate && q.ClientCase.Client.ClientCases.GroupBy(cc => cc.ClientId).Select(cc => cc.Min(c => c.FirstContactDate)).FirstOrDefault().Value <= ReportContainer.EndDate ? ReportTableHeaderEnum.New : ReportTableHeaderEnum.Ongoing,
				AbuseNeglectPetitionId = q.AbuseNeglectPetitionId,
				AdjudicatedId = q.AdjudicatedId
			});
		}

		protected override string[] CsvHeaders {
			get { return new[] { "ID", "Client ID", "Case ID", "Client Status", "Petition", "Adjudication" }; }
		}

		protected override void WriteCsvRecord(CsvWriter csv, AbuseNeglectPetitionLineItem record) {
			csv.WriteField(record.Id);
			csv.WriteField(record.ClientCode);
			csv.WriteField(record.CaseId);
			csv.WriteField(record.ClientStatus);
			csv.WriteField(Lookups.AbuseNeglectPetition[record.AbuseNeglectPetitionId]?.Description);
			csv.WriteField(Lookups.PetitionAdjudication[record.AdjudicatedId]?.Description);
		}

		protected override void CreateReportTables() {
			var totalVictimsGroup = new AbuseNeglectPetitonTotalVictimsReportTable("Total Number of Victims", 1) {
				HideTitle = true,
				HideSubtotal = true,
				HideSubheaders = true,
				Headers = GetNewAndOngoingHeaders(ReportTableSubHeaderEnum.ChildVictim, false)
			};
			totalVictimsGroup.Rows.Add(new ReportRow { Title = "Total Number of Victims" });
			ReportTableList.Add(totalVictimsGroup);

			var totalVictimCasesGroup = new AbuseNeglectPetitonTotalVictimCasesReportTable("Total Number of Victim Cases", 2) {
				HideTitle = true,
				HideSubtotal = true,
				HideSubheaders = true,
				Headers = GetNewAndOngoingHeaders(ReportTableSubHeaderEnum.ChildVictim, false)
			};
			totalVictimCasesGroup.Rows.Add(new ReportRow { Title = "Total Number of Victim Cases" });
			ReportTableList.Add(totalVictimCasesGroup);

			var pettionsGroup = new AbuseNeglectPetitonPetitionReportTable("Petitions", 3) {
				HideSubheaders = true,
				Headers = GetNewAndOngoingHeaders(ReportTableSubHeaderEnum.ChildVictim, false)
			};
			foreach (var item in Lookups.AbuseNeglectPetition)
				pettionsGroup.Rows.Add(GetReportRowFromLookup(item));
			ReportTableList.Add(pettionsGroup);

			var adjudicationGroup = new AbuseNeglectPetitonAdjudicationReportTable("Adjudications", 4) {
				HideSubheaders = true,
				Headers = GetNewAndOngoingHeaders(ReportTableSubHeaderEnum.ChildVictim, false)
			};
			foreach (var item in Lookups.PetitionAdjudication)
				adjudicationGroup.Rows.Add(GetReportRowFromLookup(item));
			ReportTableList.Add(adjudicationGroup);
		}
	}

	public class AbuseNeglectPetitionLineItem {
		public int? Id { get; set; }
		public int? ClientId { get; set; }
		public string ClientCode { get; set; }
		public int? CaseId { get; set; }
		public ReportTableHeaderEnum ClientStatus { get; set; }
		public int? AbuseNeglectPetitionId { get; set; }
		public int? AdjudicatedId { get; set; }
	}
}