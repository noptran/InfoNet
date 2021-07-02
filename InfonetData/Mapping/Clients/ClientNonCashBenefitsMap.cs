using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Clients;

namespace Infonet.Data.Mapping.Clients {
	public class ClientNonCashBenefitsMap : EntityTypeConfiguration<ClientNonCashBenefits> {
		public ClientNonCashBenefitsMap() {
			// Primary Key
			HasKey(t => new { t.ClientID, t.CaseID });

			// Properties
			Property(t => t.ClientID)
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

			Property(t => t.CaseID)
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

			// Table & Column Mappings
			ToTable("Ts_ClientNonCashBenefits");
			Property(t => t.ClientID).HasColumnName("ClientID");
			Property(t => t.CaseID).HasColumnName("CaseID");
			Property(t => t.FoodBenefit).HasColumnName("FoodBenefit");
			Property(t => t.SpecSuppNutr).HasColumnName("SpecSuppNutr");
			Property(t => t.TANFChildCare).HasColumnName("TANFChildCare");
			Property(t => t.TANFTrans).HasColumnName("TANFTrans");
			Property(t => t.TANFOther).HasColumnName("TANFOther");
			Property(t => t.PublicHousing).HasColumnName("PublicHousing");
			Property(t => t.OtherSource).HasColumnName("OtherSource");
			Property(t => t.Medicaid).HasColumnName("Medicaid");
			Property(t => t.Medicare).HasColumnName("Medicare");
			Property(t => t.StateChildHealth).HasColumnName("StateChildHealth");
			Property(t => t.VetAdminMed).HasColumnName("VetAdminMed");
			Property(t => t.PrivateIns).HasColumnName("PrivateIns");
			Property(t => t.NoHealthIns).HasColumnName("NoHealthIns");
			Property(t => t.UnknownHealthIns).HasColumnName("UnknownHealthIns");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");
			Property(t => t.NoBenefit).HasColumnName("NoBenefit");
			Property(t => t.UnknownBenefit).HasColumnName("UnknownBenefit");

			// Relationships
			HasRequired(t => t.ClientCases)
				.WithOptional(t => t.ClientNonCashBenefits);
		}
	}
}