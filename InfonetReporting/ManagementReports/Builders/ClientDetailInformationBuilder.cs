using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infonet.Core.IO;
using Infonet.Data.Looking;
using Infonet.Data.Models.Clients;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;

namespace Infonet.Reporting.ManagementReports.Builders {
	public class ClientDetailInformationSubReport : SubReportDataBuilder<ServiceDetailOfClient, ClientDetailInformationLineItem> {
		public ClientDetailInformationSubReport(SubReportSelection subReportSelectionType) : base(subReportSelectionType) {
			PreviousClientList = new HashSet<int?>();
			PreviousCaseList = new HashSet<string>();
			TotalClientList = new HashSet<int?>();
			TotalCaseList = new HashSet<string>();
			PreviousGroupValue = string.Empty;
 
            PreviousLocIdList = new HashSet<string>();
            ServiceStaffColumns = new List<ReportColumnSelectionsEnum> {
				ReportColumnSelectionsEnum.ServiceName,
				ReportColumnSelectionsEnum.Staff,
				ReportColumnSelectionsEnum.ServiceHours,
				ReportColumnSelectionsEnum.ServiceDate,
				ReportColumnSelectionsEnum.ShelterBeginDate,
				ReportColumnSelectionsEnum.ShelterEndDate
			};
			ClientColumns = new List<ReportColumnSelectionsEnum> {
				ReportColumnSelectionsEnum.ClientCode,
				ReportColumnSelectionsEnum.CaseID,
				ReportColumnSelectionsEnum.ClientType,
				ReportColumnSelectionsEnum.FirstContactDate,
				ReportColumnSelectionsEnum.Age,
				ReportColumnSelectionsEnum.Gender,
				ReportColumnSelectionsEnum.Ethnicity,
				ReportColumnSelectionsEnum.Race,
				ReportColumnSelectionsEnum.SexualOrientation
            };
            ClientLocationColumns = new List<ReportColumnSelectionsEnum> {
                ReportColumnSelectionsEnum.Town,
                ReportColumnSelectionsEnum.Township,
                ReportColumnSelectionsEnum.County,
                ReportColumnSelectionsEnum.ZipCode,
                ReportColumnSelectionsEnum.State
            };

        }

		private List<ReportColumnSelectionsEnum> ServiceStaffColumns { get; }
		private List<ReportColumnSelectionsEnum> ClientColumns { get; }
        private List<ReportColumnSelectionsEnum> ClientLocationColumns { get; }
		private bool HideClientValues { get; set; }    
        private bool LocationCoumnSelected { get; set; }
        private int ClientColumnSpan { get; set; }
		private string PreviousGroupValue { get; set; }
		private HashSet<int?> PreviousClientList { get; }
		private HashSet<string> PreviousCaseList { get; }
        private HashSet<string> PreviousLocIdList { get; }

		private double PreviousHoursCount { get; set; }
		private int PreviousShelterDaysCount { get; set; }
		private HashSet<int?> TotalClientList { get; }
		private HashSet<string> TotalCaseList { get; }
		private double TotalHoursCount { get; set; }
		private int TotalShelterDaysCount { get; set; }

		protected override void BuildLegacyHtmlRow(ClientDetailInformationLineItem record, StringBuilder sb, bool isFirst, bool isLast) {
			if (GroupingSelections.Any()) {
				string currentGroupValue = "";
				foreach (var selection in GroupingSelections)
					switch (selection.GroupingSelection) {
						case ReportOrderSelectionsEnum.Town:
							currentGroupValue = record.TwnTshipCounty?.CityOrTown ?? string.Empty;
							break;
						case ReportOrderSelectionsEnum.Township:
							currentGroupValue = record.TwnTshipCounty?.Township ?? string.Empty;
							break;
						case ReportOrderSelectionsEnum.County:
							currentGroupValue = record.CountyStr ?? string.Empty;
							break;
						case ReportOrderSelectionsEnum.State:
							currentGroupValue = record.StateStr ?? string.Empty;
							break;
						case ReportOrderSelectionsEnum.ZipCode:
							currentGroupValue = record.TwnTshipCounty?.Zipcode ?? string.Empty;
							break;
					}
				if (!isFirst && !PreviousGroupValue.Trim().Equals(currentGroupValue.Trim(), StringComparison.OrdinalIgnoreCase)) {
					ApplySummaryRow(sb);
					ResetPreviousGroupValues();
				}
			}

            if (HideClientValues || !HideClientValues && !PreviousCaseList.Contains(record.CaseIdentifier) && !LocationCoumnSelected || LocationCoumnSelected && !PreviousCaseList.Contains(record.LocIdentifier)) {
                sb.Append("<tr>");
				foreach (var columnSelection in ColumnSelections)
					switch (columnSelection.ColumnSelection) {
						case ReportColumnSelectionsEnum.ClientCode:
							if (!PreviousCaseList.Contains(record.CaseIdentifier) || !HideClientValues)
								sb.Append("<td>" + record.ClientCode + "</td>");
							else
								sb.Append("<td colspan='" + ClientColumnSpan + "'></td>");
							break;
						case ReportColumnSelectionsEnum.CaseID:
							if (!PreviousCaseList.Contains(record.CaseIdentifier) || !HideClientValues)
								sb.Append("<td>" + record.CaseId + "</td>");
							break;
						case ReportColumnSelectionsEnum.ClientType:
							if (!PreviousCaseList.Contains(record.CaseIdentifier) || !HideClientValues)
								sb.Append("<td>" + Lookups.ClientType[record.ClientTypeId]?.Description + "</td>");
							break;
						case ReportColumnSelectionsEnum.FirstContactDate:
							if (!PreviousCaseList.Contains(record.CaseIdentifier) || !HideClientValues)
								sb.Append("<td>" + record.FirstContactDate?.ToShortDateString() + "</td>");
							break;
						case ReportColumnSelectionsEnum.Age:
							if (!PreviousCaseList.Contains(record.CaseIdentifier) || !HideClientValues)
								sb.Append("<td>" + record.Age + "</td>");
							break;
						case ReportColumnSelectionsEnum.Gender:
							if (!PreviousCaseList.Contains(record.CaseIdentifier) || !HideClientValues)
								sb.Append("<td>" + Lookups.GenderIdentity[record.GenderId]?.Description + "</td>");
							break;
						case ReportColumnSelectionsEnum.Race:
							if (!PreviousCaseList.Contains(record.CaseIdentifier) || !HideClientValues)
								sb.Append("<td>" + string.Join(" | ", record.RaceHudStrs) + "</td>");
							break;
						case ReportColumnSelectionsEnum.SexualOrientation:
							if (!PreviousCaseList.Contains(record.CaseIdentifier) || !HideClientValues)
								sb.Append("<td>" + Lookups.SexualOrientation[record.SexualOrientationId]?.Description + "</td>");
							break;
						case ReportColumnSelectionsEnum.Ethnicity:
							if (!PreviousCaseList.Contains(record.CaseIdentifier) || !HideClientValues)
								sb.Append("<td>" + Lookups.Ethnicity[record.EthnicityId]?.Description + "</td>");
							break;
						case ReportColumnSelectionsEnum.Town:
							sb.Append("<td>" + record.TwnTshipCounty?.CityOrTown + "</td>");
							break;
						case ReportColumnSelectionsEnum.Township:
							sb.Append("<td>" + record.TwnTshipCounty?.Township + "</td>");
							break;
						case ReportColumnSelectionsEnum.County:
							sb.Append("<td>" + record.CountyStr + "</td>");
							break;
						case ReportColumnSelectionsEnum.ZipCode:
                            sb.Append("<td>" + record.TwnTshipCounty?.Zipcode + "</td>");
							break;
						case ReportColumnSelectionsEnum.State:
							sb.Append("<td>" + record.StateStr + "</td>");
							break;
						case ReportColumnSelectionsEnum.ServiceName:
							sb.Append("<td>" + Lookups.ProgramsAndServices[record.ServiceId]?.Description + "</td>");
							break;
						case ReportColumnSelectionsEnum.Staff:
							if (record.StaffNames.Any() && record.StaffNames.Count() > 1) {
								var names = new StringBuilder();
								string lastName = record.StaffNames.OrderBy(n => n).Last();
								foreach (string name in record.StaffNames.OrderBy(n => n))
									if (name.Equals(lastName))
										names.Append(name);
									else
										names.Append(name + ", ");
								sb.Append("<td>" + names + "</td>");
							} else {
								sb.Append("<td>" + record.StaffNames.First() + "</td>");
							}
							break;
						case ReportColumnSelectionsEnum.ServiceHours:
							sb.Append("<td>" + record.ServiceHours + "</td>");
							break;
						case ReportColumnSelectionsEnum.ServiceDate:
							sb.Append("<td>" + record.ServiceDate?.ToShortDateString() + "</td>");
							break;
						case ReportColumnSelectionsEnum.ShelterBeginDate:
							sb.Append("<td>" + record.ShelterBeginDate?.ToShortDateString() + "</td>");
							break;
						case ReportColumnSelectionsEnum.ShelterEndDate:
							sb.Append("<td>" + record.ShelterEndDate?.ToShortDateString() + "</td>");
							break;
					}
				sb.Append("</tr>");
			}

			SetGroupTotals(record);
			SetTotals(record);
			if (isLast && GroupingSelections.Any()) {
				ApplySummaryRow(sb);
				ResetPreviousGroupValues();
			}
		}

		private void ApplySummaryRow(StringBuilder sb) {
			sb.Append("<tr class='subtotal'>");
			foreach (var columnSelection in ColumnSelections)
				switch (columnSelection.ColumnSelection) {
					case ReportColumnSelectionsEnum.ClientCode:
						sb.Append("<td><b>" + PreviousClientList.Count + " Client(s)</b></td>");
						break;
					case ReportColumnSelectionsEnum.CaseID:
						sb.Append("<td><b>" + PreviousCaseList.Count + " Case(s)</b></td>");
						break;
					case ReportColumnSelectionsEnum.ServiceHours:
						sb.Append("<td><b>" + PreviousHoursCount + " Hour(s)</b></td>");
						break;
					case ReportColumnSelectionsEnum.ShelterBeginDate:
						if (ColumnSelections.Any(cs => cs.ColumnSelection == ReportColumnSelectionsEnum.ShelterEndDate))
							sb.Append("<td colspan='2'><b>" + PreviousShelterDaysCount + " Day(s) in Shelter</b></td>");
						break;
					case ReportColumnSelectionsEnum.ShelterEndDate:
						if (ColumnSelections.All(cs => cs.ColumnSelection != ReportColumnSelectionsEnum.ShelterBeginDate))
							sb.Append("<td></td>");
						break;
					case ReportColumnSelectionsEnum.ClientType:
					case ReportColumnSelectionsEnum.FirstContactDate:
					case ReportColumnSelectionsEnum.Age:
					case ReportColumnSelectionsEnum.Gender:
					case ReportColumnSelectionsEnum.Race:
					case ReportColumnSelectionsEnum.SexualOrientation:
					case ReportColumnSelectionsEnum.Ethnicity:
					case ReportColumnSelectionsEnum.Town:
					case ReportColumnSelectionsEnum.Township:
					case ReportColumnSelectionsEnum.County:
					case ReportColumnSelectionsEnum.ZipCode:
					case ReportColumnSelectionsEnum.State:
					case ReportColumnSelectionsEnum.ServiceName:
					case ReportColumnSelectionsEnum.Staff:
					case ReportColumnSelectionsEnum.ServiceDate:
						sb.Append("<td></td>");
						break;
				}
			sb.Append("</tr>");
		}

		private void ResetPreviousGroupValues() {
			PreviousGroupValue = null;
			PreviousCaseList.Clear();
			PreviousClientList.Clear();
			PreviousHoursCount = 0.0;
			PreviousShelterDaysCount = 0;
            PreviousCaseList.Clear();
        }

		private void SetGroupTotals(ClientDetailInformationLineItem record) {
			if (GroupingSelections.Any())
				foreach (var selection in GroupingSelections)
					switch (selection.GroupingSelection) {
						case ReportOrderSelectionsEnum.Town:
							PreviousGroupValue = record.TwnTshipCounty?.CityOrTown ?? string.Empty;
							break;
						case ReportOrderSelectionsEnum.Township:
							PreviousGroupValue = record.TwnTshipCounty?.Township ?? string.Empty;
							break;
						case ReportOrderSelectionsEnum.County:
							PreviousGroupValue = record.CountyStr ?? string.Empty;
							break;
						case ReportOrderSelectionsEnum.State:
							PreviousGroupValue = record.StateStr ?? string.Empty;
							break;
						case ReportOrderSelectionsEnum.ZipCode:
							PreviousGroupValue = record.TwnTshipCounty?.Zipcode ?? string.Empty;
							break;
					}
			PreviousCaseList.Add(record.CaseIdentifier);
			PreviousClientList.Add(record.ClientId);
			PreviousHoursCount += record.ServiceHours ?? 0.0;           
            PreviousCaseList.Add(record.LocIdentifier);
 
            if (record.ShelterBeginDate.HasValue) {
				var startDate = record.ShelterBeginDate.Value;
				var endDate = record.ShelterEndDate ?? ReportContainer.EndDate.Value;
				var timeSpan = endDate - startDate;
				PreviousShelterDaysCount += timeSpan.Days + 1;
			}
		}

		private void SetTotals(ClientDetailInformationLineItem record) {
			TotalCaseList.Add(record.CaseIdentifier);
			TotalClientList.Add(record.ClientId);
			TotalHoursCount += record.ServiceHours ?? 0.0;
			if (record.ShelterBeginDate.HasValue) {
				var startDate = record.ShelterBeginDate.Value;
				var endDate = record.ShelterEndDate ?? ReportContainer.EndDate.Value;
				var timeSpan = endDate - startDate;
				TotalShelterDaysCount += timeSpan.Days + 1;
			}
		}

		protected override void BuildLegacyHtmlSummaryRow(StringBuilder sb) {
			sb.Append("<tr>");
			foreach (var columnSelection in ColumnSelections)
				switch (columnSelection.ColumnSelection) {
					case ReportColumnSelectionsEnum.ClientCode:
						sb.Append("<td><b>Total Clients: " + TotalClientList.Count + "</b></td>");
						break;
					case ReportColumnSelectionsEnum.CaseID:
						sb.Append("<td><b>Total Cases: " + TotalCaseList.Count + "</b></td>");
						break;
					case ReportColumnSelectionsEnum.ServiceHours:
						sb.Append("<td><b>Total Hours: " + TotalHoursCount + "</b></td>");
						break;
					case ReportColumnSelectionsEnum.ShelterBeginDate:
						if (ColumnSelections.Any(cs => cs.ColumnSelection == ReportColumnSelectionsEnum.ShelterEndDate))
							sb.Append("<td colspan='2'><b>Total Days in Shelter: " + TotalShelterDaysCount + "</b></td>");
						break;
					case ReportColumnSelectionsEnum.ShelterEndDate:
						if (ColumnSelections.All(cs => cs.ColumnSelection != ReportColumnSelectionsEnum.ShelterBeginDate))
							sb.Append("<td></td>");
						break;
					case ReportColumnSelectionsEnum.ClientType:
					case ReportColumnSelectionsEnum.FirstContactDate:
					case ReportColumnSelectionsEnum.Age:
					case ReportColumnSelectionsEnum.Gender:
					case ReportColumnSelectionsEnum.Race:
					case ReportColumnSelectionsEnum.SexualOrientation:
					case ReportColumnSelectionsEnum.Ethnicity:
					case ReportColumnSelectionsEnum.Town:
					case ReportColumnSelectionsEnum.Township:
					case ReportColumnSelectionsEnum.County:
					case ReportColumnSelectionsEnum.ZipCode:
					case ReportColumnSelectionsEnum.State:
					case ReportColumnSelectionsEnum.ServiceName:
					case ReportColumnSelectionsEnum.Staff:
					case ReportColumnSelectionsEnum.ServiceDate:
						sb.Append("<td></td>");
						break;
				}
			sb.Append("</tr>");
		}

		protected override string BuildTrueCSVLine(ClientDetailInformationLineItem record) {
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
					case ReportColumnSelectionsEnum.ClientType:
						sb.AppendQuotedCSVData(Lookups.ClientType[record.ClientTypeId]?.Description);
						break;
					case ReportColumnSelectionsEnum.FirstContactDate:
						sb.AppendQuotedCSVData(record.FirstContactDate?.ToShortDateString() ?? string.Empty);
						break;
					case ReportColumnSelectionsEnum.Age:
						sb.AppendQuotedCSVData(record.Age);
						break;
					case ReportColumnSelectionsEnum.Gender:
						sb.AppendQuotedCSVData(Lookups.GenderIdentity[record.GenderId]?.Description);
						break;
					case ReportColumnSelectionsEnum.SexualOrientation:
						sb.AppendQuotedCSVData(Lookups.SexualOrientation[record.SexualOrientationId]?.Description);
						break;
					case ReportColumnSelectionsEnum.Race:
						sb.AppendQuotedCSVData(string.Join("|", record.RaceHudStrs));
						break;
					case ReportColumnSelectionsEnum.Ethnicity:
						if (!PreviousCaseList.Contains(record.CaseIdentifier) || !HideClientValues)
							sb.AppendQuotedCSVData(Lookups.Ethnicity[record.EthnicityId]?.Description);
						break;
					case ReportColumnSelectionsEnum.Town:
						sb.AppendQuotedCSVData(record.TwnTshipCounty?.CityOrTown);
						break;
					case ReportColumnSelectionsEnum.Township:
						sb.AppendQuotedCSVData(record.TwnTshipCounty?.Township);
						break;
					case ReportColumnSelectionsEnum.County:
						sb.AppendQuotedCSVData(record.CountyStr);
						break;
					case ReportColumnSelectionsEnum.ZipCode:
						sb.AppendQuotedCSVData(record.TwnTshipCounty?.Zipcode);
						break;
					case ReportColumnSelectionsEnum.State:
						sb.AppendQuotedCSVData(record.StateStr);
						break;
					case ReportColumnSelectionsEnum.ServiceName:
						sb.AppendQuotedCSVData(Lookups.ProgramsAndServices[record.ServiceId]?.Description);
						break;
					case ReportColumnSelectionsEnum.Staff:
						if (record.StaffNames.Any() && record.StaffNames.Count() > 1) {
							var names = new StringBuilder();
							string lastName = record.StaffNames.OrderBy(n => n).Last();
							foreach (string name in record.StaffNames.OrderBy(n => n))
								if (name.Equals(lastName))
									names.Append(name);
								else
									names.Append(name + ", ");
							sb.AppendQuotedCSVData(names);
						} else {
							sb.AppendQuotedCSVData(record.StaffNames.First());
						}
						break;
					case ReportColumnSelectionsEnum.ServiceHours:
						sb.AppendQuotedCSVData(record.ServiceHours);
						break;
					case ReportColumnSelectionsEnum.ServiceDate:
						sb.AppendQuotedCSVData(record.ServiceDate?.ToShortDateString());
						break;
					case ReportColumnSelectionsEnum.ShelterBeginDate:
						sb.AppendQuotedCSVData(record.ShelterBeginDate?.ToShortDateString());
						break;
					case ReportColumnSelectionsEnum.ShelterEndDate:
						sb.AppendQuotedCSVData(record.ShelterEndDate?.ToShortDateString());
						break;
				}
				applyComma = true;
			}
			return sb.ToString();
		}

		protected override IEnumerable<ClientDetailInformationLineItem> PerformSelect(IOrderedQueryable<ServiceDetailOfClient> query) {
			var currentColumnSelections = ColumnSelections.Select(cs => cs.ColumnSelection).ToArray();
			HideClientValues = currentColumnSelections.Intersect(ServiceStaffColumns).Any();
            LocationCoumnSelected = currentColumnSelections.Intersect(ClientLocationColumns).Any();
			ClientColumnSpan = currentColumnSelections.Intersect(ClientColumns).Count();

			return query.Select(q => new ClientDetailInformationLineItem {
				ClientId = q.ClientID,
				CaseId = q.CaseID,
				ClientCode = q.ClientCase.Client.ClientCode,
				ClientTypeId = q.ClientCase.Client.ClientTypeId,
				FirstContactDate = q.ClientCase.FirstContactDate,
				Age = q.ClientCase.Age,
				GenderId = q.ClientCase.Client.GenderIdentityId,
				SexualOrientationId = q.ClientCase.SexualOrientationId,
				EthnicityId = q.ClientCase.Client.EthnicityId,
				RaceId = q.ClientCase.Client.RaceId,
				RaceHudIDs = q.ClientCase.Client.ClientRaces.Select(r => (int?)r.RaceHudId),
				TwnTshipCounty = q.TwnTshipCounty,
				ServiceId = q.ServiceID,
				ServiceDate = q.ServiceDate,
				ServiceHours = q.ReceivedHours,
				ShelterBeginDate = q.ShelterBegDate,
				ShelterEndDate = q.ShelterEndDate,
				StaffNames = q.Tl_ProgramDetail.ProgramDetailStaff.Select(pds => pds.StaffVolunteer.FirstName + " " + pds.StaffVolunteer.LastName).DefaultIfEmpty(q.StaffVolunteer.FirstName + " " + q.StaffVolunteer.LastName)
			});
		}

		protected override void PrepareRecord(ClientDetailInformationLineItem record) {
			record.CaseIdentifier = $"{record.ClientId}:{record.CaseId}";
			record.RaceHudStrs = ReportContainer.Provider == Provider.CAC ? new[] { Lookups.Race[record.RaceId]?.Description } : record.RaceHudIDs.Where(r => r.HasValue).Select(r => Lookups.RaceHud[r].Description);
			if (record.TwnTshipCounty != null) {
				record.CountyStr = ReportContainer.UspsContext.Counties.Where(c => c.ID == record.TwnTshipCounty.CountyID).Select(c => c.CountyName).SingleOrDefault();
				record.StateStr = ReportContainer.UspsContext.States.Where(s => s.ID == record.TwnTshipCounty.StateID).Select(s => s.StateAbbreviation).SingleOrDefault();
			}
            record.LocIdentifier = $"{record.ClientId}:{record.TwnTshipCounty?.LocID}";
 
        }
	}

	public class ClientDetailInformationLineItem {
		public string ClientCode { get; set; }
		public int? ClientId { get; set; }
		public string CaseIdentifier { get; set; }
		public int? CaseId { get; set; }
		public int? ClientTypeId { get; set; }
		public DateTime? FirstContactDate { get; set; }
		public int? Age { get; set; }
		public int? GenderId { get; set; }
		public int? SexualOrientationId { get; set; }
		public int? EthnicityId { get; set; }
		public IEnumerable<int?> RaceHudIDs { get; set; }
		public IEnumerable<string> RaceHudStrs { get; set; }
		public TwnTshipCounty TwnTshipCounty { get; set; }
		public string CountyStr { get; set; }
		public string StateStr { get; set; }
		public int? ServiceId { get; set; }
		public IEnumerable<string> StaffNames { get; set; }
		public DateTime? ServiceDate { get; set; }
		public double? ServiceHours { get; set; }
		public DateTime? ShelterBeginDate { get; set; }
		public DateTime? ShelterEndDate { get; set; }
        public string LocIdentifier { get; set; }

        // CAC Only
        public int? RaceId { get; set; }
	}
}