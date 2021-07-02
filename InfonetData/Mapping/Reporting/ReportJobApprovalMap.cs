using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Reporting;

namespace Infonet.Data.Mapping.Reporting {
	public class ReportJobApprovalMap : EntityTypeConfiguration<ReportJobApproval> {
		public ReportJobApprovalMap() {
			HasKey(t => t.ReportJobId);

			Property(t => t.CenterIdsString).HasMaxLength(4000);
			Property(t => t.ApproverUserName).HasMaxLength(256);
			Property(t => t.RowVersion).IsRowVersion();

			ToTable("RPT_ReportJobApprovals");
			Property(t => t.ReportJobId).HasColumnName("ReportJobID");
			Property(t => t.CenterIdsString).HasColumnName("CenterIDs");
			Property(t => t.StatusId).HasColumnName("StatusID");
			Property(t => t.StatusDate).HasColumnName("StatusDate");
			Property(t => t.ApproverUserName).HasColumnName("ApproverUserName");
			Property(t => t.ApproverComment).HasColumnName("ApproverComment");
			Property(t => t.SystemMessageId).HasColumnName("SystemMessageID");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");
			Property(t => t.RowVersion).HasColumnName("RowVersion");

			HasRequired(t => t.Job)
				.WithOptional(t => t.Approval);

			HasRequired(t => t.SystemMessage)
				.WithMany()
				.HasForeignKey(d => d.SystemMessageId);
		}
	}
}