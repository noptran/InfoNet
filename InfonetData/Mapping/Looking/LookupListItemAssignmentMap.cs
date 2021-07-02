using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Looking;

namespace Infonet.Data.Mapping.Looking {
	public class LookupListItemAssignmentMap : EntityTypeConfiguration<LookupListItemAssignment> {
		public LookupListItemAssignmentMap() {
			// Primary Key
			HasKey(t => t.Id);

			// Properties
			// Table & Column Mappings
			ToTable("LOOKUPLIST_ItemAssignment");
			Property(t => t.Id).HasColumnName("ID");
			Property(t => t.TableId).HasColumnName("TableID");
			Property(t => t.ProviderId).HasColumnName("ProviderID");
			Property(t => t.CodeId).HasColumnName("CodeID");
			Property(t => t.DisplayOrder).HasColumnName("DisplayOrder");
			Property(t => t.IsActive).HasColumnName("IsActive");

			// Relationships
			HasRequired(t => t.Table)
				.WithMany(t => t.ItemAssignments)
				.HasForeignKey(d => d.TableId);
		}
	}
}