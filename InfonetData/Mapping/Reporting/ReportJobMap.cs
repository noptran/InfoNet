using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Reporting;

namespace Infonet.Data.Mapping.Reporting {
	public class ReportJobMap : EntityTypeConfiguration<ReportJob> {
		public ReportJobMap() {
			HasKey(t => t.Id);

			Property(t => t.ActiveThread).HasMaxLength(500);
			Property(t => t.SubmitterUserName).HasMaxLength(256);
			Property(t => t.RowVersion).IsRowVersion();

			ToTable("RPT_ReportJobs");
			Property(t => t.Id).HasColumnName("ID");
			Property(t => t.SpecificationJson).HasColumnName("SpecificationJson");
			Property(t => t.StatusId).HasColumnName("StatusID");
			Property(t => t.StatusDate).HasColumnName("StatusDate");
			Property(t => t.StatusLog).HasColumnName("StatusLog");
			Property(t => t.ScheduledForDate).HasColumnName("ScheduledForDate");
			Property(t => t.ExpirationDate).HasColumnName("ExpirationDate");
			Property(t => t.Priority).HasColumnName("Priority");
			Property(t => t.RemainingTries).HasColumnName("RemainingTries");
			Property(t => t.ActiveThread).HasColumnName("ActiveThread");
			Property(t => t.SubmittedDate).HasColumnName("SubmittedDate");
			Property(t => t.SubmitterCenterId).HasColumnName("SubmitterCenterID");
			Property(t => t.SubmitterUserName).HasColumnName("SubmitterUserName");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");
			Property(t => t.RowVersion).HasColumnName("RowVersion");

			HasRequired(t => t.SubmitterCenter)
				.WithMany()
				.HasForeignKey(t => t.SubmitterCenterId);
		}
	}
}