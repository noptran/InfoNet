using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Clients;

namespace Infonet.Data.Mapping.Clients {
	public class CancellationMap : EntityTypeConfiguration<Cancellation> {
		public CancellationMap() {
			// Primary Key
			HasKey(t => t.ID);

			// Properties
			// Table & Column Mappings
			ToTable("TL_Cancellations");
			Property(t => t.ID).HasColumnName("ID");
			Property(t => t.ClientID).HasColumnName("ClientID");
			Property(t => t.CaseID).HasColumnName("CaseID");
			Property(t => t.ServiceID).HasColumnName("ServiceID");
			Property(t => t.Date).HasColumnName("Date");
			Property(t => t.SVID).HasColumnName("SVID");
			Property(t => t.LocationID).HasColumnName("LocationID");
			Property(t => t.ReasonID).HasColumnName("ReasonID");

			// Relationships
			HasOptional(t => t.ClientCase)
				.WithMany(t => t.Cancellations)
				.HasForeignKey(d => new { d.ClientID, d.CaseID });
			HasRequired(t => t.StaffVolunteer)
				.WithMany(t => t.Cancellations)
				.HasForeignKey(d => d.SVID);
			HasOptional(t => t.TLU_Codes_ProgramsAndServices)
				.WithMany(t => t.Cancellations)
				.HasForeignKey(d => d.ServiceID);
		}
	}
}