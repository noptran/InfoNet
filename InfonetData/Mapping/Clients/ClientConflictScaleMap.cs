using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Clients;

namespace Infonet.Data.Mapping.Clients {
	public class ClientConflictScaleMap : EntityTypeConfiguration<ClientConflictScale> {
		public ClientConflictScaleMap() {
			// Primary Key
			HasKey(t => new { t.ClientID, t.CaseID });

			// Properties
			Property(t => t.ClientID)
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

			Property(t => t.CaseID)
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

			// Table & Column Mappings
			ToTable("Ts_ClientConflictScale");
			Property(t => t.ClientID).HasColumnName("ClientID");
			Property(t => t.CaseID).HasColumnName("CaseID");
			Property(t => t.Threw).HasColumnName("Threw");
			Property(t => t.Pushed).HasColumnName("Pushed");
			Property(t => t.Slapped).HasColumnName("Slapped");
			Property(t => t.Kicked).HasColumnName("Kicked");
			Property(t => t.Hit).HasColumnName("Hit");
			Property(t => t.BeatUp).HasColumnName("BeatUp");
			Property(t => t.Choked).HasColumnName("Choked");
			Property(t => t.Threatened).HasColumnName("Threatened");
			Property(t => t.Used).HasColumnName("Used");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");

			// Relationships
			HasRequired(t => t.ClientCase)
				.WithOptional(t => t.ClientConflictScale);
		}
	}
}