using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Clients;

namespace Infonet.Data.Mapping.Clients {
	public class OpActivityMap : EntityTypeConfiguration<OpActivity> {
		public OpActivityMap() {
			// Primary Key
			HasKey(t => t.OpActivityID);

			// Properties
			// Table & Column Mappings
			ToTable("Ts_OpActivity");
			Property(t => t.OpActivityID).HasColumnName("OpActivityID");
			Property(t => t.OP_ID).HasColumnName("OP_ID");
			Property(t => t.OpActivityCodeID).HasColumnName("OpActivityCodeID");
			Property(t => t.OpActivityDate).HasColumnName("OpActivityDate");
			Property(t => t.NewExpirationDate).HasColumnName("NewExpirationDate");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");

			// Relationships
			//this.HasOptional(t => t.TLU_Codes_OpActivity)
			//    .WithMany(t => t.OpActivity)
			//    .HasForeignKey(d => d.OpActivityCodeID);
			HasRequired(t => t.OrderOfProtection)
				.WithMany(t => t.OrderOfProtectionActivities)
				.HasForeignKey(d => d.OP_ID);
		}
	}
}