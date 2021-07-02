using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Clients;
#pragma warning disable 612

namespace Infonet.Data.Mapping.Clients {
	public class AbuseNeglectPetitionMap : EntityTypeConfiguration<AbuseNeglectPetition> {
		public AbuseNeglectPetitionMap() {
			// Primary Key
			HasKey(t => t.Id);

			// Properties
			// Table & Column Mappings
			ToTable("TS_AbuseNeglectPetitions");
			Property(t => t.Id).HasColumnName("ID");
			Property(t => t.ClientId).HasColumnName("ClientID");
			Property(t => t.CaseId).HasColumnName("CaseID");
			Property(t => t.AbuseNeglectPetitionId).HasColumnName("AbuseNeglectPetitionID");
			Property(t => t.PetitionDate).HasColumnName("AbuseNeglectPetitionDate");
			Property(t => t.AdjudicatedId).HasColumnName("AdjudicatedID");
			Property(t => t.AdjudicatedDate).HasColumnName("AdjudicatedDate");
			Property(t => t.DispositionId).HasColumnName("DispositionID");
			Property(t => t.DispositionDate).HasColumnName("DispositionDate");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");

			// Relationships
			HasRequired(t => t.ClientCase)
				.WithMany(t => t.AbuseNeglectPetitions)
				.HasForeignKey(d => new { d.ClientId, d.CaseId })
				.WillCascadeOnDelete();
		}
	}
}