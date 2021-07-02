using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models._TLU;

namespace Infonet.Data.Mapping._TLU {
	public class TLU_Codes_OtherStaffActivityMap : EntityTypeConfiguration<TLU_Codes_OtherStaffActivity> {
		public TLU_Codes_OtherStaffActivityMap() {
			// Primary Key
			HasKey(t => t.CodeID);

			// Properties
			Property(t => t.Description)
				.HasMaxLength(80);

			// Table & Column Mappings
			ToTable("TLU_Codes_OtherStaffActivity");
			Property(t => t.CodeID).HasColumnName("CodeID");
			Property(t => t.Description).HasColumnName("Description");
			Property(t => t.CenterID).HasColumnName("CenterID");

			// Relationships
			HasOptional(t => t.Center)
				.WithMany(t => t.TLU_Codes_OtherStaffActivity)
				.HasForeignKey(d => d.CenterID);
		}
	}
}