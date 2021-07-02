using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Clients;
#pragma warning disable 612

namespace Infonet.Data.Mapping.Clients {
	public class ClientDisabilityMap : EntityTypeConfiguration<ClientDisability> {
		public ClientDisabilityMap() {
			// Primary Key
			HasKey(t => new { t.ClientId, t.CaseId });

			// Properties
			Property(t => t.ClientId)
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

			Property(t => t.CaseId)
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

			Property(t => t.PrimaryLanguage)
				.HasMaxLength(50);

			Property(t => t.WhatOther)
				.HasMaxLength(50);

			// Table & Column Mappings
			ToTable("Ts_ClientDisability");
			Property(t => t.ClientId).HasColumnName("ClientID");
			Property(t => t.CaseId).HasColumnName("CaseID");
			Property(t => t.DevelopmentallyDisabled).HasColumnName("DevelopmentallyDisabled");
			Property(t => t.SpecialDiet).HasColumnName("SpecialDiet");
			Property(t => t.Immobil).HasColumnName("Immobil");
			Property(t => t.WheelChair).HasColumnName("WheelChair");
			Property(t => t.MedsAdministered).HasColumnName("MedsAdministered");
			Property(t => t.Deaf).HasColumnName("Deaf");
			Property(t => t.VisualProblem).HasColumnName("VisualProblem");
			Property(t => t.PrimaryLanguage).HasColumnName("PrimaryLanguage");
			Property(t => t.LimitedEnglish).HasColumnName("LimitedEnglish");
			Property(t => t.ADLProblem).HasColumnName("ADLProblem");
			Property(t => t.OtherDisability).HasColumnName("OtherDisability");
			Property(t => t.WhatOther).HasColumnName("WhatOther");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");
			Property(t => t.MentalDisability).HasColumnName("MentalDisability");
			Property(t => t.PrimaryLanguageID).HasColumnName("PrimaryLanguageID");
			Property(t => t.NoSpecialNeeds).HasColumnName("NoSpecialNeeds");
			Property(t => t.UnknownSpecialNeeds).HasColumnName("UnknownSpecialNeeds");
			Property(t => t.NotReported).HasColumnName("NotReported");

			// Relationships
			HasRequired(t => t.ClientCase)
				.WithOptional(t => t.ClientDisability);
		}
	}
}