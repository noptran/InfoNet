using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Obsolete;

namespace Infonet.Data.Mapping.Obsolete {
	[Obsolete]
	public class SecurityPrincipalPropertiesMap : EntityTypeConfiguration<SecurityPrincipalProperties> {
		public SecurityPrincipalPropertiesMap() {
			// Primary Key
			HasKey(t => t.PrincipalId);

			// Properties
			Property(t => t.PrincipalId)
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

			Property(t => t.CenterName)
				.IsRequired()
				.HasMaxLength(100);

			// Table & Column Mappings
			ToTable("SECURITY_PrincipalProperties");
			Property(t => t.PrincipalId).HasColumnName("PrincipalID");
			Property(t => t.CenterId).HasColumnName("CenterID");
			Property(t => t.ProviderId).HasColumnName("ProviderID");
			Property(t => t.CenterName).HasColumnName("CenterName");

			// Relationships
			HasRequired(t => t.SecurityPrincipal)
				.WithOptional(t => t.Properties);
		}
	}
}