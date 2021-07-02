using System;
using System.Collections.Generic;
using System.Linq;
using Infonet.Core.Collections;
using Infonet.Core.IO;
using Infonet.Data.Looking;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.Filters;
using Infonet.Reporting.StandardReports.ReportTables.Services.ServiceOutcome;
using LinqKit;

namespace Infonet.Reporting.StandardReports.Builders.Services {
	public class ServiceOutcomeSubReport : SubReportCountBuilder<ServiceOutcome, ServiceOutcomeLineItem> {
		public ServiceOutcomeSubReport(SubReportSelection subReportType) : base(subReportType) { }

		protected override string[] CsvHeaders {
			get { return new[] { "Service", "Outcome", "Yes Responses", "No Responses", "Eligible Clients" }; }
		}

		protected override void WriteCsvRecord(CsvWriter csv, ServiceOutcomeLineItem record) {
			csv.WriteField(Lookups.ServiceCategory[record.ServiceID]?.Description);
			csv.WriteField(Lookups.ServiceOutcome[record.OutcomeID]?.Description);
			csv.WriteField(record.ResponseYes);
			csv.WriteField(record.ResponseNo);
			switch ((ServiceOutcomeSurveyEnum)record.ServiceID) {
				case ServiceOutcomeSurveyEnum.Shelter:
					csv.WriteField(record.EligibleShelterClients);
					break;
				case ServiceOutcomeSurveyEnum.SupportGroups:
					csv.WriteField(record.EligibleSupportGroupClients);
					break;
				case ServiceOutcomeSurveyEnum.Counseling:
					csv.WriteField(record.EligibleCounselingClients);
					break;
				case ServiceOutcomeSurveyEnum.LegalAdvocacy:
					csv.WriteField(record.EligibleLegalAdvocacyClients);
					break;
				case ServiceOutcomeSurveyEnum.OtherSupportiveServices:
					csv.WriteField(record.EligibleOtherClients);
					break;
				case ServiceOutcomeSurveyEnum.ChildrensServices:
					csv.WriteField(record.EligibleChildServiceClients);
					break;
			}
		}

		protected override void CreateReportTables() {
			AddReportTable("Shelter", 1,()=>EligibleShelterClients, ServiceOutcomeSurveyEnum.Shelter);
			AddReportTable("Support Groups", 2, () => EligibleSupportGroupClients, ServiceOutcomeSurveyEnum.SupportGroups);
			AddReportTable("Counseling", 3, () => EligibleCounselingClients, ServiceOutcomeSurveyEnum.Counseling);
			AddReportTable("Legal Advocacy", 4, ()=>EligibleLegalAdvocacyClients, ServiceOutcomeSurveyEnum.LegalAdvocacy);
			AddReportTable("Other Supportive Services & Advocacy", 5, () => EligibleOtherClients, ServiceOutcomeSurveyEnum.OtherSupportiveServices);
			AddReportTable("Children's Services", 6, () => EligibleChildServiceClients, ServiceOutcomeSurveyEnum.ChildrensServices);
			AddReportTable("Total", 6, () => AllEligible, ServiceOutcomeSurveyEnum.All);
		}

		protected override IEnumerable<ServiceOutcomeLineItem> PerformSelect(IQueryable<ServiceOutcome> query) {
			var isServedDuring = new ServiceDetailDateFilter(ReportContainer.StartDate, ReportContainer.EndDate);
			var isServedBy = new ServiceDetailLocationFilter(ReportContainer.CenterIds);
			var isAdult = new ClientTypeFilter(new int?[] { 1 });
			var isChild = new ClientTypeFilter(new int?[] { 2 });
			var isAge6To18 = new ClientCaseAgeFilter(6, 18);
			var isSheltered = new ClientCaseShelterTypeFilter(new[] { ShelterServiceEnum.OffsiteShelter, ShelterServiceEnum.OnsiteShelter, ShelterServiceEnum.TransitionalHousing }, ReportContainer.StartDate, ReportContainer.EndDate);
			var isWalkin = new ClientCaseShelterTypeFilter(new[] { ShelterServiceEnum.Walkin }, ReportContainer.StartDate, ReportContainer.EndDate);

			var clientsQuery = ReportContainer.InfonetContext.T_Client.AsExpandable();

			var isShelteredAdult = new FilterContext().Apply(ReportContainer, isServedDuring, isServedBy, isAdult, isSheltered).Client.Build();
			int shelteredAdults = clientsQuery.Where(isShelteredAdult).Count();

			var isWalkinAdult = new FilterContext().Apply(ReportContainer, isServedDuring, isServedBy, isAdult, isWalkin).Client.Build();
			int walkinAdults = clientsQuery.Where(isWalkinAdult).Count();

			var isChildOfAge6To18 = new FilterContext().Apply(ReportContainer, isServedDuring, isServedBy, isChild, isAge6To18).Client.Build();
			int children = clientsQuery.Where(isChildOfAge6To18).Count();

            EligibleShelterClients = shelteredAdults;
            EligibleSupportGroupClients = walkinAdults;
            EligibleOtherClients = walkinAdults;
            EligibleCounselingClients = walkinAdults;
            EligibleLegalAdvocacyClients = walkinAdults;
            EligibleChildServiceClients = children;
            AllEligible = shelteredAdults + walkinAdults;

			return query.GroupBy(x => new { serviceId = x.ServiceID, outcomeId = x.OutcomeID })
				.Select(group => new ServiceOutcomeLineItem {
					ServiceID = group.Key.serviceId,
					OutcomeID = group.Key.outcomeId,
					ResponseYes = group.Sum(x => x.ResponseYes ?? 0),
					ResponseNo = group.Sum(x => x.ResponseNo ?? 0),

					EligibleShelterClients = shelteredAdults,
					EligibleSupportGroupClients = walkinAdults,
					EligibleOtherClients = walkinAdults,
					EligibleCounselingClients = walkinAdults,
					EligibleLegalAdvocacyClients = walkinAdults,
					EligibleChildServiceClients = children,
					AllEligible = shelteredAdults + walkinAdults,
					OutcomeTotalRecords = group.Count()
				});
		}

		private void AddReportTable(string title, int displayOrder, Func<int> eligibleClientSelector, ServiceOutcomeSurveyEnum surveyType) {
			var group = new SurveyReportTable(title, displayOrder, eligibleClientSelector);
			group.SurveyType = surveyType;
			group.HideSubheaders = true;
			group.HideSubtotal = true;
			group.Headers = GetHeaders();
			switch (surveyType) {
				case ServiceOutcomeSurveyEnum.All:
					foreach (ServiceOutcomeQuestionsEnum item in Enum.GetValues(typeof(ServiceOutcomeQuestionsEnum)))
						group.Rows.Add(new ReportRow { Code = item.ToInt32(), Title = item.GetDisplayName(), Order = item.GetOrder() });
					break;
				default:
					foreach (var item in GetRows(group.SurveyType))
						group.Rows.Add(new ReportRow { Code = item.CodeId, Title = item.Description });
					break;
			}
			ReportTableList.Add(group);
		}

		private List<ReportTableHeader> GetHeaders() {
			return new List<ReportTableHeader> {
				new ReportTableHeader { Code = ReportTableHeaderEnum.OutcomeTotalYes, Title = "Total Yes Responses", SubHeaders = new List<ReportTableSubHeader> { new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.Total, Title = string.Empty } } },
				new ReportTableHeader { Code = ReportTableHeaderEnum.OutcomeTotalNo, Title = "Total No Responses", SubHeaders = new List<ReportTableSubHeader> { new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.Total, Title = string.Empty } } },
				new ReportTableHeader { Code = ReportTableHeaderEnum.OutcomeTotalSurveys, Title = "Total Surveys", SubHeaders = new List<ReportTableSubHeader> { new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.Total, Title = string.Empty } } },
				new ReportTableHeader { Code = ReportTableHeaderEnum.OutcomeTotalRecords, Title = "Total Records Entered", SubHeaders = new List<ReportTableSubHeader> { new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.Total, Title = string.Empty } } },
				new ReportTableHeader { Code = ReportTableHeaderEnum.OutcomeEligibleClientsServed, Title = "Eligible Clients Served", SubHeaders = new List<ReportTableSubHeader> { new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.Total, Title = string.Empty } } }
			};
		}

		private IEnumerable<ReportLookup> GetRows(ServiceOutcomeSurveyEnum surveyType) {
			return ReportContainer.InfonetContext.Database.SqlQuery<ReportLookup>(@"
							SELECT so.CodeID, so.Description
							FROM [InfonetServer].[dbo].[TLU_Codes_ServiceOutcome] so
							JOIN TLU_Codes_ServiceCategoryXServiceOutcome x
							ON so.CodeID = x.OutcomeID
							JOIN TLU_Codes_ServiceCategory sc
							ON sc.CodeID = x.ServiceID
							JOIN LOOKUPLIST_ItemAssignment ia
							ON ia.CodeID = so.CodeID
							AND ia.TableID = 90
							AND ia.ProviderID = @p0
							WHERE sc.CodeID = @p1
							ORDER BY ia.DisplayOrder", (int)ReportContainer.Provider, surveyType);
		}

        public int EligibleShelterClients { get; set; }
        public int EligibleSupportGroupClients { get; set; }
        public int EligibleCounselingClients { get; set; }
        public int EligibleLegalAdvocacyClients { get; set; }
        public int EligibleOtherClients { get; set; }
        public int EligibleChildServiceClients { get; set; }
        public int AllEligible { get; set; }
        public int OutcomeTotalRecords { get; set; }
    }

	public class ServiceOutcomeLineItem {
		public int? ServiceID { get; set; }
		public int? OutcomeID { get; set; }
		public int ResponseYes { get; set; }
		public int ResponseNo { get; set; }
		public int EligibleShelterClients { get; set; }
		public int EligibleSupportGroupClients { get; set; }
		public int EligibleCounselingClients { get; set; }
		public int EligibleLegalAdvocacyClients { get; set; }
		public int EligibleOtherClients { get; set; }
		public int EligibleChildServiceClients { get; set; }
		public int AllEligible { get; set; }
		public int OutcomeTotalRecords { get; set; }
	}
}