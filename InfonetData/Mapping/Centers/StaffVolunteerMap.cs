using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Centers;
#pragma warning disable 612

namespace Infonet.Data.Mapping.Centers {
	public class StaffVolunteerMap : EntityTypeConfiguration<StaffVolunteer> {
		public StaffVolunteerMap() {
			// Primary Key
			HasKey(t => t.SvId);

			// Properties
			Property(t => t.SocSec)
				.HasMaxLength(11);

			Property(t => t.LastName)
				.IsRequired()
				.HasMaxLength(50);

			Property(t => t.FirstName)
				.IsRequired()
				.HasMaxLength(50);

			Property(t => t.PersonnelType)
				.HasMaxLength(50);

			Property(t => t.Title)
				.HasMaxLength(50);

			Property(t => t.Greeting)
				.HasMaxLength(50);

			Property(t => t.Department)
				.HasMaxLength(50);

			Property(t => t.Address)
				.HasMaxLength(50);

			Property(t => t.City)
				.HasMaxLength(50);

			Property(t => t.ZipCode)
				.HasMaxLength(10);

			Property(t => t.WorkPhone)
				.HasMaxLength(50);

			Property(t => t.HomePhone)
				.HasMaxLength(50);

			Property(t => t.Email)
				.HasMaxLength(50);

			Property(t => t.EmerContact)
				.HasMaxLength(100);

			Property(t => t.Type)
				.IsRequired()
				.HasMaxLength(1);

			// Table & Column Mappings
			ToTable("T_StaffVolunteer");
			Property(t => t.SvId).HasColumnName("SVID");
			Property(t => t.CenterId).HasColumnName("CenterID");
			Property(t => t.SocSec).HasColumnName("SocSec");
			Property(t => t.LastName).HasColumnName("LastName");
			Property(t => t.FirstName).HasColumnName("FirstName");
			Property(t => t.SexId).HasColumnName("SexID");
			Property(t => t.RaceId).HasColumnName("RaceID");
			Property(t => t.BirthDate).HasColumnName("BirthDate");
			Property(t => t.PersonnelTypeId).HasColumnName("PersonnelTypeID");
			Property(t => t.PersonnelType).HasColumnName("PersonnelType");
			Property(t => t.Title).HasColumnName("Title");
			Property(t => t.CollegeUnivStudent).HasColumnName("CollegeUnivStudent");
			Property(t => t.Greeting).HasColumnName("Greeting");
			Property(t => t.Department).HasColumnName("Department");
			Property(t => t.Address).HasColumnName("Address");
			Property(t => t.City).HasColumnName("City");
			Property(t => t.StateID).HasColumnName("StateID");
			Property(t => t.ZipCode).HasColumnName("ZipCode");
			Property(t => t.WorkPhone).HasColumnName("WorkPhone");
			Property(t => t.HomePhone).HasColumnName("HomePhone");
			Property(t => t.Email).HasColumnName("Email");
			Property(t => t.EmerContact).HasColumnName("EmerContact");
			Property(t => t.StartDate).HasColumnName("StartDate");
			Property(t => t.TerminationDate).HasColumnName("TerminationDate");
			Property(t => t.Type).HasColumnName("Type");
			Property(t => t.SupervisorId).HasColumnName("SupervisorID");
			Property(t => t.NumInGroup).HasColumnName("NumInGroup");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");
			Property(t => t.TypeId).HasColumnName("TypeID");

			// Relationships
			HasRequired(t => t.Center)
				.WithMany(t => t.StaffVolunteers)
				.HasForeignKey(d => d.CenterId);
			HasOptional(t => t.Supervisor)
				.WithMany(t => t.Volunteers)
				.HasForeignKey(d => d.SupervisorId);
		}
	}
}