using System;
using System.Collections.Generic;
using System.Linq;
using Infonet.Core;
using Infonet.Core.Collections;
using Infonet.Core.IO;
using Infonet.Data.Looking;
using Infonet.Data.Models.Services;
using Infonet.Reporting.Core;
using Infonet.Reporting.Core.Predicates;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.Filters;
using Infonet.Reporting.StandardReports.ReportTables.Services.CommunityGroup;
using LinqKit;

namespace Infonet.Reporting.StandardReports.Builders.Services {
	public class CommunityGroupSubReport : SubReportCountBuilder<ProgramDetail, CommunityGroupSubReportLineItem> {
		private HashSet<int?> _fundingSourceIds = null;

		public CommunityGroupSubReport(SubReportSelection subReportSelectionType) : base(subReportSelectionType) {
			IsInGroup = true;
			IsEndOfGroup = false;
		}

		protected override void PrePerformSelect(ReportContainer container) {
			_fundingSourceIds = ReportQuery.Filters.OfType<ProgramDetailFundingSourceFilter>().SingleOrDefault()?.FundingSourceIds.NotNull(ids => new HashSet<int?>(ids));
		}

		protected override IEnumerable<CommunityGroupSubReportLineItem> PerformSelect(IQueryable<ProgramDetail> query) {
			var staffPredicate = PredicateBuilder.New<ProgramDetailStaff>(true);
			var svIds = ReportQuery.Filters.OfType<ProgramDetailStaffFilter>().SingleOrDefault()?.SvIds;
			if (svIds != null)
				staffPredicate.And(pds => svIds.Contains(pds.SVID));

			return query.Select(q => new CommunityGroupSubReportLineItem {
				IcsId = q.ICS_ID,
				Center = q.Center.CenterName,
				ServiceDate = q.PDate,
				ServiceId = q.ProgramID,
				IsGroupService = q.TLU_Codes_ProgramsAndServices.IsGroupService,
				IsCommInstService = q.TLU_Codes_ProgramsAndServices.IsCommInst,
				NumberOfPresentations = q.NumOfSession ?? 0,
				NumberOfParticipants = q.ParticipantsNum ?? 0,
				PresentationHours = q.Hours ?? 0,
				Staff = q.ProgramDetailStaff.AsQueryable().Where(staffPredicate).Select(pds => new StaffLineItem {
					SvId = pds.SVID,
					ConductHours = pds.ConductHours ?? 0,
					PrepHours = pds.HoursPrep ?? 0,
					TravelHours = pds.HoursTravel ?? 0,
					Funding = StaffFunding.ProgramDetailStaff.Invoke(pds)
				})
			});
		}

		protected override string[] CsvHeaders {
			get { return new[] { "ID", "Center", "Date", "Service", "Is Group Service", "Is Comm/Inst Service", "Number of Presentations", "Number of Participants", "Number of Staff", "Presentation Hours", "Staff Conduct Hours", "Staff Travel Hours", "Staff Prep Hours" }; }
		}

		protected override void WriteCsvRecord(CsvWriter csv, CommunityGroupSubReportLineItem record) {
			csv.WriteField(record.IcsId);
			csv.WriteField(record.Center);
			csv.WriteField(record.ServiceDate, "M/d/yyyy");
			csv.WriteField(Lookups.ProgramsAndServices[record.ServiceId].Description);
			csv.WriteField(record.IsGroupService);
			csv.WriteField(record.IsCommInstService);
			csv.WriteField(record.NumberOfPresentations);
			csv.WriteField(record.NumberOfParticipants);
			csv.WriteField(record.Staff.Select(s => s.SvId).Distinct().Count());
			csv.WriteField(record.PresentationHours);
			csv.WriteField(_fundingSourceIds == null
				? record.Staff.Sum(s => s.ConductHours)
				: record.Staff.Sum(s => s.ConductHours * s.Funding.Where(f => _fundingSourceIds.Contains(f.FundingSourceId)).Sum(f => f.PercentFund / 100.0 ?? 0)));
			csv.WriteField(_fundingSourceIds == null
				? record.Staff.Sum(s => s.TravelHours)
				: record.Staff.Sum(s => s.TravelHours * s.Funding.Where(f => _fundingSourceIds.Contains(f.FundingSourceId)).Sum(f => f.PercentFund / 100.0 ?? 0)));
			csv.WriteField(_fundingSourceIds == null
				? record.Staff.Sum(s => s.PrepHours)
				: record.Staff.Sum(s => s.PrepHours * s.Funding.Where(f => _fundingSourceIds.Contains(f.FundingSourceId)).Sum(f => f.PercentFund / 100.0 ?? 0)));
		}

		protected override void CreateReportTables() {
			if (ReportContainer.Provider == Provider.DV)
				CreateDVTables();
			else if (ReportContainer.Provider == Provider.SA)
				CreateSATables();
			else if (ReportContainer.Provider == Provider.CAC)
				CreateCACTables();

			foreach (var each in ReportTableList.Cast<CommunityGroupServiceReportTable>())
				each.FundingSourceIds = _fundingSourceIds;
		}

		private void CreateDVTables() {
			var headers = GetHeaders();

			var allServices = new CommunityGroupServiceReportTable("", 0) {
				RowPredicate = (r, i) => true,
				HideTitle = true,
				Headers = GetHeaders(),
				HideSubheaders = true,
				HideSubtotal = true,
				UseNonDuplicatedSubtotal = true
			};
			allServices.Rows.Add(new ReportRow { Title = "All Services", Code = 1 });
			ReportTableList.Add(allServices);

			var groupServices = new CommunityGroupServiceReportTable("Group Services", 1) {
				Headers = headers,
				HideSubheaders = true,
				UseNonDuplicatedSubtotal = true
			};
			foreach (var item in Lookups.GroupServices[ReportContainer.Provider])
				groupServices.Rows.Add(new ReportRow { Code = item.CodeId, Title = item.Description });
			groupServices.Rows.Add(new ReportRow { Code = 103, Title = Lookups.CommunityServices[103].Description, Order = Lookups.GroupServices[ReportContainer.Provider].Count() + 1 });
			ReportTableList.Add(groupServices);

			var informationAndReferral = new CommunityGroupServiceReportTable("Information and Referral", 2) {
				Headers = headers,
				HideSubheaders = true,
				UseNonDuplicatedSubtotal = true
			};
			foreach (var item in Enums.GetValues<SACACInformationReferralServiceEnum>())
				informationAndReferral.Rows.Add(new ReportRow { Code = item.ToInt32(), Title = item.GetDisplayName() });
			ReportTableList.Add(informationAndReferral);

			var institutionalAdvocacy = new CommunityGroupServiceReportTable("Institutional Advocacy", 3) {
				Headers = headers,
				HideSubheaders = true,
				UseNonDuplicatedSubtotal = true
			};
			foreach (var item in Enums.GetValues<DVInformationReferralServiceEnum>())
				institutionalAdvocacy.Rows.Add(new ReportRow { Code = item.ToInt32(), Title = item.GetDisplayName() });
			ReportTableList.Add(institutionalAdvocacy);

			var professionalTraining = new CommunityGroupServiceReportTable("Professional Training", 4) {
				Headers = headers,
				HideSubheaders = true,
				UseNonDuplicatedSubtotal = true
			};
			foreach (var item in Enums.GetValues<DVProfessionalTrainingServiceEnum>())
				professionalTraining.Rows.Add(new ReportRow { Code = item.ToInt32(), Title = item.GetDisplayName() });
			ReportTableList.Add(professionalTraining);

			var publicEducation = new CommunityGroupServiceReportTable("Public Education", 5) {
				Headers = headers,
				HideSubheaders = true,
				UseNonDuplicatedSubtotal = true
			};
			foreach (var item in Enums.GetValues<DVPublicEducationServiceEnum>())
				publicEducation.Rows.Add(new ReportRow { Code = item.ToInt32(), Title = item.GetDisplayName() });
			ReportTableList.Add(publicEducation);

			var school = new CommunityGroupServiceReportTable("School", 6) {
				Headers = headers,
				HideSubheaders = true,
				UseNonDuplicatedSubtotal = true
			};
			foreach (var item in Enums.GetValues<DVSchoolServiceEnum>())
				school.Rows.Add(new ReportRow { Code = item.ToInt32(), Title = item.GetDisplayName(), Order = item.GetOrder() });
			ReportTableList.Add(school);

			var training = new CommunityGroupServiceReportTable("Training", 7) {
				Headers = headers,
				HideSubheaders = true,
				UseNonDuplicatedSubtotal = true
			};
			foreach (var item in Enums.GetValues<DVTrainingServiceEnum>())
				training.Rows.Add(new ReportRow { Code = item.ToInt32(), Title = item.GetDisplayName() });
			ReportTableList.Add(training);

			var otherActivities = new CommunityGroupServiceReportTable("Other Activities", 8) {
				Headers = headers,
				HideSubheaders = true,
				UseNonDuplicatedSubtotal = true
			};
			foreach (var item in Enums.GetValues<OtherActivitiesServiceEnum>())
				otherActivities.Rows.Add(new ReportRow { Code = item.ToInt32(), Title = item.GetDisplayName() });
			ReportTableList.Add(otherActivities);
		}

		private void CreateSATables() {
			var headers = GetHeaders();

			var allServices = new CommunityGroupServiceReportTable("", 1) {
				RowPredicate = (r, i) => true,
				HideTitle = true,
				Headers = GetHeaders(),
				HideSubheaders = true,
				HideSubtotal = true,
				UseNonDuplicatedSubtotal = true
			};
			allServices.Rows.Add(new ReportRow { Title = "All Services", Code = 1 });
			ReportTableList.Add(allServices);

			var groupServices = new CommunityGroupServiceReportTable("Group Services", 1) {
				Headers = headers,
				HideSubheaders = true,
				UseNonDuplicatedSubtotal = true
			};
			foreach (var item in Lookups.GroupServices[ReportContainer.Provider])
				groupServices.Rows.Add(new ReportRow { Code = item.CodeId, Title = item.Description });
			ReportTableList.Add(groupServices);

			var informationAndReferral = new CommunityGroupServiceReportTable("Information and Referral", 2) {
				Headers = headers,
				HideSubheaders = true,
				UseNonDuplicatedSubtotal = true
			};
			foreach (var item in Enums.GetValues<SACACInformationReferralServiceEnum>())
				informationAndReferral.Rows.Add(new ReportRow { Code = item.ToInt32(), Title = item.GetDisplayName() });
			ReportTableList.Add(informationAndReferral);

			var institutionalAdvocacy = new CommunityGroupServiceReportTable("Institutional Advocacy", 3) {
				Headers = headers,
				HideSubheaders = true,
				UseNonDuplicatedSubtotal = true
			};
			foreach (var item in Enums.GetValues<SACACInstitutionalAdvocacyEnum>())
				institutionalAdvocacy.Rows.Add(new ReportRow { Code = item.ToInt32(), Title = item.GetDisplayName(), Order = item.GetOrder() });
			ReportTableList.Add(institutionalAdvocacy);

			var professionalTraining = new CommunityGroupServiceReportTable("Professional Training", 4) {
				Headers = headers,
				HideSubheaders = true,
				UseNonDuplicatedSubtotal = true
			};
			foreach (var item in Enums.GetValues<SACACProfessionalTrainingServiceEnum>())
				professionalTraining.Rows.Add(new ReportRow { Code = item.ToInt32(), Title = item.GetDisplayName() });
			ReportTableList.Add(professionalTraining);

			var publicEducation = new CommunityGroupServiceReportTable("Public Education", 5) {
				Headers = headers,
				HideSubheaders = true,
				UseNonDuplicatedSubtotal = true
			};
			foreach (var item in Enums.GetValues<SAPublicEducationServiceEnum>())
				publicEducation.Rows.Add(new ReportRow { Code = item.ToInt32(), Title = item.GetDisplayName(), Order = item.GetOrder() });
			ReportTableList.Add(publicEducation);

			var training = new CommunityGroupServiceReportTable("Training", 6) {
				Headers = headers,
				HideSubheaders = true,
				UseNonDuplicatedSubtotal = true
			};
			foreach (var item in Enums.GetValues<SACACTrainingServiceEnum>())
				training.Rows.Add(new ReportRow { Code = item.ToInt32(), Title = item.GetDisplayName() });
			ReportTableList.Add(training);
		}

		private void CreateCACTables() {
			var headers = GetHeaders();

			var allServices = new CommunityGroupServiceReportTable("", 0) {
				RowPredicate = (r, i) => true,
				HideTitle = true,
				Headers = GetHeaders(),
				HideSubheaders = true,
				HideSubtotal = true,
				UseNonDuplicatedSubtotal = true
			};
			allServices.Rows.Add(new ReportRow { Title = "All Services", Code = 1 });
			ReportTableList.Add(allServices);

			var groupServices = new CommunityGroupServiceReportTable("Group Services", 1) {
				Headers = headers,
				HideSubheaders = true,
				UseNonDuplicatedSubtotal = true
			};
			foreach (var item in Lookups.GroupServices[ReportContainer.Provider])
				groupServices.Rows.Add(new ReportRow { Code = item.CodeId, Title = item.Description });
			ReportTableList.Add(groupServices);

			var informationAndReferral = new CommunityGroupServiceReportTable("Information and Referral", 2) {
				Headers = headers,
				HideSubheaders = true,
				UseNonDuplicatedSubtotal = true
			};
			foreach (var item in Enums.GetValues<SACACInformationReferralServiceEnum>())
				informationAndReferral.Rows.Add(new ReportRow { Code = item.ToInt32(), Title = item.GetDisplayName() });
			ReportTableList.Add(informationAndReferral);

			var institutionalAdvocacy = new CommunityGroupServiceReportTable("Institutional Advocacy", 3) {
				Headers = headers,
				HideSubheaders = true,
				UseNonDuplicatedSubtotal = true
			};
			foreach (var item in Enums.GetValues<SACACInstitutionalAdvocacyEnum>())
				institutionalAdvocacy.Rows.Add(new ReportRow { Code = item.ToInt32(), Title = item.GetDisplayName(), Order = item.GetOrder() });
			ReportTableList.Add(institutionalAdvocacy);

			var professionalTraining = new CommunityGroupServiceReportTable("Professional Training", 3) {
				Headers = headers,
				HideSubheaders = true,
				UseNonDuplicatedSubtotal = true
			};
			foreach (var item in Enums.GetValues<SACACProfessionalTrainingServiceEnum>())
				professionalTraining.Rows.Add(new ReportRow { Code = item.ToInt32(), Title = item.GetDisplayName() });
			ReportTableList.Add(professionalTraining);

			var publicEducation = new CommunityGroupServiceReportTable("Public Education", 4) {
				Headers = headers,
				HideSubheaders = true,
				UseNonDuplicatedSubtotal = true
			};
			foreach (var item in Enums.GetValues<CACPublicEducationServiceEnum>())
				publicEducation.Rows.Add(new ReportRow { Code = item.ToInt32(), Title = item.GetDisplayName(), Order = item.GetOrder() });
			ReportTableList.Add(publicEducation);

			var training = new CommunityGroupServiceReportTable("Training", 5) {
				Headers = headers,
				HideSubheaders = true,
				UseNonDuplicatedSubtotal = true
			};
			foreach (var item in Enums.GetValues<DVTrainingServiceEnum>())
				training.Rows.Add(new ReportRow { Code = item.ToInt32(), Title = item.GetDisplayName() });
			ReportTableList.Add(training);
		}

		public static List<ReportTableHeader> GetHeaders() {
			return new List<ReportTableHeader> {
				new ReportTableHeader { Code = ReportTableHeaderEnum.NumberOfPresentations, Title = "Number of Presentations/Contacts", SubHeaders = new List<ReportTableSubHeader> { new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.Total, Title = string.Empty } } },
				new ReportTableHeader { Code = ReportTableHeaderEnum.NumberOfParticipants, Title = "Number of Participants", SubHeaders = new List<ReportTableSubHeader> { new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.Total, Title = string.Empty } } },
				new ReportTableHeader { Code = ReportTableHeaderEnum.NumberOfStaff, Title = "Number of Staff", SubHeaders = new List<ReportTableSubHeader> { new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.Total, Title = string.Empty } } },
				new ReportTableHeader { Code = ReportTableHeaderEnum.PresentationHours, Title = "Presentation Hours", SubHeaders = new List<ReportTableSubHeader> { new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.Total, Title = string.Empty } } },
				new ReportTableHeader { Code = ReportTableHeaderEnum.StaffConductHours, Title = "Staff Conduct Hours", SubHeaders = new List<ReportTableSubHeader> { new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.Total, Title = string.Empty } } },
				new ReportTableHeader { Code = ReportTableHeaderEnum.StaffTravelHours, Title = "Staff Travel Hours", SubHeaders = new List<ReportTableSubHeader> { new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.Total, Title = string.Empty } } },
				new ReportTableHeader { Code = ReportTableHeaderEnum.StaffPreparationHours, Title = "Staff Preparation Hours", SubHeaders = new List<ReportTableSubHeader> { new ReportTableSubHeader { Code = ReportTableSubHeaderEnum.Total, Title = string.Empty } } }
			};
		}
	}

	public class CommunityGroupSubReportLineItem {
		public int? IcsId { get; set; }
		public string Center { get; set; }
		public DateTime? ServiceDate { get; set; }
		public int ServiceId { get; set; }
		public bool? IsGroupService { get; set; }
		public bool? IsCommInstService { get; set; }
		public int NumberOfPresentations { get; set; }
		public int NumberOfParticipants { get; set; }
		public double? PresentationHours { get; set; }
		public IEnumerable<StaffLineItem> Staff { get; set; }
	}
}