using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Investigations;

namespace Infonet.Data.Mapping.Investigations {
	public class InvestigationHouseHoldMap : EntityTypeConfiguration<InvestigationHouseHold> {
		public InvestigationHouseHoldMap() {
			// Primary Key
			HasKey(t => t.ID);

			// Properties
			// Table & Column Mappings
			ToTable("Ts_InvestigationHouseHolds");
			Property(t => t.ID).HasColumnName("ID");
			Property(t => t.T_CACInvestigations_FK).HasColumnName("T_CACInvestigations_FK");
			Property(t => t.TS_CACInvestigationClients_FK).HasColumnName("TS_CACInvestigationClients_FK");
			Property(t => t.HouseHoldID).HasColumnName("HouseHoldID");

			// Relationships
			HasRequired(t => t.Investigation)
				.WithMany(t => t.InvestigationHouseHold)
				.HasForeignKey(d => d.T_CACInvestigations_FK);
			HasRequired(t => t.InvestigationClient)
				.WithMany(t => t.Households)
				.HasForeignKey(t => t.TS_CACInvestigationClients_FK);
		}
	}
}