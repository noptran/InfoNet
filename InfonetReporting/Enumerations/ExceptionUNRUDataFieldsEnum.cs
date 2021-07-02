using System.ComponentModel.DataAnnotations;

namespace Infonet.Reporting.Enumerations {
	public enum UNRUDataFieldsEnum {
		[Display(Name = "@Sex", ShortName = "Sex")]
		Sex,
		[Display(Name = "@Sex", ShortName = "Gender Identity")]
		Gender,
		[Display(Name = "@Race", ShortName = "Race")]
		Race,
		[Display(Name = "@Age", ShortName = "Age")]
		Age,
		[Display(Name = "@Employment", ShortName = "Employment")]
		Employment,
		[Display(Name = "@Education", ShortName = "Education")]
		Education,
		[Display(Name = "@HealthInsuance", ShortName = "Health Insurance")]
		HealthInsurance,
		[Display(Name = "@MaritalStatus", ShortName = "Marital Status")]
		MaritalStatus,
		[Display(Name = "@Pregnant", ShortName = "Pregnant")]
		Pregnant,
		[Display(Name = "@LivesWith", ShortName = "Lives With")]
		LivesWith,
		[Display(Name = "@Custody", ShortName = "Custody")]
		Custody,
		[Display(Name = "@School", ShortName = "School")]
		School,
		[Display(Name = "@DCFSHotlineDate", ShortName = "DCFS Hotline Date")]
		DCFSHotlineDate,
		[Display(Name = "@DCFSServiceDate", ShortName = "DCFS Service Date")]
		DCFSServiceDate,
		[Display(Name = "@PoliceReportDate", ShortName = "Police Report Date")]
		PoliceReportDate,
		[Display(Name = "@DCFSOpen", ShortName = "DCFS Open")]
		DCFSOpen,
		[Display(Name = "@DCFSInvestigation", ShortName = "DCFS Investigation")]
		DCFSInvestigation,
		[Display(Name = "@CollegeUnivStudent", ShortName = "College Univ Student")]
		CollegeUnivStudent,
		[Display(Name = "@NumChildren", ShortName = "Number Of Children")]
		NumberOfChildren,
		[Display(Name = "@SignificantOtherOf", ShortName = "Significant Other Of")]
		SignifigantOtherOf,
		[Display(Name = "@InvestigationType", ShortName = "Investigation Type")]
		InvestigationType,
		[Display(Name = "@PrimaryPresentingIssue", ShortName = "Primary Presenting Issue")]
		PrimaryPresentingIssue,
		[Display(Name = "@LocationOfPrimaryOffense", ShortName = "Location Of Primary Offense")]
		LocationOfPrimaryOffense,
		[Display(Name = "@ReferralSource", ShortName = "Referral Source")]
		ReferralSourceDV,
		[Display(Name = "@ReferralSource", ShortName = "Referred By")]
		ReferralSourceSA,
		[Display(Name = "@Income", ShortName = "Income")]
		Income,
		[Display(Name = "@PrimaryIncome", ShortName = "Primary Income")]
		PrimaryIncome,
		[Display(Name = "@SpecialNeeds", ShortName = "Special Needs")]
		SpecialNeeds,
		[Display(Name = "@ServiceNeeds", ShortName = "Service Needs")]
		ServiceNeeds,
		[Display(Name = "@ServiceReceived", ShortName = "Service Received")]
		ServiceReceived,
		[Display(Name = "@ChildBehaviors", ShortName = "Child Behaviors")]
		ChildBehaviors,
		[Display(Name = "@Residence", ShortName = "Residence")]
		Residence,
		[Display(Name = "@ReferredFrom", ShortName = "Referred From")]
		ReferredFrom,
		[Display(Name = "@RelationshipToVictim", ShortName = "Relationship To Victim")]
		RelationshipToVictim,
	}
}
