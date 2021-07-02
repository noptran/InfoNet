using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Services;
#pragma warning disable 612

namespace Infonet.Data.Mapping.Services {
	public class OtherStaffActivityMap : EntityTypeConfiguration<OtherStaffActivity> {
		public OtherStaffActivityMap() {
			// Primary Key
			HasKey(t => t.OsaID);

			// Properties
			Property(t => t.ServiceType)
				.IsFixedLength()
				.HasMaxLength(1);

			// Table & Column Mappings
			ToTable("Ts_OtherStaffActivity");
			Property(t => t.OsaID).HasColumnName("OsaID");
			Property(t => t.SVID).HasColumnName("SVID");
			Property(t => t.OtherStaffActivityID).HasColumnName("OtherStaffActivityID");
			Property(t => t.ConductingHours).HasColumnName("ConductingHours");
			Property(t => t.TravelHours).HasColumnName("TravelHours");
			Property(t => t.PrepareHours).HasColumnName("PrepareHours");
			Property(t => t.OsaDate).HasColumnName("OsaDate");
			Property(t => t.ServiceType).HasColumnName("ServiceType");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");

			// Relationships
			HasRequired(t => t.StaffVolunteer)
				.WithMany(t => t.OtherStaffActivities)
				.HasForeignKey(d => d.SVID);
			HasOptional(t => t.TLU_Codes_OtherStaffActivity)
				.WithMany(t => t.OtherStaffActivities)
				.HasForeignKey(d => d.OtherStaffActivityID);
		}
	}
}