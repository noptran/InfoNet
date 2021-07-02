using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Centers;

namespace Infonet.Data.Mapping.Centers {
	public class CenterAdminFundingSourcesMap : EntityTypeConfiguration<CenterAdminFundingSources> {
		public CenterAdminFundingSourcesMap() {
			// Primary Key
			HasKey(t => new { t.ID, t.CenterAdminID, t.CodeID });

			// Properties
			Property(t => t.ID)
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

			Property(t => t.CenterAdminID)
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

			Property(t => t.CodeID)
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

			// Table & Column Mappings
			ToTable("TS_CenterAdminFundingSources");
			Property(t => t.ID).HasColumnName("ID");
			Property(t => t.CenterAdminID).HasColumnName("CenterAdminID");
			Property(t => t.CodeID).HasColumnName("CodeID");

			// Relationships
			HasRequired(t => t.TLU_Codes_FundingSource)
				.WithMany(t => t.CenterAdminFundingSources)
				.HasForeignKey(d => d.CodeID);
		}
	}
}