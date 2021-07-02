using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Offenders;

namespace Infonet.Data.Mapping.Offenders {
	public class OffenderMap : EntityTypeConfiguration<Offender> {
		public OffenderMap() {
			// Primary Key
			HasKey(t => t.OffenderId);

			// Properties
			// Table & Column Mappings
			ToTable("T_Offender");
			Property(t => t.OffenderId).HasColumnName("OffenderRecordID");
			Property(t => t.ClientId).HasColumnName("ClientID");
			Property(t => t.CaseId).HasColumnName("CaseID");
			Property(t => t.SexId).HasColumnName("SexID");
			Property(t => t.RaceId).HasColumnName("RaceID");
			Property(t => t.CountyId).HasColumnName("CountyID");
			Property(t => t.RelationshipToClientId).HasColumnName("RelationshiptoClientID");
			Property(t => t.Age).HasColumnName("Age");
			Property(t => t.VisitationId).HasColumnName("VisitationID");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");
			Property(t => t.OffenderListingId).HasColumnName("OffenderID");
			Property(t => t.StateId).HasColumnName("StateID");
			Property(t => t.RegisteredId).HasColumnName("RegisteredID");

			// Relationships
			HasRequired(t => t.ClientCase)
				.WithMany(t => t.Offenders)
				.HasForeignKey(d => new { d.ClientId, d.CaseId });
			HasOptional(t => t.OffenderListing)
				.WithMany(t => t.Offender)
				.HasForeignKey(d => d.OffenderListingId);
		}
	}
}