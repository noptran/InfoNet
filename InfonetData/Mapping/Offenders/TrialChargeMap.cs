using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Offenders;
#pragma warning disable 612

namespace Infonet.Data.Mapping.Offenders {
	public class TrialChargeMap : EntityTypeConfiguration<TrialCharge> {
		public TrialChargeMap() {
			// Primary Key
			HasKey(t => t.TrialChargeId);

			// Properties
			// Table & Column Mappings
			ToTable("Ts_TrialCharges");
			Property(t => t.TrialChargeId).HasColumnName("TC_ID");
			Property(t => t.StatuteId).HasColumnName("TrialChargeID");
			Property(t => t.OffenderId).HasColumnName("OffenderRecordID");
			Property(t => t.ChargeDate).HasColumnName("ChargeDate");
			Property(t => t.Sentence1Id).HasColumnName("Sentence1ID");
			Property(t => t.SentenceDate).HasColumnName("SentenceDate");
			Property(t => t.Sentence2Id).HasColumnName("Sentence2ID");
			Property(t => t.Sentence3Id).HasColumnName("Sentence3ID");
			Property(t => t.DispositionId).HasColumnName("DispositionID");
			Property(t => t.DispositionDate).HasColumnName("DispositionDate");
			Property(t => t.YearsSentenced).HasColumnName("YearsSentenced");
			Property(t => t.MonthsSentenced).HasColumnName("MonthsSentenced");
			Property(t => t.DaysSentenced).HasColumnName("DaysSentenced");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");
			Property(t => t.ChargeCounts).HasColumnName("ChargeCounts");
			Property(t => t.ChargesFiledId).HasColumnName("ChargesFiledID");
			Property(t => t.ChargeTypeId).HasColumnName("ChargeTypeID");

			// Relationships
			HasRequired(t => t.Offender)
				.WithMany(t => t.TrialCharges)
				.HasForeignKey(d => d.OffenderId)
				.WillCascadeOnDelete();
		}
	}
}