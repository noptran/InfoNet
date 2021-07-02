using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Clients;

namespace Infonet.Data.Mapping.Clients {
	public class ClientRaceMap : EntityTypeConfiguration<ClientRace> {
		public ClientRaceMap() {
			// Primary Key
			HasKey(t => new { t.ClientId, t.RaceHudId });

			// Properties
			Property(t => t.ClientId)
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

			Property(t => t.RaceHudId)
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

			// Table & Column Mappings
			ToTable("Ts_ClientRace");
			Property(t => t.ClientId).HasColumnName("ClientID");
			Property(t => t.RaceHudId).HasColumnName("RaceID");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");

			// Relationships
			HasRequired(t => t.Client)
				.WithMany(t => t.ClientRaces)
				.HasForeignKey(d => d.ClientId);
		}
	}
}