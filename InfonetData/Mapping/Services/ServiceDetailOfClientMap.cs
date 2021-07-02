using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Services;

namespace Infonet.Data.Mapping.Services {
	public class ServiceDetailOfClientMap : EntityTypeConfiguration<ServiceDetailOfClient> {
		public ServiceDetailOfClientMap() {
			// Primary Key
			HasKey(t => t.ServiceDetailID);

			// Properties
			// Table & Column Mappings
			ToTable("Tl_ServiceDetailOfClient");
			Property(t => t.ServiceDetailID).HasColumnName("ServiceDetailID");
			Property(t => t.ClientID).HasColumnName("ClientID");
			Property(t => t.CaseID).HasColumnName("CaseID");
			Property(t => t.ServiceID).HasColumnName("ServiceID");
			Property(t => t.SVID).HasColumnName("SVID");
			Property(t => t.ServiceDate).HasColumnName("ServiceDate");
			Property(t => t.LocationID).HasColumnName("LocationID");
			Property(t => t.ReceivedHours).HasColumnName("ReceivedHours");
			Property(t => t.FundDateID).HasColumnName("FundDateID");
			Property(t => t.ShelterBegDate).HasColumnName("ShelterBegDate");
			Property(t => t.ShelterEndDate).HasColumnName("ShelterEndDate");
			Property(t => t.CityTownTownshpID).HasColumnName("CityTownTownshpID");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");
			Property(t => t.AgencyRecID).HasColumnName("AgencyRecID");
			Property(t => t.ServiceBegDate).HasColumnName("ServiceBegDate");
			Property(t => t.ServiceEndDate).HasColumnName("ServiceEndDate");
			Property(t => t.ICS_ID).HasColumnName("ICS_ID");

			// Relationships
			HasOptional(t => t.Center)
				.WithMany(t => t.ServiceDetailsOfClients)
				.HasForeignKey(d => d.LocationID);
			HasRequired(t => t.ClientCase)
				.WithMany(t => t.ServiceDetailsOfClient)
				.HasForeignKey(d => new { d.ClientID, d.CaseID });
			HasOptional(t => t.StaffVolunteer)
				.WithMany(t => t.ServiceDetailsOfClient)
				.HasForeignKey(d => d.SVID);
			HasOptional(t => t.Tl_ProgramDetail)
				.WithMany(t => t.ServiceDetailsOfClient)
				.HasForeignKey(d => d.ICS_ID);
			HasRequired(t => t.TLU_Codes_ProgramsAndServices)
				.WithMany(t => t.ServiceDetailsOfClient)
				.HasForeignKey(d => d.ServiceID);
			HasOptional(t => t.TwnTshipCounty)
				.WithMany(t => t.ServiceDetailsOfClient)
				.HasForeignKey(d => d.CityTownTownshpID);

			// ADDED RELATIONSHIP
			HasOptional(t => t.FundingDate)
				.WithMany(t => t.ServiceDetailsOfClients)
				.HasForeignKey(d => d.FundDateID);
		}
	}
}