using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Centers;

namespace Infonet.Data.Mapping.Centers {
	public class FundServiceProgramOfStaffMap : EntityTypeConfiguration<FundServiceProgramOfStaff> {
		public FundServiceProgramOfStaffMap() {
			// Primary Key
			HasKey(t => new { t.FundDateID, t.SVID, t.ServiceProgramID, t.FundingSourceID });

			// Properties
			Property(t => t.FundDateID)
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

			Property(t => t.SVID)
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

			Property(t => t.ServiceProgramID)
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

			Property(t => t.FundingSourceID)
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

			Property(t => t.ID)
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

			// Table & Column Mappings
			ToTable("Tl_FundServiceProgramOfStaffs");
			Property(t => t.FundDateID).HasColumnName("FundDateID");
			Property(t => t.SVID).HasColumnName("SVID");
			Property(t => t.ServiceProgramID).HasColumnName("ServiceProgramID");
			Property(t => t.FundingSourceID).HasColumnName("FundingSourceID");
			Property(t => t.PercentFund).HasColumnName("PercentFund");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");
			Property(t => t.ID).HasColumnName("ID");

			// Relationships
			HasRequired(t => t.FundingDate)
				.WithMany(t => t.FundServiceProgramsOfStaff)
				.HasForeignKey(d => d.FundDateID);
			HasRequired(t => t.StaffVolunteer)
				.WithMany(t => t.FundServiceProgramsOfStaff)
				.HasForeignKey(d => d.SVID);
			HasRequired(t => t.TLU_Codes_FundingSource)
				.WithMany(t => t.FundServiceProgramsOfStaff)
				.HasForeignKey(d => d.FundingSourceID);
			HasRequired(t => t.TLU_Codes_ProgramsAndServices)
				.WithMany(t => t.FundServiceProgramsOfStaff)
				.HasForeignKey(d => d.ServiceProgramID);
		}
	}
}