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
	public class ExceptionOpenCasesSubReportBuilder : SubReportDataBuilder<ClientCase, ExceptionOpenCasesSubReportBuilderLineItem> {
		public ExceptionOpenCasesSubReportBuilder(SubReportSelection s) : base(s) {
			TotalClients = new List<string>();
			TotalCases = new List<string>();
		}

		public int DaysSinceLastService { get; set; }
		private List<string> TotalClients { get; }
		private List<string> TotalCases { get; }

		protected override void BuildLegacyHtmlRow(ExceptionOpenCasesSubReportBuilderLineItem record, StringBuilder sb, bool isFirst, bool isLast) {
			sb.Append("<tr>");
            sb.Append("<th scope='row' style='font-weight:normal;'>" + record.ClientID + "</th>");
            if (ReportContainer.Provider != Provider.SA)
				sb.Append("<td>" + record.CaseID + "</td>");
			sb.Append("<td>" + (record.ServiceDate.HasValue ? record.ServiceDate.Value.ToShortDateString() : record.ShelterDate.HasValue ? record.ShelterDate.Value.ToShortDateString() : string.Empty) + "</td>");
			sb.Append("</tr>");
			if (!TotalClients.Contains(record.ClientID))
				TotalClients.Add(record.ClientID);
			if (!TotalCases.Contains(record.ClientID + ":" + record.CaseID))
				TotalCases.Add(record.ClientID + ":" + record.CaseID);
		}

		protected override void BuildLegacyHtmlSummaryRow(StringBuilder sb) {
			sb.Append("<tr class='summaryRow'>");
			sb.Append("<td><b>Total Clients: " + TotalClients.Count + "</b></td>");
			if (ReportContainer.Provider != Provider.SA)
				sb.Append("<td><b>Total Cases: " + TotalCases.Count + "</b></td>");
			sb.Append("<td></td>");
			sb.Append("</tr>");
		}

		protected override string BuildTrueCSVLine(ExceptionOpenCasesSubReportBuilderLineItem record) {
			var sb = new StringBuilder();

			foreach (var column in ColumnSelections) {
				if (sb.Length != 0)
					sb.Append(",");
				switch (column.ColumnSelection) {
					case ReportColumnSelectionsEnum.ClientCode:
						sb.AppendQuotedCSVData(record.ClientID);
						break;
					case ReportColumnSelectionsEnum.CaseID:
						sb.AppendQuotedCSVData(record.CaseID);
						break;
					case ReportColumnSelectionsEnum.DateOfLastService:
						sb.AppendQuotedCSVData(record.ServiceDate.HasValue ? record.ServiceDate.Value.ToShortDateString() : record.ShelterDate.HasValue ? record.ShelterDate.Value.ToShortDateString() : "N/A");
						break;
				}
			}

			return sb.ToString();
		}

		protected override IEnumerable<ExceptionOpenCasesSubReportBuilderLineItem> PerformSelect(IOrderedQueryable<ClientCase> query) {
			var sb = new StringBuilder();
			sb.Append(string.Format("EXEC[dbo].[RPT_OpenClientCases_2] @CenterIDs = '{0}', @DateRange = '{1}'", string.Join(", ", ReportContainer.CenterIds), DaysSinceLastService));
			return ReportContainer.InfonetContext.Database.SqlQuery<ExceptionOpenCasesSubReportBuilderLineItem>(sb.ToString());
		}
	}

	public class ExceptionOpenCasesSubReportBuilderLineItem {
		public string ClientID { get; set; }
		public int? CaseID { get; set; }
		public DateTime? ServiceDate { get; set; }
		public DateTime? ShelterDate { get; set; }
	}
}