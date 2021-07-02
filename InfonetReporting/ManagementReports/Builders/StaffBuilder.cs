using System;
using System.Collections.Generic;
using System.Linq;
using Infonet.Core.Collections;
using Infonet.Core.IO;
using Infonet.Data.Looking;
using Infonet.Data.Models.Clients;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.ManagementReports.ReportTables.StaffService;

namespace Infonet.Reporting.ManagementReports.Builders {
	public class StaffSubReport : SubReportCountBuilder<ClientCase, ManagementClientInformationDemographicsLineItem> {
		private readonly StaffReportAgeSetSelectionsEnum _ageSetSelection;

		public StaffSubReport(SubReportSelection subReportType, StaffReportAgeSetSelectionsEnum ageSetSelection) : base(subReportType) {
			IsInGroup = true;
			_ageSetSelection = ageSetSelection;
		}

		protected override IEnumerable<ManagementClientInformationDemographicsLineItem> PerformSelect(IQueryable<ClientCase> query) {
			return query.Select(q => new ManagementClientInformationDemographicsLineItem {
				CaseID = q.CaseId,
				ClientID = q.ClientId,
				ClientCode = q.Client.ClientCode,
				ClientStatus = q.Client.ClientCases.GroupBy(cc => cc.ClientId).Select(cc => cc.Min(c => c.FirstContactDate)).FirstOrDefault().Value >= ReportContainer.StartDate && q.Client.ClientCases.GroupBy(cc => cc.ClientId).Select(cc => cc.Min(c => c.FirstContactDate)).FirstOrDefault().Value <= ReportContainer.EndDate ? ReportTableHeaderEnum.New : ReportTableHeaderEnum.Ongoing,
				Gender = q.Client.GenderIdentityId == 1 ? ReportTableHeaderEnum.Male : q.Client.GenderIdentityId == 2 ? ReportTableHeaderEnum.Female : ReportTableHeaderEnum.Other,
				AgeAtFirstContact = q.Age,
				EthnicityID = q.Client.EthnicityId,
				RaceIDs = q.Client.ClientRaces.Select(r => r.RaceHudId),
				RaceId = q.Client.RaceId
			});
		}

		protected override string[] CsvHeaders {
			get { return new[] { "Client ID", "Case ID", "Client Status", "Age at First Contact", "Gender", "Ethnicity", "Race" }; }
		}

		protected override void WriteCsvRecord(CsvWriter csv, ManagementClientInformationDemographicsLineItem record) {
			csv.WriteField(record.ClientCode);
			csv.WriteField(record.CaseID);
			csv.WriteField(record.ClientStatus);
			csv.WriteField(record.AgeAtFirstContact);
			csv.WriteField(record.Gender);
			csv.WriteField(Lookups.Ethnicity[record.EthnicityID]?.Description);
			csv.WriteField(ReportContainer.Provider == Provider.CAC ? Lookups.Race[record.RaceId]?.Description : string.Join("|", record.RaceIDs.Select(r => Lookups.RaceHud[r].Description)));
		}

		protected override void CreateReportTables() {
			var newAndOngoingTotalOnly = GetNewAndOngoingHeadersWithTotalSubheader();
			var maleFemaleHeaderWithTotal = GetMaleFemaleHeaders();

			var clientCaseCountsWrapper = new ReportTableGroup<ManagementClientInformationDemographicsLineItem>("", 1) {
				Headers = newAndOngoingTotalOnly,
				HideTitle = true,
				HideSubtotal = true,
				HideSubheaders = true
			};

			var caseCountTable = new CaseCountReportTable("Client Cases", 1) {
				HideTitle = true,
				HideSubtotal = true,
				HideSubheaders = true,
				Headers = newAndOngoingTotalOnly
			};
			caseCountTable.Rows.Add(new ReportRow { Title = "Client Cases" });
			clientCaseCountsWrapper.ReportTables.Add(caseCountTable);

			var clientCountTable = new ClientCountReportTable("Total Clients", 2) {
				Headers = newAndOngoingTotalOnly,
				HideSubtotal = true,
				HideSubheaders = true
			};
			clientCountTable.Rows.Add(new ReportRow { Title = "Total Clients" });
			clientCaseCountsWrapper.ReportTables.Add(clientCountTable);

			ReportTableList.Add(clientCaseCountsWrapper);

			var ageTable = new AgeReportTable("Age At First Contact", 3) {
				Headers = maleFemaleHeaderWithTotal,
				HideSubheaders = true,
				PreHeader = "Basic Demographic Information (New Clients)"
			};
			// DT - We'll use the Code for each row as a "minimum Age" value for that row so we can define the ranges
			if (_ageSetSelection == StaffReportAgeSetSelectionsEnum.UnderOneOverSixtyFive) {
				ageTable.Rows.Add(new ReportRow { Title = "Unknown", Code = -1, Order = 14 });
				ageTable.Rows.Add(new ReportRow { Title = "<1", Code = 0, Order = 1 });
				ageTable.Rows.Add(new ReportRow { Title = "1-3", Code = 1, Order = 2 });
				ageTable.Rows.Add(new ReportRow { Title = "4-7", Code = 4, Order = 3 });
				ageTable.Rows.Add(new ReportRow { Title = "8-9", Code = 8, Order = 4 });
				ageTable.Rows.Add(new ReportRow { Title = "10-14", Code = 10, Order = 5 });
				ageTable.Rows.Add(new ReportRow { Title = "15-17", Code = 15, Order = 6 });
				ageTable.Rows.Add(new ReportRow { Title = "18-19", Code = 18, Order = 7 });
				ageTable.Rows.Add(new ReportRow { Title = "20-29", Code = 20, Order = 8 });
				ageTable.Rows.Add(new ReportRow { Title = "30-39", Code = 30, Order = 9 });
				ageTable.Rows.Add(new ReportRow { Title = "40-49", Code = 40, Order = 10 });
				ageTable.Rows.Add(new ReportRow { Title = "50-59", Code = 50, Order = 11 });
				ageTable.Rows.Add(new ReportRow { Title = "60-64", Code = 60, Order = 12 });
				ageTable.Rows.Add(new ReportRow { Title = "65+", Code = 65, Order = 13 });
			} else if (_ageSetSelection == StaffReportAgeSetSelectionsEnum.UnderTwelveOverSixtyFive) {
				ageTable.Rows.Add(new ReportRow { Title = "Unknown", Code = -1, Order = 7 });
				ageTable.Rows.Add(new ReportRow { Title = "<12", Code = 0, Order = 1 });
				ageTable.Rows.Add(new ReportRow { Title = "13-17", Code = 13, Order = 2 });
				ageTable.Rows.Add(new ReportRow { Title = "18-29", Code = 18, Order = 3 });
				ageTable.Rows.Add(new ReportRow { Title = "30-44", Code = 30, Order = 4 });
				ageTable.Rows.Add(new ReportRow { Title = "45-64", Code = 45, Order = 5 });
				ageTable.Rows.Add(new ReportRow { Title = "65+", Code = 65, Order = 6 });
			}

			ReportTableList.Add(ageTable);

			if (ReportContainer.Provider != Provider.CAC) {
				var ethnicityTable = new EthnicityReportTable("Ethnicity", 4);
				ethnicityTable.Headers = maleFemaleHeaderWithTotal;
				ethnicityTable.HideSubheaders = true;
				foreach (var item in Lookups.Ethnicity[ReportContainer.Provider])
					ethnicityTable.Rows.Add(GetReportRowFromLookup(item));
				ReportTableList.Add(ethnicityTable);
			}

			var raceTable = new RaceHudReportTable("Race", 5);
			raceTable.Headers = maleFemaleHeaderWithTotal;
			raceTable.HideSubheaders = true;
			if (ReportContainer.Provider == Provider.CAC) {
				foreach (var item in Lookups.Race[ReportContainer.Provider])
					raceTable.Rows.Add(GetReportRowFromLookup(item));
			} else {
				foreach (var item in Lookups.RaceHud[ReportContainer.Provider])
					raceTable.Rows.Add(GetReportRowFromLookup(item));
				foreach (RaceHudCompositeEnum item in Enum.GetValues(typeof(RaceHudCompositeEnum)))
					raceTable.Rows.Add(new ReportRow { Title = item.GetDisplayName(), Code = (int)item });
			}
			ReportTableList.Add(raceTable);
		}

		private List<ReportTableHeader> GetNewAndOngoingHeadersWithTotalSubheader() {
			return new List<ReportTableHeader> {
				new ReportTableHeader { Code = ReportTableHeaderEnum.New, Title = "New", SubHeaders = new List<ReportTableSubHeader> { new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.Total, Title = string.Empty } } },
				new ReportTableHeader { Code = ReportTableHeaderEnum.Ongoing, Title = "Ongoing", SubHeaders = new List<ReportTableSubHeader> { new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.Total, Title = string.Empty } } },
				new ReportTableHeader { Code = ReportTableHeaderEnum.Total, Title = "Total", SubHeaders = new List<ReportTableSubHeader> { new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.Total, Title = string.Empty } } }
			};
		}

		private List<ReportTableHeader> GetMaleFemaleHeaders() {
			return new List<ReportTableHeader> {
				new ReportTableHeader { Code = ReportTableHeaderEnum.Female, Title = "Female", SubHeaders = new List<ReportTableSubHeader> { new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.Total, Title = string.Empty } } },
				new ReportTableHeader { Code = ReportTableHeaderEnum.Male, Title = "Male", SubHeaders = new List<ReportTableSubHeader> { new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.Total, Title = string.Empty } } },
				new ReportTableHeader { Code = ReportTableHeaderEnum.Total, Title = "Total", SubHeaders = new List<ReportTableSubHeader> { new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.Total, Title = string.Empty } } }
			};
		}
	}

	public class ManagementClientInformationDemographicsLineItem {
		public int? ClientID { get; set; }
		public string ClientCode { get; set; }
		public int? CaseID { get; set; }
		public ReportTableHeaderEnum ClientStatus { get; set; }
		public ReportTableHeaderEnum Gender { get; set; }
		public int? AgeAtFirstContact { get; set; }
		public int? EthnicityID { get; set; }
		public IEnumerable<int> RaceIDs { get; set; }
		public int? RaceId { get; set; }
	}
}