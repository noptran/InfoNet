using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Services;

namespace Infonet.Data.Mapping.Services {
	public class HivMentalSubstanceMap : EntityTypeConfiguration<HivMentalSubstance> {
		public HivMentalSubstanceMap() {
			// Primary Key
			HasKey(t => t.ID);

			// Properties
			// Table & Column Mappings
			ToTable("Ts_HivMentalSubstance");
			Property(t => t.ID).HasColumnName("ID");
			Property(t => t.LocationID).HasColumnName("LocationID");
			Property(t => t.TypeID).HasColumnName("TypeID");
			Property(t => t.HMSDate).HasColumnName("HMSDate");
			Property(t => t.AdultsNo).HasColumnName("AdultsNo");
			Property(t => t.ChildrenNo).HasColumnName("ChildrenNo");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");

			// Relationships
			HasRequired(t => t.Center)
				.WithMany(t => t.HivMentalSubstance)
				.HasForeignKey(d => d.LocationID);
			HasRequired(t => t.TLU_Codes_HivMentalSubstance)
				.WithMany(t => t.HivMentalSubstance)
				.HasForeignKey(d => d.TypeID);
		}
	}
}