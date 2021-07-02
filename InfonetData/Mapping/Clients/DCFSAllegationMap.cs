using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Clients;
#pragma warning disable 612

namespace Infonet.Data.Mapping.Clients {
	public class DCFSAllegationMap : EntityTypeConfiguration<DCFSAllegation> {
		public DCFSAllegationMap() {
			// Primary Key
			HasKey(t => t.Id);

			// Properties
			// Table & Column Mappings
			ToTable("Ts_DCFSAllegations");
			Property(t => t.Id).HasColumnName("ID");
			Property(t => t.ClientId).HasColumnName("ClientID");
			Property(t => t.CaseId).HasColumnName("CaseID");
			Property(t => t.AbuseAllegationId).HasColumnName("AbuseAllegationID");
			Property(t => t.FindingId).HasColumnName("FindingID");
			Property(t => t.FindingDate).HasColumnName("FindingDate");
			Property(t => t.Indicated).HasColumnName("Indicated");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");

			// Relationships
			HasRequired(t => t.ClientCase)
				.WithMany(t => t.DCFSAllegations)
				.HasForeignKey(d => new { d.ClientId, d.CaseId })
				.WillCascadeOnDelete();
		}
	}
}