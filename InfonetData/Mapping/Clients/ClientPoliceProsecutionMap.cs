using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Clients;

namespace Infonet.Data.Mapping.Clients {
	public class ClientPoliceProsecutionMap : EntityTypeConfiguration<ClientPoliceProsecution> {
		public ClientPoliceProsecutionMap() {
			// Primary Key
			HasKey(t => t.Id);

			// Properties
			// Table & Column Mappings
			ToTable("Ts_ClientPoliceProsecution");
			Property(t => t.Id).HasColumnName("ID");
			Property(t => t.AppealStatusId).HasColumnName("AppealStatusID");
			Property(t => t.CaseId).HasColumnName("CaseID");
			Property(t => t.ClientId).HasColumnName("ClientID");
			Property(t => t.DateReportPolice).HasColumnName("DateReportPolice");
			Property(t => t.DetectiveInterview).HasColumnName("DetectiveInterview");
			Property(t => t.PatrolInterview).HasColumnName("PatrolInterview");
			Property(t => t.SAInterview).HasColumnName("SAInterview");
			Property(t => t.TrialTypeId).HasColumnName("TrialTypeID");
			Property(t => t.TrialScheduled).HasColumnName("TrialScheduled");
			Property(t => t.VWParticipateID).HasColumnName("VWParticipateID");
			Property(t => t.VWProgram).HasColumnName("VWProgram");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");
			
			// ADDED RELATIONSHIP
			HasOptional(t => t.ClientCase)
				.WithMany(t => t.ClientPoliceProsecutions)
				.HasForeignKey(d => new { d.ClientId, d.CaseId }); 
		}
	}
}