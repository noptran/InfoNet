using System.Collections.Generic;
using System.Linq;
using Infonet.Core.IO;
using Infonet.Data.Looking;
using Infonet.Data.Models.Clients;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.ManagementReports.ReportTables.Client;

namespace Infonet.Reporting.ManagementReports.Builders {
	public class ClientIncomeSourceSubReport : SubReportCountBuilder<ClientCase, IncomeLineItem> {
		public ClientIncomeSourceSubReport(SubReportSelection subReportType) : base(subReportType) { }

		public decimal[] IncomeSourceIncomeRangeLowerBounds { get; set; }

		public decimal?[] IncomeSourceIncomeRangeUpperBounds { get; set; }

		protected override IEnumerable<IncomeLineItem> PerformSelect(IQueryable<ClientCase> query) {
			return query.Where(q => q.Client.ClientTypeId == (int)ClientTypeEnum.DVAdult).Select(q => new IncomeLineItem {
				CaseId = q.CaseId,
				ClientId = q.ClientId,
				ClientCode = q.Client.ClientCode,
				ClientStatus = q.Client.ClientCases.GroupBy(cc => cc.ClientId).Select(cc => cc.Min(c => c.FirstContactDate)).FirstOrDefault().Value >= ReportContainer.StartDate && q.Client.ClientCases.GroupBy(cc => cc.ClientId).Select(cc => cc.Min(c => c.FirstContactDate)).FirstOrDefault().Value <= ReportContainer.EndDate ? ReportTableHeaderEnum.New : ReportTableHeaderEnum.Ongoing,
				PrimaryIncomeSourceId = q.FinancialResources.Where(f => q.FinancialResources.GroupBy(cf => cf.ClientID).Select(cf => cf.Max(a => a.Amount)).FirstOrDefault() == f.Amount && -1 != f.Amount && f.IncomeSource2ID != -1).Select(f => f.IncomeSource2ID).OrderBy(f => f).FirstOrDefault(),
				AnnualIncome = q.FinancialResources.Where(a => -1 != a.Amount && a.IncomeSource2ID != -1).Sum(a => a.Amount) * 12
			});
		}

		protected override string[] CsvHeaders {
			get { return new[] { "Client ID", "Case ID", "Client Status", "Annual Income", "Primary Income Source" }; }
		}

		protected override void WriteCsvRecord(CsvWriter csv, IncomeLineItem record) {
			csv.WriteField(record.ClientCode);
			csv.WriteField(record.CaseId);
			csv.WriteField(record.ClientStatus);
			csv.WriteField(record.AnnualIncome);
			csv.WriteField(Lookups.IncomeSource2[record.PrimaryIncomeSourceId]?.Description);
		}

		protected override void CreateReportTables() {
			var newAndOngoingTotalOnly = GetHeaders();

			var incomeSource = new ClientPrimaryIncomeReportTable("Primary Income Source", 1) {
				Headers = newAndOngoingTotalOnly,
				HideSubheaders = true,
				LowerBounds = IncomeSourceIncomeRangeLowerBounds.First(),
				UpperBounds = IncomeSourceIncomeRangeUpperBounds.Last(),
				LastLowerBounds = IncomeSourceIncomeRangeLowerBounds.Last()
			};
			foreach (var item in Lookups.IncomeSource2[ReportContainer.Provider])
				incomeSource.Rows.Add(GetReportRowFromLookup(item));
			ReportTableList.Add(incomeSource);

			var aggregateIncome = new ClientAnnualIncomeReportTable("Annual Income Ranges", 2) {
				Headers = newAndOngoingTotalOnly,
				HideSubheaders = true,
				LowerBounds = IncomeSourceIncomeRangeLowerBounds,
				UpperBounds = IncomeSourceIncomeRangeUpperBounds
			};
			for (int i = 0; i < IncomeSourceIncomeRangeLowerBounds.Length; i++) {
				string low = "$" + IncomeSourceIncomeRangeLowerBounds[i];
				string high = IncomeSourceIncomeRangeUpperBounds[i] == null ? " and up" : " -- $" + IncomeSourceIncomeRangeUpperBounds[i];
				aggregateIncome.Rows.Add(new ReportRow { Title = low + high, Code = i, Order = i });
			}
            aggregateIncome.Footer = "Note: Clients with <b>No Financial Resources</b> count as <b>$0</b> income in the <b>Annual Income Ranges</b> table. Since these clients have no <b>Primary Income Source</b>, subtotals in the two tables may not match. Subtotals in <b>Annual Income Ranges</b> table may be higher than those displayed in the <b>Primary Income Source table</b>.";

            ReportTableList.Add(aggregateIncome);
		}
        
        private List<ReportTableHeader> GetHeaders() {
			return new List<ReportTableHeader> {
				new ReportTableHeader { Code = ReportTableHeaderEnum.New, Title = "New", SubHeaders = new List<ReportTableSubHeader> { new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.Total, Title = string.Empty } } },
				new ReportTableHeader { Code = ReportTableHeaderEnum.Ongoing, Title = "Ongoing", SubHeaders = new List<ReportTableSubHeader> { new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.Total, Title = string.Empty } } },
				new ReportTableHeader { Code = ReportTableHeaderEnum.Total, Title = "Total", SubHeaders = new List<ReportTableSubHeader> { new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.Total, Title = string.Empty } } }
			};
		}
	}

	public class IncomeLineItem {
		public int? ClientId { get; set; }
		public int? CaseId { get; set; }
		public string ClientCode { get; set; }
		public ReportTableHeaderEnum ClientStatus { get; set; }
		public int? PrimaryIncomeSourceId { get; set; }
		public decimal? AnnualIncome { get; internal set; }
	}
}