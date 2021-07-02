using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Infonet.Core.Entity;
using Infonet.Core.Entity.Binding;

namespace Infonet.Data.Models.Clients {
	[BindHint(Include = "Afraid,CantLeave,Accepts,Cries,Mood,NoInteract,Nightmares,HurtsSelf,Suicidal,BedWet,Illnesses,Weight,MoreActive,SpecialClassActive,AbuseDrugs,AbuseAlcohol,Fire,RoleReversal,Protective,Resists,Possessive,HitsKicksBites,BehavesYoung,HarmsAnimals,MissSchool,DropOut,SchoolRules,BehaviorProblems,SpecialClassBehavioral,LearningProblems,SpecialClassLearning,NoneObserved,AfraidAtOuttake,CantLeaveAtOuttake,AcceptsAtOuttake,CriesAtOuttake,MoodAtOuttake,NoInteractAtOuttake,NightmaresAtOuttake,HurtsSelfAtOuttake,SuicidalAtOuttake,BedWetAtOuttake,IllnessesAtOuttake,WeightAtOuttake,MoreActiveAtOuttake,SpecialClassActiveAtOuttake,AbuseDrugsAtOuttake,AbuseAlcoholAtOuttake,FireAtOuttake,RoleReversalAtOuttake,ProtectiveAtOuttake,ResistsAtOuttake,PossessiveAtOuttake,HitsKicksBitesAtOuttake,BehavesYoungAtOuttake,HarmsAnimalsAtOuttake,MissSchoolAtOuttake,DropOutAtOuttake,SchoolRulesAtOuttake,BehaviorProblemsAtOuttake,SpecialClassBehavioralAtOuttake,LearningProblemsAtOuttake,SpecialClassLearningAtOuttake,NoneObservedAtOuttake")]
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public class ChildBehavioralIssues : IRevisable {
		public int ClientId { get; set; }

		public int CaseId { get; set; }

		[Display(Name = "Is often afraid")]
		public bool Afraid { get; set; }

		[Display(Name = "Can't leave parent")]
		public bool CantLeave { get; set; }

		[Display(Name = "Accepts w/o question")]
		public bool Accepts { get; set; }

		[Display(Name = "Cries often")]
		public bool Cries { get; set; }

		[Display(Name = "Mood swings")]
		public bool Mood { get; set; }

		[Display(Name = "Little interaction")]
		public bool NoInteract { get; set; }

		[Display(Name = "Nightmares")]
		public bool Nightmares { get; set; }

		[Display(Name = "Hurts self")]
		public bool HurtsSelf { get; set; }

		[Display(Name = "Suicidal")]
		public bool Suicidal { get; set; }

		[Display(Name = "Bed wets")]
		public bool BedWet { get; set; }

		[Display(Name = "Illness often")]
		public bool Illnesses { get; set; }

		[Display(Name = "Weight problem")]
		public bool Weight { get; set; }

		[Display(Name = "More active")]
		public bool MoreActive { get; set; }

		[Display(Name = "Special class active")]
		public bool SpecialClassActive { get; set; }

		[Display(Name = "Abuses drugs")]
		public bool AbuseDrugs { get; set; }

		[Display(Name = "Abuses alcohol")]
		public bool AbuseAlcohol { get; set; }

		[Display(Name = "Plays with fire")]
		public bool Fire { get; set; }

		[Display(Name = "Role reversal")]
		public bool RoleReversal { get; set; }

		[Display(Name = "Protective")]
		public bool Protective { get; set; }

		[Display(Name = "Resists guidance")]
		public bool Resists { get; set; }

		[Display(Name = "Possessive")]
		public bool Possessive { get; set; }

		[Display(Name = "Hits, kicks, bites")]
		public bool HitsKicksBites { get; set; }

		[Display(Name = "Behaves young")]
		public bool BehavesYoung { get; set; }

		[Display(Name = "Harms animals")]
		public bool HarmsAnimals { get; set; }

		[Display(Name = "Misses school")]
		public bool MissSchool { get; set; }

		[Display(Name = "Drop out")]
		public bool DropOut { get; set; }

		[Display(Name = "Disobeys the rules")]
		public bool SchoolRules { get; set; }

		[Display(Name = "Behavior problems")]
		public bool BehaviorProblems { get; set; }

		[Display(Name = "Special class behavioral problems")]
		public bool SpecialClassBehavioral { get; set; }

		[Display(Name = "Learning problems")]
		public bool LearningProblems { get; set; }

		[Display(Name = "Special class learning problems")]
		public bool SpecialClassLearning { get; set; }

		[NotMapped]
		[Display(Name = "In")]
		public bool NoneObserved {
			get { return NoneObserved_intake ?? false; }
			set { NoneObserved_intake = value; }
		}
		
		public bool? NoneObserved_intake { get; set; }

		public DateTime? RevisionStamp { get; set; }

		[NotMapped]
		public bool AfraidAtOuttake {
			get { return Afraid_depart ?? false; }
			set { Afraid_depart = value; }
		}

		public bool? Afraid_depart { get; set; }

		[NotMapped]
		public bool CantLeaveAtOuttake {
			get { return CantLeave_depart ?? false; }
			set { CantLeave_depart = value; }
		}

		public bool? CantLeave_depart { get; set; }

		[NotMapped]
		public bool AcceptsAtOuttake {
			get { return Accepts_depart ?? false; }
			set { Accepts_depart = value; }
		}

		public bool? Accepts_depart { get; set; }

		[NotMapped]
		public bool CriesAtOuttake {
			get { return Cries_depart ?? false; }
			set { Cries_depart = value; }
		}

		public bool? Cries_depart { get; set; }

		[NotMapped]
		public bool MoodAtOuttake {
			get { return Mood_depart ?? false; }
			set { Mood_depart = value; }
		}

		public bool? Mood_depart { get; set; }

		[NotMapped]
		public bool NoInteractAtOuttake {
			get { return NoInteract_depart ?? false; }
			set { NoInteract_depart = value; }
		}

		public bool? NoInteract_depart { get; set; }

		[NotMapped]
		public bool NightmaresAtOuttake {
			get { return Nightmares_depart ?? false; }
			set { Nightmares_depart = value; }
		}

		public bool? Nightmares_depart { get; set; }

		[NotMapped]
		public bool HurtsSelfAtOuttake {
			get { return HurtsSelf_depart ?? false; }
			set { HurtsSelf_depart = value; }
		}

		public bool? HurtsSelf_depart { get; set; }

		[NotMapped]
		public bool SuicidalAtOuttake {
			get { return Suicidal_depart ?? false; }
			set { Suicidal_depart = value; }
		}

		public bool? Suicidal_depart { get; set; }

		[NotMapped]
		public bool BedWetAtOuttake {
			get { return BedWet_depart ?? false; }
			set { BedWet_depart = value; }
		}

		public bool? BedWet_depart { get; set; }

		[NotMapped]
		public bool IllnessesAtOuttake {
			get { return Illnesses_depart ?? false; }
			set { Illnesses_depart = value; }
		}

		public bool? Illnesses_depart { get; set; }

		[NotMapped]
		public bool WeightAtOuttake {
			get { return Weight_depart ?? false; }
			set { Weight_depart = value; }
		}

		public bool? Weight_depart { get; set; }

		[NotMapped]
		public bool MoreActiveAtOuttake {
			get { return MoreActive_depart ?? false; }
			set { MoreActive_depart = value; }
		}

		public bool? MoreActive_depart { get; set; }

		[NotMapped]
		public bool SpecialClassActiveAtOuttake {
			get { return SpecialClassActive_depart ?? false; }
			set { SpecialClassActive_depart = value; }
		}

		public bool? SpecialClassActive_depart { get; set; }

		[NotMapped]
		public bool AbuseDrugsAtOuttake {
			get { return AbuseDrugs_depart ?? false; }
			set { AbuseDrugs_depart = value; }
		}

		public bool? AbuseDrugs_depart { get; set; }

		[NotMapped]
		public bool AbuseAlcoholAtOuttake {
			get { return AbuseAlcohol_depart ?? false; }
			set { AbuseAlcohol_depart = value; }
		}

		public bool? AbuseAlcohol_depart { get; set; }

		[NotMapped]
		public bool FireAtOuttake {
			get { return Fire_depart ?? false; }
			set { Fire_depart = value; }
		}

		public bool? Fire_depart { get; set; }

		[NotMapped]
		public bool RoleReversalAtOuttake {
			get { return RoleReversal_depart ?? false; }
			set { RoleReversal_depart = value; }
		}

		public bool? RoleReversal_depart { get; set; }

		[NotMapped]
		public bool ProtectiveAtOuttake {
			get { return Protective_depart ?? false; }
			set { Protective_depart = value; }
		}

		public bool? Protective_depart { get; set; }

		[NotMapped]
		public bool ResistsAtOuttake {
			get { return Resists_depart ?? false; }
			set { Resists_depart = value; }
		}

		public bool? Resists_depart { get; set; }

		[NotMapped]
		public bool PossessiveAtOuttake {
			get { return Possessive_depart ?? false; }
			set { Possessive_depart = value; }
		}

		public bool? Possessive_depart { get; set; }

		[NotMapped]
		public bool HitsKicksBitesAtOuttake {
			get { return HitsKicksBites_depart ?? false; }
			set { HitsKicksBites_depart = value; }
		}

		public bool? HitsKicksBites_depart { get; set; }

		[NotMapped]
		public bool BehavesYoungAtOuttake {
			get { return BehavesYoung_depart ?? false; }
			set { BehavesYoung_depart = value; }
		}

		public bool? BehavesYoung_depart { get; set; }

		[NotMapped]
		public bool HarmsAnimalsAtOuttake {
			get { return HarmsAnimals_depart ?? false; }
			set { HarmsAnimals_depart = value; }
		}

		public bool? HarmsAnimals_depart { get; set; }

		[NotMapped]
		public bool MissSchoolAtOuttake {
			get { return MissSchool_depart ?? false; }
			set { MissSchool_depart = value; }
		}

		public bool? MissSchool_depart { get; set; }

		[NotMapped]
		public bool DropOutAtOuttake {
			get { return DropOut_depart ?? false; }
			set { DropOut_depart = value; }
		}

		public bool? DropOut_depart { get; set; }

		[NotMapped]
		public bool SchoolRulesAtOuttake {
			get { return SchoolRules_depart ?? false; }
			set { SchoolRules_depart = value; }
		}

		public bool? SchoolRules_depart { get; set; }

		[NotMapped]
		public bool BehaviorProblemsAtOuttake {
			get { return BehaviorProblems_depart ?? false; }
			set { BehaviorProblems_depart = value; }
		}

		public bool? BehaviorProblems_depart { get; set; }

		[NotMapped]
		public bool SpecialClassBehavioralAtOuttake {
			get { return SpecialClassBehavioral_depart ?? false; }
			set { SpecialClassBehavioral_depart = value; }
		}

		public bool? SpecialClassBehavioral_depart { get; set; }

		[NotMapped]
		public bool LearningProblemsAtOuttake {
			get { return LearningProblems_depart ?? false; }
			set { LearningProblems_depart = value; }
		}

		public bool? LearningProblems_depart { get; set; }

		[NotMapped]
		public bool SpecialClassLearningAtOuttake {
			get { return SpecialClassLearning_depart ?? false; }
			set { SpecialClassLearning_depart = value; }
		}

		public bool? SpecialClassLearning_depart { get; set; }

		[NotMapped]
		[Display(Name = "Out")]
		public bool NoneObservedAtOuttake {
			get { return NoneObserved_depart ?? false; }
			set { NoneObserved_depart = value; }
		}

		public bool? NoneObserved_depart { get; set; }

		public virtual ClientCase ClientCase { get; set; }
	}
}