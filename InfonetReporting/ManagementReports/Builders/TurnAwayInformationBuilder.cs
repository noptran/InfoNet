using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infonet.Core.IO;
using Infonet.Data.Looking;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;

namespace Infonet.Reporting.ManagementReports.Builders {
	public class TurnAwayInformationSubReport : SubReportDataBuilder<TurnAwayService, TurnAwayInformationLineItem> {
		public TurnAwayInformationSubReport(SubReportSelection subReportSelectionType) : base(subReportSelectionType) {
			PreviousGroupValue = string.Empty;
		}

		private string PreviousGroupValue { get; set; }
		private int PreviousAdultCount { get; set; }
		private int PreviousChildCount { get; set; }
		private int PreviousServiceCount { get; set; }
		private double PreviousFamilyCount { get; set; }
		private int TotalAdultCount { get; set; }
		private int TotalChildCount { get; set; }
		private int TotalServiceCount { get; set; }
		private double TotalFamiliesCount { get; set; }

		protected override void BuildLegacyHtmlRow(TurnAwayInformationLineItem record, StringBuilder sb, bool isFirst, bool isLast) {
			if (!isFirst && GroupingSelections.Any()) {
				string currentGroupValue = string.Empty;
				switch (GroupingSelections.OrderBy(g => g.Order).ThenBy(g => g.GroupingSelection).First().GroupingSelection) {
					case ReportOrderSelectionsEnum.ReferralMade:
						currentGroupValue = Lookups.YesNo[record.ReferralMadeId]?.Description ?? string.Empty;
						break;
				}

				if (!PreviousGroupValue.Trim().Equals(currentGroupValue.Trim(), StringComparison.OrdinalIgnoreCase)) {
					ApplySummaryRow(sb);
					PreviousGroupValue = null;
					PreviousChildCount = 0;
					PreviousServiceCount = 0;
					PreviousAdultCount = 0;
					PreviousFamilyCount = 0.0;
				}
			}

			sb.Append("<tr>");
			foreach (var columnSelection in ColumnSelections)
				switch (columnSelection.ColumnSelection) {
					case ReportColumnSelectionsEnum.Date:
						sb.Append("<th scope='row' style='font-weight:normal;'>" + (record.TurnAwayDate.HasValue ? record.TurnAwayDate.Value.ToShortDateString() : "") + "</th>");
						break;
					case ReportColumnSelectionsEnum.TurnAwayNumOfAdultVictims:
						sb.Append("<td>" + record.AdultsNo + "</td>");
						break;
					case ReportColumnSelectionsEnum.TurnAwayNumOfChildren:
						sb.Append("<td>" + record.ChildrenNo + "</td>");
						break;
					case ReportColumnSelectionsEnum.TurnAwayNumOfFamily:
						sb.Append("<td>" + record.FamilyNo + "</td>");
						break;
					case ReportColumnSelectionsEnum.TurnAwayReferralMade:
						sb.Append("<td>" + Lookups.YesNo[record.ReferralMadeId]?.Description + "</td>");
						break;
				}
			sb.Append("</tr>");

			SetGroupTotals(record);
			SetTotals(record);

			if (isLast) {
				ApplySummaryRow(sb);
				PreviousGroupValue = null;
				PreviousChildCount = 0;
				PreviousServiceCount = 0;
				PreviousAdultCount = 0;
				PreviousFamilyCount = 0.0;
			}
		}

		private void ApplySummaryRow(StringBuilder sb) {
			if (ColumnSelections.Count > 1) {
				sb.Append("<tr class='subtotal'>");
				foreach (var columnSelection in ColumnSelections)
					switch (columnSelection.ColumnSelection) {
						case ReportColumnSelectionsEnum.Date:
							sb.Append("<th scope='row' style='font-weight:normal;'>Subtotal " + PreviousGroupValue + "</th>");
							break;
						case ReportColumnSelectionsEnum.TurnAwayNumOfAdultVictims:
							sb.Append("<td><b>" + PreviousAdultCount + "</b></td>");
							break;
						case ReportColumnSelectionsEnum.TurnAwayNumOfChildren:
							sb.Append("<td><b>" + PreviousChildCount + "</b></td>");
							break;
						case ReportColumnSelectionsEnum.TurnAwayReferralMade:
							sb.Append("<td><b>" + PreviousServiceCount + "</b></td>");
							break;
						case ReportColumnSelectionsEnum.TurnAwayNumOfFamily:
							sb.Append("<td><b>" + PreviousFamilyCount + "</b></td>");
							break;
					}
				sb.Append("</tr>");
			}
		}

		private void SetGroupTotals(TurnAwayInformationLineItem record) {
			if (GroupingSelections.Any())
				if (GroupingSelections.OrderBy(g => g.Order).ThenBy(g => g.GroupingSelection).First().GroupingSelection == ReportOrderSelectionsEnum.ReferralMade)
					PreviousGroupValue = Lookups.YesNo[record.ReferralMadeId]?.Description ?? string.Empty;
			PreviousAdultCount += record.AdultsNo ?? 0;
			PreviousChildCount += record.ChildrenNo ?? 0;
			PreviousServiceCount++;
			PreviousFamilyCount++;
		}

		private void SetTotals(TurnAwayInformationLineItem record) {
			TotalAdultCount += record.AdultsNo ?? 0;
			TotalChildCount += record.ChildrenNo ?? 0;
			TotalServiceCount++;
			TotalFamiliesCount++;
		}

		protected override void BuildLegacyHtmlSummaryRow(StringBuilder sb) {
			sb.Append("<tr>");
			foreach (var columnSelection in ColumnSelections)
				switch (columnSelection.ColumnSelection) {
					case ReportColumnSelectionsEnum.Date:
						sb.Append("<th scope='row' style='font-weight:normal;'>Total</th>");
						break;
					case ReportColumnSelectionsEnum.TurnAwayNumOfAdultVictims:
						sb.Append("<td><b>" + TotalAdultCount + "</b></td>");
						break;
					case ReportColumnSelectionsEnum.TurnAwayNumOfChildren:
						sb.Append("<td><b>" + TotalChildCount + "</b></td>");
						break;
					case ReportColumnSelectionsEnum.TurnAwayNumOfFamily:
						sb.Append("<td><b>" + TotalFamiliesCount + "</b></td>");
						break;
					case ReportColumnSelectionsEnum.TurnAwayReferralMade:
						sb.Append("<td><b>" + TotalServiceCount + "</b></td>");
						break;
				}
			sb.Append("</tr>");
		}

		protected override string BuildTrueCSVLine(TurnAwayInformationLineItem record) {
			bool applyComma = false;
			var sb = new StringBuilder();
			foreach (var columnSelection in ColumnSelections) {
				if (applyComma)
					sb.Append(",");
				switch (columnSelection.ColumnSelection) {
					case ReportColumnSelectionsEnum.Date:
						sb.AppendQuotedCSVData(record.TurnAwayDate.HasValue ? record.TurnAwayDate.Value.ToShortDateString() : string.Empty);
						break;
					case ReportColumnSelectionsEnum.TurnAwayNumOfAdultVictims:
						sb.AppendQuotedCSVData(record.AdultsNo);
						break;
					case ReportColumnSelectionsEnum.TurnAwayNumOfChildren:
						sb.AppendQuotedCSVData(record.ChildrenNo);
						break;
					case ReportColumnSelectionsEnum.TurnAwayNumOfFamily:
						sb.AppendQuotedCSVData(record.FamilyNo);
						break;
					case ReportColumnSelectionsEnum.TurnAwayReferralMade:
						sb.AppendQuotedCSVData(Lookups.YesNo[record.ReferralMadeId]?.Description);
						break;
				}
				applyComma = true;
			}
			return sb.ToString();
		}

		protected override IEnumerable<TurnAwayInformationLineItem> PerformSelect(IOrderedQueryable<TurnAwayService> query) {
			return query.Select(q => new TurnAwayInformationLineItem {
				TurnAwayDate = q.TurnAwayDate,
				ChildrenNo = q.ChildrenNo,
				AdultsNo = q.AdultsNo,
				ReferralMadeId = q.ReferralMadeId
			});
		}
	}

	public class TurnAwayInformationLineItem {
		public TurnAwayInformationLineItem() {
			FamilyNo = 1; //KMS DO this is never modified...this can't be right, can it?
		}

		public int? AdultsNo { get; set; }
		public int? ChildrenNo { get; set; }
		public int? ReferralMadeId { get; set; }
		public DateTime? TurnAwayDate { get; set; }
		public int FamilyNo { get; set; }
	}
}