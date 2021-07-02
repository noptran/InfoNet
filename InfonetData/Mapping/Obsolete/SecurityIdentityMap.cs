using System;
using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Obsolete;

namespace Infonet.Data.Mapping.Obsolete {
	[Obsolete]
	public class SecurityIdentityMap : EntityTypeConfiguration<SecurityIdentity> {
		public SecurityIdentityMap() {
			// Primary Key
			HasKey(t => t.IdentityId);

			// Properties
			Property(t => t.SigninName)
				.IsRequired()
				.HasMaxLength(50);

			Property(t => t.Pwd)
				.IsRequired()
				.HasMaxLength(50);

			// Table & Column Mappings
			ToTable("SecurityIdentities");
			Property(t => t.IdentityId).HasColumnName("IdentityID");
			Property(t => t.SigninName).HasColumnName("SigninName");
			Property(t => t.Pwd).HasColumnName("Pwd");
			Property(t => t.PrincipalId).HasColumnName("PrincipalID");

			// Relationships
			HasMany(t => t.SecurityRoles)
				.WithMany(t => t.SecurityIdentities)
				.Map(m => {
					m.ToTable("SecurityIdentitiesXSecurityRoles");
					m.MapLeftKey("IdentityID");
					m.MapRightKey("RoleID");
				});

			HasOptional(t => t.SecurityPrincipal)
				.WithMany(t => t.SecurityIdentities)
				.HasForeignKey(d => d.PrincipalId);
		}
	}
}