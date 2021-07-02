using System.Data.Entity;
using System.Diagnostics.CodeAnalysis;
using Infonet.Core.Threading;

namespace Infonet.Data.Looking {
	[SuppressMessage("ReSharper", "UnusedMember.Global")]
	public static class Lookups {
		private static readonly LazyHolder<Lookup> _AbuseAllegation = CreateLookupHolder("TLU_Codes_AbuseAllegations");
		private static readonly LazyHolder<Lookup> _AbuseAllegationFinding = CreateLookupHolder("TLU_Codes_AbuseAllegationsFindings");
		private static readonly LazyHolder<Lookup> _AbuseNeglectPetition = CreateLookupHolder("TLU_Codes_AbuseNeglectPetitions");
		private static readonly LazyHolder<Lookup> _AppealStatus = CreateLookupHolder("TLU_Codes_AppealStatus");
		private static readonly LazyHolder<Lookup> _ArrestMade = CreateLookupHolder("TLU_Codes_PoliceArrestMade");
		private static readonly LazyHolder<Lookup> _BeforeAfter = CreateLookupHolder("TLU_Codes_BeforeAfter");
		private static readonly LazyHolder<Lookup> _CancellationReason = CreateLookupHolder("TLU_Codes_CancellationReasons");
		private static readonly LazyHolder<Lookup> _CaseClosedReason = CreateLookupHolder("TLU_Codes_CaseClosed");
		private static readonly LazyHolder<Lookup> _CenterAdministrator = CreateLookupHolder("TLU_Codes_CenterAdministrators");
		private static readonly LazyHolder<Lookup> _ChildCustody = CreateLookupHolder("TLU_Codes_ChildCustody");
		private static readonly LazyHolder<Lookup> _ChildLivesWith = CreateLookupHolder("TLU_Codes_ChildLivesWith");
		private static readonly LazyHolder<Lookup> _ChildBehavioralIssues = CreateLookupHolder("TLU_Codes_ChildBehavioralIssues");
		private static readonly LazyHolder<Lookup> _ClientRelationship = CreateLookupHolder("TLU_Codes_ClientToClientRelation");
		private static readonly LazyHolder<Lookup> _ClientType = CreateLookupHolder("TLU_Codes_ClientType");
		private static readonly LazyHolder<Lookup> _CommunityServices = CreateLookupHolder("TLU_Codes_ProgramsAndServices", 0.4, "l.IsCommInst = 1");
		private static readonly LazyHolder<Lookup> _Counties = CreateUspsLookupHolder("Counties", "Id", "CountyName", "0");
		private static readonly LazyHolder<Lookup> _CourtContinuance = CreateLookupHolder("TLU_Codes_CourtContinuance");
		private static readonly LazyHolder<Lookup> _CrimeClass = CreateLookupHolder("TLU_Codes_CrimeClass");
		private static readonly LazyHolder<Lookup> _Destination = CreateLookupHolder("TLU_Codes_Destination");
		private static readonly LazyHolder<Lookup> _DestinationSubsidy = CreateLookupHolder("TLU_Codes_DestinationSubsidy");
		private static readonly LazyHolder<Lookup> _DestinationTenure = CreateLookupHolder("TLU_Codes_DestinationTenure");
		private static readonly LazyHolder<Lookup> _DirectServices = CreateLookupHolder("TLU_Codes_ProgramsAndServices", filterSql: "l.IsService = 1 AND l.IsShelter = 0"); //KMS DO update so isShelter check not needed?
		private static readonly LazyHolder<Lookup> _DirectOrGroupServices = CreateLookupHolder("TLU_Codes_ProgramsAndServices", filterSql: "(l.IsGroupService = 1 OR l.IsService = 1) AND l.IsShelter = 0"); //KMS DO update so isShelter check not needed?
		private static readonly LazyHolder<Lookup> _Disposition = CreateLookupHolder("TLU_Codes_Disposition");
		private static readonly LazyHolder<Lookup> _Education = CreateLookupHolder("TLU_Codes_Education");
		private static readonly LazyHolder<Lookup> _EmploymentType = CreateLookupHolder("TLU_Codes_EmploymentType");
		private static readonly LazyHolder<Lookup> _Ethnicity = CreateLookupHolder("TLU_Codes_Ethnicity");
		private static readonly LazyHolder<Lookup> _EventTypes = CreateLookupHolder("TLU_Codes_ProgramsAndServices", 0.01, "l.IsEvent = 1");
		private static readonly LazyHolder<Lookup> _FedClass = CreateLookupHolder("TLU_Codes_FedClass");
		private static readonly LazyHolder<Lookup> _GenderIdentity = CreateLookupHolder("TLU_Codes_GenderIdentity");
		private static readonly LazyHolder<Lookup> _GroupServices = CreateLookupHolder("TLU_Codes_ProgramsAndServices", 0.15, "l.IsGroupService = 1");
		private static readonly LazyHolder<Lookup> _HealthInsurance = CreateLookupHolder("TLU_Codes_HealthInsurance");
		private static readonly LazyHolder<Lookup> _HealthInsurance2 = CreateLookupHolder("TLU_Codes_HealthInsurance2");
		private static readonly LazyHolder<Lookup> _HivMentalSubstance = CreateLookupHolder("TLU_Codes_HivMentalSubstance");
		private static readonly LazyHolder<Lookup> _HotlineCallType = CreateLookupHolder("TLU_Codes_HotlineCall");
		private static readonly LazyHolder<Lookup> _HotlineService = CreateLookupHolder("TLU_Codes_ProgramsAndServices", 0.01, "l.IsHotline = 1");
		private static readonly LazyHolder<Lookup> _HousingServices = CreateLookupHolder("TLU_Codes_ProgramsAndServices", 0.05, "l.IsShelter = 1");
		private static readonly LazyHolder<Lookup> _HudGroupServices = CreateLookupHolder("TLU_Codes_HUDService", filterSql: "l.CodeId IN (SELECT HudSvcId FROM TL_InfonetHUDServiceMapping WHERE InfonetSvcId IN (SELECT CodeId FROM TLU_Codes_ProgramsAndServices WHERE IsGroupService = 1))");
		private static readonly LazyHolder<Lookup> _HudServices = CreateLookupHolder("TLU_Codes_HUDService");
		private static readonly LazyHolder<Lookup> _HudShelterServices = CreateLookupHolder("TLU_Codes_HUDService", filterSql: "l.CodeId IN (SELECT HudSvcId FROM TL_InfonetHUDServiceMapping WHERE InfonetSvcId IN (SELECT CodeId FROM TLU_Codes_ProgramsAndServices WHERE IsShelter = 1))");
		private static readonly LazyHolder<Lookup> _IncomeSource = CreateLookupHolder("TLU_Codes_IncomeSource");
		private static readonly LazyHolder<Lookup> _IncomeSource2 = CreateLookupHolder("TLU_Codes_IncomeSource2");
		private static readonly LazyHolder<Lookup> _InjurySeverity = CreateLookupHolder("TLU_Codes_Injury");
		private static readonly LazyHolder<Lookup> _InvestigationType = CreateLookupHolder("TLU_Codes_InvestigationType");
		private static readonly LazyHolder<Lookup> _Language = CreateLookupHolder("TLU_Codes_Language");
		private static readonly LazyHolder<Lookup> _LengthOfStay = CreateLookupHolder("TLU_Codes_LengthOfStayInResidence");
		private static readonly LazyHolder<Lookup> _MaritalStatus = CreateLookupHolder("TLU_Codes_MaritalStatus");
		private static readonly LazyHolder<Lookup> _MedicalExamFinding = CreateLookupHolder("TLU_Codes_Finding");
		private static readonly LazyHolder<Lookup> _MedicalExamType = CreateLookupHolder("TLU_Codes_MedicalExamType");
		private static readonly LazyHolder<Lookup> _MedicalTreatmentLocation = CreateLookupHolder("TLU_Codes_MedicalTreatmentLocation");
		private static readonly LazyHolder<Lookup> _NonCashBenefits = CreateLookupHolder("TLU_Codes_NonCashBenefits");
		private static readonly LazyHolder<Lookup> _ObserverPosition = CreateLookupHolder("TLU_Codes_Observer");
		private static readonly LazyHolder<Lookup> _OrderOfProtectionActivity = CreateLookupHolder("TLU_Codes_OpActivity");
		private static readonly LazyHolder<Lookup> _OrderOfProtectionForum = CreateLookupHolder("TLU_Codes_ForumOfOP");
		private static readonly LazyHolder<Lookup> _OrderOfProtectionStatus = CreateLookupHolder("TLU_Codes_StatusOfOP");
		private static readonly LazyHolder<Lookup> _OrderOfProtectionType = CreateLookupHolder("TLU_Codes_TypeOfOP");
		private static readonly LazyHolder<Lookup> _PersonnelType = CreateLookupHolder("TLU_Codes_PersonnelType");
		private static readonly LazyHolder<Lookup> _PetitionAdjudication = CreateLookupHolder("TLU_Codes_PetitionAdjudication");
		private static readonly LazyHolder<Lookup> _PhotosTaken = CreateLookupHolder("TLU_Codes_PhotoTaken");
		private static readonly LazyHolder<Lookup> _Pregnant = CreateLookupHolder("TLU_Codes_Pregnant");
		private static readonly LazyHolder<Lookup> _PresentingIssueLocation = CreateLookupHolder("TLU_Codes_PrimaryPresentingIssueLocation");
		private static readonly LazyHolder<Lookup> _PrimaryPresentingIssue = CreateLookupHolder("TLU_Codes_PresentingIssue");
		private static readonly LazyHolder<Lookup> _ProgramsAndServices = CreateLookupHolder("TLU_Codes_ProgramsAndServices");
		private static readonly LazyHolder<Lookup> _Provider = CreateLookupHolder("TLU_Codes_ProviderID");
		private static readonly LazyHolder<Lookup> _PublicationTypes = CreateLookupHolder("TLU_Codes_ProgramsAndServices", 0.05, "l.IsPublication = 1");
		private static readonly LazyHolder<Lookup> _Race = CreateLookupHolder("TLU_Codes_Race");
		private static readonly LazyHolder<Lookup> _RaceHud = CreateLookupHolder("TLU_Codes_RaceHUD");
		private static readonly LazyHolder<Lookup> _ReasonForLeaving = CreateLookupHolder("TLU_Codes_ReasonForLeaving");
		private static readonly LazyHolder<Lookup> _RecordingType = CreateLookupHolder("TLU_Codes_RecordType");
		private static readonly LazyHolder<Lookup> _ReferralResponse = CreateLookupHolder("TLU_Codes_Response");
		private static readonly LazyHolder<Lookup> _ReferralSource = CreateLookupHolder("TLU_Codes_ReferralSource");
		private static readonly LazyHolder<Lookup> _ReferralType = CreateLookupHolder("TLU_Codes_ReferralType");
		private static readonly LazyHolder<Lookup> _RelationshipToClient = CreateLookupHolder("TLU_Codes_RelationshipToClient");
		private static readonly LazyHolder<Lookup> _ReportJobStatus = CreateLookupHolder("TLU_Codes_ReportJobStatus");
		private static readonly LazyHolder<Lookup> _ReportJobApprovalStatus = CreateLookupHolder("TLU_Codes_ReportJobApprovalStatus");
		private static readonly LazyHolder<Lookup> _ReportOutput = CreateLookupHolder("TLU_Codes_ReportOutput");
		private static readonly LazyHolder<Lookup> _ResidenceType = CreateLookupHolder("TLU_Codes_ResidenceType");
		private static readonly LazyHolder<Lookup> _School = CreateLookupHolder("TLU_Codes_School");
		private static readonly LazyHolder<Lookup> _Sentence = CreateLookupHolder("TLU_Codes_Sentence");
		private static readonly LazyHolder<Lookup> _ServiceCategory = CreateLookupHolder("TLU_Codes_ServiceCategory");
		private static readonly LazyHolder<Lookup> _ServiceClass = CreateLookupHolder("TLU_Codes_ServiceClass");
		private static readonly LazyHolder<Lookup> _ServiceOutcome = CreateLookupHolder("TLU_Codes_ServiceOutcome");
		private static readonly LazyHolder<Lookup> _Sex = CreateLookupHolder("TLU_Codes_Sex");
		private static readonly LazyHolder<Lookup> _SexualOrientation = CreateLookupHolder("TLU_Codes_SexualOrientation");
		private static readonly LazyHolder<Lookup> _SignificantOtherOf = CreateLookupHolder("TLU_Codes_SignificantOthers");
		private static readonly LazyHolder<Lookup> _SiteLocation = CreateLookupHolder("TLU_Codes_SiteLocation");
		private static readonly LazyHolder<Lookup> _StateAbbreviations = CreateUspsLookupHolder("States", "Id", "StateAbbreviation", "DisplayOrder");
		private static readonly LazyHolder<Lookup> _StateNames = CreateUspsLookupHolder("States", "Id", "StateName", "DisplayOrder");
		private static readonly LazyHolder<Lookup> _Statute = CreateLookupHolder("TLU_Codes_NewStatuteCodes"); /* Disregarding additional non-null column called Statute */
		private static readonly LazyHolder<Lookup> _SystemMessageMode = CreateLookupHolder("TLU_Codes_SystemMessageMode");
		private static readonly LazyHolder<Lookup> _TeamMemberPosition = CreateLookupHolder("TLU_Codes_PositionRole");
		private static readonly LazyHolder<Lookup> _TrialChargeFiled = CreateLookupHolder("TLU_Codes_TrialChargeFiled");
		private static readonly LazyHolder<Lookup> _TrialType = CreateLookupHolder("TLU_Codes_TrialType");
		private static readonly LazyHolder<Lookup> _TurnAwayReason = CreateLookupHolder("TLU_Codes_TurnAwayReason");
		private static readonly LazyHolder<Lookup> _VictimWitnessParticipation = CreateLookupHolder("TLU_Codes_VWParticipate");
		private static readonly LazyHolder<Lookup> _Visitation = CreateLookupHolder("TLU_Codes_Visitation");
		private static readonly LazyHolder<Lookup> _YesNo = CreateLookupHolder("TLU_Codes_YesNo");
		private static readonly LazyHolder<Lookup> _YesNo2 = CreateLookupHolder("TLU_Codes_YesNo2");

		public static Lookup AbuseAllegation {
			get { return _AbuseAllegation.Value; }
		}

		public static Lookup AbuseAllegationFinding {
			get { return _AbuseAllegationFinding.Value; }
		}

		public static Lookup AbuseNeglectPetition {
			get { return _AbuseNeglectPetition.Value; }
		}

		public static Lookup AppealStatus {
			get { return _AppealStatus.Value; }
		}

		public static Lookup ArrestMade {
			get { return _ArrestMade.Value; }
		}

		public static Lookup BeforeAfter {
			get { return _BeforeAfter.Value; }
		}

		public static Lookup CancellationReason {
			get { return _CancellationReason.Value; }
		}

		public static Lookup CaseClosedReason {
			get { return _CaseClosedReason.Value; }
		}

		public static Lookup CenterAdministrator {
			get { return _CenterAdministrator.Value; }
		}

		public static Lookup ChildCustody {
			get { return _ChildCustody.Value; }
		}

		public static Lookup ChildLivesWith {
			get { return _ChildLivesWith.Value; }
		}

		public static Lookup ChildBehavioralIssues {
			get { return _ChildBehavioralIssues.Value; }
		}

		public static Lookup ClientRelationship {
			get { return _ClientRelationship.Value; }
		}

		public static Lookup ClientType {
			get { return _ClientType.Value; }
		}

		public static Lookup CommunityServices {
			get { return _CommunityServices.Value; }
		}

		public static Lookup Counties {
			get { return _Counties.Value; }
		}

		public static Lookup CourtContinuance {
			get { return _CourtContinuance.Value; }
		}

		public static Lookup CrimeClass {
			get { return _CrimeClass.Value; }
		}

		public static Lookup Destination {
			get { return _Destination.Value; }
		}

		public static Lookup DestinationSubsidy {
			get { return _DestinationSubsidy.Value; }
		}

		public static Lookup DestinationTenure {
			get { return _DestinationTenure.Value; }
		}

		public static Lookup DirectServices {
			get { return _DirectServices.Value; }
		}

		public static Lookup DirectOrGroupServices {
			get { return _DirectOrGroupServices.Value; }
		}

		public static Lookup Disposition {
			get { return _Disposition.Value; }
		}

		public static Lookup Education {
			get { return _Education.Value; }
		}

		public static Lookup EmploymentType {
			get { return _EmploymentType.Value; }
		}

		public static Lookup Ethnicity {
			get { return _Ethnicity.Value; }
		}

		public static Lookup EventTypes {
			get { return _EventTypes.Value; }
		}

		public static Lookup FedClass {
			get { return _FedClass.Value; }
		}

		public static Lookup GenderIdentity {
			get { return _GenderIdentity.Value; }
		}

		public static Lookup GroupServices {
			get { return _GroupServices.Value; }
		}

		public static Lookup HealthInsurance {
			get { return _HealthInsurance.Value; }
		}

		public static Lookup HealthInsurance2 {
			get { return _HealthInsurance2.Value; }
		}

		public static Lookup HivMentalSubstance {
			get { return _HivMentalSubstance.Value; }
		}

		public static Lookup HotlineCallType {
			get { return _HotlineCallType.Value; }
		}

		public static Lookup HotlineService {
			get { return _HotlineService.Value; }
		}

		public static Lookup HousingServices {
			get { return _HousingServices.Value; }
		}

		public static Lookup HudGroupServices {
			get { return _HudGroupServices.Value; }
		}

		public static Lookup HudServices {
			get { return _HudServices.Value; }
		}

		public static Lookup HudShelterServices {
			get { return _HudShelterServices.Value; }
		}

		public static Lookup IncomeSource {
			get { return _IncomeSource.Value; }
		}

		public static Lookup IncomeSource2 {
			get { return _IncomeSource2.Value; }
		}

		public static Lookup InjurySeverity {
			get { return _InjurySeverity.Value; }
		}

		public static Lookup InvestigationType {
			get { return _InvestigationType.Value; }
		}

		public static Lookup Language {
			get { return _Language.Value; }
		}

		public static Lookup LengthOfStay {
			get { return _LengthOfStay.Value; }
		}

		public static Lookup MaritalStatus {
			get { return _MaritalStatus.Value; }
		}

		public static Lookup MedicalExamFinding {
			get { return _MedicalExamFinding.Value; }
		}

		public static Lookup MedicalExamType {
			get { return _MedicalExamType.Value; }
		}

		public static Lookup MedicalTreatmentLocation {
			get { return _MedicalTreatmentLocation.Value; }
		}

		public static Lookup NonCashBenefits {
			get { return _NonCashBenefits.Value; }
		}

		public static Lookup ObserverPosition {
			get { return _ObserverPosition.Value; }
		}

		public static Lookup OrderOfProtectionActivity {
			get { return _OrderOfProtectionActivity.Value; }
		}

		public static Lookup OrderOfProtectionForum {
			get { return _OrderOfProtectionForum.Value; }
		}

		public static Lookup OrderOfProtectionStatus {
			get { return _OrderOfProtectionStatus.Value; }
		}

		public static Lookup OrderOfProtectionType {
			get { return _OrderOfProtectionType.Value; }
		}

		public static Lookup PersonnelType {
			get { return _PersonnelType.Value; }
		}

		public static Lookup PetitionAdjudication {
			get { return _PetitionAdjudication.Value; }
		}

		public static Lookup PhotosTaken {
			get { return _PhotosTaken.Value; }
		}

		public static Lookup Pregnant {
			get { return _Pregnant.Value; }
		}

		public static Lookup PresentingIssueLocation {
			get { return _PresentingIssueLocation.Value; }
		}

		public static Lookup PrimaryPresentingIssue {
			get { return _PrimaryPresentingIssue.Value; }
		}

		public static Lookup ProgramsAndServices {
			get { return _ProgramsAndServices.Value; }
		}

		public static Lookup Provider {
			get { return _Provider.Value; }
		}

		public static Lookup PublicationTypes {
			get { return _PublicationTypes.Value; }
		}

		public static Lookup Race {
			get { return _Race.Value; }
		}

		public static Lookup RaceHud {
			get { return _RaceHud.Value; }
		}

		public static Lookup RecordingType {
			get { return _RecordingType.Value; }
		}

		public static Lookup ReasonForLeaving {
			get { return _ReasonForLeaving.Value; }
		}

		public static Lookup ReferralResponse {
			get { return _ReferralResponse.Value; }
		}

		public static Lookup ReferralSource {
			get { return _ReferralSource.Value; }
		}

		public static Lookup ReferralType {
			get { return _ReferralType.Value; }
		}

		public static Lookup RelationshipToClient {
			get { return _RelationshipToClient.Value; }
		}

		public static Lookup ReportJobStatus {
			get { return _ReportJobStatus.Value; }
		}

		public static Lookup ReportJobApprovalStatus {
			get { return _ReportJobApprovalStatus.Value; }
		}

		public static Lookup ReportOutput {
			get { return _ReportOutput.Value; }
		}

		public static Lookup ResidenceType {
			get { return _ResidenceType.Value; }
		}

		public static Lookup School {
			get { return _School.Value; }
		}

		public static Lookup Sentence {
			get { return _Sentence.Value; }
		}

		public static Lookup ServiceCategory {
			get { return _ServiceCategory.Value; }
		}

		public static Lookup ServiceClass {
			get { return _ServiceClass.Value; }
		}

		public static Lookup ServiceOutcome {
			get { return _ServiceOutcome.Value; }
		}

		public static Lookup Sex {
			get { return _Sex.Value; }
		}

		public static Lookup SexualOrientation {
			get { return _SexualOrientation.Value; }
		}

		public static Lookup SignificantOtherOf {
			get { return _SignificantOtherOf.Value; }
		}

		public static Lookup SiteLocation {
			get { return _SiteLocation.Value; }
		}

		public static Lookup StateAbbreviations {
			get { return _StateAbbreviations.Value; }
		}

		public static Lookup StateNames {
			get { return _StateNames.Value; }
		}

		public static Lookup Statute {
			get { return _Statute.Value; }
		}

		public static Lookup SystemMessageMode {
			get { return _SystemMessageMode.Value; }
		}

		public static Lookup TeamMemberPosition {
			get { return _TeamMemberPosition.Value; }
		}

		public static Lookup TrialChargeFiled {
			get { return _TrialChargeFiled.Value; }
		}

		public static Lookup TrialType {
			get { return _TrialType.Value; }
		}

		public static Lookup TurnAwayReason {
			get { return _TurnAwayReason.Value; }
		}

		public static Lookup VictimWitnessParticipation {
			get { return _VictimWitnessParticipation.Value; }
		}

		public static Lookup Visitation {
			get { return _Visitation.Value; }
		}

		public static Lookup YesNo {
			get { return _YesNo.Value; }
		}

		public static Lookup YesNo2 {
			get { return _YesNo2.Value; }
		}

		public static void Reset() {
			foreach (var each in typeof(Lookups).GetFields())
				if (each.IsStatic)
					(each.GetValue(null) as LazyHolder<Lookup>)?.Reset();
		}

		#region building lookups
		private const string QUERY_TEMPLATE = @"SELECT
										t.TableId,
										t.TableName,
										t.DisplayName AS TableDisplayName,
										t.Description AS TableDescription,
										i.ProviderId,
										l.CodeId,
										l.Description,
										COALESCE(i.DisplayOrder, 0) AS DisplayOrder,
										COALESCE(i.IsActive, CAST(0 AS BIT)) AS IsActive
									FROM {0} l
										LEFT JOIN LookupList_Tables t ON t.TableName = '{0}'
										LEFT JOIN LookupList_ItemAssignment i ON i.TableId = t.TableId AND i.CodeId = l.CodeId{1}
									ORDER BY l.CodeId";

		/* suppressed messages below because they are untrue; see below for usage. */
		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
		[SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
		private class Row {
			public int TableId { get; set; }
			public string TableName { get; set; }
			public string TableDisplayName { get; set; }
			public string TableDescription { get; set; }
			public int CodeId { get; set; }
			public string Description { get; set; }
			public int? ProviderId { get; set; }
			public int DisplayOrder { get; set; }
			public bool IsActive { get; set; }
		}

		private static LazyHolder<Lookup> CreateLookupHolder(string tableName, double minLoadFactor = Lookup.DEFAULT_MIN_LOAD_FACTOR, string filterSql = null) {
			if (filterSql != null)
				filterSql = " AND " + filterSql;
			return new LazyHolder<Lookup>(() => {
				using (var db = new DbContext("Name=InfonetServerContext")) {
					var b = new LookupBuilder();
					int? lastCodeId = null;
					foreach (var each in db.Database.SqlQuery<Row>(string.Format(QUERY_TEMPLATE, tableName, filterSql))) {
						if (lastCodeId == null) {
							b.TableId = each.TableId;
							b.TableName = each.TableName;
							b.DisplayName = each.TableDisplayName;
							b.Description = each.TableDescription;
						}
						if (lastCodeId != each.CodeId)
							b.Codes.Add(new LookupBuilder.Code { CodeId = each.CodeId, Description = each.Description });
						if (each.ProviderId != null)
							b.Codes[b.Codes.Count - 1].Entries.Add(new LookupBuilder.Entry { Provider = ProviderEnum.For(each.ProviderId), DisplayOrder = each.DisplayOrder, IsActive = each.IsActive });
						lastCodeId = each.CodeId;
					}
					return b.ToLookup(minLoadFactor);
				}
			});
		}
		#endregion

		#region building usps lookups
		//these shouldn't really be here using LookupCodes, but for our purposes at this time, it's not worth building a utility specific to USPS.
		private const string USPS_QUERY_TEMPLATE = @"select {1} as CodeId, {2} as Description, {3} as DisplayOrder from {0}";

		/* suppressed messages below because they are untrue; see below for usage. */
		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
		[SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
		private class UspsRow {
			public int CodeId { get; set; }
			public string Description { get; set; }
			public int DisplayOrder { get; set; }
		}

		private static LazyHolder<Lookup> CreateUspsLookupHolder(string tableName, string codeIdColumn, string descriptionColumn, string displayOrderColumn) {
			return new LazyHolder<Lookup>(() => {
				using (var db = new DbContext("Name=UspsContext")) {
					var b = new LookupBuilder();
					b.TableName = b.DisplayName = b.Description = tableName;
					foreach (var each in db.Database.SqlQuery<UspsRow>(string.Format(USPS_QUERY_TEMPLATE, tableName, codeIdColumn, descriptionColumn, displayOrderColumn))) {
						var c = new LookupBuilder.Code { CodeId = each.CodeId, Description = each.Description };
						foreach (var eachProvider in ProviderEnum.All)
							c.Entries.Add(new LookupBuilder.Entry { Provider = eachProvider, DisplayOrder = each.DisplayOrder, IsActive = true });
						b.Codes.Add(c);
					}
					return b.ToLookup();
				}
			});
		}
		#endregion
	}
}