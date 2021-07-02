using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Clients;

namespace Infonet.Data.Mapping.Clients {
	public class ClientMap : EntityTypeConfiguration<Client> {
		public ClientMap() {
			// Primary Key
			HasKey(t => t.ClientId);

			// Properties
			Property(t => t.ClientCode).HasMaxLength(50);

			// Table & Column Mappings
			ToTable("T_Client");
			Property(t => t.ClientId).HasColumnName("ClientID");
			Property(t => t.CenterId).HasColumnName("CenterID");
			Property(t => t.ClientCode).HasColumnName("ClientCode");
			Property(t => t.GenderIdentityId).HasColumnName("SexID");
			Property(t => t.RaceId).HasColumnName("RaceID");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");
			Property(t => t.ClientTypeId).HasColumnName("TypeID"); //KMS DO rename property back to TypeId?
			Property(t => t.EthnicityId).HasColumnName("EthnicityID");

			// Relationships
			HasRequired(t => t.Center)
				.WithMany(t => t.Clients)
				.HasForeignKey(d => d.CenterId);
		}
	}
}