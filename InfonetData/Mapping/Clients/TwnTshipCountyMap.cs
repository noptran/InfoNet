using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Clients;
#pragma warning disable 612

namespace Infonet.Data.Mapping.Clients {
	public class TwnTshipCountyMap : EntityTypeConfiguration<TwnTshipCounty> {
		public TwnTshipCountyMap() {
			// Primary Key
			HasKey(t => t.LocID);

			// Properties
			Property(t => t.CityOrTown)
				.HasMaxLength(50);

			Property(t => t.Township)
				.HasMaxLength(50);

			Property(t => t.Zipcode)
				.HasMaxLength(10);

			// Table & Column Mappings
			ToTable("Ts_TwnTshipCounty");
			Property(t => t.LocID).HasColumnName("LocID");
			Property(t => t.ClientID).HasColumnName("ClientID");
			Property(t => t.CaseID).HasColumnName("CaseID");
			Property(t => t.CityOrTown).HasColumnName("CityOrTown");
			Property(t => t.Township).HasColumnName("Township");
			Property(t => t.CountyID).HasColumnName("CountyID");
			Property(t => t.Zipcode).HasColumnName("Zipcode");
			Property(t => t.MoveDate).HasColumnName("MoveDate");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");
			Property(t => t.CityID).HasColumnName("CityID");
			Property(t => t.StateID).HasColumnName("StateID");
			Property(t => t.TownshipID).HasColumnName("townshipID");
			Property(t => t.ZipcodeID).HasColumnName("ZipcodeID");
			Property(t => t.ZipcodeSuffix).HasColumnName("ZipcodeSuffix");
			Property(t => t.ResidenceTypeID).HasColumnName("ResidenceTypeID");
			Property(t => t.LengthOfStayInResidenceID).HasColumnName("LengthOfStayInResidenceID");

			// Relationships
			HasRequired(t => t.Client)
				.WithMany(t => t.TwnTshipCounty)
				.HasForeignKey(d => d.ClientID);
		}
	}
}