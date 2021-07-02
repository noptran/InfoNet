using System;
using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Obsolete;

namespace Infonet.Data.Mapping.Obsolete {
	[Obsolete]
	public class SecurityPrivilegeMap : EntityTypeConfiguration<SecurityPrivilege> {
		public SecurityPrivilegeMap() {
			// Primary Key
			HasKey(t => t.PrivilegeId);

			// Properties
			Property(t => t.Code)
				.IsRequired()
				.HasMaxLength(50);

			Property(t => t.Description)
				.IsRequired()
				.HasMaxLength(100);

			// Table & Column Mappings
			ToTable("SecurityPrivileges");
			Property(t => t.PrivilegeId).HasColumnName("PrivilegeID");
			Property(t => t.Code).HasColumnName("Code");
			Property(t => t.Description).HasColumnName("Description");

			// Relationships
			HasMany(t => t.SecurityRoles)
				.WithMany(t => t.SecurityPrivileges)
				.Map(m => {
					m.ToTable("SecurityRolesXSecurityPrivileges");
					m.MapLeftKey("PrivilegeID");
					m.MapRightKey("RoleID");
				});
		}
	}
}