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
	public class HotlineCrisisInformationSubReport : SubReportDataBuilder<PhoneHotline, CrisisInterventionLineItem> {
		public HotlineCrisisInformationSubReport(SubReportSelection subReportSelectionType) : base(subReportSelectionType) {
			PreviousStaffList = new List<int?>();
			TotalStaffList = new List<int?>();
			PreviousGroupValue = string.Empty;
			FirstColumnString = string.Empty;
		}

		private string FirstColumnString { get; set; }
		private CrisisInterventionLineItem PreviousLineItem { get; set; }
		private string PreviousGroupValue { get; set; }
		private List<int?> PreviousStaffList { get; }
		private int PreviousNumOfContacts { get; set; }
		private double PreviousNumOfHours { get; set; }
		private int TotalContacts { get; set; }
		private List<int?> TotalStaffList { get; }
		private double TotalHours { get; set; }

		protected override void BuildLegacyHtmlRow(CrisisInterventionLineItem record, StringBuilder sb, bool isFirst, bool isLast) {
			if (!isFirst && GroupingSelections.Any()) {
				ApplyItemRow(PreviousLineItem, sb);
				PreviousLineItem = record;
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

		private string GetCurrentGroupValue(CrisisInterventionLineItem record) {
			string currentGroupValue = string.Empty;
			switch (GroupingSelections.OrderBy(g => g.Order).ThenBy(g => g.GroupingSelection).First().GroupingSelection) {
				case ReportOrderSelectionsEnum.Staff:
					currentGroupValue = record.StaffName ?? string.Empty;
					break;
				case ReportOrderSelectionsEnum.Town:
					currentGroupValue = record.Town ?? string.Empty;
					break;
				case ReportOrderSelectionsEnum.Township:
					currentGroupValue = record.Township ?? string.Empty;
					break;
				case ReportOrderSelectionsEnum.County:
					currentGroupValue = record.CountyName ?? string.Empty;
					break;
				case ReportOrderSelectionsEnum.ZipCode:
					currentGroupValue = record.ZipCode ?? string.Empty;
					break;
			}
			return currentGroupValue;
		}

		private void ResetPreviousGroupValues() {
			PreviousGroupValue = null;
			PreviousStaffList.Clear();
			PreviousNumOfContacts = 0;
			PreviousNumOfHours = 0;
		}

		private void ApplyItemRow(CrisisInterventionLineItem record, StringBuilder sb) {
			sb.Append("<tr>");
			if (record != null)
				foreach (var columnSelection in ColumnSelections)
					switch (columnSelection.ColumnSelection) {
						case ReportColumnSelectionsEnum.Staff:
							if (!FirstColumnString.Equals(record.StaffName)) {
								sb.Append("<th scope='row' style='font-weight:normal;' data-svid='" + record.SvId + "'>" + record.StaffName + "</th>");
								FirstColumnString = record.StaffName;
							} else {
                                sb.Append("<th scope='row' style='font-weight:normal;' data-svid='" + record.SvId + "'><span class='sr-only'>" + record.StaffName + "</span></th>");
                            }
							break;
						case ReportColumnSelectionsEnum.ContactType:
							sb.Append("<td>" + Lookups.HotlineCallType[record.ContactTypeId]?.Description + "</td>");
							break;
						case ReportColumnSelectionsEnum.ContactDate:
							sb.Append("<td>" + (record.ContactDate.HasValue ? record.ContactDate.Value.ToShortDateString() : "") + "</td>");
							break;
						case ReportColumnSelectionsEnum.NumOfContacts:
							sb.Append("<td>" + record.NumberOfContacts + "</td>");
							break;
						case ReportColumnSelectionsEnum.ContactTime:
							sb.Append("<td>" + record.TotalTime + "</td>");
							break;
						case ReportColumnSelectionsEnum.HotlineCallType:
							sb.Append("<td>" + Lookups.HotlineCallType[record.ContactTypeId]?.Description + "</td>");
							break;
						case ReportColumnSelectionsEnum.HotlineCallDate:
							sb.Append("<td>" + (record.ContactDate.HasValue ? record.ContactDate.Value.ToShortDateString() : "") + "</td>");
							break;
						case ReportColumnSelectionsEnum.HotlineCallTime:
							sb.Append("<td>" + record.TotalTime + "</td>");
							break;
						case ReportColumnSelectionsEnum.HotlineCallContacts:
							sb.Append("<td>" + record.NumberOfContacts + "</td>");
							break;
						case ReportColumnSelectionsEnum.Town:
							sb.Append("<td>" + record.Town + "</td>");
							break;
						case ReportColumnSelectionsEnum.Township:
							sb.Append("<td>" + record.Township + "</td>");
							break;
						case ReportColumnSelectionsEnum.County:
							sb.Append("<td>" + record.CountyName + "</td>");
							break;
						case ReportColumnSelectionsEnum.ZipCode:
							sb.Append("<td>" + record.ZipCode + "</td>");
							break;
					}
			sb.Append("</tr>");
		}

		private void ApplySummaryRow(StringBuilder sb) {
			if (ColumnSelections.Count > 1) {
				sb.Append("<tr class='subtotal'>");
				foreach (var columnSelection in ColumnSelections)
					switch (columnSelection.ColumnSelection) {
						case ReportColumnSelectionsEnum.NumOfContacts:
							sb.Append("<td><b>" + PreviousNumOfContacts + " Contact(s)</b></td>");
							break;
						case ReportColumnSelectionsEnum.ContactTime:
							sb.Append("<td><b>" + PreviousNumOfHours + " Hrs</b></td>");
							break;
						case ReportColumnSelectionsEnum.HotlineCallTime:
							sb.Append("<td><b>" + PreviousNumOfHours + " Mins</b></td>");
							break;
						case ReportColumnSelectionsEnum.HotlineCallContacts:
							sb.Append("<td><b>" + PreviousNumOfContacts + " Contact(s)</b></td>");
							break;
						default:
							sb.Append("<td></td>");
							break;
					}
				sb.Append("</tr>");
			}
		}

		private void SetGroupTotals(CrisisInterventionLineItem record) {
			if (GroupingSelections.Any())
				switch (GroupingSelections.OrderBy(g => g.Order).ThenBy(g => g.GroupingSelection).First().GroupingSelection) {
					case ReportOrderSelectionsEnum.Staff:
						PreviousGroupValue = record.StaffName ?? string.Empty;
						break;
					case ReportOrderSelectionsEnum.Town:
						PreviousGroupValue = record.Town ?? string.Empty;
						break;
					case ReportOrderSelectionsEnum.Township:
						PreviousGroupValue = record.Township ?? string.Empty;
						break;
					case ReportOrderSelectionsEnum.County:
						PreviousGroupValue = record.CountyName ?? string.Empty;
						break;
					case ReportOrderSelectionsEnum.ZipCode:
						PreviousGroupValue = record.ZipCode ?? string.Empty;
						break;
				}
			if (!PreviousStaffList.Contains(record.SvId))
				PreviousStaffList.Add(record.SvId);

			PreviousNumOfContacts += record.NumberOfContacts;
			PreviousNumOfHours += record.TotalTime;
		}

		private void SetTotals(CrisisInterventionLineItem record) {
			if (!TotalStaffList.Contains(record.SvId))
				TotalStaffList.Add(record.SvId);

			TotalHours += record.TotalTime;
			TotalContacts += record.NumberOfContacts;
		}

		protected override void BuildLegacyHtmlSummaryRow(StringBuilder sb) {
			ApplyItemRow(PreviousLineItem, sb);
			ApplySummaryRow(sb);
			sb.Append("<tr>");
			foreach (var columnSelection in ColumnSelections)
				switch (columnSelection.ColumnSelection) {
					case ReportColumnSelectionsEnum.Staff:
						sb.Append("<td><b>Total Staff: " + TotalStaffList.Count + "</b></td>");
						break;
					case ReportColumnSelectionsEnum.NumOfContacts:
						sb.Append("<td><b>Total Contacts: " + TotalContacts + "</b></td>");
						break;
					case ReportColumnSelectionsEnum.ContactTime:
						sb.Append("<td><b>Total Hrs: " + TotalHours + "</b></td>");
						break;
					case ReportColumnSelectionsEnum.HotlineCallTime:
						sb.Append("<td><b>Total: " + TotalHours + " Mins (" + TotalHours / 60 + " Hrs)" + "</b></td>");
						break;
					case ReportColumnSelectionsEnum.HotlineCallContacts:
						sb.Append("<td><b>Total Contacts: " + TotalContacts + "</b></td>");
						break;
					case ReportColumnSelectionsEnum.ContactType:
					case ReportColumnSelectionsEnum.ContactDate:
					case ReportColumnSelectionsEnum.HotlineCallType:
					case ReportColumnSelectionsEnum.HotlineCallDate:
					case ReportColumnSelectionsEnum.Town:
					case ReportColumnSelectionsEnum.Township:
					case ReportColumnSelectionsEnum.County:
					case ReportColumnSelectionsEnum.ZipCode:
						sb.Append("<td></td>");
						break;
				}
			sb.Append("</tr>");
		}

		protected override string BuildTrueCSVLine(CrisisInterventionLineItem record) {
			bool applyComma = false;
			var sb = new StringBuilder();
			foreach (var columnSelection in ColumnSelections) {
				if (applyComma)
					sb.Append(",");
				switch (columnSelection.ColumnSelection) {
					case ReportColumnSelectionsEnum.Staff:
						sb.AppendQuotedCSVData(record.StaffName);
						break;
					case ReportColumnSelectionsEnum.ContactType:
						sb.AppendQuotedCSVData(Lookups.HotlineCallType[record.ContactTypeId]?.Description);
						break;
					case ReportColumnSelectionsEnum.ContactDate:
						sb.AppendQuotedCSVData(record.ContactDate?.ToShortDateString() ?? string.Empty);
						break;
					case ReportColumnSelectionsEnum.NumOfContacts:
						sb.AppendQuotedCSVData(record.NumberOfContacts);
						break;
					case ReportColumnSelectionsEnum.ContactTime:
						sb.AppendQuotedCSVData(record.TotalTime);
						break;
					case ReportColumnSelectionsEnum.HotlineCallType:
						sb.AppendQuotedCSVData(Lookups.HotlineCallType[record.ContactTypeId]?.Description);
						break;
					case ReportColumnSelectionsEnum.HotlineCallDate:
						sb.AppendQuotedCSVData(record.ContactDate?.ToShortDateString() ?? string.Empty);
						break;
					case ReportColumnSelectionsEnum.HotlineCallTime:
						sb.AppendQuotedCSVData(record.TotalTime);
						break;
					case ReportColumnSelectionsEnum.HotlineCallContacts:
						sb.AppendQuotedCSVData(record.NumberOfContacts);
						break;
					case ReportColumnSelectionsEnum.Town:
						sb.AppendQuotedCSVData(record.Town);
						break;
					case ReportColumnSelectionsEnum.Township:
						sb.AppendQuotedCSVData(record.Township);
						break;
					case ReportColumnSelectionsEnum.County:
						sb.AppendQuotedCSVData(record.CountyName);
						break;
					case ReportColumnSelectionsEnum.ZipCode:
						sb.AppendQuotedCSVData(record.ZipCode);
						break;
				}
				applyComma = true;
			}
			return sb.ToString();
		}

		protected override IEnumerable<CrisisInterventionLineItem> PerformSelect(IOrderedQueryable<PhoneHotline> query) {
			return query.Select(q => new CrisisInterventionLineItem {
				SvId = q.SVID,
				StaffName = q.StaffVolunteer.FirstName + " " + q.StaffVolunteer.LastName,
				ContactTypeId = q.CallTypeID,
				NumberOfContacts = q.NumberOfContacts ?? 0,
				ContactDate = q.Date,
				TotalTime = q.TotalTime ?? 0.0,
				CountyId = q.CountyID,
				Town = q.Town,
				Township = q.Township,
				ZipCode = q.ZipCode
			});
		}

		protected override void PrepareRecord(CrisisInterventionLineItem record) {
			record.CountyName = record.CountyId.HasValue ? ReportContainer.UspsContext.Helper.IllinoisCountiesAndOutOfIllinois.Where(i => i.ID == record.CountyId).Select(c => c.CountyName).Single() : string.Empty;
		}
	}

	public class CrisisInterventionLineItem {
		public int? SvId { get; set; }
		public string StaffName { get; set; }
		public int? ContactTypeId { get; set; }
		public DateTime? ContactDate { get; set; }
		public int NumberOfContacts { get; set; }
		public string Town { get; set; }
		public string Township { get; set; }
		public string ZipCode { get; set; }
		public int? CountyId { get; set; }
		public string CountyName { get; set; }
		public double TotalTime { get; set; }
	}
}