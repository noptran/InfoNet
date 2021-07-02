using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Reporting;

namespace Infonet.Data.Mapping.Reporting {
	public class AdHocTrackingMap : EntityTypeConfiguration<AdHocTracking> {
		public AdHocTrackingMap() {
			// Primary Key
			HasKey(t => t.Id);

			// Table & Column Mappings
			ToTable("RPT_AdHocTracking");
			Property(t => t.Id).HasColumnName("ID");
			Property(t => t.Json).HasColumnName("Json");
			Property(t => t.OutputId).HasColumnName("OutputID");
			Property(t => t.UserId).HasColumnName("UserID");
			Property(t => t.RunDate).HasColumnName("RunDate");
			Property(t => t.QueryId).HasColumnName("QueryID");
		}
	}
}