using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infonet.Core.IO;
using Infonet.Data.Models.Clients;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;

namespace Infonet.Reporting.ExceptionReports.Builders {
	public class ExceptionOrdersOfProtectionWithoutExpirationDateSubReportBuilder : SubReportDataBuilder<OrderOfProtection, ExceptionOrdersOfProtectionWithoutExpirationDateSubReportBuilderLineItem> {
		public ExceptionOrdersOfProtectionWithoutExpirationDateSubReportBuilder(SubReportSelection s) : base(s) {
			TotalClients = new List<string>();
			TotalCases = new List<string>();
		}

		private List<string> TotalClients { get; }
		private List<string> TotalCases { get; }

		protected override void BuildLegacyHtmlRow(ExceptionOrdersOfProtectionWithoutExpirationDateSubReportBuilderLineItem record, StringBuilder sb, bool isFirst, bool isLast) {
			sb.Append("<tr>");
            sb.Append("<th scope='row' style='font-weight:normal;'>" + record.ClientCode + "</th>");
            sb.Append("<td>" + record.CaseId + "</td>");
			sb.Append("<td>" + (record.DateIssued.HasValue ? record.DateIssued.Value.ToShortDateString() : string.Empty) + "</td>");
			sb.Append("<td>" + (record.ExpirationDate.HasValue ? record.ExpirationDate.Value.ToShortDateString() : string.Empty) + "</td>");
			sb.Append("</tr>");
			if (!TotalClients.Contains(record.ClientCode))
				TotalClients.Add(record.ClientCode);
			if (!TotalCases.Contains(record.ClientCode + ":" + record.CaseId))
				TotalCases.Add(record.ClientCode + ":" + record.CaseId);
		}

		protected override void BuildLegacyHtmlSummaryRow(StringBuilder sb) {
			sb.Append("<tr class='summaryRow'>");
			sb.Append("<td><b>Total Clients: " + TotalClients.Count + "</b></td>");
			sb.Append("<td><b>Total Cases: " + TotalCases.Count + "</b></td>");
			sb.Append("<td></td>");
			sb.Append("</tr>");
		}

		protected override string BuildTrueCSVLine(ExceptionOrdersOfProtectionWithoutExpirationDateSubReportBuilderLineItem record) {
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
					case ReportColumnSelectionsEnum.DateIssued:
						sb.AppendQuotedCSVData(record.DateIssued.HasValue ? record.DateIssued.Value.ToShortDateString() : string.Empty);
						break;
					case ReportColumnSelectionsEnum.DateExpired:
						sb.AppendQuotedCSVData(record.ExpirationDate.HasValue ? record.ExpirationDate.Value.ToShortDateString() : string.Empty);
						break;
				}
			}

			return sb.ToString();
		}

		protected override IEnumerable<ExceptionOrdersOfProtectionWithoutExpirationDateSubReportBuilderLineItem> PerformSelect(IOrderedQueryable<OrderOfProtection> query) {
			query = query.OrderBy(q => q.ClientCase.Client.ClientCode).ThenBy(q => q.ClientCase.CaseId).ThenBy(q => q.DateIssued).ThenBy(q => q.OriginalExpirationDate);
			return query.Select(q => new ExceptionOrdersOfProtectionWithoutExpirationDateSubReportBuilderLineItem {
				ClientCode = q.ClientCase.Client.ClientCode,
				CaseId = q.ClientCase.CaseId,
				DateIssued = q.DateIssued,
				ExpirationDate = q.OriginalExpirationDate
			});
		}
	}

	public class ExceptionOrdersOfProtectionWithoutExpirationDateSubReportBuilderLineItem {
		public string ClientCode { get; set; }
		public int? CaseId { get; set; }
		public DateTime? DateIssued { get; set; }
		public DateTime? ExpirationDate { get; set; }
	}
}