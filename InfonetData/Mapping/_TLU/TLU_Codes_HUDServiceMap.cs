using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models._TLU;

namespace Infonet.Data.Mapping._TLU {
	public class TLU_Codes_HUDServiceMap : EntityTypeConfiguration<TLU_Codes_HUDService> {
		public TLU_Codes_HUDServiceMap() {
			// Primary Key
			HasKey(t => t.CodeId);

			// Properties
			Property(t => t.CodeId)
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

			Property(t => t.Description)
				.HasMaxLength(80);

			// Table & Column Mappings
			ToTable("TLU_Codes_HUDService");
			Property(t => t.CodeId).HasColumnName("CodeID");
			Property(t => t.Description).HasColumnName("Description");
		}
	}
}