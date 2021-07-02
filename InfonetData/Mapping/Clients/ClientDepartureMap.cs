using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Clients;

namespace Infonet.Data.Mapping.Clients {
	public class ClientDepartureMap : EntityTypeConfiguration<ClientDeparture> {
		public ClientDepartureMap() {
			// Primary Key
			HasKey(t => t.DepartureID);

			// Properties
			// Table & Column Mappings
			ToTable("Ts_ClientDeparture");
			Property(t => t.DepartureID).HasColumnName("DepartureID");
			Property(t => t.ClientID).HasColumnName("ClientID");
			Property(t => t.CaseID).HasColumnName("CaseID");
			Property(t => t.DestinationID).HasColumnName("DestinationID");
			Property(t => t.DestinationTenureID).HasColumnName("DestinationTenureID");
			Property(t => t.DestinationSubsidyID).HasColumnName("DestinationSubsidyID");
			Property(t => t.ReasonForLeavingID).HasColumnName("ReasonForLeavingID");
			Property(t => t.DepartureDate).HasColumnName("DepartureDate");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");

			// Relationships
			HasRequired(t => t.ClientCase)
				.WithMany(t => t.ClientDepartures)
				.HasForeignKey(d => new { d.ClientID, d.CaseID });
		}
	}
}