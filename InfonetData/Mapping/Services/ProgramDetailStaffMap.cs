using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Services;

namespace Infonet.Data.Mapping.Services {
	public class ProgramDetailStaffMap : EntityTypeConfiguration<ProgramDetailStaff> {
		public ProgramDetailStaffMap() {
			// Primary Key
			HasKey(t => t.ICS_Staff_ID);

			// Properties
			// Table & Column Mappings
			ToTable("Ts_ProgramDetail_Staffs");
			Property(t => t.ICS_Staff_ID).HasColumnName("ICS_Staff_ID");
			Property(t => t.ICS_ID).HasColumnName("ICS_ID");
			Property(t => t.SVID).HasColumnName("SVID");
			Property(t => t.ConductHours).HasColumnName("ConductHours");
			Property(t => t.HoursPrep).HasColumnName("HoursPrep");
			Property(t => t.HoursTravel).HasColumnName("HoursTravel");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");

			// Relationships
			HasRequired(t => t.StaffVolunteer)
				.WithMany(t => t.ProgramDetailStaff)
				.HasForeignKey(d => d.SVID);
			HasRequired(t => t.ProgramDetail)
				.WithMany(t => t.ProgramDetailStaff)
				.HasForeignKey(d => d.ICS_ID)
				.WillCascadeOnDelete(true);
		}
	}
}