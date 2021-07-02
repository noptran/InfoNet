using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Clients;

namespace Infonet.Data.Mapping.Clients {
	public class PreviousServiceUseMap : EntityTypeConfiguration<PreviousServiceUse> {
		public PreviousServiceUseMap() {
			// Primary Key
			HasKey(t => new { t.ClientId, t.CaseId });

			// Properties
			Property(t => t.ClientId)
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

			Property(t => t.CaseId)
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

			// Table & Column Mappings
			ToTable("Ts_PreviousServiceUse");
			Property(t => t.ClientId).HasColumnName("ClientID");
			Property(t => t.CaseId).HasColumnName("CaseID");
			Property(t => t.PrevShelterUseId).HasColumnName("PrevShelterUse");
			Property(t => t.PrevShelterDate).HasColumnName("PrevShelterDate");
			Property(t => t.PrevServiceUseId).HasColumnName("PrevServiceUse");
			Property(t => t.PrevServiceDate).HasColumnName("PrevServiceDate");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");

			// Relationships
			HasRequired(t => t.ClientCase)
				.WithOptional(t => t.PreviousServiceUse);
		}
	}
}