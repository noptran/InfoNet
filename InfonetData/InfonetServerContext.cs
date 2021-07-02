using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics.CodeAnalysis;
using Infonet.Core.Entity;
using Infonet.Data.Helpers;
using Infonet.Data.Mapping.Centers;
using Infonet.Data.Mapping.Clients;
using Infonet.Data.Mapping.Investigations;
using Infonet.Data.Mapping.Looking;
using Infonet.Data.Mapping.Obsolete;
using Infonet.Data.Mapping.Offenders;
using Infonet.Data.Mapping.Reporting;
using Infonet.Data.Mapping.Services;
using Infonet.Data.Mapping._TLU;
using Infonet.Data.Models.Centers;
using Infonet.Data.Models.Clients;
using Infonet.Data.Models.Investigations;
using Infonet.Data.Models.Looking;
using Infonet.Data.Models.Obsolete;
using Infonet.Data.Models.Offenders;
using Infonet.Data.Models.Reporting;
using Infonet.Data.Models.Services;
using Infonet.Data.Models._TLU;
using Serilog;

namespace Infonet.Data {
	public class InfonetServerContext : EnhancedDbContext {
		static InfonetServerContext() {
			Database.SetInitializer<InfonetServerContext>(null);
		}

		public InfonetServerContext() : base("Name=InfonetServerContext") {
			Helpers = new HelperGroup(this);
			Database.Log = LogAction;
		}

		[SuppressMessage("ReSharper", "UnusedMember.Global")]
		public int? CommandTimeout {
			get { return ((IObjectContextAdapter)this).ObjectContext.CommandTimeout; }
			set { ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = value; }
		}

		public HelperGroup Helpers { get; }

		#region entities
		// ReSharper disable UnusedAutoPropertyAccessor.Global
		// ReSharper disable InconsistentNaming
		// ReSharper disable UnusedMember.Global
		public DbSet<LookupListItemAssignment> LookupList_ItemAssignment { get; set; }
		public DbSet<LookupListTable> LookupList_Tables { get; set; }
		public DbSet<AdHocQuery> RPT_AdHocQueries { get; set; }
		public DbSet<AdHocTracking> RPT_AdHocTracking { get; set; }
		public DbSet<Agency> T_Agency { get; set; }
		public DbSet<Center> T_Center { get; set; }
		public DbSet<Client> T_Client { get; set; }
		public DbSet<ClientCase> T_ClientCases { get; set; }
		public DbSet<Contact> T_Contact { get; set; }
		public DbSet<FundingDate> T_FundingDates { get; set; }
		public DbSet<Investigation> T_Investigations { get; set; }
		public DbSet<Offender> T_Offender { get; set; }
		public DbSet<OffenderListing> T_OffenderList { get; set; }
		public DbSet<PhoneHotline> T_PhoneHotline { get; set; }
		public DbSet<StaffVolunteer> T_StaffVolunteer { get; set; }
		public DbSet<Cancellation> Tl_Cancellations { get; set; }
		public DbSet<ClientMDT> Tl_ClientMDT { get; set; }
		public DbSet<EventDetail> Tl_EventDetail { get; set; }
		public DbSet<FundServiceProgramOfStaff> Tl_FundServiceProgramOfStaffs { get; set; }
		public DbSet<HudServiceMapping> Tl_InfoNetHUDServiceMapping { get; set; }
		public DbSet<ProgramDetail> Tl_ProgramDetail { get; set; }
		public DbSet<PublicationDetail> Tl_PublicationDetail { get; set; }
		public DbSet<ServiceDetailOfClient> Tl_ServiceDetailOfClient { get; set; }
		public DbSet<TLU_Codes_FundingSource> TLU_Codes_FundingSource { get; set; }
		// ReSharper restore UnusedMember.Global
		//KMS DO cleanup more unused entities???
		public DbSet<TLU_Codes_HUDService> TLU_Codes_HUDService { get; set; } //KMS DO remove?
		public DbSet<TLU_Codes_OtherStaffActivity> TLU_Codes_OtherStaffActivity { get; set; }
		public DbSet<TLU_Codes_ProgramsAndServices> TLU_Codes_ProgramsAndServices { get; set; }
		public DbSet<TLU_Codes_ServiceCategory> TLU_Codes_ServiceCategory { get; set; }
		public DbSet<TLU_Codes_ServiceOutcome> TLU_Codes_ServiceOutcome { get; set; }
		public DbSet<AbuseNeglectPetition> Ts_AbuseNeglectPetitions { get; set; }
		public DbSet<AbuseNeglectPetitionRespondent> Ts_AbuseNeglectPetitionsRespondents { get; set; }
		public DbSet<CenterAdminFundingSources> Ts_CenterAdminFundingSources { get; set; }
		public DbSet<CenterAdministrators> Ts_CenterAdministrators { get; set; }
		public DbSet<ChildBehavioralIssues> Ts_ClientChildBehavioralIssues { get; set; }
		public DbSet<ClientCJProcess> Ts_ClientCJProcess { get; set; }
		public DbSet<ClientConflictScale> Ts_ClientConflictScale { get; set; }
		public DbSet<ClientCourtAppearance> Ts_ClientCourtAppearance { get; set; }
		public DbSet<ClientDeparture> Ts_ClientDeparture { get; set; }
		public DbSet<ClientDisability> Ts_ClientDisability { get; set; }
		public DbSet<FinancialResource> Ts_ClientFinancialResources { get; set; }
		public DbSet<ClientIncome> Ts_ClientIncome { get; set; }
		public DbSet<ClientNonCashBenefits> Ts_ClientNonCashBenefits { get; set; }
		public DbSet<ClientPoliceProsecution> Ts_ClientPoliceProsecution { get; set; }
		public DbSet<PresentingIssues> Ts_ClientPresentingIssue { get; set; }
		public DbSet<ClientRace> Ts_ClientRace { get; set; }
		public DbSet<ClientReferralDetail> Ts_ClientReferralDetail { get; set; }
		public DbSet<ClientReferralSource> Ts_ClientReferralSource { get; set; }
		public DbSet<ServiceGot> Ts_ClientServiceGot { get; set; }
		public DbSet<ServiceNeeds> Ts_ClientServiceNeeds { get; set; }
		public DbSet<DCFSAllegation> Ts_DCFSAllegations { get; set; }
		public DbSet<DCFSAllegationRespondent> Ts_DCFSAllegationsRespondents { get; set; }
		public DbSet<EventDetailStaff> Ts_EventDetail_Staffs { get; set; }
		public DbSet<HivMentalSubstance> Ts_HivMentalSubstance { get; set; }
		public DbSet<InvestigationClient> Ts_InvestigationClients { get; set; }
		public DbSet<InvestigationHouseHold> Ts_InvestigationHouseHolds { get; set; }
		public DbSet<OpActivity> Ts_OpActivity { get; set; }
		public DbSet<OrderOfProtection> Ts_OrderOfProtection { get; set; }
		public DbSet<OtherStaffActivity> Ts_OtherStaffActivity { get; set; }
		public DbSet<PoliceCharge> Ts_PoliceCharge { get; set; }
		public DbSet<PreviousServiceUse> Ts_PreviousServiceUse { get; set; }
		public DbSet<ProgramDetailStaff> Ts_ProgramDetail_Staffs { get; set; }
		public DbSet<PublicationDetailStaff> Ts_PublicationDetail_Staffs { get; set; }
		public DbSet<Sentence> Ts_Sentences { get; set; }
		public DbSet<ServiceOutcome> Ts_ServiceOutcome { get; set; }
		public DbSet<TrialCharge> Ts_TrialCharges { get; set; }
		public DbSet<TurnAwayService> Ts_TurnAwayServices { get; set; }
		public DbSet<TwnTshipCounty> Ts_TwnTshipCounty { get; set; }
		public DbSet<VictimSensitiveInterview> Ts_VictimSensitiveInterviews { get; set; }
		public DbSet<VSIObserver> Ts_VSIObservers { get; set; }
		public DbSet<SystemMessage> T_SystemMessages { get; set; }
		public DbSet<ReportJob> RPT_ReportJobs { get; set; }
		public DbSet<ReportJobApproval> RPT_ReportJobApprovals { get; set; }

		[Obsolete]
		public DbSet<SecurityIdentity> SecurityIdentities { get; set; }

		[Obsolete]
		public DbSet<SecurityRole> SecurityRoles { get; set; }
		// ReSharper restore InconsistentNaming
		// ReSharper restore UnusedAutoPropertyAccessor.Global
		#endregion

		#region mapping
		protected override void OnModelCreating(DbModelBuilder modelBuilder) {
#pragma warning disable 612
			modelBuilder.Configurations.Add(new LookupListItemAssignmentMap());
			modelBuilder.Configurations.Add(new LookupListTableMap());
			modelBuilder.Configurations.Add(new AdHocQueryMap());
			modelBuilder.Configurations.Add(new AdHocTrackingMap());
			modelBuilder.Configurations.Add(new SecurityPrincipalPropertiesMap());
			modelBuilder.Configurations.Add(new SecurityIdentityMap());
			modelBuilder.Configurations.Add(new SecurityPrincipalMap());
			modelBuilder.Configurations.Add(new SecurityPrivilegeMap());
			modelBuilder.Configurations.Add(new SecurityRoleMap());
			modelBuilder.Configurations.Add(new AgencyMap());
			modelBuilder.Configurations.Add(new CenterMap());
			modelBuilder.Configurations.Add(new ClientMap());
			modelBuilder.Configurations.Add(new ClientCaseMap());
			modelBuilder.Configurations.Add(new ContactMap());
			modelBuilder.Configurations.Add(new FundingDateMap());
			modelBuilder.Configurations.Add(new InvestigationMap());
			modelBuilder.Configurations.Add(new OffenderMap());
			modelBuilder.Configurations.Add(new OffenderListingMap());
			modelBuilder.Configurations.Add(new PhoneHotlineMap());
			modelBuilder.Configurations.Add(new StaffVolunteerMap());
			modelBuilder.Configurations.Add(new CancellationMap());
			modelBuilder.Configurations.Add(new ClientMDTMap());
			modelBuilder.Configurations.Add(new EventDetailMap());
			modelBuilder.Configurations.Add(new FundServiceProgramOfStaffMap());
			modelBuilder.Configurations.Add(new HudServiceMappingMap());
			modelBuilder.Configurations.Add(new ProgramDetailMap());
			modelBuilder.Configurations.Add(new PublicationDetailMap());
			modelBuilder.Configurations.Add(new ServiceDetailOfClientMap());
			modelBuilder.Configurations.Add(new TLU_Codes_FundingSourceMap());
			modelBuilder.Configurations.Add(new TLU_Codes_HivMentalSubstanceMap());
			modelBuilder.Configurations.Add(new TLU_Codes_HUDServiceMap());
			modelBuilder.Configurations.Add(new TLU_Codes_OtherStaffActivityMap());
			modelBuilder.Configurations.Add(new TLU_Codes_ProgramsAndServicesMap());
			modelBuilder.Configurations.Add(new TLU_Codes_ServiceCategoryMap());
			modelBuilder.Configurations.Add(new TLU_Codes_ServiceOutcomeMap());
			modelBuilder.Configurations.Add(new AbuseNeglectPetitionMap());
			modelBuilder.Configurations.Add(new AbuseNeglectPetitionRespondentMap());
			modelBuilder.Configurations.Add(new CenterAdminFundingSourcesMap());
			modelBuilder.Configurations.Add(new CenterAdministratorsMap());
			modelBuilder.Configurations.Add(new ChildBehavioralIssuesMap());
			modelBuilder.Configurations.Add(new ClientCJProcessMap());
			modelBuilder.Configurations.Add(new ClientConflictScaleMap());
			modelBuilder.Configurations.Add(new ClientCourtAppearanceMap());
			modelBuilder.Configurations.Add(new ClientDepartureMap());
			modelBuilder.Configurations.Add(new ClientDisabilityMap());
			modelBuilder.Configurations.Add(new FinancialResourceMap());
			modelBuilder.Configurations.Add(new ClientIncomeMap());
			modelBuilder.Configurations.Add(new ClientNonCashBenefitsMap());
			modelBuilder.Configurations.Add(new ClientPoliceProsecutionMap());
			modelBuilder.Configurations.Add(new PresentingIssuesMap());
			modelBuilder.Configurations.Add(new ClientRaceMap());
			modelBuilder.Configurations.Add(new ClientReferralDetailMap());
			modelBuilder.Configurations.Add(new ClientReferralSourceMap());
			modelBuilder.Configurations.Add(new ServiceGotMap());
			modelBuilder.Configurations.Add(new ServiceNeedsMap());
			modelBuilder.Configurations.Add(new DCFSAllegationMap());
			modelBuilder.Configurations.Add(new DCFSAllegationRespondentMap());
			modelBuilder.Configurations.Add(new EventDetailStaffMap());
			modelBuilder.Configurations.Add(new HivMentalSubstanceMap());
			modelBuilder.Configurations.Add(new InvestigationClientMap());
			modelBuilder.Configurations.Add(new InvestigationHouseHoldMap());
			modelBuilder.Configurations.Add(new OpActivityMap());
			modelBuilder.Configurations.Add(new OrderOfProtectionMap());
			modelBuilder.Configurations.Add(new OtherStaffActivityMap());
			modelBuilder.Configurations.Add(new PoliceChargeMap());
			modelBuilder.Configurations.Add(new PreviousServiceUseMap());
			modelBuilder.Configurations.Add(new ProgramDetailStaffMap());
			modelBuilder.Configurations.Add(new PublicationDetailStaffMap());
			modelBuilder.Configurations.Add(new SentenceMap());
			modelBuilder.Configurations.Add(new ServiceOutcomeMap());
			modelBuilder.Configurations.Add(new TrialChargeMap());
			modelBuilder.Configurations.Add(new TurnAwayServiceMap());
			modelBuilder.Configurations.Add(new TwnTshipCountyMap());
			modelBuilder.Configurations.Add(new VictimSensitiveInterviewMap());
			modelBuilder.Configurations.Add(new VSIObserverMap());
			modelBuilder.Configurations.Add(new SystemMessageMap());
			modelBuilder.Configurations.Add(new ReportJobMap());
			modelBuilder.Configurations.Add(new ReportJobApprovalMap());

			modelBuilder.Entity<HudServiceMapping>()
				.HasKey(c => new { c.ServiceProgramId, c.HudServiceId });

			modelBuilder.Entity<TLU_Codes_HUDService>()
				.HasMany(c => c.Tl_InfoNetHUDServiceMappings)
				.WithRequired()
				.HasForeignKey(c => c.HudServiceId);

			modelBuilder.Entity<TLU_Codes_ProgramsAndServices>()
				.HasMany(c => c.HudServices)
				.WithRequired()
				.HasForeignKey(c => c.ServiceProgramId);

#pragma warning restore 612
		}
		#endregion

		/*
		 * Previously, intent was to have standard logging for all (Enhanced)DbContexts,
		 * but EF appears to have a memory leak that hangs onto the Database.Log Action<string>
		 * indefinintely.  Therefore, using a closure for the Action<string> leaks anything
		 * referenced by that closure.  If such a closure references a DbContext to access its
		 * datasource name, then that DbContext is leaked too.
		 */
		private static void LogAction(string s) {
			if (!string.IsNullOrWhiteSpace(s))
				Log.Verbose("{DbContext:l}: {Message:l}", "InfonetServerContext", s.Trim());
		}

		#region inner
		public struct HelperGroup {
			internal HelperGroup(InfonetServerContext context) {
				Center = new CenterHelper(context);
				FundingForStaff = new FundingForStaffHelper(context);
			}

			public CenterHelper Center { get; }

			public FundingForStaffHelper FundingForStaff { get; }
		}
		#endregion
	}
}