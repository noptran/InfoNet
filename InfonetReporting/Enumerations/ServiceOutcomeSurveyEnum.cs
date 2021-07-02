using System.ComponentModel.DataAnnotations;

namespace Infonet.Reporting.Enumerations {
	public enum ServiceOutcomeSurveyEnum {
		Shelter = 1,
		OtherSupportiveServices = 2,
		SupportGroups = 3,
		Counseling = 4,
		LegalAdvocacy = 5,
		ChildrensServices = 6,
		All = 999
	}

	public enum ServiceOutcomeQuestionsEnum {
		[Display(Name = "Community resources", Order = 2)]
		CommunityResources = 1,
		[Display(Name = "Safety planning", Order = 1)]
		SafetyPlanning,
		[Display(Name = "Safer from abuse", Order = 3)]
		SaferFromAbuse,
		[Display(Name = "Hopeful future", Order = 4)]
		HopefulFuture,
		[Display(Name = "Abuse effects on life", Order = 5)]
		AbuseEffectsOnLife,
		[Display(Name = "Abuse effects on children", Order = 6)]
		AbuseEffectsOnChildren,
		[Display(Name = "Legal rights as DV victim", Order = 7)]
		LegalRightsAsDVVictim,
		[Display(Name = "Report OP violations", Order = 8)]
		ReportOPViolations,
		[Display(Name = "Support myself and children", Order = 9)]
		SupportMyselfAndChildren,
		[Display(Name = "Abuse not my fault", Order = 10)]
		AbuseNotMyFault,
		[Display(Name = "Two things to feel safe", Order = 11)]
		TwoThingsToFeelSafe
	}
}