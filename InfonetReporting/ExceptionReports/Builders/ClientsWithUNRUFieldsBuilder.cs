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
	public class ExceptionClientsWithUNRUFieldsSubReportBuilder : SubReportDataBuilder<ClientCase, ExceptionUNRULineItem> {
		public ExceptionClientsWithUNRUFieldsSubReportBuilder(SubReportSelection s) : base(s) {
			DataFieldSelections = new List<UNRUDataFieldsEnum>();
		}

		public List<UNRUDataFieldsEnum> DataFieldSelections { get; }
		private int TotalNewClients { get; set; }
		private int TotalOngoingClients { get; set; }

		protected override void BuildLegacyHtmlRow(ExceptionUNRULineItem record, StringBuilder sb, bool isFirst, bool isLast) {
			if (string.IsNullOrEmpty(record.ClientInfo))
				return;

			if (string.IsNullOrEmpty(record.ClientCode) || string.IsNullOrWhiteSpace(record.ClientCode)) {
				if (record.ClientInfo.ToLower().Contains("total") || record.ClientInfo == "TotNumChild")
					BuildSummary(sb);
				else
					BuildHeader(record, sb);
			} else {
				BuildDataRow(record, sb);
			}
		}

		private void BuildHeader(ExceptionUNRULineItem record, StringBuilder sb) {
			string text = record.ClientInfo;
			if (record.ClientInfo == "Sex" && ReportContainer.Provider == Provider.SA)
				text = "Gender";
			sb.Append("<tr>");
			sb.Append("<td colspan='6'><b>" + text + "</b></td>");
			sb.Append("</tr>");
		}

		private void BuildSummary(StringBuilder sb) {
			sb.Append("<tr>");
			sb.Append("<td colspan='6' align='right'><b>Total New: " + TotalNewClients + ", Total Ongoing: " + TotalOngoingClients + "</b></td>");
			sb.Append("</tr>");
			TotalNewClients = 0;
			TotalOngoingClients = 0;
		}

		private void BuildDataRow(ExceptionUNRULineItem record, StringBuilder sb) {
			sb.Append("<tr>");
			sb.Append("<td>" + record.ClientInfo + "</td>");
			sb.Append("<td>" + record.ClientCode + "</td>");
			if (ReportContainer.Provider != Provider.SA)
				sb.Append("<td>" + record.CaseId + "</td>");

			sb.Append("<td>" + record.ClientType + "</td>");
			sb.Append("<td>" + (record.FirstContactDate.HasValue ? record.FirstContactDate.Value.ToShortDateString() : "None") + "</td>");
			sb.Append("<td>" + record.ClientStatus + "</td>");
			sb.Append("</tr>");
			if (record.ClientStatus.Contains("New"))
				TotalNewClients++;
			if (record.ClientStatus.Contains("Ongoing"))
				TotalOngoingClients++;
		}

		protected override void BuildLegacyHtmlSummaryRow(StringBuilder sb) {
			// DO NOTHING
		}

		protected override string BuildTrueCSVHeaders() {
			// Override this method to do nothing and handle everything in the BuildTrueCSVLine method due to how the data is formatted
			return "";
		}

		protected override string BuildTrueCSVLine(ExceptionUNRULineItem record) {
			if (string.IsNullOrEmpty(record.ClientInfo))
				return "";

			if (string.IsNullOrEmpty(record.ClientCode) || string.IsNullOrWhiteSpace(record.ClientCode)) {
				if (record.ClientInfo.ToLower().Contains("total") || record.ClientInfo == "TotNumChild")
					return ""; // Don't write total records to the CSV file

				string columnTitle = record.ClientInfo == "Sex" && ReportContainer.Provider == Provider.SA ? "Gender" : record.ClientInfo;
				return base.BuildTrueCSVHeaders().Replace(ReportColumnSelectionsEnum.ClientInfo.GetShortName(), columnTitle);
			}

			var sb = new StringBuilder();

			foreach (var column in ColumnSelections) {
				if (sb.Length != 0)
					sb.Append(",");

				switch (column.ColumnSelection) {
					case ReportColumnSelectionsEnum.ClientInfo:
						sb.AppendQuotedCSVData(record.ClientInfo);
						break;
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

		protected override IEnumerable<ExceptionUNRULineItem> PerformSelect(IOrderedQueryable<ClientCase> query) {
			var sb = new StringBuilder();
			ReportContainer.StartDate = ReportContainer.StartDate ?? DateTime.Parse("01/01/1970");
			ReportContainer.EndDate = ReportContainer.EndDate ?? DateTime.Today;
			sb.Append(string.Format("EXEC	[dbo].[RPT_ClientNotReportedUnknownUnassignDemographicInfo_3] @CenterIDs = '{0}', @PID = '{1}', @StartDate = '{2}', @EndDate = '{3}'", string.Join(", ", ReportContainer.CenterIds), ReportContainer.Provider.ToInt32(), ReportContainer.StartDate.Value.ToShortDateString(), ReportContainer.EndDate.Value.ToShortDateString()));
			foreach (var field in DataFieldSelections)
				sb.Append(", " + field.GetDisplayName() + " = N'1'");
			return ReportContainer.InfonetContext.Database.SqlQuery<ExceptionUNRULineItem>(sb.ToString());
		}

		protected override void PrepareRecord(ExceptionUNRULineItem record) {
			record.ClientInfo = record.ClientInfo.Trim();
			record.ClientCode = record.ClientCode.Trim();
			record.CaseId = record.CaseId.Trim();
			record.ClientType = record.ClientType.Trim();
			record.ClientStatus = record.ClientStatus?.Trim();
		}
	}

	public class ExceptionUNRULineItem {
		public string ClientInfo { get; set; }
		public string ClientCode { get; set; }
		public string CaseId { get; set; }
		public string ClientType { get; set; }
		public DateTime? FirstContactDate { get; set; }
		public string ClientStatus { get; set; }
	}
}