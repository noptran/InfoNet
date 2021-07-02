using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infonet.Core.Collections;
using Infonet.Core.IO;
using Infonet.Data.Looking;
using Infonet.Data.Models.Clients;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;

namespace Infonet.Reporting.ExceptionReports.Builders {
	public class ClientsWithoutOffendersDataFieldsSubReportBuilder : SubReportDataBuilder<ClientCase, ClientWithoutOffenderInformationLineItem> {
		public ClientsWithoutOffendersDataFieldsSubReportBuilder(SubReportSelection s) : base(s) { }

		private int TotalNewClients { get; set; }
		private int TotalOngoingClients { get; set; }

		protected override void BuildLegacyHtmlRow(ClientWithoutOffenderInformationLineItem record, StringBuilder sb, bool isFirst, bool isLast) {
			sb.Append("<tr>");
            sb.Append("<th scope='row' style='font-weight:normal;'>" + record.ClientCode + "</th>");
            if (ReportContainer.Provider != Provider.SA)
				sb.Append("<td>" + record.CaseId + "</td>");
			sb.Append("<td>" + record.ClientType + "</td>");
			sb.Append("<td>" + (record.FirstContactDate.HasValue ? record.FirstContactDate.Value.ToShortDateString() : string.Empty) + "</td>");
			sb.Append("<td>" + record.ClientStatus + "</td>");
			sb.Append("</tr>");
			if (record.ClientStatus.Contains("New"))
				TotalNewClients++;
			if (record.ClientStatus.Contains("Ongoing"))
				TotalOngoingClients++;
		}

		protected override void BuildLegacyHtmlSummaryRow(StringBuilder sb) {
			sb.Append("<tr>");
			sb.Append("<td colspan='" + ColumnSelections.Count + "' align='right'><b>Total New: " + TotalNewClients + ", Total Ongoing: " + TotalOngoingClients + "</b></td>");
			sb.Append("</tr>");
		}

		protected override string BuildTrueCSVLine(ClientWithoutOffenderInformationLineItem record) {
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
					case ReportColumnSelectionsEnum.ClientType:
						sb.AppendQuotedCSVData(record.ClientType);
						break;
					case ReportColumnSelectionsEnum.FirstContactDate:
						sb.AppendQuotedCSVData(record.FirstContactDate.HasValue ? record.FirstContactDate.Value.ToShortDateString() : string.Empty);
						break;
					case ReportColumnSelectionsEnum.ClientStatus:
						sb.AppendQuotedCSVData(record.ClientStatus);
						break;
				}
			}

			return sb.ToString();
		}

		protected override IEnumerable<ClientWithoutOffenderInformationLineItem> PerformSelect(IOrderedQueryable<ClientCase> query) {
			ReportContainer.StartDate = ReportContainer.StartDate ?? DateTime.Parse("01/01/1970");
			ReportContainer.EndDate = ReportContainer.EndDate ?? DateTime.Today;
			string sql = $"EXEC [dbo].[RPT_ClientWithoutOffenderInfo_3] @CenterIDs = '{string.Join(", ", ReportContainer.CenterIds)}', @PID = '{ReportContainer.Provider.ToInt32()}', @StartDate = '{ReportContainer.StartDate.Value.ToShortDateString()}', @EndDate = '{ReportContainer.EndDate.Value.ToShortDateString()}'";
			return ReportContainer.InfonetContext.Database.SqlQuery<ClientWithoutOffenderInformationLineItem>(sql);
		}

		protected override void PrepareRecord(ClientWithoutOffenderInformationLineItem record) {
			record.ClientCode = record.ClientCode.Trim();
			record.ClientType = record.ClientType.Trim();
			record.ClientStatus = record.ClientStatus.Trim();
		}
	}

	public class ClientWithoutOffenderInformationLineItem {
		public string ClientCode { get; set; }
		public int? CaseId { get; set; }
		public string ClientType { get; set; }
		public DateTime? FirstContactDate { get; set; }
		public string ClientStatus { get; set; }
	}
}