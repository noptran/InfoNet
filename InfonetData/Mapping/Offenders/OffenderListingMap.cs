using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Offenders;

namespace Infonet.Data.Mapping.Offenders {
	public class OffenderListingMap : EntityTypeConfiguration<OffenderListing> {
		public OffenderListingMap() {
			// Primary Key
			HasKey(t => t.OffenderListingId);

			// Properties
			Property(t => t.OffenderCode)
				.IsRequired()
				.HasMaxLength(20);

			// Table & Column Mappings
			ToTable("T_OffenderList");
			Property(t => t.OffenderListingId).HasColumnName("OffenderID");
			Property(t => t.OffenderCode).HasColumnName("OffenderCode");
			Property(t => t.ParentCenterId).HasColumnName("ParentCenterID");
			Property(t => t.SexId).HasColumnName("SexID");
			Property(t => t.RaceId).HasColumnName("RaceID");
			Property(t => t.BirthYear).HasColumnName("BirthYear");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");
		}
	}
}