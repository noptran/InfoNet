using System.Collections.Generic;
using System.Linq;
using Infonet.Core.IO;
using Infonet.Data.Looking;
using Infonet.Data.Models.Clients;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.ReportTables.Investigation.ClientMDT;

namespace Infonet.Reporting.StandardReports.Builders.Investigation {
	public class ClientMDTSubReportBuilder : SubReportCountBuilder<ClientMDT, ClientMDTLineItem> {
		public ClientMDTSubReportBuilder(SubReportSelection suvbReportType) : base(suvbReportType) { }

		protected override string[] CsvHeaders {
			get { return new[] { "ID", "Client ID", "Case ID", "Client Status", "Position" }; }
		}

		protected override void WriteCsvRecord(CsvWriter csv, ClientMDTLineItem record) {
			csv.WriteField(record.Id);
			csv.WriteField(record.ClientCode);
			csv.WriteField(record.CaseId);
			csv.WriteField(record.ClientStatus);
			csv.WriteField(Lookups.TeamMemberPosition[record.PositionId]?.Description);
		}

		protected override void CreateReportTables() {
			var totalVictimsTable = new ClientMDTTotalVictimsReportTable("Total Number of Victims", 1) {
				Headers = GetNewAndOngoingHeaders(ReportTableSubHeaderEnum.ChildVictim, false),
				HideSubtotal = true,
				HideSubheaders = true,
				HideTitle = true
			};
			totalVictimsTable.Rows.Add(new ReportRow { Title = "Total Number of Victims" });
			ReportTableList.Add(totalVictimsTable);

			var totalCasesTable = new ClientMDTTotalCasesReportTable("Total Number of Victim Cases", 2) {
				Headers = GetNewAndOngoingHeaders(ReportTableSubHeaderEnum.ChildVictim, false),
				HideTitle = true,
				HideSubheaders = true,
				HideSubtotal = true
			};
			totalCasesTable.Rows.Add(new ReportRow { Title = "Total Number of Victim Cases" });
			ReportTableList.Add(totalCasesTable);

			var positionTable = new ClientMDTPositionReportTable("MDT Member Positions", 3) {
				Headers = GetNewAndOngoingHeaders(ReportTableSubHeaderEnum.ChildVictim, false),
				HideSubheaders = true
			};
			foreach (var item in Lookups.TeamMemberPosition)
				positionTable.Rows.Add(GetReportRowFromLookup(item));
			ReportTableList.Add(positionTable);
		}

		protected override IEnumerable<ClientMDTLineItem> PerformSelect(IQueryable<ClientMDT> query) {
			query = query.Where(q => q.ClientCase.Client.ClientTypeId == (int)ClientTypeEnum.CACChildVictim);
			return query.Select(q => new ClientMDTLineItem {
				Id = q.MDT_ID,
				ClientId = q.ClientID,
				ClientCode = q.ClientCase.Client.ClientCode,
				CaseId = q.CaseID,
				ClientStatus = q.ClientCase.Client.ClientCases.GroupBy(cc => cc.ClientId).Select(cc => cc.Min(c => c.FirstContactDate)).FirstOrDefault().Value >= ReportContainer.StartDate && q.ClientCase.Client.ClientCases.GroupBy(cc => cc.ClientId).Select(cc => cc.Min(c => c.FirstContactDate)).FirstOrDefault().Value <= ReportContainer.EndDate ? ReportTableHeaderEnum.New : ReportTableHeaderEnum.Ongoing,
				PositionId = q.PositionID
			});
		}
	}

	public class ClientMDTLineItem {
		public int? Id { get; set; }
		public int? ClientId { get; set; }
		public string ClientCode { get; set; }
		public int? CaseId { get; set; }
		public ReportTableHeaderEnum ClientStatus { get; set; }
		public int? PositionId { get; set; }
	}
}