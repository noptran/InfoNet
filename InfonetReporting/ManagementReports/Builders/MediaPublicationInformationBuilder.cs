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
	public class StaffMediaPublicationInformationSubReport : SubReportDataBuilder<PublicationDetailStaff, MediaPublicationInformationLineItem> {
		private readonly HashSet<int?> _staffIds = new HashSet<int?>();
		private readonly HashSet<int?> _icsIds = new HashSet<int?>();

		public StaffMediaPublicationInformationSubReport(SubReportSelection subReportSelectionType) : base(subReportSelectionType) {
			PreviousGroupValue = string.Empty;
		}

		private string PreviousGroupValue { get; set; }
		private int PreviousServiceCount { get; set; }
		private int PreviousSegmentCount { get; set; }
		private double PreviousPrepareHrs { get; set; }
		private double PreviousStaffPrepareHrs { get; set; }
		private int TotalServiceCount { get; set; }
		private int TotalSegmentCount { get; set; }
		private double TotalPrepareHrs { get; set; }
		private double TotalStaffPrepareHrs { get; set; }

		protected override void BuildLegacyHtmlRow(MediaPublicationInformationLineItem record, StringBuilder sb, bool isFirst, bool isLast) {
			if (!isFirst && GroupingSelections.Any()) {
				string currentGroupValue = string.Empty;
				switch (GroupingSelections.OrderBy(g => g.Order).ThenBy(g => g.GroupingSelection).First().GroupingSelection) {
					case ReportOrderSelectionsEnum.Staff:
						currentGroupValue = !string.IsNullOrEmpty(record.StaffName) ? record.StaffName : string.Empty;
						break;
				}
				if (!PreviousGroupValue.Trim().Equals(currentGroupValue.Trim(), StringComparison.OrdinalIgnoreCase)) {
					ApplySummaryRow(sb);
					PreviousGroupValue = null;
					PreviousServiceCount = 0;
					PreviousPrepareHrs = 0;
					PreviousSegmentCount = 0;
					PreviousStaffPrepareHrs = 0;
				}
			}

			sb.Append("<tr>");
			foreach (var columnSelection in ColumnSelections)
				switch (columnSelection.ColumnSelection) {
					case ReportColumnSelectionsEnum.Staff:
						if (string.IsNullOrEmpty(PreviousGroupValue))
							sb.Append("<th scope='row' style='font-weight:normal;'>" + record.StaffName + "</th>");
						else
							sb.Append("<th scope='row' style='font-weight:normal;'><span class='sr-only'>"  + record.StaffName + "</span></td>");
						break;
					case ReportColumnSelectionsEnum.MediaPublicationType:
						sb.Append("<td>" + Lookups.ProgramsAndServices[record.ProgramId].Description + "</td>");
						break;
					case ReportColumnSelectionsEnum.Date:
						sb.Append("<td>" + (record.PDate?.ToShortDateString() ?? string.Empty) + "</td>");
						break;
					case ReportColumnSelectionsEnum.Title:
						sb.Append("<td>" + record.Title + "</td>");
						break;
					case ReportColumnSelectionsEnum.PrepareHours:
						sb.Append("<td>" + record.PrepareHours + "</td>");
						break;
					case ReportColumnSelectionsEnum.NumOfSegments:
						sb.Append("<td>" + record.NumberOfSegments + "</td>");
						break;
					case ReportColumnSelectionsEnum.StaffPrepHours:
						sb.Append("<td>" + record.StaffPrepareHours + "</td>");
						break;
				}
			sb.Append("</tr>");

			SetGroupTotals(record);
			SetTotals(record);

			if (isLast) {
				ApplySummaryRow(sb);
				PreviousGroupValue = null;
				PreviousServiceCount = 0;
				PreviousSegmentCount = 0;
				PreviousPrepareHrs = 0.0;
				PreviousStaffPrepareHrs = 0.0;
			}
		}

		private void ApplySummaryRow(StringBuilder sb) {
			sb.Append("<tr class='subtotal'>");
			foreach (var columnSelection in ColumnSelections)
				switch (columnSelection.ColumnSelection) {
					case ReportColumnSelectionsEnum.MediaPublicationType:
						sb.Append("<td><b>" + PreviousServiceCount + " Event(s)</b></td>");
						break;
					case ReportColumnSelectionsEnum.PrepareHours:
						sb.Append("<td><b>" + PreviousPrepareHrs + " Hrs(s)</b></td>");
						break;
					case ReportColumnSelectionsEnum.NumOfSegments:
						sb.Append("<td><b>" + PreviousSegmentCount + " Segment(s):</b></td>");
						break;
					case ReportColumnSelectionsEnum.StaffPrepHours:
						sb.Append("<td><b> Total " + PreviousStaffPrepareHrs + " Hr(s):</b></td>");
						break;
					case ReportColumnSelectionsEnum.Staff:
					case ReportColumnSelectionsEnum.Date:
					case ReportColumnSelectionsEnum.Title:
						sb.Append("<td></td>");
						break;
				}
			sb.Append("</tr>");
		}

		private void SetGroupTotals(MediaPublicationInformationLineItem record) {
			if (GroupingSelections.Any())
				switch (GroupingSelections.OrderBy(g => g.Order).ThenBy(g => g.GroupingSelection).First().GroupingSelection) {
					case ReportOrderSelectionsEnum.Staff:
						PreviousGroupValue = !string.IsNullOrEmpty(record.StaffName) ? record.StaffName : string.Empty;
						break;
				}
			PreviousPrepareHrs += record.PrepareHours ?? 0.0;
			PreviousStaffPrepareHrs += record.StaffPrepareHours ?? 0.0;
			PreviousSegmentCount += record.NumberOfSegments ?? 0;
			PreviousServiceCount++;
		}

		private void SetTotals(MediaPublicationInformationLineItem record) {
			_staffIds.Add(record.SvId);
			if (!_icsIds.Contains(record.IcsId)) {
				_icsIds.Add(record.IcsId);
				TotalPrepareHrs += record.PrepareHours ?? 0;
				TotalSegmentCount += record.NumberOfSegments ?? 0;
			}
			TotalStaffPrepareHrs += record.StaffPrepareHours ?? 0;
			TotalServiceCount++;
		}

		protected override void BuildLegacyHtmlSummaryRow(StringBuilder sb) {
			sb.Append("<tr>");
			foreach (var columnSelection in ColumnSelections)
				switch (columnSelection.ColumnSelection) {
					case ReportColumnSelectionsEnum.Staff:
						sb.Append("<td><b> Total Staff: " + _staffIds.Count + "</b></td>");
						break;
					case ReportColumnSelectionsEnum.MediaPublicationType:
						sb.Append("<td><b> Total Record(s): " + _icsIds.Count + "</b></td>");
						break;
					case ReportColumnSelectionsEnum.PrepareHours:
						sb.Append("<td><b> Total Hours(s): " + TotalPrepareHrs + "</b></td>");
						break;
					case ReportColumnSelectionsEnum.NumOfSegments:
						sb.Append("<td><b> Total Segment(s): " + TotalSegmentCount + "</b></td>");
						break;
					case ReportColumnSelectionsEnum.StaffPrepHours:
						sb.Append("<td><b> Total Hour(s): " + TotalStaffPrepareHrs + "</b></td>");
						break;
					case ReportColumnSelectionsEnum.Date:
					case ReportColumnSelectionsEnum.Title:
						sb.Append("<td></td>");
						break;
				}
			sb.Append("</tr>");
		}

		protected override string BuildTrueCSVLine(MediaPublicationInformationLineItem record) {
			bool applyComma = false;
			var sb = new StringBuilder();
			foreach (var columnSelection in ColumnSelections) {
				if (applyComma)
					sb.Append(",");
				switch (columnSelection.ColumnSelection) {
					case ReportColumnSelectionsEnum.Staff:
						sb.AppendQuotedCSVData(record.StaffName);
						break;
					case ReportColumnSelectionsEnum.MediaPublicationType:
						sb.AppendQuotedCSVData(Lookups.ProgramsAndServices[record.ProgramId].Description);
						break;
					case ReportColumnSelectionsEnum.Date:
						sb.AppendQuotedCSVData(record.PDate?.ToShortDateString() ?? string.Empty);
						break;
					case ReportColumnSelectionsEnum.Title:
						sb.AppendQuotedCSVData(record.Title);
						break;
					case ReportColumnSelectionsEnum.PrepareHours:
						sb.AppendQuotedCSVData(record.PrepareHours);
						break;
					case ReportColumnSelectionsEnum.NumOfSegments:
						sb.AppendQuotedCSVData(record.NumberOfSegments);
						break;
					case ReportColumnSelectionsEnum.StaffPrepHours:
						sb.AppendQuotedCSVData(record.StaffPrepareHours);
						break;
				}
				applyComma = true;
			}
			return sb.ToString();
		}

		protected override IEnumerable<MediaPublicationInformationLineItem> PerformSelect(IOrderedQueryable<PublicationDetailStaff> query) {
			return query.Select(q => new MediaPublicationInformationLineItem {
				SvId = q.SVID,
				StaffName = q.StaffVolunteer.FirstName + " " + q.StaffVolunteer.LastName,
				StaffPrepareHours = q.HoursPrep ?? 0.0,
				ProgramId = q.PublicationDetail.ProgramID,
				PDate = q.PublicationDetail.PDate,
				PrepareHours = q.PublicationDetail.PrepareHours ?? 0.0,
				Title = q.PublicationDetail.Title,
				NumberOfSegments = q.PublicationDetail.NumOfBrochure ?? 0,
				IcsId = q.ICS_ID
			});
		}
	}

	public class MediaPublicationInformationLineItem {
		public int? SvId { get; set; }
		public int? IcsId { get; set; }
		public string StaffName { get; set; }
		public int ProgramId { get; set; }
		public DateTime? PDate { get; set; }
		public double? PrepareHours { get; set; }
		public double? StaffPrepareHours { get; set; }
		public string Title { get; set; }
		public int? NumberOfSegments { get; set; }
	}
}