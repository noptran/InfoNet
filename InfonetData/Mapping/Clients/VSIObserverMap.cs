using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Clients;

namespace Infonet.Data.Mapping.Clients {
	public class VSIObserverMap : EntityTypeConfiguration<VSIObserver> {
		public VSIObserverMap() {
			// Primary Key
			HasKey(t => t.ID);

			// Properties
			// Table & Column Mappings
			ToTable("Ts_VSIObservers");
			Property(t => t.ID).HasColumnName("ID");
			Property(t => t.AgencyID).HasColumnName("AgencyID");
			Property(t => t.ContactID).HasColumnName("ContactID");
			Property(t => t.ObserverID).HasColumnName("ObserverID");
			Property(t => t.VSI_ID).HasColumnName("VSI_ID");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");

			// Relationships
			HasOptional(t => t.Agency)
				.WithMany(t => t.VSIObservers)
				.HasForeignKey(d => d.AgencyID);
			HasOptional(t => t.Contact)
				.WithMany(t => t.VSIObservers)
				.HasForeignKey(d => d.ContactID);
			//this.HasOptional(t => t.TLU_Codes_Observer)
			//    .WithMany(t => t.VSIObservers)
			//    .HasForeignKey(d => d.ObserverID);
			HasOptional(t => t.VictimSensitiveInterview)
				.WithMany(t => t.VSIObservers)
				.HasForeignKey(d => d.VSI_ID);
		}
	}
}