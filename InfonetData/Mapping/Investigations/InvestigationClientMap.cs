using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Investigations;

namespace Infonet.Data.Mapping.Investigations {
	public class InvestigationClientMap : EntityTypeConfiguration<InvestigationClient> {
		public InvestigationClientMap() {
			// Primary Key
			HasKey(t => t.ID);

			// Properties
			// Table & Column Mappings
			ToTable("TS_InvestigationClients");
			Property(t => t.ID).HasColumnName("ID");
			Property(t => t.T_CACInvestigations_FK).HasColumnName("T_CACInvestigations_FK");
			Property(t => t.ClientID).HasColumnName("ClientID");
			Property(t => t.CaseID).HasColumnName("CaseID");

			// Relationships
			HasRequired(t => t.ClientCase)
				.WithMany(t => t.InvestigationClients)
				.HasForeignKey(d => new { d.ClientID, d.CaseID });
			HasRequired(t => t.Investigation)
				.WithMany(t => t.InvestigationClient)
				.HasForeignKey(d => d.T_CACInvestigations_FK);
		}
	}
}