using System.Collections.Generic;
using System.Linq;
using Infonet.Core.Collections;
using Infonet.Core.IO;
using Infonet.Data.Looking;
using Infonet.Data.Models.Clients;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.ReportTables.Investigation.VictimSensitiveInterviews;

namespace Infonet.Reporting.StandardReports.Builders.Investigation {
	public class VictimSensitiveInterviewSubReportBuilder : SubReportCountBuilder<VictimSensitiveInterview, VictimSensitiveInterviewLineItem> {
		public VictimSensitiveInterviewSubReportBuilder(SubReportSelection subReportType) : base(subReportType) { }

		protected override string[] CsvHeaders {
			get { return new[] { "ID", "Client ID", "Case ID", "Client Status", "Client Type", "Location", "Record Type", "Courtesy Interview", "Observers" }; }
		}

		protected override void WriteCsvRecord(CsvWriter csv, VictimSensitiveInterviewLineItem record) {
			csv.WriteField(record.Id);
			//KMS DO include Center here if LocationID is used by application
			csv.WriteField(record.ClientCode);
			csv.WriteField(record.CaseID);
			csv.WriteField(record.ClientStatus);
			csv.WriteField(Lookups.ClientType[record.ClientTypeID]?.Description);
			csv.WriteField(Lookups.SiteLocation[record.SiteLocationID]?.Description);
			csv.WriteField(Lookups.RecordingType[record.RecordTypeID]?.Description);
			csv.WriteField(record.CourtesyInterview);
			csv.WriteField(string.Join("|", record.ObserversList.Select(o => Lookups.ObserverPosition[o.ObserverID]?.Description)));
		}

		protected override void CreateReportTables() {
			var vsiGroup = new ReportTableGroup<VictimSensitiveInterviewLineItem>("", 2);
			vsiGroup.Headers = GetNewAndOngoingHeaders();
			vsiGroup.HideTitle = true;
			vsiGroup.HideSubtotal = true;

			// Total Victims
			var totalVictimsTable = new VictimSensitiveInterviewTotalVictimsReportTable("Total Number of Victims", 1);
			totalVictimsTable.Headers = GetNewAndOngoingHeaders();
			totalVictimsTable.HideTitle = true;
			totalVictimsTable.HideSubtotal = true;
			totalVictimsTable.Rows.Add(new ReportRow { Title = "Total Number of Victims" });
			vsiGroup.ReportTables.Add(totalVictimsTable);

			// Total Victim Cases
			var totalCasesTable = new VictimSensitiveInterviewTotalCasesReportTable("Total Number of Victim Cases", 2);
			totalCasesTable.Headers = GetNewAndOngoingHeaders();
			totalCasesTable.HideTitle = true;
			totalCasesTable.HideSubtotal = true;
			totalCasesTable.Rows.Add(new ReportRow { Title = "Total Number of Victim Cases" });
			vsiGroup.ReportTables.Add(totalCasesTable);
			ReportTableList.Add(vsiGroup);

			// Location
			var locationTable = new VictimSensitiveInterviewLocationReportTable("Location", 3);
			locationTable.Headers = GetNewAndOngoingHeaders();
			foreach (var item in Lookups.SiteLocation)
				locationTable.Rows.Add(new ReportRow { Code = item.CodeId, Title = item.Description });
			ReportTableList.Add(locationTable);

			// Record Type
			var recordTypeTable = new VictimSensitiveInterviewRecordTypeReportTable("How VSI Recorded", 4);
			recordTypeTable.Headers = GetNewAndOngoingHeaders();
			foreach (var item in Lookups.RecordingType)
				recordTypeTable.Rows.Add(new ReportRow { Code = item.CodeId, Title = item.Description, Order = item.Entries.ToDictionary()[ReportContainer.Provider].DisplayOrder });
			ReportTableList.Add(recordTypeTable);

			// Courtesy Interview
			var courtesyTable = new VictimSensitiveInterviewCourtesyInterviewReportTable("Courtesy Interview", 5);
			courtesyTable.Headers = GetNewAndOngoingHeaders();
			courtesyTable.HideSubtotal = true;
			courtesyTable.Rows.Add(new ReportRow { Title = "Yes" });
			ReportTableList.Add(courtesyTable);

			// Observers
			var observerTable = new VictimSensitiveInterviewObserversReportTable("Observers", 6);
			observerTable.Headers = GetNewAndOngoingHeaders();
			foreach (var item in Lookups.ObserverPosition)
				observerTable.Rows.Add(new ReportRow { Code = item.CodeId, Title = item.Description, Order = item.Entries.ToDictionary()[ReportContainer.Provider].DisplayOrder });
			ReportTableList.Add(observerTable);
		}

		protected override IEnumerable<VictimSensitiveInterviewLineItem> PerformSelect(IQueryable<VictimSensitiveInterview> query) {
			return query.Select(q => new VictimSensitiveInterviewLineItem {
				Id = q.VSI_ID,
				ClientID = q.ClientId,
				ClientCode = q.ClientCase.Client.ClientCode,
				CaseID = q.CaseId,
				ClientTypeID = q.ClientCase.Client.ClientTypeId,
				ClientStatus = q.ClientCase.Client.ClientCases.GroupBy(cc => cc.ClientId).Select(cc => cc.Min(c => c.FirstContactDate)).FirstOrDefault().Value >= ReportContainer.StartDate && q.ClientCase.Client.ClientCases.GroupBy(cc => cc.ClientId).Select(cc => cc.Min(c => c.FirstContactDate)).FirstOrDefault().Value <= ReportContainer.EndDate ? ReportTableHeaderEnum.New : ReportTableHeaderEnum.Ongoing,
				SiteLocationID = q.SiteLocationId,
				RecordTypeID = q.RecordTypeID,
				CourtesyInterview = q.CourtesyInterview,
				ObserversList = q.VSIObservers.Where(o => o.ObserverID.HasValue).ToList()
			});
		}

		protected override List<ReportTableHeader> GetNewAndOngoingHeaders() {
			var newOngoingHeaders = new List<ReportTableHeader>();
			var subheaders = GetClientTypeSubHeaders(ReportContainer.Provider);
			newOngoingHeaders.Add(new ReportTableHeader("New", ReportTableHeaderEnum.New, subheaders));
			newOngoingHeaders.Add(new ReportTableHeader("Ongoing", ReportTableHeaderEnum.Ongoing, subheaders));
			newOngoingHeaders.Add(new ReportTableHeader("Total", ReportTableHeaderEnum.Total, subheaders));
			return newOngoingHeaders;
		}

		protected override List<ReportTableSubHeader> GetClientTypeSubHeaders(Provider provider) {
			return new List<ReportTableSubHeader> {
				new ReportTableSubHeader { Title = ReportTableSubHeaderEnum.ChildVictim.GetDisplayName(), Code = ReportTableSubHeaderEnum.ChildVictim },
				new ReportTableSubHeader { Title = ReportTableSubHeaderEnum.ChildNonVictim.GetDisplayName(), Code = ReportTableSubHeaderEnum.ChildNonVictim },
				new ReportTableSubHeader { Title = "Total", Code = ReportTableSubHeaderEnum.Total }
			};
		}
	}

	public class VictimSensitiveInterviewLineItem {
		public int? Id { get; set; }
		public int? ClientID { get; set; }
		public string ClientCode { get; set; }
		public int? CaseID { get; set; }
		public int? ClientTypeID { get; set; }
		public ReportTableHeaderEnum ClientStatus { get; set; }
		public int? SiteLocationID { get; set; }
		public int? RecordTypeID { get; set; }
		public bool? CourtesyInterview { get; set; }
		public IEnumerable<VSIObserver> ObserversList { get; set; }
	}
}