using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Centers;

namespace Infonet.Data.Mapping.Centers {
	public class FundingDateMap : EntityTypeConfiguration<FundingDate> {
		public FundingDateMap() {
			// Primary Key
			HasKey(t => t.FundDateID);

			// Properties
			// Table & Column Mappings
			ToTable("T_FundingDates");
			Property(t => t.FundDateID).HasColumnName("FundDateID");
			Property(t => t.CenterID).HasColumnName("CenterID");
			Property(t => t.Date).HasColumnName("FundingDate");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");

			// Relationships
			HasRequired(t => t.Center)
				.WithMany(t => t.FundingDates)
				.HasForeignKey(d => d.CenterID);
		}
	}
}