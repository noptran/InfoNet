using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Clients;

namespace Infonet.Data.Mapping.Clients {
	public class FinancialResourceMap : EntityTypeConfiguration<FinancialResource> {
		public FinancialResourceMap() {
			// Primary Key
			HasKey(t => t.ID);

			// Properties
			// Table & Column Mappings
			ToTable("Ts_ClientFinancialResources");
			Property(t => t.ID).HasColumnName("ID");
			Property(t => t.ClientID).HasColumnName("ClientID");
			Property(t => t.CaseID).HasColumnName("CaseID");
			Property(t => t.IncomeSource2ID).HasColumnName("IncomeID");
			Property(t => t.Amount).HasColumnName("Amount");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");

			// Relationships
			HasRequired(t => t.ClientCase)
				.WithMany(t => t.FinancialResources)
				.HasForeignKey(d => new { d.ClientID, d.CaseID });
			//this.HasOptional(t => t.TLU_Codes_IncomeSource2)
			//    .WithMany(t => t.Ts_ClientFinancialResources)
			//    .HasForeignKey(d => d.IncomeID);
		}
	}
}