using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models._TLU;

namespace Infonet.Data.Mapping._TLU {
	public class TLU_Codes_ProgramsAndServicesMap : EntityTypeConfiguration<TLU_Codes_ProgramsAndServices> {
		public TLU_Codes_ProgramsAndServicesMap() {
			// Primary Key
			HasKey(t => t.CodeID);

			// Properties
			Property(t => t.Code)
				.IsFixedLength()
				.HasMaxLength(1);

			Property(t => t.Description)
				.IsRequired()
				.HasMaxLength(80);

			// Table & Column Mappings
			ToTable("TLU_Codes_ProgramsAndServices");
			Property(t => t.CodeID).HasColumnName("CodeID");
			Property(t => t.Code).HasColumnName("Code");
			Property(t => t.Description).HasColumnName("Description");
			Property(t => t.FedClass).HasColumnName("FedClass");
			Property(t => t.StandardClass).HasColumnName("StandardClass");
			Property(t => t.ShowCancellation).HasColumnName("ShowCancellation");
			Property(t => t.IsCommInst).HasColumnName("IsCommInst");
			Property(t => t.IsService).HasColumnName("IsService");
			Property(t => t.IsGroupService).HasColumnName("IsGroupService");
			Property(t => t.IsHotline).HasColumnName("IsHotline");
			Property(t => t.IsPublication).HasColumnName("IsPublication");
			Property(t => t.IsEvent).HasColumnName("IsEvent");
			Property(t => t.IsShelter).HasColumnName("IsShelter");
		}
	}
}