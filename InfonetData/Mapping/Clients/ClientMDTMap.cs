using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Clients;

namespace Infonet.Data.Mapping.Clients {
	public class ClientMDTMap : EntityTypeConfiguration<ClientMDT> {
		public ClientMDTMap() {
			// Primary Key
			HasKey(t => t.MDT_ID);

			// Properties
			// Table & Column Mappings
			ToTable("Tl_ClientMDT");
			Property(t => t.MDT_ID).HasColumnName("MDT_ID");
			Property(t => t.ClientID).HasColumnName("ClientID");
			Property(t => t.CaseID).HasColumnName("CaseID");
			Property(t => t.ContactID).HasColumnName("ContactID");
			Property(t => t.AgencyID).HasColumnName("AgencyID");
			Property(t => t.PositionID).HasColumnName("PositionID");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");

			// Relationships
			HasRequired(t => t.ClientCase)
				.WithMany(t => t.ClientMDT)
				.HasForeignKey(d => new { d.ClientID, d.CaseID });
			HasRequired(t => t.Contact)
				.WithMany(t => t.ClientMDTs)
				.HasForeignKey(d => d.ContactID);
			HasRequired(t => t.Agency)
				.WithMany(t => t.ClientMDTs)
				.HasForeignKey(d => d.AgencyID);
		}
	}
}