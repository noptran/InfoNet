using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Clients;
#pragma warning disable 612

namespace Infonet.Data.Mapping.Clients {
	public class ClientIncomeMap : EntityTypeConfiguration<ClientIncome> {
		public ClientIncomeMap() {
			// Primary Key
			HasKey(t => new { t.ClientId, t.CaseId });

			// Properties
			Property(t => t.ClientId)
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

			Property(t => t.CaseId)
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

			Property(t => t.WhatOther)
				.HasMaxLength(50);

			Property(t => t.OtherPrimaryIncome)
				.HasMaxLength(100);

			// Table & Column Mappings
			ToTable("Ts_ClientIncome");
			Property(t => t.ClientId).HasColumnName("ClientID");
			Property(t => t.CaseId).HasColumnName("CaseID");
			Property(t => t.PrimaryIncomeId).HasColumnName("PrimaryIncomeID");
			Property(t => t.AFDC).HasColumnName("AFDC");
			Property(t => t.Unknown).HasColumnName("Unknown");
			Property(t => t.GeneralAssistance).HasColumnName("GeneralAssistance");
			Property(t => t.SocialSecurity).HasColumnName("SocialSecurity");
			Property(t => t.SSI).HasColumnName("SSI");
			Property(t => t.AlimonyChildSupport).HasColumnName("AlimonyChildSupport");
			Property(t => t.Employment).HasColumnName("Employment");
			Property(t => t.OtherIncome).HasColumnName("OtherIncome");
			Property(t => t.WhatOther).HasColumnName("WhatOther");
			Property(t => t.OtherPrimaryIncome).HasColumnName("OtherPrimaryIncome");
			Property(t => t.AmountOfPrimaryIncome).HasColumnName("AmountOfPrimaryIncome");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");

			// Relationships
			HasRequired(t => t.ClientCase)
				.WithOptional(t => t.ClientIncome);
			//this.HasOptional(t => t.TLU_Codes_IncomeSource)
			//    .WithMany(t => t.ClientIncome)
			//    .HasForeignKey(d => d.PrimaryIncomeId);
		}
	}
}