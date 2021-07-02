using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Services;

namespace Infonet.Data.Mapping.Services {
	public class EventDetailStaffMap : EntityTypeConfiguration<EventDetailStaff> {
		public EventDetailStaffMap() {
			// Primary Key
			HasKey(t => t.ICS_Staff_ID);

			// Properties
			// Table & Column Mappings
			ToTable("Ts_EventDetail_Staffs");
			Property(t => t.ICS_Staff_ID).HasColumnName("ICS_Staff_ID");
			Property(t => t.ICS_ID).HasColumnName("ICS_ID");
			Property(t => t.SVID).HasColumnName("SVID");
			Property(t => t.HoursConduct).HasColumnName("HoursConduct");
			Property(t => t.HoursPrep).HasColumnName("HoursPrep");
			Property(t => t.HoursTravel).HasColumnName("HoursTravel");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");

			// Relationships
			HasRequired(t => t.StaffVolunteer)
				.WithMany(t => t.EventDetailStaff)
				.HasForeignKey(d => d.SVID);
			HasRequired(t => t.EventDetail)
				.WithMany(t => t.EventDetailStaff)
				.HasForeignKey(d => d.ICS_ID)
				.WillCascadeOnDelete(true);
		}
	}
}