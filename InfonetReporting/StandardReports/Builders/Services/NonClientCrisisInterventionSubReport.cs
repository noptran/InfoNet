using System;
using System.Collections.Generic;
using System.Linq;
using Infonet.Core;
using Infonet.Core.IO;
using Infonet.Data.Looking;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.Filters;
using Infonet.Reporting.StandardReports.ReportTables.Services.NonClientCrisisIntervention;
using LinqKit;

namespace Infonet.Reporting.StandardReports.Builders.Services {
	public class NonClientCrisisInterventionSubReport : SubReportCountBuilder<PhoneHotline, CrisisInterventionLineItem> {
		private HashSet<int?> _fundingSourceIds = null;

		public NonClientCrisisInterventionSubReport(SubReportSelection subReportSelectionType) : base(subReportSelectionType) { }

		public bool ShowDemographics { get; set; }

		protected override void PrePerformSelect(ReportContainer container) {
			_fundingSourceIds = ReportQuery.Filters.OfType<HotlineFundingSourceFilter>().SingleOrDefault()?.FundingSourceIds.NotNull(ids => new HashSet<int?>(ids));
		}

		protected override IEnumerable<CrisisInterventionLineItem> PerformSelect(IQueryable<PhoneHotline> query) {
			query = query.Where(h => 8 == h.CallTypeID || 9 == h.CallTypeID); //other types exist but shouldn't
			return query.Select(q => new CrisisInterventionLineItem {
				Id = q.PH_ID,
				Center = q.Center.CenterName,
				CallTypeId = q.CallTypeID,
				CallDate = q.Date,
				NumberOfContacts = q.NumberOfContacts,
				TotalTime = q.TotalTime,
				GenderId = q.SexID,
				RaceId = q.RaceID,
				Age = q.Age,
				StaffAndFunding = StaffFunding.Hotline.Invoke(q)
			});
		}

		protected override string[] CsvHeaders {
			get { return new[] { "ID", "Center", "Type of Intervention", "Intervention Date", "Number of Contacts", "Gender Identity", "Race/Ethnicity", "Age" }; }
		}

		protected override void WriteCsvRecord(CsvWriter csv, CrisisInterventionLineItem record) {
			csv.WriteField(record.Id);
			csv.WriteField(record.Center);
			csv.WriteField(Lookups.HotlineCallType[record.CallTypeId]?.Description);
			csv.WriteField(record.CallDate, "M/d/yyyy");
			csv.WriteField(record.NumberOfContacts);
			csv.WriteField(Lookups.Sex[record.GenderId]?.Description);
			csv.WriteField(Lookups.Race[record.RaceId]?.Description);
			csv.WriteField(record.Age);
		}

		protected override void CreateReportTables() {
			var numberOfContacts = new CrisisInterventionReportTable("Non-Client Crisis Intervention", 1) {
				Headers = GetHeaders(),
				HideSubheaders = true,
				HideSubtotal = true
			};
			numberOfContacts.Rows.Add(new ReportRow { Title = "Number Of Contacts", Code = null });
			ReportTableList.Add(numberOfContacts);

			var totalHoursTable = new CrisisInterventionTotalHoursReportTable("Total Hours", 2) {
				Headers = GetHeaders(),
				HideSubheaders = true,
				HideSubtotal = true,
				FundingSourceIds = _fundingSourceIds
			};
			totalHoursTable.Rows.Add(new ReportRow { Title = "Total Hours", Code = null });
			ReportTableList.Add(totalHoursTable);

			if (ShowDemographics) {
				var genderTable = new CrisisInterventionGenderReportTable("Gender Identity", 3) {
					Headers = GetHeaders(),
					HideSubheaders = true
				};
				foreach (var item in Lookups.Sex[ReportContainer.Provider])
					genderTable.Rows.Add(GetReportRowFromLookup(item));
				genderTable.Rows.Add(new ReportRow { Title = "Unassigned", Code = null });
				ReportTableList.Add(genderTable);

				var raceTable = new CrisisInterventionRaceReportTable("Race", 4) {
					Headers = GetHeaders(),
					HideSubheaders = true
				};
				foreach (var item in Lookups.Race[ReportContainer.Provider])
					raceTable.Rows.Add(GetReportRowFromLookup(item));
				raceTable.Rows.Add(new ReportRow { Title = "Unassigned", Code = null });
				ReportTableList.Add(raceTable);

				var ageTable = new CrisisInterventionAgeReportTable("Age", 5) {
					Headers = GetHeaders(),
					HideSubheaders = true
				};
				ageTable.Rows.Add(new ReportRow { Title = "Unknown", Code = (int)AgeRangeEnum.Unknown });
				ageTable.Rows.Add(new ReportRow { Title = "0-7", Code = (int)AgeRangeEnum.ZeroToSeven });
				ageTable.Rows.Add(new ReportRow { Title = "8-9", Code = (int)AgeRangeEnum.EightToNine });
				ageTable.Rows.Add(new ReportRow { Title = "10-11", Code = (int)AgeRangeEnum.TenToEleven });
				ageTable.Rows.Add(new ReportRow { Title = "12-13", Code = (int)AgeRangeEnum.TwelveToThirteen });
				ageTable.Rows.Add(new ReportRow { Title = "14-15", Code = (int)AgeRangeEnum.FourteenToFifteen });
				ageTable.Rows.Add(new ReportRow { Title = "16-17", Code = (int)AgeRangeEnum.SixteenToSeventeen });
				ageTable.Rows.Add(new ReportRow { Title = "18-19", Code = (int)AgeRangeEnum.EighteenToNineteen });
				ageTable.Rows.Add(new ReportRow { Title = "20-29", Code = (int)AgeRangeEnum.Twenties });
				ageTable.Rows.Add(new ReportRow { Title = "30-39", Code = (int)AgeRangeEnum.Thirties });
				ageTable.Rows.Add(new ReportRow { Title = "40-49", Code = (int)AgeRangeEnum.Fourties });
				ageTable.Rows.Add(new ReportRow { Title = "50-59", Code = (int)AgeRangeEnum.Fifties });
				ageTable.Rows.Add(new ReportRow { Title = "60-64", Code = (int)AgeRangeEnum.SixtyToSixtyFour });
				ageTable.Rows.Add(new ReportRow { Title = "65+", Code = (int)AgeRangeEnum.SixtyFiveAndUp });
				ageTable.Rows.Add(new ReportRow { Title = "Unassigned", Code = (int)AgeRangeEnum.Unassigned });
				ReportTableList.Add(ageTable);
			}
		}

		private List<ReportTableHeader> GetHeaders() {
			return new List<ReportTableHeader> {
				new ReportTableHeader { Code = ReportTableHeaderEnum.InPersonContacts, Title = "In Person Contacts", SubHeaders = new List<ReportTableSubHeader> { new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.Total, Title = string.Empty } } },
				new ReportTableHeader { Code = ReportTableHeaderEnum.CrisisInterventionPhoneContacts, Title = "Phone Contacts", SubHeaders = new List<ReportTableSubHeader> { new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.Total, Title = string.Empty } } },
				new ReportTableHeader { Code = ReportTableHeaderEnum.Total, Title = "Total Non-Client Contacts", SubHeaders = new List<ReportTableSubHeader> { new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.Total, Title = string.Empty } } }
			};
		}
	}

	public class CrisisInterventionLineItem {
		public int? Id { get; set; }
		public string Center { get; set; }
		public int? CallTypeId { get; set; }
		public DateTime? CallDate { get; set; }
		public int? NumberOfContacts { get; set; }
		public double? TotalTime { get; set; }
		public int? GenderId { get; set; }
		public int? RaceId { get; set; }
		public int? Age { get; set; }
		public IEnumerable<StaffFunding> StaffAndFunding { get; set; }
	}
}