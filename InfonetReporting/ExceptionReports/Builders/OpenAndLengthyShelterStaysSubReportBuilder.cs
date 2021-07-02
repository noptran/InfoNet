using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infonet.Core.IO;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;

namespace Infonet.Reporting.ExceptionReports.Builders {
	public class OpenAndLengthyShelterStaysSubReportBuilder : SubReportDataBuilder<ServiceDetailOfClient, OpenAndLengthyShelterStaysSubReportBuilderLineItem> {
		public OpenAndLengthyShelterStaysSubReportBuilder(SubReportSelection s) : base(s) {
			TotalClients = new HashSet<int?>();
		}

		public int NumberOfDays { get; set; }
		private HashSet<int?> TotalClients { get; }

		protected override void BuildLegacyHtmlRow(OpenAndLengthyShelterStaysSubReportBuilderLineItem record, StringBuilder sb, bool isFirst, bool isLast) {
			sb.Append("<tr>");
            sb.Append("<th scope='row' style='font-weight:normal;'>" + record.ClientCode + "</th>");
            sb.Append("<td>");
			if (record.ShelterEndDate == null)
				sb.Append("Shelter End Date Is Not Closed");
			else if (record.ShelterBeginDate > record.ShelterEndDate)
				sb.Append("Shelter Begin Date > Shelter End Date");
			else
				sb.Append("Shelter End Date - Shelter Begin Date > " + NumberOfDays);
			sb.Append("</td>");
			sb.Append("<td>" + record.ShelterBeginDate?.ToShortDateString() + "</td>");
			sb.Append("<td>" + record.ShelterEndDate?.ToShortDateString() + "</td>");
			sb.Append("</tr>");
			TotalClients.Add(record.ClientId);
		}

		protected override void BuildLegacyHtmlSummaryRow(StringBuilder sb) {
			sb.Append("<tr class='summaryRow'>");
			sb.Append("<td><b>Total Clients: " + TotalClients.Count + "</b></td>");
			sb.Append("<td></td>");
			sb.Append("<td></td>");
			sb.Append("</tr>");
		}

		protected override string BuildTrueCSVLine(OpenAndLengthyShelterStaysSubReportBuilderLineItem record) {
			var sb = new StringBuilder();

			foreach (var column in ColumnSelections) {
				if (sb.Length != 0)
					sb.Append(",");
				switch (column.ColumnSelection) {
					case ReportColumnSelectionsEnum.ClientCode:
						sb.AppendQuotedCSVData(record.ClientCode);
						break;
					case ReportColumnSelectionsEnum.Comment:
						sb.AppendQuotedCSVData(record.ShelterEndDate.HasValue ? "Shelter End Date - Shelter Begin Date > " + NumberOfDays : "Shelter End Date Is Not Closed");
						break;
					case ReportColumnSelectionsEnum.ShelterBeginDate:
						sb.AppendQuotedCSVData(record.ShelterBeginDate.HasValue ? record.ShelterBeginDate.Value.ToShortDateString() : string.Empty);
						break;
					case ReportColumnSelectionsEnum.ShelterEndDate:
						sb.AppendQuotedCSVData(record.ShelterEndDate.HasValue ? record.ShelterEndDate.Value.ToShortDateString() : string.Empty);
						break;
				}
			}

			return sb.ToString();
		}

		protected override IEnumerable<OpenAndLengthyShelterStaysSubReportBuilderLineItem> PerformSelect(IOrderedQueryable<ServiceDetailOfClient> query) {
			query = query.OrderByDescending(c => c.ShelterEndDate.HasValue).ThenBy(c => c.ClientCase.Client.ClientCode).ThenBy(c => c.ShelterBegDate).ThenBy(c => c.ShelterEndDate);
			return query.Select(q => new OpenAndLengthyShelterStaysSubReportBuilderLineItem {
				ClientId = q.ClientID,
				ClientCode = q.ClientCase.Client.ClientCode,
				ShelterBeginDate = q.ShelterBegDate,
				ShelterEndDate = q.ShelterEndDate
			});
		}
	}

	public class OpenAndLengthyShelterStaysSubReportBuilderLineItem {
		public int? ClientId { get; set; }
		public string ClientCode { get; set; }
		public DateTime? ShelterBeginDate { get; set; }
		public DateTime? ShelterEndDate { get; set; }
	}
}