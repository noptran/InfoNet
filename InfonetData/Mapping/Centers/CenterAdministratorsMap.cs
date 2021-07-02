using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Centers;

namespace Infonet.Data.Mapping.Centers {
	public class CenterAdministratorsMap : EntityTypeConfiguration<CenterAdministrators> {
		public CenterAdministratorsMap() {
			// Primary Key
			HasKey(t => t.Id);

			// Properties
			// Table & Column Mappings
			ToTable("TS_CenterAdministrators");
			Property(t => t.Id).HasColumnName("ID");
			Property(t => t.CenterId).HasColumnName("CenterID");
			Property(t => t.CenterAdminId).HasColumnName("CenterAdminID");
			Property(t => t.CenterAdminActive).HasColumnName("CenterAdminActive");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");

			// Relationships
			HasRequired(t => t.Center)
				.WithMany(t => t.Administrators)
				.HasForeignKey(d => d.CenterId);
		}
	}
}