using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Offenders;

namespace Infonet.Data.Mapping.Offenders {
	public class SentenceMap : EntityTypeConfiguration<Sentence> {
		public SentenceMap() {
			// Primary Key
			HasKey(t => t.Id);

			// Properties
			// Table & Column Mappings
			ToTable("Ts_Sentences");
			Property(t => t.Id).HasColumnName("ID");
			Property(t => t.TrialChargeId).HasColumnName("TC_ID");
			Property(t => t.SentenceId).HasColumnName("SentenceID");
			Property(t => t.SentenceDate).HasColumnName("SentenceDate");
			Property(t => t.YearsSentenced).HasColumnName("YearsSentenced");
			Property(t => t.MonthsSentenced).HasColumnName("MonthsSentenced");
			Property(t => t.DaysSentenced).HasColumnName("DaysSentenced");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");

			// Relationships
			HasRequired(t => t.TrialCharge)
				.WithMany(t => t.Sentences)
				.HasForeignKey(d => d.TrialChargeId)
				.WillCascadeOnDelete();
		}
	}
}