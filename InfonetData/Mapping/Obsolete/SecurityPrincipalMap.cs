using System;
using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Obsolete;

namespace Infonet.Data.Mapping.Obsolete {
	[Obsolete]
	public class SecurityPrincipalMap : EntityTypeConfiguration<SecurityPrincipal> {
		public SecurityPrincipalMap() {
			// Primary Key
			HasKey(t => t.PrincipalId);

			// Properties
			Property(t => t.FirstName)
				.HasMaxLength(100);

			Property(t => t.LastName)
				.HasMaxLength(50);

			Property(t => t.Email)
				.HasMaxLength(50);

			// Table & Column Mappings
			ToTable("SecurityPrincipals");
			Property(t => t.PrincipalId).HasColumnName("PrincipalID");
			Property(t => t.FirstName).HasColumnName("FName");
			Property(t => t.LastName).HasColumnName("LName");
			Property(t => t.Email).HasColumnName("Email");
			Property(t => t.IsActive).HasColumnName("IsActive");

			// Relationships
			HasMany(t => t.SecurityPrivileges)
				.WithMany(t => t.SecurityPrincipals)
				.Map(m => {
					m.ToTable("SecurityPrincipalsXSecurityPrivileges");
					m.MapLeftKey("PrincipalID");
					m.MapRightKey("PrivilegeID");
				});

			HasMany(t => t.SecurityRoles)
				.WithMany(t => t.SecurityPrincipals)
				.Map(m => {
					m.ToTable("SecurityPrincipalsXSecurityRoles");
					m.MapLeftKey("PrincipalID");
					m.MapRightKey("RoleID");
				});
		}
	}
}