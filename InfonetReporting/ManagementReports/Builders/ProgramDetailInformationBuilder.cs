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
	public class StaffProgramDetailInformationSubReport : SubReportDataBuilder<ProgramDetailStaff, ProgramDetailLineItem> {
		public StaffProgramDetailInformationSubReport(SubReportSelection subReportSelectionType) : base(subReportSelectionType) {
			PreviousGroupValue = string.Empty;
            IcsIds = new HashSet<int?>();
            Svid = new HashSet<int>();
        }

		private string PreviousGroupValue { get; set; }
		private int PreviousNumOfPresentations { get; set; }
		private int PreviousNumOfParticipants { get; set; }
		private double PreviousServiceHrs { get; set; }
		private double PreviousStaffPrepareHrs { get; set; }
		private double PreviousStaffPresentationHrs { get; set; }
		private double PreviousStaffTravelHrs { get; set; }
        private HashSet<int?> IcsIds { get; }
        private HashSet<int> Svid { get; }
        private int TotalNumOfParticipants { get; set; }
		private double TotalPresentationHrs { get; set; }
		private double TotalStaffPrepareHrs { get; set; }
		private double TotalStaffPresentationHrs { get; set; }
		private double TotalStaffTravelHrs { get; set; }
        private int TotalNumOfPresentations { get; set; }

		protected override void BuildLegacyHtmlRow(ProgramDetailLineItem record, StringBuilder sb, bool isFirst, bool isLast) {
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
					PreviousNumOfPresentations = 0;
					PreviousServiceHrs = 0;
					PreviousNumOfParticipants = 0;
					PreviousStaffPrepareHrs = 0;
					PreviousStaffPresentationHrs = 0;
					PreviousStaffTravelHrs = 0;
				}
			}

			sb.Append("<tr>");
			foreach (var columnSelection in ColumnSelections)
				switch (columnSelection.ColumnSelection) {
					case ReportColumnSelectionsEnum.Staff:
						if (string.IsNullOrEmpty(PreviousGroupValue))
							sb.Append("<th scope='row' style='font-weight:normal;'>" + record.StaffName + "</th>");
						else
							sb.Append("<th scope='row' style='font-weight:normal;'><span class='sr-only'>" + record.StaffName  + "</span></td>");
						break;
					case ReportColumnSelectionsEnum.ServiceName:
						sb.Append("<td>" + Lookups.ProgramsAndServices[record.ProgramId].Description + "</td>");
						break;
					case ReportColumnSelectionsEnum.ServiceDate:
						sb.Append("<td>" + (record.ServiceDate.HasValue ? record.ServiceDate.Value.ToShortDateString() : string.Empty) + "</td>");
						break;
					case ReportColumnSelectionsEnum.NumOfPresentations:
						sb.Append("<td>" + record.NumOfPresentations + "</td>");
						break;
					case ReportColumnSelectionsEnum.PresentationHrs:
						sb.Append("<td>" + record.PresentationHours + "</td>");
						break;
					case ReportColumnSelectionsEnum.NumOfParticipants:
						sb.Append("<td>" + record.NumOfParticipants + "</td>");
						break;
					case ReportColumnSelectionsEnum.StaffPresentationHrs:
						sb.Append("<td>" + record.StaffPresentationHrs + "</td>");
						break;
					case ReportColumnSelectionsEnum.StaffPrepHours:
						sb.Append("<td>" + record.StaffPrepareHrs + "</td>");
						break;
					case ReportColumnSelectionsEnum.StaffTravelHours:
						sb.Append("<td>" + record.StaffTravelHrs + "</td>");
						break;
					case ReportColumnSelectionsEnum.Agency:
						sb.Append("<td>" + record.AgencyName + "</td>");
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
				PreviousNumOfPresentations = 0;
				PreviousNumOfParticipants = 0;
				PreviousServiceHrs = 0.0;
				PreviousStaffPrepareHrs = 0.0;
				PreviousStaffPresentationHrs = 0;
				PreviousStaffTravelHrs = 0;
			}
		}

		private void ApplySummaryRow(StringBuilder sb) {
			sb.Append("<tr class='subtotal'>");
			foreach (var columnSelection in ColumnSelections)
				switch (columnSelection.ColumnSelection) {
					case ReportColumnSelectionsEnum.NumOfPresentations:
						sb.Append("<td><b>" + PreviousNumOfPresentations + " Contact(s)</b></td>");
						break;
					case ReportColumnSelectionsEnum.PresentationHrs:
						sb.Append("<td><b>" + PreviousServiceHrs + " Hr(s)</b></td>");
						break;
					case ReportColumnSelectionsEnum.NumOfParticipants:
						sb.Append("<td><b>" + PreviousNumOfParticipants + " Participant(s)</b></td>");
						break;
					case ReportColumnSelectionsEnum.StaffPresentationHrs:
						sb.Append("<td><b>" + PreviousStaffPresentationHrs + " Hr(s)</b></td>");
						break;
					case ReportColumnSelectionsEnum.StaffPrepHours:
						sb.Append("<td><b>" + PreviousStaffPrepareHrs + " Hr(s)</b></td>");
						break;
					case ReportColumnSelectionsEnum.StaffTravelHours:
						sb.Append("<td><b>" + PreviousStaffTravelHrs + " Hr(s)</b></td>");
						break;
					case ReportColumnSelectionsEnum.Staff:
					case ReportColumnSelectionsEnum.ServiceName:
					case ReportColumnSelectionsEnum.ServiceDate:
					case ReportColumnSelectionsEnum.Agency:
					case ReportColumnSelectionsEnum.Location:
						sb.Append("<td></td>");
						break;
				}
			sb.Append("</tr>");
		}

		private void SetGroupTotals(ProgramDetailLineItem record) {
			if (GroupingSelections.Any())
				switch (GroupingSelections.OrderBy(g => g.Order).ThenBy(g => g.GroupingSelection).First().GroupingSelection) {
					case ReportOrderSelectionsEnum.Staff:
						PreviousGroupValue = !string.IsNullOrEmpty(record.StaffName) ? record.StaffName : string.Empty;
						break;
				}
			PreviousServiceHrs += record.PresentationHours;
			PreviousStaffPrepareHrs += record.StaffPrepareHrs;
			PreviousStaffTravelHrs += record.StaffTravelHrs;
			PreviousStaffPresentationHrs += record.StaffPresentationHrs;
			PreviousNumOfParticipants += record.NumOfParticipants;
			PreviousNumOfPresentations += record.NumOfPresentations;
		}

		private void SetTotals(ProgramDetailLineItem record) {
            if (!IcsIds.Contains(record.IcsId)) {
                IcsIds.Add(record.IcsId);
				TotalPresentationHrs += record.PresentationHours;
				TotalNumOfParticipants += record.NumOfParticipants;
				TotalNumOfPresentations += record.NumOfPresentations;
			}

            if (!Svid.Contains(record.SVID)){
                Svid.Add(record.SVID);
            }

            TotalStaffPrepareHrs += record.StaffPrepareHrs;
            TotalStaffPresentationHrs += record.StaffPresentationHrs;
            TotalStaffTravelHrs += record.StaffTravelHrs;
        }

		protected override void BuildLegacyHtmlSummaryRow(StringBuilder sb) {
			sb.Append("<tr>");
			foreach (var columnSelection in ColumnSelections)
				switch (columnSelection.ColumnSelection) {
					case ReportColumnSelectionsEnum.Staff:
						sb.Append("<td><b> Total Staff: " + Svid.Count + "</b></td>");
						break;
					case ReportColumnSelectionsEnum.NumOfPresentations:
						sb.Append("<td><b> Total Contact(s): " + TotalNumOfPresentations + "</b></td>");
						break;
					case ReportColumnSelectionsEnum.PresentationHrs:
						sb.Append("<td><b> Total Hours(s): " + TotalPresentationHrs + "</b></td>");
						break;
					case ReportColumnSelectionsEnum.NumOfParticipants:
						sb.Append("<td><b> Total Participant(s): " + TotalNumOfParticipants + "</b></td>");
						break;
					case ReportColumnSelectionsEnum.StaffPresentationHrs:
						sb.Append("<td><b> Total Hour(s): " + TotalStaffPresentationHrs + "</b></td>");
						break;
					case ReportColumnSelectionsEnum.StaffPrepHours:
						sb.Append("<td><b> Total Hour(s): " + TotalStaffPrepareHrs + "</b></td>");
						break;
					case ReportColumnSelectionsEnum.StaffTravelHours:
						sb.Append("<td><b> Total Hour(s): " + TotalStaffTravelHrs + "</b></td>");
						break;
					case ReportColumnSelectionsEnum.ServiceName:
					case ReportColumnSelectionsEnum.ServiceDate:
					case ReportColumnSelectionsEnum.Agency:
					case ReportColumnSelectionsEnum.Location:
						sb.Append("<td></td>");
						break;
				}
			sb.Append("</tr>");
		}

		protected override string BuildTrueCSVLine(ProgramDetailLineItem record) {
			bool applyComma = false;
			var sb = new StringBuilder();
			foreach (var columnSelection in ColumnSelections) {
				if (applyComma)
					sb.Append(",");
				switch (columnSelection.ColumnSelection) {
					case ReportColumnSelectionsEnum.Staff:
						sb.AppendQuotedCSVData(record.StaffName);
						break;
					case ReportColumnSelectionsEnum.ServiceName:
						sb.AppendQuotedCSVData(Lookups.ProgramsAndServices[record.ProgramId].Description);
						break;
					case ReportColumnSelectionsEnum.ServiceDate:
						sb.AppendQuotedCSVData(record.ServiceDate.HasValue ? record.ServiceDate.Value.ToShortDateString() : string.Empty);
						break;
					case ReportColumnSelectionsEnum.NumOfPresentations:
						sb.AppendQuotedCSVData(record.NumOfPresentations);
						break;
					case ReportColumnSelectionsEnum.PresentationHrs:
						sb.AppendQuotedCSVData(record.PresentationHours);
						break;
					case ReportColumnSelectionsEnum.NumOfParticipants:
						sb.AppendQuotedCSVData(record.NumOfParticipants);
						break;
					case ReportColumnSelectionsEnum.StaffPresentationHrs:
						sb.AppendQuotedCSVData(record.StaffPresentationHrs);
						break;
					case ReportColumnSelectionsEnum.StaffPrepHours:
						sb.AppendQuotedCSVData(record.StaffPrepareHrs);
						break;
					case ReportColumnSelectionsEnum.StaffTravelHours:
						sb.AppendQuotedCSVData(record.StaffTravelHrs);
						break;
					case ReportColumnSelectionsEnum.Agency:
						sb.AppendQuotedCSVData(record.AgencyName);
						break;
					case ReportColumnSelectionsEnum.Location:
						sb.AppendQuotedCSVData(record.Location);
						break;
				}
				applyComma = true;
			}
			return sb.ToString();
		}

		protected override IEnumerable<ProgramDetailLineItem> PerformSelect(IOrderedQueryable<ProgramDetailStaff> query) {
			var ret = query.Select(q => new ProgramDetailLineItem {
                SVID = q.StaffVolunteer.SvId,
				StaffName = q.StaffVolunteer.FirstName + " " + q.StaffVolunteer.LastName,
				ProgramId = q.ProgramDetail.ProgramID,
				ServiceDate = q.ProgramDetail.PDate,
				PresentationHours = q.ProgramDetail.Hours ?? 0.0,
				Location = q.ProgramDetail.Location,
				NumOfPresentations = q.ProgramDetail.NumOfSession ?? 0,
				AgencyName = q.ProgramDetail.Agency.AgencyName,
				StaffPrepareHrs = q.HoursPrep ?? 0.0,
				StaffPresentationHrs = q.ConductHours ?? 0.0,
				StaffTravelHrs = q.HoursTravel ?? 0.0,
				NumOfParticipants = q.ProgramDetail.ParticipantsNum ?? 0,
                IcsId = q.ICS_ID
			});
			return ret;
		}
	}

	public class ProgramDetailLineItem {
        public int SVID { get; set; }
        public int? IcsId { get; set; }
		public string StaffName { get; set; }
		public DateTime? ServiceDate { get; set; }
		public int ProgramId { get; set; }
		public string AgencyName { get; set; }
		public string Location { get; set; }
		public int NumOfPresentations { get; set; }
		public int NumOfParticipants { get; set; }
		public double PresentationHours { get; set; }
		public double StaffPrepareHrs { get; set; }
		public double StaffPresentationHrs { get; set; }
		public double StaffTravelHrs { get; set; }
	}
}