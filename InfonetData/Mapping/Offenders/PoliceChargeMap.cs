using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Offenders;
#pragma warning disable 612

namespace Infonet.Data.Mapping.Offenders {
	public class PoliceChargeMap : EntityTypeConfiguration<PoliceCharge> {
		public PoliceChargeMap() {
			// Primary Key
			HasKey(t => t.PoliceChargeId);

			// Properties
			// Table & Column Mappings
			ToTable("Ts_PoliceCharges");
			Property(t => t.PoliceChargeId).HasColumnName("PC_ID");
			Property(t => t.OffenderId).HasColumnName("OffenderRecordID");
			Property(t => t.ArrestDate).HasColumnName("ArrestDate");
			Property(t => t.StatuteId).HasColumnName("PoliceChargeID");
			Property(t => t.ChargeDate).HasColumnName("ChargeDate");
			Property(t => t.ChargeTypeId).HasColumnName("ChargeTypeID");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");
			Property(t => t.AR_Id).HasColumnName("AR_ID");
			Property(t => t.ChargeCounts).HasColumnName("ChargeCounts");
			Property(t => t.ArrestMadeId).HasColumnName("ArrestMadeID");

			// Relationships
			HasRequired(t => t.Offender)
				.WithMany(t => t.PoliceCharges)
				.HasForeignKey(d => d.OffenderId)
				.WillCascadeOnDelete();
		}
	}
}