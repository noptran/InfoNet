using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infonet.Core.IO;
using Infonet.Data.Looking;
using Infonet.Data.Models.Clients;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;

namespace Infonet.Reporting.ExceptionReports.Builders {
	public class ExceptionClientsWithoutPresentingIssueSubReportBuilder : SubReportDataBuilder<ClientCase, ExceptionClientsWithoutPresentingIssueLineItem> {
		public ExceptionClientsWithoutPresentingIssueSubReportBuilder(SubReportSelection s) : base(s) {
			TotalClients = new List<string>();
			TotalCases = new List<string>();
		}

		private List<string> TotalClients { get; }
		private List<string> TotalCases { get; }

		protected override void BuildLegacyHtmlRow(ExceptionClientsWithoutPresentingIssueLineItem record, StringBuilder sb, bool isFirst, bool isLast) {
			sb.Append("<tr>");
            sb.Append("<th scope='row' style='font-weight:normal;'>" + record.ClientCode + "</th>");
            if (ReportContainer.Provider != Provider.SA)
				sb.Append("<td>" + record.CaseId + "</td>");
			sb.Append("<td>" + (record.FirstContactDate.HasValue ? record.FirstContactDate.Value.ToShortDateString() : string.Empty) + "</td>");
			sb.Append("</tr>");
			if (!TotalClients.Contains(record.ClientCode))
				TotalClients.Add(record.ClientCode);
			if (!TotalCases.Contains(record.ClientCode + ":" + record.CaseId))
				TotalCases.Add(record.ClientCode + ":" + record.CaseId);
		}

		protected override void BuildLegacyHtmlSummaryRow(StringBuilder sb) {
			sb.Append("<tr class='summaryRow'>");
			sb.Append("<td><b>Total Clients: " + TotalClients.Count + "</b></td>");
			if (ReportContainer.Provider != Provider.SA)
				sb.Append("<td><b>Total Cases: " + TotalCases.Count + "</b></td>");
			sb.Append("<td></td>");
			sb.Append("</tr>");
		}

		protected override string BuildTrueCSVLine(ExceptionClientsWithoutPresentingIssueLineItem record) {
			var sb = new StringBuilder();

			foreach (var column in ColumnSelections) {
				if (sb.Length != 0)
					sb.Append(",");
				switch (column.ColumnSelection) {
					case ReportColumnSelectionsEnum.ClientCode:
						sb.AppendQuotedCSVData(record.ClientCode);
						break;
					case ReportColumnSelectionsEnum.CaseID:
						sb.AppendQuotedCSVData(record.CaseId);
						break;
					case ReportColumnSelectionsEnum.FirstContactDate:
						sb.AppendQuotedCSVData(record.FirstContactDate.HasValue ? record.FirstContactDate.Value.ToShortDateString() : string.Empty);
						break;
				}
			}

			return sb.ToString();
		}

		protected override IEnumerable<ExceptionClientsWithoutPresentingIssueLineItem> PerformSelect(IOrderedQueryable<ClientCase> query) {
			query = query.OrderBy(c => c.Client.ClientCode).ThenBy(q => q.CaseId).ThenBy(q => q.FirstContactDate);
			return query.Select(q => new ExceptionClientsWithoutPresentingIssueLineItem { ClientCode = q.Client.ClientCode, CaseId = q.CaseId, FirstContactDate = q.FirstContactDate });
		}
	}

	public class ExceptionClientsWithoutPresentingIssueLineItem {
		public string ClientCode { get; set; }
		public int? CaseId { get; set; }
		public DateTime? FirstContactDate { get; set; }
	}
}