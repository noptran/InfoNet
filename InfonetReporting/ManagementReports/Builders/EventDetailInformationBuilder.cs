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
	public class StaffEventDetailInformationSubReport : SubReportDataBuilder<EventDetailStaff, EventInformationLineItem> {
		public StaffEventDetailInformationSubReport(SubReportSelection subReportSelectionType) : base(subReportSelectionType) {
			PreviousGroupValue = string.Empty;
			StaffIds = new List<int?>();
		}

		private string PreviousGroupValue { get; set; }
		private int PreviousEventCount { get; set; }
		private int PreviousPplReachedCount { get; set; }
		private double PreviousEventHrs { get; set; }
		private double PreviousStaffPrepareHrs { get; set; }
		private double PreviousStaffConductHrs { get; set; }
		private double PreviousStaffTravelHrs { get; set; }

		private int TotalEventCount { get; set; }
		private int TotalPeopleReached { get; set; }
		private double TotalEventHrs { get; set; }
		private double TotalStaffPrepareHrs { get; set; }
		private double TotalStaffConductHrs { get; set; }
		private double TotalStaffTravelHrs { get; set; }
		private List<int?> StaffIds { get; }

		protected override void BuildLegacyHtmlRow(EventInformationLineItem record, StringBuilder sb, bool isFirst, bool isLast) {
			if (!isFirst && GroupingSelections.Any()) {
				string currentGroupValue = string.Empty;
				if (GroupingSelections.OrderBy(g => g.Order).ThenBy(g => g.GroupingSelection).First().GroupingSelection == ReportOrderSelectionsEnum.Staff)
					currentGroupValue = !string.IsNullOrEmpty(record.StaffName) ? record.StaffName : string.Empty;
				if (!PreviousGroupValue.Trim().Equals(currentGroupValue.Trim(), StringComparison.OrdinalIgnoreCase)) {
					ApplySummaryRow(sb);
					PreviousGroupValue = null;
					PreviousEventCount = 0;
					PreviousEventHrs = 0;
					PreviousPplReachedCount = 0;
					PreviousStaffPrepareHrs = 0;
					PreviousStaffConductHrs = 0;
					PreviousStaffTravelHrs = 0;
				}
			}

			sb.Append("<tr>");
			foreach (var columnSelection in ColumnSelections)
				switch (columnSelection.ColumnSelection) {
					case ReportColumnSelectionsEnum.Staff:
						if (string.IsNullOrEmpty(PreviousGroupValue))
							sb.Append("<td>" + record.StaffName + "</td>");
						else
							sb.Append("<td></td>");
						break;
					case ReportColumnSelectionsEnum.EventType:
						sb.Append("<td>" + Lookups.ProgramsAndServices[record.EventId].Description + "</td>");
						break;
					case ReportColumnSelectionsEnum.Date:
						sb.Append("<td>" + (record.EventDate.HasValue ? record.EventDate.Value.ToShortDateString() : string.Empty) + "</td>");
						break;
					case ReportColumnSelectionsEnum.EventName:
						sb.Append("<td>" + record.EventName + "</td>");
						break;
					case ReportColumnSelectionsEnum.EventHrs:
						sb.Append("<td>" + record.EventHours + "</td>");
						break;
					case ReportColumnSelectionsEnum.PeopleReached:
						sb.Append("<td>" + record.NumPeopleReached + "</td>");
						break;
					case ReportColumnSelectionsEnum.StaffConductHours:
						sb.Append("<td>" + record.StaffConductHrs + "</td>");
						break;
					case ReportColumnSelectionsEnum.StaffPrepHours:
						sb.Append("<td>" + record.StaffPrepareHrs + "</td>");
						break;
					case ReportColumnSelectionsEnum.StaffTravelHours:
						sb.Append("<td>" + record.StaffTravelHrs + "</td>");
						break;
					case ReportColumnSelectionsEnum.Location:
						sb.Append("<td>" + record.Location + "</td>");
						break;
				}
			sb.Append("</tr>");

			SetGroupTotals(record);
			SetTotals(record);

			if (isLast) {
				ApplySummaryRow(sb);
				PreviousGroupValue = null;
				PreviousEventCount = 0;
				PreviousPplReachedCount = 0;
				PreviousEventHrs = 0.0;
				PreviousStaffPrepareHrs = 0.0;
				PreviousStaffConductHrs = 0;
				PreviousStaffTravelHrs = 0;
			}
		}

		private void ApplySummaryRow(StringBuilder sb) {
			sb.Append("<tr class='subtotal'>");
			foreach (var columnSelection in ColumnSelections)
				switch (columnSelection.ColumnSelection) {
					case ReportColumnSelectionsEnum.Staff:
						sb.Append("<td></td>");
						break;
					case ReportColumnSelectionsEnum.EventType:
						sb.Append("<td><b>" + PreviousEventCount + " Event(s)</b></td>");
						break;
					case ReportColumnSelectionsEnum.EventHrs:
						sb.Append("<td><b>" + PreviousEventHrs + " Hr(s)</b></td>");
						break;
					case ReportColumnSelectionsEnum.PeopleReached:
						sb.Append("<td><b>" + PreviousPplReachedCount + " People</b></td>");
						break;
					case ReportColumnSelectionsEnum.StaffConductHours:
						sb.Append("<td><b>" + PreviousStaffConductHrs + " Hr(s)</b></td>");
						break;
					case ReportColumnSelectionsEnum.StaffPrepHours:
						sb.Append("<td><b>" + PreviousStaffPrepareHrs + " Hr(s)</b></td>");
						break;
					case ReportColumnSelectionsEnum.StaffTravelHours:
						sb.Append("<td><b>" + PreviousStaffTravelHrs + " Hr(s)</b></td>");
						break;
					case ReportColumnSelectionsEnum.Date:
					case ReportColumnSelectionsEnum.EventName:
					case ReportColumnSelectionsEnum.Location:
						sb.Append("<td></td>");
						break;
				}
			sb.Append("</tr>");
		}

		private void SetGroupTotals(EventInformationLineItem record) {
			if (GroupingSelections.Any())
				switch (GroupingSelections.OrderBy(g => g.Order).ThenBy(g => g.GroupingSelection).First().GroupingSelection) {
					case ReportOrderSelectionsEnum.Staff:
						PreviousGroupValue = !string.IsNullOrEmpty(record.StaffName) ? record.StaffName : string.Empty;
						break;
				}
			PreviousEventHrs += record.EventHours;
			PreviousStaffPrepareHrs += record.StaffPrepareHrs;
			PreviousStaffTravelHrs += record.StaffTravelHrs;
			PreviousStaffConductHrs += record.StaffConductHrs;
			PreviousPplReachedCount += record.NumPeopleReached;
			PreviousEventCount++;
		}

		private void SetTotals(EventInformationLineItem record) {
			if (!StaffIds.Contains(record.SvId))
				StaffIds.Add(record.SvId);

			TotalEventHrs += record.EventHours;
			TotalStaffPrepareHrs += record.StaffPrepareHrs;
			TotalStaffConductHrs += record.StaffConductHrs;
			TotalStaffTravelHrs += record.StaffTravelHrs;
			TotalPeopleReached += record.NumPeopleReached;
			TotalEventCount++;
		}

		protected override void BuildLegacyHtmlSummaryRow(StringBuilder sb) {
			sb.Append("<tr>");
			foreach (var columnSelection in ColumnSelections)
				switch (columnSelection.ColumnSelection) {
					case ReportColumnSelectionsEnum.Staff:
						sb.Append("<td><b> Total Staff: " + StaffIds.Count + "</b></td>");
						break;
					case ReportColumnSelectionsEnum.EventType:
						sb.Append("<td><b> Total Event(s): " + TotalEventCount + "</b></td>");
						break;
					case ReportColumnSelectionsEnum.EventHrs:
						sb.Append("<td><b> Total Hours(s): " + TotalEventHrs + "</b></td>");
						break;
					case ReportColumnSelectionsEnum.PeopleReached:
						sb.Append("<td><b> Total People: " + TotalPeopleReached + "</b></td>");
						break;
					case ReportColumnSelectionsEnum.StaffConductHours:
						sb.Append("<td><b> Total Hour(s): " + TotalStaffConductHrs + "</b></td>");
						break;
					case ReportColumnSelectionsEnum.StaffPrepHours:
						sb.Append("<td><b> Total Hour(s): " + TotalStaffPrepareHrs + "</b></td>");
						break;
					case ReportColumnSelectionsEnum.StaffTravelHours:
						sb.Append("<td><b> Total Hour(s): " + TotalStaffTravelHrs + "</b></td>");
						break;
					case ReportColumnSelectionsEnum.Date:
					case ReportColumnSelectionsEnum.EventName:
					case ReportColumnSelectionsEnum.Location:
						sb.Append("<td></td>");
						break;
				}
			sb.Append("</tr>");
		}

		protected override string BuildTrueCSVLine(EventInformationLineItem record) {
			bool applyComma = false;
			var sb = new StringBuilder();
			foreach (var columnSelection in ColumnSelections) {
				if (applyComma)
					sb.Append(",");
				switch (columnSelection.ColumnSelection) {
					case ReportColumnSelectionsEnum.Staff:
						sb.AppendQuotedCSVData(record.StaffName);
						break;
					case ReportColumnSelectionsEnum.EventType:
						sb.AppendQuotedCSVData(Lookups.ProgramsAndServices[record.EventId].Description);
						break;
					case ReportColumnSelectionsEnum.Date:
						sb.AppendQuotedCSVData(record.EventDate?.ToShortDateString() ?? string.Empty);
						break;
					case ReportColumnSelectionsEnum.EventName:
						sb.AppendQuotedCSVData(record.EventName);
						break;
					case ReportColumnSelectionsEnum.EventHrs:
						sb.AppendQuotedCSVData(record.EventHours);
						break;
					case ReportColumnSelectionsEnum.PeopleReached:
						sb.AppendQuotedCSVData(record.NumPeopleReached);
						break;
					case ReportColumnSelectionsEnum.StaffConductHours:
						sb.AppendQuotedCSVData(record.StaffConductHrs);
						break;
					case ReportColumnSelectionsEnum.StaffPrepHours:
						sb.AppendQuotedCSVData(record.StaffPrepareHrs);
						break;
					case ReportColumnSelectionsEnum.StaffTravelHours:
						sb.AppendQuotedCSVData(record.StaffTravelHrs);
						break;
					case ReportColumnSelectionsEnum.Location:
						sb.AppendQuotedCSVData(record.Location);
						break;
				}
				applyComma = true;
			}
			return sb.ToString();
		}

		protected override IEnumerable<EventInformationLineItem> PerformSelect(IOrderedQueryable<EventDetailStaff> query) {
			var ret = query.Select(q => new EventInformationLineItem {
				SvId = q.SVID,
				StaffName = q.StaffVolunteer.FirstName + " " + q.StaffVolunteer.LastName,
				EventId = q.EventDetail.ProgramID,
				EventDate = q.EventDetail.EventDate,
				EventHours = q.EventDetail.EventHours ?? 0.0,
				Location = q.EventDetail.Location,
				NumPeopleReached = q.EventDetail.NumPeopleReached ?? 0,
				EventName = q.EventDetail.EventName,
				StaffPrepareHrs = q.HoursPrep ?? 0.0,
				StaffConductHrs = q.HoursConduct ?? 0.0,
				StaffTravelHrs = q.HoursTravel ?? 0.0
			});
			return ret;
		}
	}

	public class EventInformationLineItem {
		public int? SvId { get; set; }
		public string StaffName { get; set; }
		public int EventId { get; set; }
		public DateTime? EventDate { get; set; }
		public double EventHours { get; set; }
		public double StaffPrepareHrs { get; set; }
		public string Location { get; set; }
		public int NumPeopleReached { get; set; }
		public string EventName { get; set; }
		public double StaffConductHrs { get; set; }
		public double StaffTravelHrs { get; set; }
	}
}