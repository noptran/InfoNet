using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Services;

namespace Infonet.Data.Mapping.Services {
	public class PublicationDetailStaffMap : EntityTypeConfiguration<PublicationDetailStaff> {
		public PublicationDetailStaffMap() {
			// Primary Key
			HasKey(t => t.ICS_Staff_ID);

			// Properties
			// Table & Column Mappings
			ToTable("Ts_PublicationDetail_Staffs");
			Property(t => t.ICS_Staff_ID).HasColumnName("ICS_Staff_ID");
			Property(t => t.ICS_ID).HasColumnName("ICS_ID");
			Property(t => t.SVID).HasColumnName("SVID");
			Property(t => t.HoursPrep).HasColumnName("HoursPrep");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");

			// Relationships
			HasRequired(t => t.StaffVolunteer)
				.WithMany(t => t.PublicationDetailStaff)
				.HasForeignKey(d => d.SVID);
			HasRequired(t => t.PublicationDetail)
				.WithMany(t => t.PublicationDetailStaff)
				.HasForeignKey(d => d.ICS_ID);
		}
	}
}