using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infonet.Core.IO;
using Infonet.Data.Looking;
using Infonet.Data.Models.Clients;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;

namespace Infonet.Reporting.ManagementReports.Builders {
	public class CancellationNoshowInformationSubReport : SubReportDataBuilder<Cancellation, CancellationLineItem> {
		public CancellationNoshowInformationSubReport(SubReportSelection subReportSelectionType) : base(subReportSelectionType) {
			PreviousClientList = new List<int?>();
			PreviousStaffList = new List<int?>();
			TotalClientList = new List<int?>();
			TotalStaffList = new List<int?>();
			PreviousGroupValue = string.Empty;
			PreviousGroupCount = 1;
			FirstColumnString = string.Empty;
		}

		private string FirstColumnString { get; set; }
		private CancellationLineItem PreviousLineItem { get; set; }
		private int PreviousGroupCount { get; set; }
		private string PreviousGroupValue { get; set; }
		private List<int?> PreviousClientList { get; }
		private List<int?> PreviousStaffList { get; }
		private int PreviousServiceCount { get; set; }
		private int PreviousCancellationsCount { get; set; }
		private int PreviousNoShowCount { get; set; }
		private List<int?> TotalClientList { get; }
		private List<int?> TotalStaffList { get; }
		private int TotalServiceCount { get; set; }
		private int TotalCancellationsCount { get; set; }
		private int TotalNoShowCount { get; set; }

		protected override void BuildLegacyHtmlRow(CancellationLineItem record, StringBuilder sb, bool isFirst, bool isLast) {
			if (!isFirst && GroupingSelections.Any()) {
				if (IsGrouped(record)) {
					PreviousGroupCount++;
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

		private bool IsGrouped(CancellationLineItem record) {
			bool isGrouped = true;
			foreach (var columnSelection in ColumnSelections) {
				switch (columnSelection.ColumnSelection) {
					case ReportColumnSelectionsEnum.ClientCode:
						isGrouped = PreviousLineItem.ClientCode == record.ClientCode;
						break;
					case ReportColumnSelectionsEnum.CaseID:
						isGrouped = PreviousLineItem.CaseId == record.CaseId;
						break;
					case ReportColumnSelectionsEnum.ServiceName:
						isGrouped = PreviousLineItem.ServiceStr == record.ServiceStr;
						break;
					case ReportColumnSelectionsEnum.Staff:
						isGrouped = PreviousLineItem.StaffName == record.StaffName;
						break;
					case ReportColumnSelectionsEnum.Date:
						isGrouped = PreviousLineItem.CancellationDate == record.CancellationDate;
						break;
					case ReportColumnSelectionsEnum.Reason:
						isGrouped = PreviousLineItem.CancellationReasonId == record.CancellationReasonId;
						break;
				}
				if (!isGrouped)
					return false;
			}
			return true;
		}

		private string GetCurrentGroupValue(CancellationLineItem record) {
			string currentGroupValue = string.Empty;
			switch (GroupingSelections.OrderBy(g => g.Order).ThenBy(g => g.GroupingSelection).First().GroupingSelection) {
				case ReportOrderSelectionsEnum.Staff:
					currentGroupValue = !string.IsNullOrEmpty(record.StaffName) ? record.StaffName : string.Empty;
					break;
				case ReportOrderSelectionsEnum.Client:
					currentGroupValue = !string.IsNullOrEmpty(record.ClientCode) ? record.ClientCode : string.Empty;
					break;
				case ReportOrderSelectionsEnum.Service:
					currentGroupValue = !string.IsNullOrEmpty(record.ServiceStr) ? record.ServiceStr : string.Empty;
					break;
			}
			return currentGroupValue;
		}

		private void ResetPreviousGroupValues() {
			PreviousGroupValue = null;
			PreviousStaffList.Clear();
			PreviousClientList.Clear();
			PreviousServiceCount = 0;
			PreviousCancellationsCount = 0;
			PreviousNoShowCount = 0;
		}

		private void ApplyItemRow(CancellationLineItem record, StringBuilder sb) {
			string serviceOutputFormat = PreviousGroupCount > 1 ? "{0} ({1})" : "{0}";
			sb.Append("<tr>");
			if (record != null)
				foreach (var columnSelection in ColumnSelections)
					switch (columnSelection.ColumnSelection) {
						case ReportColumnSelectionsEnum.ClientCode:
							if (FirstColumn == ReportColumnSelectionsEnum.ClientCode) {
								if (!FirstColumnString.Equals(record.ClientCode)) {
									sb.Append("<th scope='row' style='font-weight:normal;'>" + record.ClientCode + "</th>");
									FirstColumnString = record.ClientCode ?? string.Empty;
								} else {
									sb.Append("<th scope='row' style='font-weight:normal;'><span class='sr-only'>" + record.ClientCode + "</span></th>");
								}
							} else {
								sb.Append("<td>" + record.ClientCode + "</td>");
							}
							break;
						case ReportColumnSelectionsEnum.CaseID:
							sb.Append("<td>" + record.CaseId + "</td>");
							break;
						case ReportColumnSelectionsEnum.ServiceName:
							if (FirstColumn == ReportColumnSelectionsEnum.ServiceName) {
								if (!FirstColumnString.Equals(record.ServiceStr)) {
									sb.Append("<th scope='row' style='font-weight:normal;'>" + string.Format(serviceOutputFormat, record.ServiceStr, PreviousGroupCount) + "</th>");
									FirstColumnString = record.ServiceStr;
								} else {
									sb.Append("<th scope='row' style='font-weight:normal;'><span class='sr-only'>" + string.Format(serviceOutputFormat, record.ServiceStr, PreviousGroupCount) + "</span></th>");
								}
							} else {
								sb.Append("<td>" + string.Format(serviceOutputFormat, record.ServiceStr, PreviousGroupCount) + "</td>");
							}
							break;
						case ReportColumnSelectionsEnum.Staff:
							if (FirstColumn == ReportColumnSelectionsEnum.Staff) {
								if (!FirstColumnString.Equals(record.StaffName)) {
									sb.Append("<th scope='row' style='font-weight:normal;' data-svid='" + record.SvId + "'>" + record.StaffName + "</th>");
									FirstColumnString = record.StaffName;
								} else {
									sb.Append("<th scope='row' style='font-weight:normal;' data-svid='" + record.SvId +"><span class='sr-only'>" + record.StaffName + "</span></td>");
								}
							} else {
								sb.Append("<td data-svid='" + record.SvId + "'>" + record.StaffName + "</td>");
							}
							break;
						case ReportColumnSelectionsEnum.Date:
							sb.Append("<td>" + (record.CancellationDate.HasValue ? record.CancellationDate.Value.ToShortDateString() : "") + "</td>");
							break;
						case ReportColumnSelectionsEnum.Reason:
							sb.Append("<td>" + Lookups.CancellationReason[record.CancellationReasonId].Description + "</td>");
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
							case ReportColumnSelectionsEnum.ClientCode:
								sb.Append("<td><b>" + PreviousClientList.Count + "</b></td>");
								break;
							case ReportColumnSelectionsEnum.ServiceName:
								sb.Append("<td><b>" + PreviousServiceCount + " Services</b></td>");
								break;
							case ReportColumnSelectionsEnum.Staff:
								sb.Append("<td><b>" + PreviousStaffList.Count + " Staff</b></td>");
								break;
							case ReportColumnSelectionsEnum.Reason:
								sb.Append("<td><b>C: " + PreviousCancellationsCount + " N: " + PreviousNoShowCount + " </b></td>");
								break;
							case ReportColumnSelectionsEnum.CaseID:
							case ReportColumnSelectionsEnum.Date:
								sb.Append("<td></td>");
								break;
						}
					else
						sb.Append("<th scope='row'>Subtotal</th>");
				sb.Append("</tr>");
			}
		}

		private void SetGroupTotals(CancellationLineItem record) {
			if (GroupingSelections.Any())
				switch (GroupingSelections.OrderBy(g => g.Order).ThenBy(g => g.GroupingSelection).First().GroupingSelection) {
					case ReportOrderSelectionsEnum.Staff:
						PreviousGroupValue = !string.IsNullOrEmpty(record.StaffName) ? record.StaffName : string.Empty;
						break;
					case ReportOrderSelectionsEnum.Client:
						PreviousGroupValue = !string.IsNullOrEmpty(record.ClientCode) ? record.ClientCode : string.Empty;
						break;
					case ReportOrderSelectionsEnum.Service:
						PreviousGroupValue = !string.IsNullOrEmpty(record.ServiceStr) ? record.ServiceStr : string.Empty;
						break;
				}
			if (!PreviousClientList.Contains(record.ClientId))
				PreviousClientList.Add(record.ClientId);
			if (!PreviousStaffList.Contains(record.SvId))
				PreviousStaffList.Add(record.SvId);

			PreviousServiceCount++;
			if (record.CancellationReasonId == 1)
				PreviousCancellationsCount += 1;
			else
				PreviousNoShowCount += 1;
		}

		private void SetTotals(CancellationLineItem record) {
			if (!TotalClientList.Contains(record.ClientId))
				TotalClientList.Add(record.ClientId);
			if (!TotalStaffList.Contains(record.SvId))
				TotalStaffList.Add(record.SvId);
			TotalServiceCount++;

			if (record.CancellationReasonId == 1)
				TotalCancellationsCount += 1;
			else
				TotalNoShowCount += 1;
		}

		protected override void BuildLegacyHtmlSummaryRow(StringBuilder sb) {
			ApplyItemRow(PreviousLineItem, sb);
			ApplySummaryRow(sb);
			sb.Append("<tr>");
			foreach (var columnSelection in ColumnSelections) {
				bool isFirst = columnSelection.ColumnSelection == FirstColumn;
				switch (columnSelection.ColumnSelection) {
					case ReportColumnSelectionsEnum.ClientCode:
                        if (isFirst)
                            sb.Append("<th scope='row'> Grand Total: " + TotalClientList.Count + " Clients</th>");
                        else
                            sb.Append("<td><b>" + TotalClientList.Count + " Clients</b></td>");
                        break;
					case ReportColumnSelectionsEnum.ServiceName:
                        if (isFirst)
                            sb.Append("<th scope='row'> Grand Total: " + TotalServiceCount + " Services</th>");
                        else
                            sb.Append("<td><b>" + TotalServiceCount + " Services</b></td>");
                        break;
					case ReportColumnSelectionsEnum.Staff:
                        if (isFirst)
                            sb.Append("<th scope='row'> Grand Total: " + TotalStaffList.Count + " Staff</th>");
                        else
                            sb.Append("<td><b>" + TotalStaffList.Count + " Staff</b></td>");
                        break;
					case ReportColumnSelectionsEnum.Reason:
						sb.Append("<td><b>C: " + TotalCancellationsCount + " N: " + TotalNoShowCount + " </b></td>");
						break;
					case ReportColumnSelectionsEnum.CaseID:
					case ReportColumnSelectionsEnum.Date:
						sb.Append("<td></td>");
						break;
				}
			}
			sb.Append("</tr>");
		}

		protected override string BuildTrueCSVLine(CancellationLineItem record) {
			bool applyComma = false;
			var sb = new StringBuilder();
			foreach (var columnSelection in ColumnSelections) {
				if (applyComma)
					sb.Append(",");
				switch (columnSelection.ColumnSelection) {
					case ReportColumnSelectionsEnum.ClientCode:
						sb.AppendQuotedCSVData(record.ClientCode);
						break;
					case ReportColumnSelectionsEnum.CaseID:
						sb.AppendQuotedCSVData(record.CaseId);
						break;
					case ReportColumnSelectionsEnum.ServiceName:
						sb.AppendQuotedCSVData(record.ServiceStr);
						break;
					case ReportColumnSelectionsEnum.Staff:
						sb.AppendQuotedCSVData(record.StaffName);
						break;
					case ReportColumnSelectionsEnum.Date:
						sb.AppendQuotedCSVData(record.CancellationDate);
						break;
					case ReportColumnSelectionsEnum.Reason:
						sb.AppendQuotedCSVData(Lookups.CancellationReason[record.CancellationReasonId].Description);
						break;
				}
				applyComma = true;
			}
			return sb.ToString();
		}

		protected override IEnumerable<CancellationLineItem> PerformSelect(IOrderedQueryable<Cancellation> query) {
			var ret = query.Select(q => new CancellationLineItem {
				ClientId = q.ClientID,
				CaseId = q.CaseID,
				ClientCode = q.ClientCase.Client.ClientCode,
				SvId = q.SVID,
				StaffName = q.StaffVolunteer.FirstName + " " + q.StaffVolunteer.LastName,
				ServiceStr = q.TLU_Codes_ProgramsAndServices.Description,
				CancellationReasonId = q.ReasonID,
				CancellationDate = q.Date
			});
			return ret;
		}
	}

	public class CancellationLineItem {
		public int? ClientId { get; set; }
		public int? CaseId { get; set; }
		public string ClientCode { get; set; }
		public int? SvId { get; set; }
		public string StaffName { get; set; }
		public string ServiceStr { get; set; }
		public DateTime? CancellationDate { get; set; }
		public int? CancellationReasonId { get; set; }
	}
}