using System.ComponentModel.DataAnnotations;

namespace Infonet.Reporting.Filters.Enums {
	public enum FilterTypeEnum {
		Hidden,
		[Display(Name = "Staff")]
		Staff,
		[Display(Name = "Relationship to Client")]
		OffenderRelationship,
		[Display(Name = "Funding Source")]
		FundingSources,
		[Display(Name = "Shelter Client Type")]
		ShelterType,
		[Display(Name = "Gender")]
		Gender,
		[Display(Name = "Ethnicity")]
		Ethnicity,
		[Display(Name = "Race")]
		Race,
		[Display(Name = "City or Town")]
		CityOrTown,
		[Display(Name = "Township")]
		Township,
		[Display(Name = "County")]
		County,
		[Display(Name = "State")]
		State,
		[Display(Name = "Zip Code")]
		ZipCode,
		[Display(Name = "Age")]
		Age,
		[Display(Name = "Service")]
		Service,
		[Display(Name = "Agency")]
		Agency,
		[Display(Name = "Event Location")]
		EventLocation,
		[Display(Name = "Publication")]
		Publication,
		[Display(Name = "Reason for Cancellation")]
		CancellationReason,
		[Display(Name = "Primary Presenting Issue")]
		PrimaryPresentingIssue,
		[Display(Name = "Primary Offense Location")]
		PrimaryOffenseLoction,
		[Display(Name = "Offender Visitation Type")]
		OffenderVisitation,
		[Display(Name = "Offender State")]
		OffenderState,
		[Display(Name = "Offender County")]
		OffenderCounty,
		[Display(Name = "Yes/No")]
		YesNo,
		[Display(Name = "Yes/No")]
		YesNo2,
		[Display(Name = "Injury Severity")]
		InjurySeverity,
		[Display(Name = "Medical Treatment Location")]
		MedicalTreatmentLocation,
		[Display(Name = "Residence Type")]
		ResidenceType,
		[Display(Name = "Length Of Stay In Residence")]
		LengthOfStay,
        [Display(Name ="Income Source")]
        IncomeSource2,
		[Display(Name = "Appeal Status")]
		AppealStatus,
		[Display(Name = "Trial Type")]
		TrialType,
		[Display(Name = "VW Participate")]
		VWParticipate,
		[Display(Name = "OP Forum")]
		OpForum,
		[Display(Name = "OP Status")]
		OpStatus,
		[Display(Name = "OP Type")]
		OpType,
		[Display(Name = "OP Activity")]
		OpActivity,
		[Display(Name = "Court Continuance")]
		CourtContinuance,
		[Display(Name = "Destination")]
		Destination,
		[Display(Name = "Destination Tenure")]
		DestinationTenure,
		[Display(Name = "Destination Subsidy")]
		DestinationSubsidy,
		[Display(Name = "Reason For Leaving")]
		DestinationReason,
		[Display(Name = "Statute")]
		Statute,
		[Display(Name = "Charge Type")]
		ChargeType,
		[Display(Name = "Arrest Made")]
		ArrestMade,
		[Display(Name = "Charges Filed")]
		ChargesFiled,
		[Display(Name = "Disposition")]
		Disposition,
		[Display(Name = "Abuse Petition")]
		AbusePetition,
		[Display(Name = "Adjudicated")]
		Adjudicated,
		[Display(Name = "Contacts")]
		Contact,
		[Display(Name = "Position")]
		Position,
		[Display(Name = "Before Or After")]
		BeforeAfter,
		[Display(Name = "Medical Exam Finding")]
		MedicalExamFinding,
		[Display(Name = "Site Location")]
		SiteLocation,
		[Display(Name = "Medical Exam Type")]
		MedicalExamType,
		[Display(Name = "Response")]
		ReferralResponse,
		[Display(Name = "Referral Type")]
		ReferralType,
		[Display(Name = "DCFS Allegation")]
		AbuseAllegation,
		[Display(Name = "Abuse Allegation Finding")]
		AbuseAllegationFinding,
		[Display(Name = "Recording Type")]
		RecordingType,
		[Display(Name = "Observer Position")]
		ObserverPosition,
		[Display(Name = "Service Category")]
		ServiceCategory,
		[Display(Name = "Service Outcome")]
		ServiceOutcome,
		[Display(Name = "Event Type")]
		EventType,
		[Display(Name = "HIV/AIDS/Mental Substance Type")]
		HIVMentalType,
        [Display(Name = "Date")]
		Date = 98,
		[Display(Name = "Center")]
		Center = 99
	}
}