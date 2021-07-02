using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Services;
#pragma warning disable 612

namespace Infonet.Data.Mapping.Services {
	public class ProgramDetailMap : EntityTypeConfiguration<ProgramDetail> {
		public ProgramDetailMap() {
			// Primary Key
			HasKey(t => t.ICS_ID);

			// Properties
			Property(t => t.Comment_Act)
				.HasMaxLength(200);

			Property(t => t.AgencyName)
				.HasMaxLength(100);

			Property(t => t.Location)
				.HasMaxLength(100);

			// Table & Column Mappings
			ToTable("Tl_ProgramDetail");
			Property(t => t.ICS_ID).HasColumnName("ICS_ID");
			Property(t => t.CenterID).HasColumnName("CenterID");
			Property(t => t.ProgramID).HasColumnName("ProgramID");
			Property(t => t.NumOfSession).HasColumnName("NumOfSession");
			Property(t => t.Hours).HasColumnName("Hours");
			Property(t => t.ParticipantsNum).HasColumnName("ParticipantsNum");
			Property(t => t.PDate).HasColumnName("PDate");
			Property(t => t.ConductStaffNum).HasColumnName("ConductStaffNum");
			Property(t => t.Comment_Act).HasColumnName("Comment_Act");
			Property(t => t.FundDateID).HasColumnName("FundDateID");
			Property(t => t.ChildSpecific).HasColumnName("ChildSpecific");
			Property(t => t.AgencyName).HasColumnName("Agency");
			Property(t => t.Location).HasColumnName("Location");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");
			Property(t => t.AgencyID).HasColumnName("AgencyID");
			Property(t => t.Agency_ICS_ID).HasColumnName("Agency_ICS_ID");
			Property(t => t.CountyID).HasColumnName("CountyID");
			Property(t => t.StateID).HasColumnName("StateID");

			// Relationships
			HasOptional(t => t.Agency)
				.WithMany(t => t.ProgramDetails)
				.HasForeignKey(d => d.AgencyID);
			HasRequired(t => t.Center)
				.WithMany(t => t.ProgramDetails)
				.HasForeignKey(d => d.CenterID);
			HasRequired(t => t.TLU_Codes_ProgramsAndServices)
				.WithMany(t => t.ProgramDetails)
				.HasForeignKey(d => d.ProgramID);
			// ADDED RELATIONSHIP
			HasOptional(t => t.FundingDate)
				.WithMany(t => t.ProgramsDetail)
				.HasForeignKey(d => d.FundDateID);
		}
	}
}