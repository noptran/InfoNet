using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Clients;

namespace Infonet.Data.Mapping.Clients {
	public class ClientReferralDetailMap : EntityTypeConfiguration<ClientReferralDetail> {
		public ClientReferralDetailMap() {
			// Primary Key
			HasKey(t => t.ReferralDetailID);

			// Properties
			// Table & Column Mappings
			ToTable("Ts_ClientReferralDetail");
			Property(t => t.ReferralDetailID).HasColumnName("ReferralDetailID");
			Property(t => t.ClientID).HasColumnName("ClientID");
			Property(t => t.CaseID).HasColumnName("CaseID");
			Property(t => t.ReferralDate).HasColumnName("ReferralDate");
			Property(t => t.ReferralTypeID).HasColumnName("ReferralTypeID");
			Property(t => t.AgencyID).HasColumnName("AgencyID");
			Property(t => t.ResponseID).HasColumnName("ResponseID");
			Property(t => t.LocationID).HasColumnName("LocationID");
			Property(t => t.CityTownTownshpID).HasColumnName("CityTownTownshpID");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");

			// Relationships
			HasOptional(t => t.Agency)
				.WithMany(t => t.ClientReferralDetails)
				.HasForeignKey(d => d.AgencyID);
			HasOptional(t => t.Center)
				.WithMany(t => t.ClientReferralDetails)
				.HasForeignKey(d => d.LocationID);
			HasRequired(t => t.ClientCase)
				.WithMany(t => t.ClientReferralDetail)
				.HasForeignKey(d => new { d.ClientID, d.CaseID });
			HasOptional(t => t.TwnTshipCounty)
				.WithMany(t => t.ClientReferralDetails)
				.HasForeignKey(d => d.CityTownTownshpID);
		}
	}
}