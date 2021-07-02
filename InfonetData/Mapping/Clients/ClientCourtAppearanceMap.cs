using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Clients;

namespace Infonet.Data.Mapping.Clients {
	public class ClientCourtAppearanceMap : EntityTypeConfiguration<ClientCourtAppearance> {
		public ClientCourtAppearanceMap() {
			// Primary Key
			HasKey(t => t.ID);

			// Properties
			// Table & Column Mappings
			ToTable("Ts_ClientCourtAppearance");
			Property(t => t.ID).HasColumnName("ID");
			Property(t => t.ClientId).HasColumnName("ClientID");
			Property(t => t.CaseId).HasColumnName("CaseID");
			Property(t => t.CourtContinuanceID).HasColumnName("CourtContinuanceID");
			Property(t => t.CourtDate).HasColumnName("CourtDate");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");

			// Relationships
			HasRequired(t => t.ClientCase)
				.WithMany(t => t.ClientCourtAppearances)
				.HasForeignKey(d => new { d.ClientId, d.CaseId });
		}
	}
}