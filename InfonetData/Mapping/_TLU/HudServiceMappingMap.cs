using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models._TLU;

namespace Infonet.Data.Mapping._TLU {
	public class HudServiceMappingMap : EntityTypeConfiguration<HudServiceMapping> {
		public HudServiceMappingMap() {
			HasKey(t => new { t.ServiceProgramId, t.HudServiceId });
			Property(t => t.ServiceProgramId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
			Property(t => t.HudServiceId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

			ToTable("Tl_InfoNetHUDServiceMapping");
			Property(t => t.ServiceProgramId).HasColumnName("InfoNetSvcID");
			Property(t => t.HudServiceId).HasColumnName("HUDSvcID");
		}
	}
}