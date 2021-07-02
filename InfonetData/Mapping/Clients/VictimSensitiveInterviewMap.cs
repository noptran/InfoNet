using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Clients;

namespace Infonet.Data.Mapping.Clients {
	public class VictimSensitiveInterviewMap : EntityTypeConfiguration<VictimSensitiveInterview> {
		public VictimSensitiveInterviewMap() {
			// Primary Key
			HasKey(t => t.VSI_ID);

			// Properties
			// Table & Column Mappings
			ToTable("Ts_VictimSensitiveInterviews");
			Property(t => t.VSI_ID).HasColumnName("VSI_ID");
			Property(t => t.ClientId).HasColumnName("ClientID");
			Property(t => t.CaseId).HasColumnName("CaseID");
			Property(t => t.InterviewDate).HasColumnName("InterviewDate");
			Property(t => t.InterviewerID).HasColumnName("InterviewerID");
			Property(t => t.LocationID).HasColumnName("LocationID");
			Property(t => t.SiteLocationId).HasColumnName("SiteLocationID");
			Property(t => t.RecordTypeID).HasColumnName("RecordTypeID");
			Property(t => t.CourtesyInterview).HasColumnName("CourtesyInterview");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");

			// Relationships
			HasOptional(t => t.Center)
				.WithMany(t => t.VictimSensitiveInterviews)
				.HasForeignKey(d => d.LocationID);
			HasRequired(t => t.ClientCase)
				.WithMany(t => t.VictimSensitiveInterviews)
				.HasForeignKey(d => new { d.ClientId, d.CaseId });
			HasOptional(t => t.Interviewer)
				.WithMany(t => t.VictimSensitiveInterview)
				.HasForeignKey(d => d.InterviewerID);
		}
	}
}