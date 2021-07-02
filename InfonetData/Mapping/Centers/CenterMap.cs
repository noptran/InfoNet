using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Centers;
#pragma warning disable 612

namespace Infonet.Data.Mapping.Centers {
	public class CenterMap : EntityTypeConfiguration<Center> {
		public CenterMap() {
			// Primary Key
			HasKey(t => t.CenterID);

			// Properties
			Property(t => t.CenterName)
				.IsRequired()
				.HasMaxLength(100);

			Property(t => t.Address)
				.HasMaxLength(50);

			Property(t => t.City)
				.HasMaxLength(50);

			Property(t => t.Zipcode)
				.HasMaxLength(10);

			Property(t => t.LegisDistrict)
				.HasMaxLength(50);

			Property(t => t.JudicialDistrict)
				.HasMaxLength(50);

			Property(t => t.ServiceArea)
				.HasMaxLength(100);

			Property(t => t.Fax)
				.HasMaxLength(15);

			Property(t => t.FederalEmployerID)
				.HasMaxLength(50);

			Property(t => t.Email)
				.HasMaxLength(100);

			Property(t => t.Telephone)
				.HasMaxLength(15);

			Property(t => t.DirectorEmail)
				.HasMaxLength(255);

			// Table & Column Mappings
			ToTable("T_Center");
			Property(t => t.CenterID).HasColumnName("CenterID");
			Property(t => t.ParentCenterID).HasColumnName("ParentCenterID");
			Property(t => t.ProviderID).HasColumnName("ProviderID");
			Property(t => t.CenterName).HasColumnName("CenterName");
			Property(t => t.Address).HasColumnName("Address");
			Property(t => t.City).HasColumnName("City");
			Property(t => t.Zipcode).HasColumnName("Zipcode");
			Property(t => t.CountyID).HasColumnName("CountyID");
			Property(t => t.EmpNumber).HasColumnName("EmpNumber");
			Property(t => t.AgeAnnBudget).HasColumnName("AgeAnnBudget");
			Property(t => t.ProgAnnBudget).HasColumnName("ProgAnnBudget");
			Property(t => t.BoardMemberNum).HasColumnName("BoardMemberNum");
			Property(t => t.LegisDistrict).HasColumnName("LegisDistrict");
			Property(t => t.JudicialDistrict).HasColumnName("JudicialDistrict");
			Property(t => t.ServiceArea).HasColumnName("ServiceArea");
			Property(t => t.Population).HasColumnName("Population");
			Property(t => t.Fax).HasColumnName("Fax");
			Property(t => t.FederalEmployerID).HasColumnName("FederalEmployerID");
			Property(t => t.Email).HasColumnName("Email");
			Property(t => t.Telephone).HasColumnName("Telephone");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");
			Property(t => t.IsRealCenter).HasColumnName("IsRealCenter");
			Property(t => t.DirectorEmail).HasColumnName("DirectorEmail");
			Property(t => t.CreationDate).HasColumnName("CreationDate");
			Property(t => t.TerminationDate).HasColumnName("TerminationDate");
			Property(t => t.StateID).HasColumnName("StateID");
			Property(t => t.CityID).HasColumnName("CityID");
			Property(t => t.ShelterStatus).HasColumnName("ShelterStatus");

			// Relationships
			HasOptional(t => t.Parent)
				.WithMany(t => t.Satellites)
				.HasForeignKey(d => d.ParentCenterID);
		}
	}
}