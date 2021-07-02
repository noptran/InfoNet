using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Services;
#pragma warning disable 612

namespace Infonet.Data.Mapping.Services {
	public class PublicationDetailMap : EntityTypeConfiguration<PublicationDetail> {
		public PublicationDetailMap() {
			// Primary Key
			HasKey(t => t.ICS_ID);

			// Properties
			Property(t => t.Title)
				.HasMaxLength(100);

			Property(t => t.Comment_Pub)
				.HasMaxLength(200);

			// Table & Column Mappings
			ToTable("Tl_PublicationDetail");
			Property(t => t.ICS_ID).HasColumnName("ICS_ID");
			Property(t => t.CenterID).HasColumnName("CenterID");
			Property(t => t.ProgramID).HasColumnName("ProgramID");
			Property(t => t.PDate).HasColumnName("PDate");
			Property(t => t.FundDateID).HasColumnName("FundDateID");
			Property(t => t.Title).HasColumnName("Title");
			Property(t => t.NumStaff).HasColumnName("NumStaff");
			Property(t => t.PrepareHours).HasColumnName("PrepareHours");
			Property(t => t.NumOfBrochure).HasColumnName("NumOfBrochure");
			Property(t => t.Comment_Pub).HasColumnName("Comment_Pub");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");

			// Relationships
			HasRequired(t => t.Center)
				.WithMany(t => t.PublicationDetails)
				.HasForeignKey(d => d.CenterID);
			HasRequired(t => t.TLU_Codes_ProgramsAndServices)
				.WithMany(t => t.PublicationDetails)
				.HasForeignKey(d => d.ProgramID);
			// ADDED RELATIONSHIP
			HasOptional(t => t.FundingDate)
				.WithMany(t => t.PublicationsDetail)
				.HasForeignKey(d => d.FundDateID);
		}
	}
}