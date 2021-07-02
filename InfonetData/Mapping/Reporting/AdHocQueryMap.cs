using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Reporting;

namespace Infonet.Data.Mapping.Reporting {
	public class AdHocQueryMap : EntityTypeConfiguration<AdHocQuery> {
		public AdHocQueryMap() {
			// Primary Key
			HasKey(t => t.Id);

			// Properties
			Property(t => t.Name).HasMaxLength(100);

			// Table & Column Mappings
			ToTable("RPT_AdHocQueries");
			Property(t => t.Id).HasColumnName("ID");
			Property(t => t.Name).HasColumnName("Name");
			Property(t => t.Json).HasColumnName("Json");
			Property(t => t.UserId).HasColumnName("UserID");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");
		}
	}
}