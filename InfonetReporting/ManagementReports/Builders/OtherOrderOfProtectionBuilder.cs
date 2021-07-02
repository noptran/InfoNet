using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infonet.Core.IO;
using Infonet.Data.Looking;
using Infonet.Data.Models.Clients;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;

namespace Infonet.Reporting.ManagementReports.Builders {
	public class OtherOrderOfProtectionSubReport : SubReportDataBuilder<OrderOfProtection, OrderOfProtectionLineItem> {
		public OtherOrderOfProtectionSubReport(SubReportSelection subReportSelectionType) : base(subReportSelectionType) {
			TotalClientList = new HashSet<int?>();
			TotalUniqueRecordList = new HashSet<string>();
		}

		public OrderOfProtectionIssuedOrExpiredSelectionsEnum DateFilter { get; set; }
		private HashSet<int?> TotalClientList { get; }
		private HashSet<string> TotalUniqueRecordList { get; }

		protected override void BuildLegacyHtmlRow(OrderOfProtectionLineItem record, StringBuilder sb, bool isFirst, bool isLast) {
			sb.Append("<tr>");
			foreach (var columnSelection in ColumnSelections)
				switch (columnSelection.ColumnSelection) {
					case ReportColumnSelectionsEnum.ClientCode:
						sb.Append("<th scope='row' style='font-weight:normal;'>" + record.ClientCode + "</th>");
						break;
					case ReportColumnSelectionsEnum.DateIssued:
						sb.Append("<td>" + (record.DateIssued.HasValue ? record.DateIssued.Value.ToShortDateString() : "") + "</td>");
						break;
					case ReportColumnSelectionsEnum.DateExpired:
						sb.Append("<td>" + (record.ExpirationDate.HasValue ? record.ExpirationDate.Value.ToShortDateString() : "") + "</td>");
						break;
					case ReportColumnSelectionsEnum.OriginalOpType:
						sb.Append("<td>" + Lookups.OrderOfProtectionType[record.TypeOfOpId]?.Description + "</td>");
						break;
				}
			sb.Append("</tr>");
			SetTotals(record);
		}

		private void SetTotals(OrderOfProtectionLineItem record) {
			if (!TotalClientList.Contains(record.ClientId))
				TotalClientList.Add(record.ClientId);

			string recordIdentifier = $"{record.ClientId}:{record.DateIssued}:{record.ExpirationDate}";
			if (!TotalUniqueRecordList.Contains(recordIdentifier))
				TotalUniqueRecordList.Add(recordIdentifier);
		}

		protected override void BuildLegacyHtmlSummaryRow(StringBuilder sb) {
			string datefilter = DateFilter == OrderOfProtectionIssuedOrExpiredSelectionsEnum.DateIssued ? "Issued" : "Expired";

			sb.Append("<tr>");
			sb.Append("<th scope='row'> Number of victims with orders " + datefilter + " this period </th>");
			sb.Append("<td><b>" + TotalClientList.Count + "</b></td>");
			sb.Append("</tr>");

			sb.Append("<tr>");
			sb.Append("<th scope='row'> Number of orders " + datefilter + " this period  </th>");
			sb.Append("<td><b>" + TotalUniqueRecordList.Count + "</b></td>");
			sb.Append("</tr>");
		}

		protected override string BuildTrueCSVLine(OrderOfProtectionLineItem record) {
			bool applyComma = false;
			var sb = new StringBuilder();
			foreach (var columnSelection in ColumnSelections) {
				if (applyComma)
					sb.Append(",");
				switch (columnSelection.ColumnSelection) {
					case ReportColumnSelectionsEnum.ClientCode:
						sb.AppendQuotedCSVData(record.ClientCode);
						break;
					case ReportColumnSelectionsEnum.DateIssued:
						sb.AppendQuotedCSVData(record.DateIssued.HasValue ? record.DateIssued.Value.ToShortDateString() : string.Empty);
						break;
					case ReportColumnSelectionsEnum.DateExpired:
						sb.AppendQuotedCSVData(record.ExpirationDate.HasValue ? record.ExpirationDate.Value.ToShortDateString() : string.Empty);
						break;
					case ReportColumnSelectionsEnum.OriginalOpType:
						sb.AppendQuotedCSVData(Lookups.OrderOfProtectionType[record.TypeOfOpId]?.Description);
						break;
				}
				applyComma = true;
			}
			return sb.ToString();
		}

		protected override IEnumerable<OrderOfProtectionLineItem> PerformSelect(IOrderedQueryable<OrderOfProtection> query) {
			return query.Select(q => new OrderOfProtectionLineItem {
				ClientId = q.ClientId,
				DateIssued = q.DateIssued,
				ExpirationDate = q.OrderOfProtectionActivities.Any(op => op.NewExpirationDate.HasValue) ? q.OrderOfProtectionActivities.Max(op => op.NewExpirationDate) : q.OriginalExpirationDate,
				TypeOfOpId = q.TypeOfOPID,
				ClientCode = q.ClientCase.Client.ClientCode
			});
		}
	}

	public class OrderOfProtectionLineItem {
		public int? ClientId { get; set; }
		public DateTime? DateIssued { get; set; }
		public DateTime? ExpirationDate { get; set; }
		public int? TypeOfOpId { get; set; }
		public string ClientCode { get; set; }
	}
}