using System.ComponentModel.DataAnnotations;

namespace Infonet.Reporting.Enumerations {
	public enum ChildBehaviorsEnum {
		[Display(Name = "Is often afraid")]
		Afraid = 1,
		[Display(Name = "Can't leave parent")]
		CantLeave,
		[Display(Name = "Accepts w/o question")]
		Accepts,
		[Display(Name = "Cries often")]
		Cries,
		[Display(Name = "Mood swings")]
		Mood,
		[Display(Name = "Little interaction")]
		NoInteract,
		[Display(Name = "Nightmares")]
		Nightmares,
		[Display(Name = "Hurts self")]
		HurtsSelf,
		[Display(Name = "Suicidal")]
		Suicidal,
		[Display(Name = "Bed wets")]
		BedWet,
		[Display(Name = "Illness often")]
		Illnesses,
		[Display(Name = "Weight problem")]
		Weight,
		[Display(Name = "More active")]
		MoreActive,
		[Display(Name = "Special class active")]
		SpecialClassActive,
		[Display(Name = "Abuses drugs")]
		AbuseDrugs,
		[Display(Name = "Abuses alcohol")]
		AbuseAlcohol,
		[Display(Name = "Plays with fire")]
		Fire,
		[Display(Name = "Role reversal")]
		RoleReversal,
		[Display(Name = "Protective")]
		Protective,
		[Display(Name = "Resists guidance")]
		Resists,
		[Display(Name = "Possessive")]
		Possessive,
		[Display(Name = "Hits, kicks, bites")]
		HitsKicksBites,
		[Display(Name = "Behaves young")]
		BehavesYoung,
		[Display(Name = "Harms animals")]
		HarmsAnimals,
		[Display(Name = "Misses school")]
		MissSchool,
		[Display(Name = "Drop out")]
		DropOut,
		[Display(Name = "Disobeys the rules")]
		SchoolRules,
		[Display(Name = "Behavior problems")]
		BehaviorProblems,
		[Display(Name = "Special class behavioral problems")]
		SpecClassBeh,
		[Display(Name = "Learning problems")]
		LearningProblems,
		[Display(Name = "Special class learning problems")]
		SpecClassLearn,
	}
}