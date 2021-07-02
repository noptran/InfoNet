using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Clients;

namespace Infonet.Data.Mapping.Clients {
	public class ServiceGotMap : EntityTypeConfiguration<ServiceGot> {
		public ServiceGotMap() {
			// Primary Key
			HasKey(t => new { t.ClientID, t.CaseID });

			// Properties
			Property(t => t.ClientID)
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

			Property(t => t.CaseID)
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

			// Table & Column Mappings
			ToTable("Ts_ClientServiceGot");
			Property(t => t.ClientID).HasColumnName("ClientID");
			Property(t => t.CaseID).HasColumnName("CaseID");
			Property(t => t.Shelter).HasColumnName("Shelter");
			Property(t => t.Housing).HasColumnName("Housing");
			Property(t => t.Medical).HasColumnName("Medical");
			Property(t => t.Transportation).HasColumnName("Transportation");
			Property(t => t.Emotional).HasColumnName("Emotional");
			Property(t => t.ChildCare).HasColumnName("ChildCare");
			Property(t => t.Financial).HasColumnName("Financial");
			Property(t => t.Legal).HasColumnName("Legal");
			Property(t => t.Employment).HasColumnName("Employment");
			Property(t => t.Education).HasColumnName("Education");
			Property(t => t.Referral).HasColumnName("Referral");
			Property(t => t.LegalAdvocacy).HasColumnName("LegalAdvocacy");
			Property(t => t.MedicalAdvocacy).HasColumnName("MedicalAdvocacy");
			Property(t => t.CrisisIntervention).HasColumnName("CrisisIntervention");
			Property(t => t.LockUp).HasColumnName("LockUp");
			Property(t => t.Therapy).HasColumnName("Therapy");
			Property(t => t.IndividualSupportChild).HasColumnName("IndividualSupportChild");
			Property(t => t.GroupActivity).HasColumnName("GroupActivity");
			Property(t => t.SchoolAdvocacyChild).HasColumnName("SchoolAdvocacyChild");
			Property(t => t.ParentChildSupport).HasColumnName("ParentChildSupport");
			Property(t => t.CommunityAdvocacyChild).HasColumnName("CommunityAdvocacyChild");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");

			// Relationships
			HasRequired(t => t.ClientCase)
				.WithOptional(t => t.ServiceGot);
		}
	}
}