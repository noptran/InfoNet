using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models._TLU;

namespace Infonet.Data.Mapping._TLU {
	[Obsolete]
	public class TLU_Codes_HivMentalSubstanceMap : EntityTypeConfiguration<TLU_Codes_HivMentalSubstance> {
		public TLU_Codes_HivMentalSubstanceMap() {
			// Primary Key
			HasKey(t => t.CodeID);

			// Properties
			Property(t => t.CodeID)
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

			Property(t => t.Description)
				.HasMaxLength(80);

			// Table & Column Mappings
			ToTable("TLU_Codes_HivMentalSubstance");
			Property(t => t.CodeID).HasColumnName("CodeID");
			Property(t => t.Description).HasColumnName("Description");
		}
	}
}