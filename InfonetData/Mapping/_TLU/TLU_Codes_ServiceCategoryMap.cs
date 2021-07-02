using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models._TLU;

namespace Infonet.Data.Mapping._TLU {
	public class TLU_Codes_ServiceCategoryMap : EntityTypeConfiguration<TLU_Codes_ServiceCategory> {
		public TLU_Codes_ServiceCategoryMap() {
			// Primary Key
			HasKey(t => t.CodeID);

			// Properties
			Property(t => t.Description)
				.HasMaxLength(50);

			// Table & Column Mappings
			ToTable("TLU_Codes_ServiceCategory");
			Property(t => t.CodeID).HasColumnName("CodeID");
			Property(t => t.Description).HasColumnName("Description");
		}
	}
}