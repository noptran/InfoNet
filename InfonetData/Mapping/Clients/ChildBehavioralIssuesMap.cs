using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Clients;

namespace Infonet.Data.Mapping.Clients {
	public class ChildBehavioralIssuesMap : EntityTypeConfiguration<ChildBehavioralIssues> {
		public ChildBehavioralIssuesMap() {
			// Primary Key
			HasKey(t => new { t.ClientId, t.CaseId });

			// Properties
			Property(t => t.ClientId)
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

			Property(t => t.CaseId)
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

			// Table & Column Mappings
			ToTable("Ts_ClientChildBehavioralIssues");
			Property(t => t.ClientId).HasColumnName("ClientID");
			Property(t => t.CaseId).HasColumnName("CaseID");
			Property(t => t.Afraid).HasColumnName("Afraid");
			Property(t => t.CantLeave).HasColumnName("CantLeave");
			Property(t => t.Accepts).HasColumnName("Accepts");
			Property(t => t.Cries).HasColumnName("Cries");
			Property(t => t.Mood).HasColumnName("Mood");
			Property(t => t.NoInteract).HasColumnName("NoInteract");
			Property(t => t.Nightmares).HasColumnName("Nightmares");
			Property(t => t.HurtsSelf).HasColumnName("HurtsSelf");
			Property(t => t.Suicidal).HasColumnName("Suicidal");
			Property(t => t.BedWet).HasColumnName("BedWet");
			Property(t => t.Illnesses).HasColumnName("Illnesses");
			Property(t => t.Weight).HasColumnName("Weight");
			Property(t => t.MoreActive).HasColumnName("MoreActive");
			Property(t => t.SpecialClassActive).HasColumnName("SpecialClassActive");
			Property(t => t.AbuseDrugs).HasColumnName("AbuseDrugs");
			Property(t => t.AbuseAlcohol).HasColumnName("AbuseAlcohol");
			Property(t => t.Fire).HasColumnName("Fire");
			Property(t => t.RoleReversal).HasColumnName("RoleReversal");
			Property(t => t.Protective).HasColumnName("Protective");
			Property(t => t.Resists).HasColumnName("Resists");
			Property(t => t.Possessive).HasColumnName("Possessive");
			Property(t => t.HitsKicksBites).HasColumnName("HitsKicksBites");
			Property(t => t.BehavesYoung).HasColumnName("BehavesYoung");
			Property(t => t.HarmsAnimals).HasColumnName("HarmsAnimals");
			Property(t => t.MissSchool).HasColumnName("MissSchool");
			Property(t => t.DropOut).HasColumnName("DropOut");
			Property(t => t.SchoolRules).HasColumnName("SchoolRules");
			Property(t => t.BehaviorProblems).HasColumnName("BehaviorProblems");
			Property(t => t.SpecialClassBehavioral).HasColumnName("SpecClassBeh");
			Property(t => t.LearningProblems).HasColumnName("LearningProblems");
			Property(t => t.SpecialClassLearning).HasColumnName("SpecClassLearn");
			Property(t => t.NoneObserved_intake).HasColumnName("NoneObserved");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");
			Property(t => t.Afraid_depart).HasColumnName("Afraid_depart");
			Property(t => t.CantLeave_depart).HasColumnName("CantLeave_depart");
			Property(t => t.Accepts_depart).HasColumnName("Accepts_depart");
			Property(t => t.Cries_depart).HasColumnName("Cries_depart");
			Property(t => t.Mood_depart).HasColumnName("Mood_depart");
			Property(t => t.NoInteract_depart).HasColumnName("NoInteract_depart");
			Property(t => t.Nightmares_depart).HasColumnName("Nightmares_depart");
			Property(t => t.HurtsSelf_depart).HasColumnName("HurtsSelf_depart");
			Property(t => t.Suicidal_depart).HasColumnName("Suicidal_depart");
			Property(t => t.BedWet_depart).HasColumnName("BedWet_depart");
			Property(t => t.Illnesses_depart).HasColumnName("Illnesses_depart");
			Property(t => t.Weight_depart).HasColumnName("Weight_depart");
			Property(t => t.MoreActive_depart).HasColumnName("MoreActive_depart");
			Property(t => t.SpecialClassActive_depart).HasColumnName("SpecialClassActive_depart");
			Property(t => t.AbuseDrugs_depart).HasColumnName("AbuseDrugs_depart");
			Property(t => t.AbuseAlcohol_depart).HasColumnName("AbuseAlcohol_depart");
			Property(t => t.Fire_depart).HasColumnName("Fire_depart");
			Property(t => t.RoleReversal_depart).HasColumnName("RoleReversal_depart");
			Property(t => t.Protective_depart).HasColumnName("Protective_depart");
			Property(t => t.Resists_depart).HasColumnName("Resists_depart");
			Property(t => t.Possessive_depart).HasColumnName("Possessive_depart");
			Property(t => t.HitsKicksBites_depart).HasColumnName("HitsKicksBites_depart");
			Property(t => t.BehavesYoung_depart).HasColumnName("BehavesYoung_depart");
			Property(t => t.HarmsAnimals_depart).HasColumnName("HarmsAnimals_depart");
			Property(t => t.MissSchool_depart).HasColumnName("MissSchool_depart");
			Property(t => t.DropOut_depart).HasColumnName("DropOut_depart");
			Property(t => t.SchoolRules_depart).HasColumnName("SchoolRules_depart");
			Property(t => t.BehaviorProblems_depart).HasColumnName("BehaviorProblems_depart");
			Property(t => t.SpecialClassBehavioral_depart).HasColumnName("SpecClassBeh_depart");
			Property(t => t.LearningProblems_depart).HasColumnName("LearningProblems_depart");
			Property(t => t.SpecialClassLearning_depart).HasColumnName("SpecClassLearn_depart");
			Property(t => t.NoneObserved_depart).HasColumnName("NoneObserved_depart");

			// Relationships
			HasRequired(t => t.ClientCase)
				.WithOptional(t => t.ChildBehavioralIssues);
		}
	}
}