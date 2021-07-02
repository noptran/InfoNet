using System.Collections.Generic;
using System.Linq;
using Infonet.Core.IO;
using Infonet.Data.Looking;
using Infonet.Data.Models.Clients;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports.ReportTables.ClientInformation.PresentingIssues;

namespace Infonet.Reporting.StandardReports.Builders.ClientInformation {

	public class ClientInformationPresentingIssuesSubReport : SubReportCountBuilder<ClientCase, ClientInformationPresentingIssuesLineItem> {
		public ClientInformationPresentingIssuesSubReport(SubReportSelection subReportSelectionType) : base(subReportSelectionType) { }

		protected override string[] CsvHeaders {
			get { return new[] { "ID", "Client ID", "Case ID", "Client Status", "Primary Presenting Issue", "Primary Presenting Issue Location" }; }
		}

		protected override void WriteCsvRecord(CsvWriter csv, ClientInformationPresentingIssuesLineItem record) {
			csv.WriteField(record.ClientID);
			csv.WriteField(record.ClientCode);
			csv.WriteField(record.CaseID);
			csv.WriteField(record.ClientStatus);
			csv.WriteField(Lookups.PrimaryPresentingIssue[record.PrimaryPresentingIssueID]?.Description);
			csv.WriteField(Lookups.PresentingIssueLocation[record.LocOfPrimOffenseID]?.Description);
		}

		protected override IEnumerable<ClientInformationPresentingIssuesLineItem> PerformSelect(IQueryable<ClientCase> query) {
            switch (ReportContainer.Provider) {
                case Provider.DV:
                    query = query.Where(m => m.Client.ClientTypeId == (int)ClientTypeEnum.DVAdult);
                    break;
                case Provider.CAC:
                    query = query.Where(m => m.Client.ClientTypeId == (int)ClientTypeEnum.CACChildVictim);
                    break;
                case Provider.SA:
                    query = query.Where(m => m.Client.ClientTypeId == (int)ClientTypeEnum.SAVictim);
                    break;
            }
            return query.Select(m => new ClientInformationPresentingIssuesLineItem {
				ClientID = m.ClientId,
				ClientCode = m.Client.ClientCode,
				CaseID = m.CaseId,
                ClientStatus = m.Client.ClientCases.GroupBy(cc => cc.ClientId).Select(cc => cc.Min(c => c.FirstContactDate)).FirstOrDefault().Value >= ReportContainer.StartDate && m.Client.ClientCases.GroupBy(cc => cc.ClientId).Select(cc => cc.Min(c => c.FirstContactDate)).FirstOrDefault().Value <= ReportContainer.EndDate ? ReportTableHeaderEnum.New : ReportTableHeaderEnum.Ongoing,
                ClientTypeID = m.Client.ClientTypeId,
                PrimaryPresentingIssueID = m.PresentingIssues.PrimaryPresentingIssueID,
				LocOfPrimOffenseID = m.PresentingIssues.LocOfPrimOffenseID
			});
		}

        protected override void CreateReportTables() {
            List<ReportTableHeader> newAndOnGoingHeaders = null;
            string preHeader = "";
            switch (ReportContainer.Provider) {
                case Provider.DV:
                    newAndOnGoingHeaders = GetNewAndOngoingHeaders(ReportTableSubHeaderEnum.Adult, false);
                    preHeader = "Primary Presenting Issue Information (Adult)";
                    break;
                case Provider.CAC:
                    newAndOnGoingHeaders = GetNewAndOngoingHeaders(ReportTableSubHeaderEnum.ChildVictim, false);
                    preHeader = "Primary Presenting Issue Information (Child Victim)";
                    break;
                case Provider.SA:
                    newAndOnGoingHeaders = GetNewAndOngoingHeaders(ReportTableSubHeaderEnum.Victim, false);
                    preHeader = "Primary Presenting Issue Information (Victim)";
                    break;
            }

            // Primary Presenting Issue
	        var primaryPresentingIssueGroup = new PresentingIssuesReportTable("Primary Presenting Issue", 1) {
		        PreHeader = preHeader,
		        Headers = newAndOnGoingHeaders,
		        HideSubheaders = true
	        };
	        foreach (var item in Lookups.PrimaryPresentingIssue[ReportContainer.Provider]) {
                primaryPresentingIssueGroup.Rows.Add(GetReportRowFromLookup(item));
            }
            primaryPresentingIssueGroup.Rows.Add(new ReportRow { Title = "Unassigned", Code = null });
            ReportTableList.Add(primaryPresentingIssueGroup);

            // Primary Presenting Issue Location
	        var presentingIssueLocationGroup = new PresentingIssueLocationReportTable("Primary Presenting Issue Location", 2) {
		        Headers = newAndOnGoingHeaders,
		        HideSubheaders = true
	        };
	        foreach (var item in Lookups.PresentingIssueLocation[ReportContainer.Provider]) {
                presentingIssueLocationGroup.Rows.Add(GetReportRowFromLookup(item));
            }
            presentingIssueLocationGroup.Rows.Add(new ReportRow { Title = "Unassigned", Code = null });
            ReportTableList.Add(presentingIssueLocationGroup);
        }
	}

	public class ClientInformationPresentingIssuesLineItem {
		public int? ClientID { get; set; }
		public string ClientCode { get; set; }
		public int? CaseID { get; set; }
		public int? PrimaryPresentingIssueID { get; set; }
		public int? LocOfPrimOffenseID { get; set; }
		public ReportTableHeaderEnum ClientStatus { get; set; }
        public int? ClientTypeID { get; set; }
    }
}