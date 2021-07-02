using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Services;

namespace Infonet.Data.Mapping.Services {
	public class ServiceOutcomeMap : EntityTypeConfiguration<ServiceOutcome> {
		public ServiceOutcomeMap() {
			// Primary Key
			HasKey(t => t.ID);

			// Properties
			// Table & Column Mappings
			ToTable("Ts_ServiceOutcome");
			Property(t => t.ID).HasColumnName("ID");
			Property(t => t.LocationID).HasColumnName("LocationID");
			Property(t => t.OutcomeDate).HasColumnName("OutcomeDate");
			Property(t => t.ServiceID).HasColumnName("ServiceID");
			Property(t => t.OutcomeID).HasColumnName("OutcomeID");
			Property(t => t.ResponseYes).HasColumnName("ResponseYes");
			Property(t => t.ResponseNo).HasColumnName("ResponseNo");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");

			// Relationships
			HasOptional(t => t.Center)
				.WithMany(t => t.ServiceOutcomes)
				.HasForeignKey(d => d.LocationID);
			HasOptional(t => t.TLU_Codes_ServiceCategory)
				.WithMany(t => t.ServiceOutcome)
				.HasForeignKey(d => d.ServiceID);
			HasOptional(t => t.TLU_Codes_ServiceOutcome)
				.WithMany(t => t.ServiceOutcome)
				.HasForeignKey(d => d.OutcomeID);
		}
	}
}