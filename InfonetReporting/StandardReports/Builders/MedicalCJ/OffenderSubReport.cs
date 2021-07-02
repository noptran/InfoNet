using System.Collections.Generic;
using System.Linq;
using Infonet.Core.IO;
using Infonet.Data.Looking;
using Infonet.Data.Models.Offenders;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.ReportTables.Medical.Offender;

namespace Infonet.Reporting.StandardReports.Builders.MedicalCJ {
	public class OffendersSubReport : SubReportCountBuilder<Offender, MedicalCJOffendersLineItem> {
		public OffendersSubReport(SubReportSelection subReportSelectionType) : base(subReportSelectionType) { }

		protected override IEnumerable<MedicalCJOffendersLineItem> PerformSelect(IQueryable<Offender> query) {
			if (ReportContainer.Provider == Provider.DV)
				query = query.Where(q => q.ClientCase.Client.ClientTypeId == 1);

			return query.Select(o => new MedicalCJOffendersLineItem {
				OffenderRecordID = o.OffenderId,
				OffenderID = o.OffenderListingId,
				OffenderCode = o.OffenderListing.OffenderCode,
				ClientID = o.ClientId,
				ClientCode = o.ClientCase.Client.ClientCode,
				CaseID = o.CaseId,
				ClientStatus = o.ClientCase.Client.ClientCases.GroupBy(cc => cc.ClientId).Select(cc => cc.Min(c => c.FirstContactDate)).FirstOrDefault().Value >= ReportContainer.StartDate && o.ClientCase.Client.ClientCases.GroupBy(cc => cc.ClientId).Select(cc => cc.Min(c => c.FirstContactDate)).FirstOrDefault().Value <= ReportContainer.EndDate ? ReportTableHeaderEnum.New : ReportTableHeaderEnum.Ongoing,
				RaceID = o.OffenderListing.RaceId,
				Age = o.Age,
				VisitationID = o.VisitationId,
				RelationshipToVictimID = o.RelationshipToClientId,
				GenderID = o.OffenderListing.SexId,
				RegisteredID = o.RegisteredId
			});
		}

		protected override string[] CsvHeaders {
			get { return new[] { "ID", "Offender ID", "Client ID", "Client Status", "Age", "Gender", "Race", "Relationship to Victim", "Visitation" }; }
		}

		protected override void WriteCsvRecord(CsvWriter csv, MedicalCJOffendersLineItem record) {
			csv.WriteField(record.OffenderRecordID);
			if (ReportContainer.Provider == Provider.CAC)
				csv.WriteField(record.OffenderCode);
			else
				csv.WriteField(record.OffenderRecordID);
			csv.WriteField(record.ClientCode);
			csv.WriteField(record.ClientStatus);
			csv.WriteField(record.Age);
			csv.WriteField(Lookups.GenderIdentity[record.GenderID]?.Description);
			csv.WriteField(Lookups.Race[record.RaceID]?.Description);
			csv.WriteField(Lookups.RelationshipToClient[record.RelationshipToVictimID]?.Description);
			csv.WriteField(Lookups.Visitation[record.VisitationID]?.Description);
		}

		protected override void CreateReportTables() {
			var newAndOnGoingTotal = GetNewAndOngoingHeaders(ReportTableSubHeaderEnum.Total, false);

			var totalVictimCasesGroup = new MedicalCJOffendersTotalVictimCasesReportTable("Total Victims", 1);
			totalVictimCasesGroup.HideTitle = true;
			totalVictimCasesGroup.HideSubtotal = true;
			totalVictimCasesGroup.HideSubheaders = true;
			totalVictimCasesGroup.Headers = newAndOnGoingTotal;
			totalVictimCasesGroup.Rows.Add(new ReportRow { Title = "Total Victims" });
			ReportTableList.Add(totalVictimCasesGroup);

			var totalOffendersGroup = new MedicalCJOffendersTotalOffendersReportTable("Total Offenders", 2);
			totalOffendersGroup.HideTitle = true;
			totalOffendersGroup.HideSubtotal = true;
			totalOffendersGroup.HideSubheaders = true;
			totalOffendersGroup.Headers = newAndOnGoingTotal;
			totalOffendersGroup.Rows.Add(new ReportRow { Title = "Total Offenders" });
			ReportTableList.Add(totalOffendersGroup);

			if (ReportContainer.Provider == Provider.CAC) {
				var totalOffenderCasesGroup = new MedicalCJOffendersTotalOffenderCasesReportTable("Total Offender Cases", 3);
				totalOffenderCasesGroup.HideTitle = true;
				totalOffenderCasesGroup.HideSubtotal = true;
				totalOffenderCasesGroup.HideSubheaders = true;
				totalOffenderCasesGroup.Headers = newAndOnGoingTotal;
				totalOffenderCasesGroup.Rows.Add(new ReportRow { Title = "Total Offender Cases" });
				ReportTableList.Add(totalOffenderCasesGroup);

				var totalOffendersVictimizedMultiple = new MedicalCJOffendersVictimizedMultipleReportTable("Number of Offenders (above) who victimized multiple clients", 4);
				totalOffendersVictimizedMultiple.HideTitle = true;
				totalOffendersVictimizedMultiple.HideSubtotal = true;
				totalOffendersVictimizedMultiple.HideSubheaders = true;
				totalOffendersVictimizedMultiple.Headers = newAndOnGoingTotal;
				totalOffendersVictimizedMultiple.Rows.Add(new ReportRow { Title = "Number of Offenders (above) who victimized multiple clients" });
				ReportTableList.Add(totalOffendersVictimizedMultiple);
			}

			var genderReportTable = new MedicalCJOffendersGenderReportTable("Gender", 4);
			genderReportTable.HideSubheaders = true;
			genderReportTable.Headers = newAndOnGoingTotal;
			foreach (var item in Lookups.GenderIdentity[ReportContainer.Provider])
				genderReportTable.Rows.Add(GetReportRowFromLookup(item));
			ReportTableList.Add(genderReportTable);

			var raceReportTable = new MedicalCJOffendersRaceReportTable("Race", 5);
			raceReportTable.HideSubheaders = true;
			raceReportTable.Headers = newAndOnGoingTotal;
			foreach (var item in Lookups.Race[ReportContainer.Provider])
				raceReportTable.Rows.Add(GetReportRowFromLookup(item));
			raceReportTable.Rows.Add(new ReportRow { Title = "Unassigned", Code = null });
			ReportTableList.Add(raceReportTable);

			var ageReportTable = new MedicalCJOffendersAgeReportTable("Age", 6);
			ageReportTable.HideSubheaders = true;
			ageReportTable.Headers = newAndOnGoingTotal;
			ageReportTable.Rows.Add(new ReportRow { Title = "Unassigned", Code = (int)MedicalCJOffendersAgeRangeEnum.Unassigned });
			ageReportTable.Rows.Add(new ReportRow { Title = "Unknown", Code = (int)MedicalCJOffendersAgeRangeEnum.Unknown });
			ageReportTable.Rows.Add(new ReportRow { Title = "0-15", Code = (int)MedicalCJOffendersAgeRangeEnum.ZeroToFifteen });
			ageReportTable.Rows.Add(new ReportRow { Title = "16-17", Code = (int)MedicalCJOffendersAgeRangeEnum.SixteenToSeventeen });
			ageReportTable.Rows.Add(new ReportRow { Title = "18-19", Code = (int)MedicalCJOffendersAgeRangeEnum.EighteenToNineteen });
			ageReportTable.Rows.Add(new ReportRow { Title = "20-29", Code = (int)MedicalCJOffendersAgeRangeEnum.Twenties });
			ageReportTable.Rows.Add(new ReportRow { Title = "30-39", Code = (int)MedicalCJOffendersAgeRangeEnum.Thirties });
			ageReportTable.Rows.Add(new ReportRow { Title = "40-49", Code = (int)MedicalCJOffendersAgeRangeEnum.Fourties });
			ageReportTable.Rows.Add(new ReportRow { Title = "50-59", Code = (int)MedicalCJOffendersAgeRangeEnum.Fifties });
			ageReportTable.Rows.Add(new ReportRow { Title = "60-64", Code = (int)MedicalCJOffendersAgeRangeEnum.SixtyToSixtyFour });
			ageReportTable.Rows.Add(new ReportRow { Title = "65+", Code = (int)MedicalCJOffendersAgeRangeEnum.SixtyFiveAndUp });
			ReportTableList.Add(ageReportTable);

			// RP DO in these kind of tables that the rows are added dynamically if there is none, how should it be presented?

			var relationshipGroup = new MedicalCJOffendersRelationshipReportTable("Relationship of Offender to Victim", 7);
			relationshipGroup.HideSubheaders = true;
			relationshipGroup.Headers = newAndOnGoingTotal;
			ReportTableList.Add(relationshipGroup);

			// DV ONLY
			if (ReportContainer.Provider == Provider.DV) {
				var visitationGroup = new MedicalCJOffendersVisitationReportTable("Visitation", 8);
				visitationGroup.HideSubheaders = true;
				visitationGroup.Headers = newAndOnGoingTotal;
				foreach (var item in Lookups.Visitation[ReportContainer.Provider])
					visitationGroup.Rows.Add(GetReportRowFromLookup(item));
				visitationGroup.Rows.Add(new ReportRow { Title = "Unassigned", Code = null });
				ReportTableList.Add(visitationGroup);
			}

			// CAC ONLY
			if (ReportContainer.Provider == Provider.CAC) {
				var registeredGroup = new MedicalCJOffendersRegisteredReportTable("Registered Offender", 9);
				registeredGroup.HideSubheaders = true;
				registeredGroup.Headers = newAndOnGoingTotal;
				foreach (var item in Lookups.YesNo[ReportContainer.Provider])
					registeredGroup.Rows.Add(GetReportRowFromLookup(item));
				registeredGroup.Rows.Add(new ReportRow { Title = "Unassigned", Code = null });
				ReportTableList.Add(registeredGroup);
			}
		}
	}

	public class MedicalCJOffendersLineItem {
		public int? OffenderRecordID { get; set; }
		public int? OffenderID { get; set; }
		public string OffenderCode { get; set; }
		public int? ClientID { get; set; }
		public string ClientCode { get; set; }
		public int? CaseID { get; set; }
		public ReportTableHeaderEnum ClientStatus { get; set; }
		public int? RaceID { get; set; }
		public int? Age { get; set; }
		public int? VisitationID { get; set; }
		public int? RelationshipToVictimID { get; set; }
		public int? GenderID { get; set; }
		public int? RegisteredID { get; set; }
	}
}