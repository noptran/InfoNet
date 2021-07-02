using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Infonet.Core.Collections;
using Infonet.Core.Entity;
using Infonet.Core.Entity.Binding;
using Infonet.Core.Entity.Validation;
using Infonet.Data.Entity;
using Infonet.Data.Looking;
using Infonet.Data.Models.Investigations;
using Infonet.Data.Models.Offenders;
using Infonet.Data.Models.Services;
using Infonet.Usps.Data.Helpers;
using LinqKit;

namespace Infonet.Data.Models.Clients {
	[BindHint(Include = "Age,RelationSOtoClientId,EmploymentId,EducationId,MaritalStatusId,PregnantId,NumberOfChildren,FirstContactDate,ClosedReasonId,IsClosed,ClosedDate,CustodyId,LivesWithId,SchoolId,VetStatusId,SexualOrientationId,IsDCFSOpen,IsDCFSInvestigation,Client,PresentingIssues,ClientNonCashBenefits,ServiceNeeds,ServiceGot,FinancialResources,IsNoneIncomeSourceSelected,IsUnknownIncomeSourceSelected,ClientDisability,ClientReferralSource,ChildBehavioralIssues,OffendersById,PreviousServiceUse,ClientConflictScale,ClientDepartures,OrdersOfProtectionById,ClientCourtAppearancesById,ClientCourtProsectionById,ClientIncome,HealthInsuranceId,IsStudent,DCFSHotlineDate,DCFSServiceDate,PoliceReportDate,InvestigationTypeId,IsInformationOnlyCase,SignificantOtherOfId,ClientCJProcessesById,DCFSAllegationsById,AbuseNeglectPetitionsById,ClientMDTById,VictimSensitiveInterviewsById,ClientPoliceProsecutionsById")]
	public class ClientCase : IValidatableObject, IRevisable, INotifyContextSavedChanges, IProvided {
		private bool _isNoneIncomeSourceSelected;
		private bool _isUnknownIncomeSourceSelected;

		public ClientCase() {
			Offenders = new List<Offender>();
			OffendersById = new DerivedDictionary<Offender>(() => Offenders, true, e => e.OffenderId?.ToString()) { Template = () => new Offender { StateId = UspsHelper.IllinoisId } };
			Cancellations = new List<Cancellation>();
			ClientMDT = new List<ClientMDT>();
			ClientMDTById = new DerivedDictionary<ClientMDT>(() => ClientMDT, true, e => e.MDT_ID?.ToString()) { Template = () => new ClientMDT() };
			ServiceDetailsOfClient = new List<ServiceDetailOfClient>();
			AbuseNeglectPetitions = new List<AbuseNeglectPetition>();
			AbuseNeglectPetitionsById = new DerivedDictionary<AbuseNeglectPetition>(() => AbuseNeglectPetitions, true, e => e.Id?.ToString()) { Template = () => new AbuseNeglectPetition() };
			InvestigationClients = new List<InvestigationClient>();
			ClientCJProcesses = new List<ClientCJProcess>();
			ClientCJProcessesById = new DerivedDictionary<ClientCJProcess>(() => ClientCJProcesses, true, e => e.Med_ID?.ToString()) { Template = () => new ClientCJProcess() };
			ClientCourtAppearances = new List<ClientCourtAppearance>();
			ClientCourtAppearancesById = new DerivedDictionary<ClientCourtAppearance>(() => ClientCourtAppearances, true, e => e.ID?.ToString()) { Template = () => new ClientCourtAppearance() };
			ClientDepartures = new List<ClientDeparture>();
			ClientPoliceProsecutions = new List<ClientPoliceProsecution>();
			ClientPoliceProsecutionsById = new DerivedDictionary<ClientPoliceProsecution>(() => ClientPoliceProsecutions, true, e => e.Id?.ToString()) { Template = () => new ClientPoliceProsecution() };
			FinancialResources = new List<FinancialResource>();
			ClientReferralDetail = new List<ClientReferralDetail>();
			DCFSAllegations = new List<DCFSAllegation>();
			DCFSAllegationsById = new DerivedDictionary<DCFSAllegation>(() => DCFSAllegations, true, e => e.Id?.ToString()) { Template = () => new DCFSAllegation() };
			OrdersOfProtection = new List<OrderOfProtection>();
			OrdersOfProtectionById = new DerivedDictionary<OrderOfProtection>(() => OrdersOfProtection, true, e => e.OP_ID?.ToString()) { Template = () => new OrderOfProtection() };
			VictimSensitiveInterviews = new List<VictimSensitiveInterview>();
			VictimSensitiveInterviewsById = new DerivedDictionary<VictimSensitiveInterview>(() => VictimSensitiveInterviews, true, e => e.VSI_ID?.ToString()) { Template = () => new VictimSensitiveInterview() };
		}

		public int? ClientId { get; set; }

		public int? CaseId { get; set; }

		[Required]
		[Range(-1, 120)]
		[WholeNumber]
		[OnBindException("The field {1} must be a whole number.", typeof(FormatException))]
		[Display(Name = "Age at First Contact")]
		public int? Age { get; set; }

		[Display(Name = "College/University Student")]
		[NotMapped]
		public bool IsStudent {
			get { return CollegeUnivStudentId != null && CollegeUnivStudentId > 0; }
			set { CollegeUnivStudentId = value ? 1 : (int?)null; }
		}

		/* has FK to TLU_Codes_YesNo but functions as boolean */
		[LogicalBoolean]
		public int? CollegeUnivStudentId { get; set; }

		[Help("Select your client's employment status. If they choose not to answer, select Not Reported.")]
		[Help(Provider.CAC, "Select the client's employment status from the drop-down menu.")]
		[Lookup("EmploymentType")]
		[Display(Name = "Employment")]
		public int? EmploymentId { get; set; }

		[Lookup("HealthInsurance")]
		[Display(Name = "Health Insurance")]
		public int? HealthInsuranceId { get; set; }

		[Lookup("Education")]
		[Display(Name = "Education")]
		public int? EducationId { get; set; }

		[Help("Select your client's marital status.  If they choose not to answer, select Not Reported.")]
		[Help(Provider.DV, "Select your client's marital status.  If they choose not to answer, select Unknown.")]
		[Help(Provider.CAC, "Select the client's marital status from the drop-down menu.")]
		[Lookup("MaritalStatus")]
		[Display(Name = "Marital Status")]
		public int? MaritalStatusId { get; set; }

		[Help("Select Yes if your client is pregnant, No if they are not pregnant, and Not Applicable if your client is male. If they choose not to answer, select Not Reported.")]
		[Help(Provider.SA, "Select to indicate if your client is pregnant. If they choose not to answer or for male clients, select Not Reported.")]
		[Help(Provider.CAC, "Indicate if the client is pregnant. For male clients, select Not Reported.")]
		[Lookup("Pregnant")]
		[Display(Name = "Pregnant")]
		public int? PregnantId { get; set; }

		[Help("Enter the number of children your client has living in his/her household, even if only part time. Do not include grown, independent adult children for which the client has legal custody.  If they choose not to report answer, enter -1 for Unknown.")]
		[Help(Provider.CAC, "Indicate the number of children (including the victim) living in the Caretaker's household.")]
		[WholeNumber]
		[OnBindException("The field {1} must be a whole number.", typeof(FormatException))]
		[Display(Name = "Number of Children")]
		public int? NumberOfChildren { get; set; }

		[Required]
		[BetweenNineteenSeventyToday]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
		[Display(Name = "First Contact Date")]
		public DateTime? FirstContactDate { get; set; }

		[Lookup("CaseClosedReason")]
		[Display(Name = "Reason Case Closed")]
		public int? ClosedReasonId { get; set; }

		[Display(Name = "Case Closed")]
		[NotMapped]
		public bool IsClosed {
			get { return CaseClosed != null && Convert.ToBoolean(CaseClosed); }
			set { CaseClosed = Convert.ToInt32(value); }
		}

		[LogicalBoolean]
		public int? CaseClosed { get; set; }

		[BetweenNineteenSeventyToday]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
		[Display(Name = "Date Closed")]
		public DateTime? ClosedDate { get; set; }

		[Lookup("SignificantOtherOf")]
		[Display(Name = "Significant Other Of")]
		public int? SignificantOtherOfId { get; set; }

		[Help("Select the option that applies to this client's custody status. If the client chooses not to answer, select Unknown.")]
		[Help(Provider.CAC, "Select the option that most closely aligns with who has legal custody of the child client.")]
		[Lookup("ChildCustody")]
		[Display(Name = "Custody")]
		public int? CustodyId { get; set; }

		[Lookup("ChildLivesWith")]
		[Display(Name = "Lives With")]
		public int? LivesWithId { get; set; }

		[Display(Name = "DCFS Investigation")]
		[NotMapped]
		public bool IsDCFSInvestigation {
			get { return DCFSInvestigation != null && Convert.ToBoolean(DCFSInvestigation); }
			set { DCFSInvestigation = Convert.ToInt32(value); }
		}

		[LogicalBoolean]
		public int? DCFSInvestigation { get; set; }

		[Display(Name = "DCFS Open")]
		[NotMapped]
		public bool IsDCFSOpen {
			get { return DCFSOpen != null && Convert.ToBoolean(DCFSOpen); }
			set { DCFSOpen = Convert.ToInt32(value); }
		}

		[LogicalBoolean]
		public int? DCFSOpen { get; set; }

		[Lookup("School")]
		[Display(Name = "School")]
		public int? SchoolId { get; set; }

		#region obsolete
		[Obsolete]
		public string AldermanicWard { get; set; }

		[Obsolete]
		public string LegislativeDistricts { get; set; }
		#endregion

		[Lookup("RelationshipToClient")]
		[Display(Name = "Relationship to Victim")]
		public int? RelationSOtoClientId { get; set; }

		public DateTime? RevisionStamp { get; set; }

		[BetweenNineteenNinetyToday]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
		[Display(Name = "DCFS Hotline Date")]
		public DateTime? DCFSHotlineDate { get; set; }

		[BetweenNineteenNinetyToday]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
		[Display(Name = "DCFS Service Date")]
		public DateTime? DCFSServiceDate { get; set; }

		[Lookup("InvestigationType")]
		[Display(Name = "Investigation Type")]
		public int? InvestigationTypeId { get; set; }

		[BetweenNineteenNinetyToday]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
		[Display(Name = "Police Report Date")]
		public DateTime? PoliceReportDate { get; set; }

		public bool? InformationOnlyCase { get; set; }

		[NotMapped]
		[Display(Name = "Information Only Case")]
		public bool IsInformationOnlyCase {
			get { return InformationOnlyCase != null && InformationOnlyCase == true; }
			set { InformationOnlyCase = value; }
		}

		[Lookup("YesNo")]
		[Display(Name = "Veteran's Status")]
		public int? VetStatusId { get; set; }

		[Lookup("SexualOrientation")]
		[Display(Name = "Sexual Orientation")]
		public int? SexualOrientationId { get; set; }

		public virtual Client Client { get; set; }

		public Provider Provider {
			get { return Client?.Provider ?? Provider.None; }
		}

		public CaseType CaseType {
			get { return Client?.CaseType ?? CaseType.None; }
		}

		public virtual ICollection<Offender> Offenders { get; set; }

		[NotMapped]
		public virtual DerivedDictionary<Offender> OffendersById { get; }

		public virtual IList<Cancellation> Cancellations { get; set; }
		public virtual ICollection<ClientMDT> ClientMDT { get; set; }

		[NotMapped]
		public virtual DerivedDictionary<ClientMDT> ClientMDTById { get; }

		public virtual IList<ServiceDetailOfClient> ServiceDetailsOfClient { get; set; }

		public virtual ICollection<AbuseNeglectPetition> AbuseNeglectPetitions { get; set; }

		[NotMapped]
		public virtual DerivedDictionary<AbuseNeglectPetition> AbuseNeglectPetitionsById { get; }

		public virtual ICollection<InvestigationClient> InvestigationClients { get; set; }

		public virtual ChildBehavioralIssues ChildBehavioralIssues { get; set; }

		public virtual IList<ClientPoliceProsecution> ClientPoliceProsecutions { get; set; }

		[NotMapped]
		public virtual DerivedDictionary<ClientPoliceProsecution> ClientPoliceProsecutionsById { get; }

		public virtual IList<ClientCJProcess> ClientCJProcesses { get; set; }

		[NotMapped]
		public virtual DerivedDictionary<ClientCJProcess> ClientCJProcessesById { get; }

		public virtual ClientConflictScale ClientConflictScale { get; set; }

		public virtual IList<ClientCourtAppearance> ClientCourtAppearances { get; set; }

		[NotMapped]
		public virtual DerivedDictionary<ClientCourtAppearance> ClientCourtAppearancesById { get; }

		public virtual IList<ClientDeparture> ClientDepartures { get; set; }

		public virtual ClientDisability ClientDisability { get; set; }

		public virtual IList<FinancialResource> FinancialResources { get; set; }

		public virtual ClientIncome ClientIncome { get; set; }

		public virtual ClientNonCashBenefits ClientNonCashBenefits { get; set; }

		public virtual PresentingIssues PresentingIssues { get; set; }

		public virtual ICollection<ClientReferralDetail> ClientReferralDetail { get; set; }

		public virtual ClientReferralSource ClientReferralSource { get; set; }

		public virtual PreviousServiceUse PreviousServiceUse { get; set; }

		public virtual ServiceGot ServiceGot { get; set; }

		public virtual ServiceNeeds ServiceNeeds { get; set; }

		public virtual ICollection<DCFSAllegation> DCFSAllegations { get; set; }

		[NotMapped]
		public virtual DerivedDictionary<DCFSAllegation> DCFSAllegationsById { get; }

		public virtual ICollection<OrderOfProtection> OrdersOfProtection { get; set; }

		[NotMapped]
		public virtual DerivedDictionary<OrderOfProtection> OrdersOfProtectionById { get; }

		public virtual ICollection<VictimSensitiveInterview> VictimSensitiveInterviews { get; set; }

		[NotMapped]
		public virtual DerivedDictionary<VictimSensitiveInterview> VictimSensitiveInterviewsById { get; }

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
			var results = new List<ValidationResult>();
			if (Provider == Provider.DV)
				foreach (var offender in OffendersById)
					if (offender.Value.VisitationId == null)
						results.Add(new ValidationResult("The Visitation field is required.", new[] { $"OffendersById[{offender.Key}].VisitationId" }));

			if (FirstContactDate != null && ClosedDate != null && ClosedDate < FirstContactDate)
				results.Add(new ValidationResult("Closed Date must not be before First Contact Date.", new[] { "FirstContactDate", "ClosedDate" }));

			// Validation for Previous Service Use Dates
			if (FirstContactDate != null && PreviousServiceUse != null) {
				if (PreviousServiceUse.PrevShelterDate < FirstContactDate.Value.AddYears(-1))
					results.Add(new ValidationResult("Date must not be greater than a year before First Contact Date.", new[] { "PreviousServiceUse.PrevShelterDate" }));

				if (PreviousServiceUse.PrevServiceDate < FirstContactDate.Value.AddYears(-1))
					results.Add(new ValidationResult("Date must not be greater than a year before First Contact Date.", new[] { "PreviousServiceUse.PrevServiceDate" }));
			}

			//KMS DO couldn't/shouldn't this be moved to TrialCharge?
			foreach (var offender in OffendersById)
				foreach (var trialCharge in offender.Value.TrialChargesById)
					foreach (var sentence in trialCharge.Value.SentencesById)
						if (trialCharge.Value.ChargeDate != null && sentence.Value.SentenceDate != null && sentence.Value.SentenceDate < trialCharge.Value.ChargeDate)
							results.Add(new ValidationResult("Sentence Date must be later than the Charge Date.", new[] { $"OffendersById[{offender.Key}].TrialChargesById[{trialCharge.Key}].SentencesById[{sentence.Key}].SentenceDate", $"OffendersById[{offender.Key}].TrialChargesById[{trialCharge.Key}].ChargeDate" }));

			if (Provider == Provider.CAC)
				foreach (var medicalVisit in ClientCJProcessesById)
					if (medicalVisit.Value.MedicalVisitId == null)
						results.Add(new ValidationResult("The Visited Medical Facility? field is required.", new[] { $"ClientCJProcessesById[{medicalVisit.Key}].MedicalVisitId" }));

			foreach (var each in OrdersOfProtectionById) {
				var eachOop = each.Value;
				if (Provider != Provider.SA) {
					if (eachOop.StatusID == null)
						results.Add(new ValidationResult("The Originally Sought Order field is required.", new[] { $"OrdersOfProtectionById[{each.Key}].StatusID" }));
					if (eachOop.CountyID == null)
						results.Add(new ValidationResult("The County field is required.", new[] { $"OrdersOfProtectionById[{each.Key}].CountyID" }));
					if (eachOop.TypeOfOPID == null)
						results.Add(new ValidationResult("The Type field is required.", new[] { $"OrdersOfProtectionById[{each.Key}].TypeOfOPID" }));
					if (eachOop.DateFiled == null)
						results.Add(new ValidationResult("The Date Filed field is required.", new[] { $"OrdersOfProtectionById[{each.Key}].DateFiled" }));
				} else {
					if (eachOop.ForumID == null && eachOop.TypeOfOPID != null)
						results.Add(new ValidationResult("Forum required when Type is selected.", new[] { $"OrdersOfProtectionById[{each.Key}].ForumID" }));
					if (eachOop.TypeOfOPID == null && eachOop.ForumID != null)
						results.Add(new ValidationResult("Type required when Forum is selected.", new[] { $"OrdersOfProtectionById[{each.Key}].TypeOfOPID" }));
					if (eachOop.CivilNoContactOrderId == null && eachOop.CivilNoContactOrderTypeId != null)
						results.Add(new ValidationResult("Civil No Contact Order required when there is a Civil No Contact Order Type.", new[] { $"OrdersOfProtectionById[{each.Key}].CivilNoContactOrderId" }));
					if (eachOop.CivilNoContactOrderTypeId == null && eachOop.CivilNoContactOrderId != null)
						results.Add(new ValidationResult("Civil No Contact Order Type required when there is a Civil No Contact Order.", new[] { $"OrdersOfProtectionById[{each.Key}].CivilNoContactOrderTypeId" }));
					if (eachOop.CivilNoContactOrderRequestId == null && eachOop.CivilNoContactOrderTypeId != null)
						results.Add(new ValidationResult("Civil No Contact Order Request required when Civil No Contact Order Type is selected.", new[] { $"OrdersOfProtectionById[{each.Key}].CivilNoContactOrderRequestId" }));
					if (eachOop.CivilNoContactOrderRequestId == null && eachOop.CivilNoContactOrderId != null)
						results.Add(new ValidationResult("Civil No Contact Order Request required when Civil No Contact Order is selected.", new[] { $"OrdersOfProtectionById[{each.Key}].CivilNoContactOrderRequestId" }));
				}
			}

			return results;
		}

		public void OnContextSavedChanges(EntityState prior) {
			OffendersById.RestorableKeys.Clear();
			ClientMDTById.RestorableKeys.Clear();
			AbuseNeglectPetitionsById.RestorableKeys.Clear();
			ClientCJProcessesById.RestorableKeys.Clear();
			ClientCourtAppearancesById.RestorableKeys.Clear();
			ClientPoliceProsecutionsById.RestorableKeys.Clear();
			DCFSAllegationsById.RestorableKeys.Clear();
			OrdersOfProtectionById.RestorableKeys.Clear();
			VictimSensitiveInterviewsById.RestorableKeys.Clear();
		}

		//KMS DO these need to go...
		[NotMapped]
		public bool IsNoneIncomeSourceSelected {
			get { return _isNoneIncomeSourceSelected; }
			set { _isNoneIncomeSourceSelected = value; }
		}

		[NotMapped]
		public bool IsUnknownIncomeSourceSelected {
			get { return _isUnknownIncomeSourceSelected; }
			set { _isUnknownIncomeSourceSelected = value; }
		}

		//KMS DO add a struct for this???
		public string CaseIdentifier {
			get { return ClientId + ":" + CaseId; }
		}

		#region predicates
		public static Expression<Func<ClientCase, bool>> AgeBetween(int? minAge, int? maxAge) {
			var predicate = PredicateBuilder.New<ClientCase>(true);
			if (minAge != null)
				predicate.And(c => c.Age >= minAge);
			if (maxAge != null)
				predicate.And(c => c.Age <= maxAge);
			return predicate;
		}

		public static Expression<Func<ClientCase, bool>> TotalAnnualIncomeBetween(decimal? minAmount, decimal? maxAmount) {
			var predicate = PredicateBuilder.New<ClientCase>(true);
			if (minAmount != null)
				predicate = predicate.And(cc => cc.FinancialResources.AsQueryable().Where(FinancialResource.HasAmount()).Sum(fr => fr.Amount) * 12 >= minAmount);
			if (maxAmount != null)
				predicate = predicate.And(cc => cc.FinancialResources.AsQueryable().Where(FinancialResource.HasAmount()).Sum(fr => fr.Amount) * 12 <= maxAmount);
			return predicate;

		}
		#endregion
	}
}