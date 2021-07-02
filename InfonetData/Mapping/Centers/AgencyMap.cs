using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Centers;

namespace Infonet.Data.Mapping.Centers {
	public class AgencyMap : EntityTypeConfiguration<Agency> {
		public AgencyMap() {
			// Primary Key
			HasKey(t => t.AgencyID);

			// Properties
			Property(t => t.AgencyName)
				.HasMaxLength(100);

			// Table & Column Mappings
			ToTable("T_Agency");
			Property(t => t.AgencyID).HasColumnName("AgencyID");
			Property(t => t.AgencyName).HasColumnName("AgencyName");
			Property(t => t.CenterID).HasColumnName("CenterID");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");
		}
	}
}