using System;
using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Obsolete;

namespace Infonet.Data.Mapping.Obsolete {
	[Obsolete]
	public class SecurityRoleMap : EntityTypeConfiguration<SecurityRole> {
		public SecurityRoleMap() {
			// Primary Key
			HasKey(t => t.RoleId);

			// Properties
			Property(t => t.Code)
				.IsRequired()
				.HasMaxLength(50);

			Property(t => t.Description)
				.IsRequired()
				.HasMaxLength(100);

			// Table & Column Mappings
			ToTable("SecurityRoles");
			Property(t => t.RoleId).HasColumnName("RoleID");
			Property(t => t.Code).HasColumnName("Code");
			Property(t => t.Description).HasColumnName("Description");
		}
	}
}