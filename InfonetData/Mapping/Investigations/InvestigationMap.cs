using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Investigations;

namespace Infonet.Data.Mapping.Investigations {
	public class InvestigationMap : EntityTypeConfiguration<Investigation> {
		public InvestigationMap() {
			// Primary Key
			HasKey(t => t.ID);

			// Properties
			Property(t => t.InvestigationID)
				.IsRequired()
				.HasMaxLength(50);

			// Table & Column Mappings
			ToTable("T_Investigations");
			Property(t => t.ID).HasColumnName("ID");
			Property(t => t.InvestigationID).HasColumnName("InvestigationID");
			Property(t => t.CenterID).HasColumnName("CenterID");
			Property(t => t.CreationDate).HasColumnName("CreationDate");

			// Relationships
			HasRequired(t => t.Center)
				.WithMany(t => t.Investigations)
				.HasForeignKey(d => d.CenterID);
		}
	}
}