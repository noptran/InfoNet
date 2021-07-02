using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infonet.Core.IO;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;

namespace Infonet.Reporting.ManagementReports.Builders {
	public class OtherStaffActivitySubReport : SubReportDataBuilder<OtherStaffActivity, OtherStaffActivityLineItem> {
		public OtherStaffActivitySubReport(SubReportSelection subReportSelectionType) : base(subReportSelectionType) {
			PreviousStaffList = new List<int?>();
			TotalStaffList = new List<int?>();
			TotalDistinctActivity = new List<int?>();
			PreviousGroupValue = string.Empty;
			FirstColumnString = string.Empty;
			PreviousGroupCount = 1;
		}

		public RecordDetailOrderSelectionsEnum DetailOrGroupSelection { get; set; }
		private string FirstColumnString { get; set; }
		private OtherStaffActivityLineItem PreviousLineItem { get; set; }
		private string PreviousGroupValue { get; set; }
		private List<int?> PreviousStaffList { get; }
		private int PreviousActivityCount { get; set; }
		private float PreviousConductHours { get; set; }
		private float PreviousTravelHours { get; set; }
		private float PreviousPrepareHours { get; set; }
		private int PreviousGroupCount { get; set; }
		private List<int?> TotalStaffList { get; }
		private int TotalActivityCount { get; set; }
		private List<int?> TotalDistinctActivity { get; }
		private float TotalConductHours { get; set; }
		private float TotalTravelHours { get; set; }
		private float TotalPrepareHours { get; set; }

		protected override void BuildLegacyHtmlRow(OtherStaffActivityLineItem record, StringBuilder sb, bool isFirst, bool isLast) {
			if (!isFirst && GroupingSelections.Any()) {
				if (IsGrouped(record) || IsActivityWithSameDate(record)) {
					PreviousGroupCount++;
					if (!(IsGrouped(record) && GroupingSelections.FirstOrDefault().GroupingSelection == ReportOrderSelectionsEnum.Staff))
						TotalActivityCount--;
					PreviousLineItem.ConductingHours = PreviousLineItem.ConductingHours + record.ConductingHours;
					PreviousLineItem.PrepareHours = PreviousLineItem.PrepareHours + record.PrepareHours;
					PreviousLineItem.TravelHours = PreviousLineItem.TravelHours + record.TravelHours;
				} else {
					ApplyItemRow(PreviousLineItem, sb);
					PreviousLineItem = record;
					PreviousGroupCount = 1;
				}
				if (!PreviousGroupValue.Trim().Equals(GetCurrentGroupValue(record).Trim(), StringComparison.OrdinalIgnoreCase)) {
					ApplySummaryRow(sb);
					ResetPreviousGroupValues();
				}
			} else {
				PreviousLineItem = record;
			}
			SetGroupTotals(record);
			SetTotals(record);
		}

		private bool IsActivityWithSameDate(OtherStaffActivityLineItem record) {
			bool isActivityWithSameDate = true;
			foreach (var columnSelection in ColumnSelections) {
				switch (columnSelection.ColumnSelection) {
					case ReportColumnSelectionsEnum.Activity:
						isActivityWithSameDate = PreviousLineItem.OtherStaffActivity == record.OtherStaffActivity && PreviousLineItem.OtherStaffActivityDate == record.OtherStaffActivityDate;
						break;
				}
				if (!isActivityWithSameDate)
					return false;
			}

			PreviousActivityCount--;
			return true;
		}

		private bool IsGrouped(OtherStaffActivityLineItem record) {
			if (DetailOrGroupSelection == RecordDetailOrderSelectionsEnum.RecordDetail)
				return false;

			bool isGrouped = true;
			foreach (var columnSelection in ColumnSelections) {
				switch (columnSelection.ColumnSelection) {
					case ReportColumnSelectionsEnum.Staff:
						isGrouped = PreviousLineItem.StaffName == record.StaffName;
						break;
					case ReportColumnSelectionsEnum.Activity:
						isGrouped = PreviousLineItem.OtherStaffActivity == record.OtherStaffActivity;
						break;
					case ReportColumnSelectionsEnum.Date:
						isGrouped = PreviousLineItem.OtherStaffActivityDate == record.OtherStaffActivityDate;
						break;
				}
				if (!isGrouped)
					return false;
			}
			return true;
		}

		private string GetCurrentGroupValue(OtherStaffActivityLineItem record) {
			string currentGroupValue = string.Empty;
			switch (GroupingSelections.OrderBy(g => g.Order).ThenBy(g => g.GroupingSelection).First().GroupingSelection) {
				case ReportOrderSelectionsEnum.Staff:
					currentGroupValue = !string.IsNullOrEmpty(record.StaffName) ? record.StaffName : string.Empty;
					break;
				case ReportOrderSelectionsEnum.Activity:
					currentGroupValue = !string.IsNullOrEmpty(record.OtherStaffActivity) ? record.OtherStaffActivity : string.Empty;
					break;
			}
			return currentGroupValue;
		}

		private void ResetPreviousGroupValues() {
			PreviousGroupValue = null;
			PreviousStaffList.Clear();
			PreviousActivityCount = 0;
			PreviousPrepareHours = 0;
			PreviousTravelHours = 0;
			PreviousConductHours = 0;
		}

		private void ApplyItemRow(OtherStaffActivityLineItem record, StringBuilder sb) {
			string serviceOutputFormat = PreviousGroupCount > 1 ? "{0} ({1})" : "{0}";
			sb.Append("<tr>");
			if (record != null)
				foreach (var columnSelection in ColumnSelections)
					switch (columnSelection.ColumnSelection) {
						case ReportColumnSelectionsEnum.Staff:
							if (FirstColumn == ReportColumnSelectionsEnum.Staff) {
								if (!FirstColumnString.Equals(record.StaffName)) {
									sb.Append("<th scope='row' style='font-weight:normal;' data-svid='" + record.SvId + "'>" + record.StaffName + "</th>");
									FirstColumnString = record.StaffName;
								} else {
                                    sb.Append("<th scope='row' style='font-weight:normal;' data-svid='" + record.SvId + "'><span class='sr-only'>" + record.StaffName + "</span></th>");
                                }
							} else {
								sb.Append("<td data-svid='" + record.SvId + "'>" + string.Format(serviceOutputFormat, record.StaffName, PreviousGroupCount) + "</td>");
							}
							break;
						case ReportColumnSelectionsEnum.Activity:
							if (FirstColumn == ReportColumnSelectionsEnum.Activity) {
								if (!FirstColumnString.Equals(record.OtherStaffActivity)) {
									sb.Append("<th scope='row' style='font-weight:normal;'>" + record.OtherStaffActivity + "</th>");
									FirstColumnString = record.OtherStaffActivity;
								} else {
									sb.Append("<th scope='row' style='font-weight:normal;'><span class='sr-only'>" + record.OtherStaffActivity + "</td>");
								}
							} else {
								sb.Append("<td>" + string.Format(serviceOutputFormat, record.OtherStaffActivity, PreviousGroupCount) + "</td>");
							}
							break;
						case ReportColumnSelectionsEnum.Date:
							sb.Append("<td>" + (record.OtherStaffActivityDate.HasValue ? record.OtherStaffActivityDate.Value.ToShortDateString() : "") + "</td>");
							break;
						case ReportColumnSelectionsEnum.ConductHours:
							sb.Append("<td>" + record.ConductingHours + "</td>");
							break;
						case ReportColumnSelectionsEnum.TravelHours:
							sb.Append("<td>" + record.TravelHours + "</td>");
							break;
						case ReportColumnSelectionsEnum.PrepareHours:
							sb.Append("<td>" + record.PrepareHours + "</td>");
							break;
					}
			sb.Append("</tr>");
		}

		private void ApplySummaryRow(StringBuilder sb) {
			if (ColumnSelections.Count > 1) {
				sb.Append("<tr>");
				foreach (var columnSelection in ColumnSelections)
					if (columnSelection.ColumnSelection != FirstColumn)
						switch (columnSelection.ColumnSelection) {
							case ReportColumnSelectionsEnum.Staff:
								sb.Append("<td><b>" + PreviousStaffList.Count + " Staff</b></td>");
								break;
							case ReportColumnSelectionsEnum.Activity:
								sb.Append("<td><b>" + PreviousActivityCount + " Activities</b></td>");
								break;
							case ReportColumnSelectionsEnum.Date:
								sb.Append("<td></td>");
								break;
							case ReportColumnSelectionsEnum.ConductHours:
								sb.Append("<td><b>" + PreviousConductHours + " </b></td>");
								break;
							case ReportColumnSelectionsEnum.TravelHours:
								sb.Append("<td><b>" + PreviousTravelHours + " </b></td>");
								break;
							case ReportColumnSelectionsEnum.PrepareHours:
								sb.Append("<td><b>" + PreviousPrepareHours + " </b></td>");
								break;
						}
					else
						sb.Append("<th scope='row'>Subtotal</th>");
				sb.Append("</tr>");
			}
		}

		private void SetGroupTotals(OtherStaffActivityLineItem record) {
			if (GroupingSelections.Any())
				switch (GroupingSelections.OrderBy(g => g.Order).ThenBy(g => g.GroupingSelection).First().GroupingSelection) {
					case ReportOrderSelectionsEnum.Staff:
						PreviousGroupValue = !string.IsNullOrEmpty(record.StaffName) ? record.StaffName : string.Empty;
						break;
					case ReportOrderSelectionsEnum.Activity:
						PreviousGroupValue = !string.IsNullOrEmpty(record.OtherStaffActivity) ? record.OtherStaffActivity : string.Empty;
						break;
				}
			if (!PreviousStaffList.Contains(record.SvId))
				PreviousStaffList.Add(record.SvId);
			PreviousActivityCount++;
			PreviousConductHours += record.ConductingHours ?? 0;
			PreviousTravelHours += record.TravelHours ?? 0;
			PreviousPrepareHours += record.PrepareHours ?? 0;
		}

		private void SetTotals(OtherStaffActivityLineItem record) {
			TotalConductHours += record.ConductingHours ?? 0;
			TotalTravelHours += record.TravelHours ?? 0;
			TotalPrepareHours += record.PrepareHours ?? 0;
			TotalActivityCount++;
			if (!TotalDistinctActivity.Contains(record.OtherStaffActivityId))
				TotalDistinctActivity.Add(record.OtherStaffActivityId);
			if (!TotalStaffList.Contains(record.SvId))
				TotalStaffList.Add(record.SvId);
		}

		protected override void BuildLegacyHtmlSummaryRow(StringBuilder sb) {
			ApplyItemRow(PreviousLineItem, sb);
			ApplySummaryRow(sb);
			sb.Append("<tr>");
			foreach (var columnSelection in ColumnSelections) {
				bool isFirst = columnSelection.ColumnSelection == FirstColumn;
				switch (columnSelection.ColumnSelection) {
					case ReportColumnSelectionsEnum.Staff:
                        if (isFirst)
                            sb.Append("<th scope='row'> Grand Total: " + TotalStaffList.Count + "</th>");
                        else
                            sb.Append("<td><b>" + TotalStaffList.Count + "</b></td>");
                        break;
					case ReportColumnSelectionsEnum.Activity:
                        if (isFirst)
                            sb.Append("<th scope='row'> Grand Total: " + TotalDistinctActivity.Count + "</th>");
                        else
                            sb.Append("<td><b>" +  TotalActivityCount + "</b></td>");
                        break;
					case ReportColumnSelectionsEnum.Date:
						sb.Append("<td></td>");
						break;
					case ReportColumnSelectionsEnum.ConductHours:
						sb.Append("<td><b>" + TotalConductHours + "</b></td>");
						break;
					case ReportColumnSelectionsEnum.TravelHours:
						sb.Append("<td><b>" + TotalTravelHours + " </b></td>");
						break;
					case ReportColumnSelectionsEnum.PrepareHours:
						sb.Append("<td><b>" + TotalPrepareHours + " </b></td>");
						break;
				}
			}
			sb.Append("</tr>");
		}

		protected override string BuildTrueCSVLine(OtherStaffActivityLineItem record) {
			bool applyComma = false;
			var sb = new StringBuilder();
			foreach (var columnSelection in ColumnSelections) {
				if (applyComma)
					sb.Append(",");
				switch (columnSelection.ColumnSelection) {
					case ReportColumnSelectionsEnum.Staff:
						sb.AppendQuotedCSVData(record.StaffName);
						break;
					case ReportColumnSelectionsEnum.Activity:
						sb.AppendQuotedCSVData(record.OtherStaffActivity);
						break;
					case ReportColumnSelectionsEnum.Date:
						sb.AppendQuotedCSVData(record.OtherStaffActivityDate.HasValue ? record.OtherStaffActivityDate.Value.ToShortDateString() : string.Empty);
						break;
					case ReportColumnSelectionsEnum.ConductHours:
						sb.AppendQuotedCSVData(record.ConductingHours);
						break;
					case ReportColumnSelectionsEnum.TravelHours:
						sb.AppendQuotedCSVData(record.TravelHours);
						break;
					case ReportColumnSelectionsEnum.PrepareHours:
						sb.AppendQuotedCSVData(record.PrepareHours);
						break;
				}
				applyComma = true;
			}
			return sb.ToString();
		}

		protected override IEnumerable<OtherStaffActivityLineItem> PerformSelect(IOrderedQueryable<OtherStaffActivity> query) {
			var ret = query.Select(q => new OtherStaffActivityLineItem {
				SvId = q.SVID,
				StaffName = q.StaffVolunteer.FirstName + " " + q.StaffVolunteer.LastName,
				OtherStaffActivityId = q.OtherStaffActivityID,
				OtherStaffActivity = q.TLU_Codes_OtherStaffActivity.Description,
				OtherStaffActivityDate = q.OsaDate,
				ConductingHours = q.ConductingHours,
				PrepareHours = q.PrepareHours,
				TravelHours = q.TravelHours
			});
			return ret;
		}
	}

	public class OtherStaffActivityLineItem {
		public int? SvId { get; set; }
		public string StaffName { get; set; }
		public DateTime? OtherStaffActivityDate { get; set; }
		public int? OtherStaffActivityId { get; set; }
		public string OtherStaffActivity { get; set; }
		public float? ConductingHours { get; set; }
		public float? TravelHours { get; set; }
		public float? PrepareHours { get; set; }
	}
}