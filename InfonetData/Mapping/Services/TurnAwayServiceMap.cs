using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Services;
#pragma warning disable 612

namespace Infonet.Data.Mapping.Services {
	public class TurnAwayServiceMap : EntityTypeConfiguration<TurnAwayService> {
		public TurnAwayServiceMap() {
			// Primary Key
			HasKey(t => t.Id);

			// Properties
			// Table & Column Mappings
			ToTable("Ts_TurnAwayServices");
			Property(t => t.Id).HasColumnName("ID");
			Property(t => t.AdultsNo).HasColumnName("AdultsNo");
			Property(t => t.ChildrenNo).HasColumnName("ChildrenNo");
			Property(t => t.LocationId).HasColumnName("LocationID");
			Property(t => t.ReasonId).HasColumnName("ReasonID");
			Property(t => t.ReferralMadeId).HasColumnName("ReferralMadeID");
			Property(t => t.TurnAwayDate).HasColumnName("TurnAwayDate");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");

			// Relationships
			HasOptional(t => t.Center)
				.WithMany(t => t.TurnAwayService)
				.HasForeignKey(d => d.LocationId);
		}
	}
}