using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Looking;

namespace Infonet.Data.Mapping.Looking {
	public class LookupListTableMap : EntityTypeConfiguration<LookupListTable> {
		public LookupListTableMap() {
			// Primary Key
			HasKey(t => t.TableId);

			// Properties
			Property(t => t.TableName)
				.IsRequired()
				.HasMaxLength(50);

			Property(t => t.Description)
				.HasMaxLength(255);

			Property(t => t.DisplayName)
				.HasMaxLength(50);

			// Table & Column Mappings
			ToTable("LOOKUPLIST_Tables");
			Property(t => t.TableId).HasColumnName("TableID");
			Property(t => t.TableName).HasColumnName("TableName");
			Property(t => t.Description).HasColumnName("Description");
			Property(t => t.DisplayName).HasColumnName("DisplayName");
		}
	}
}