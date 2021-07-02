using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models._TLU;

namespace Infonet.Data.Mapping._TLU {
	public class TLU_Codes_FundingSourceMap : EntityTypeConfiguration<TLU_Codes_FundingSource> {
		public TLU_Codes_FundingSourceMap() {
			// Primary Key
			HasKey(t => t.CodeID);

			// Properties
			Property(t => t.Description)
				.HasMaxLength(80);

			// Table & Column Mappings
			ToTable("TLU_Codes_FundingSource");
			Property(t => t.CodeID).HasColumnName("CodeID");
			Property(t => t.Description).HasColumnName("Description");
			Property(t => t.CenterID).HasColumnName("CenterID");
			Property(t => t.ICADVAdmin).HasColumnName("ICADVAdmin");
			Property(t => t.ICASAAdmin).HasColumnName("ICASAAdmin");
			Property(t => t.BeginDate).HasColumnName("BeginDate");
			Property(t => t.EndDate).HasColumnName("EndDate");

			// Relationships
			HasOptional(t => t.Center)
				.WithMany(t => t.TLU_Codes_FundingSource)
				.HasForeignKey(d => d.CenterID);
		}
	}
}