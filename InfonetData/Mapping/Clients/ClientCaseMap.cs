using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Clients;
#pragma warning disable 612

namespace Infonet.Data.Mapping.Clients {
	public class ClientCaseMap : EntityTypeConfiguration<ClientCase> {
		public ClientCaseMap() {
			// Primary Key
			HasKey(t => new { t.ClientId, t.CaseId });

			// Properties
			Property(t => t.ClientId)
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

			Property(t => t.CaseId)
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

			Property(t => t.AldermanicWard)
				.HasMaxLength(50);

			Property(t => t.LegislativeDistricts)
				.HasMaxLength(50);

			// Table & Column Mappings
			ToTable("T_ClientCases");
			Property(t => t.ClientId).HasColumnName("ClientID");
			Property(t => t.CaseId).HasColumnName("CaseID");
			Property(t => t.Age).HasColumnName("Age");
			Property(t => t.CollegeUnivStudentId).HasColumnName("CollegeUnivStudent");
			Property(t => t.EmploymentId).HasColumnName("EmploymentID");
			Property(t => t.HealthInsuranceId).HasColumnName("HealthInsuranceID");
			Property(t => t.EducationId).HasColumnName("EducationID");
			Property(t => t.MaritalStatusId).HasColumnName("MaritalStatusID");
			Property(t => t.PregnantId).HasColumnName("PregnantID");
			Property(t => t.NumberOfChildren).HasColumnName("NumChildren");
			Property(t => t.FirstContactDate).HasColumnName("FirstContactDate");
			Property(t => t.ClosedReasonId).HasColumnName("CaseClosedReasonID");
			Property(t => t.CaseClosed).HasColumnName("CaseClosed");
			Property(t => t.ClosedDate).HasColumnName("CaseClosedDate");
			Property(t => t.SignificantOtherOfId).HasColumnName("SignificantOtherOfID");
			Property(t => t.CustodyId).HasColumnName("CustodyID");
			Property(t => t.LivesWithId).HasColumnName("LivesWithID");
			Property(t => t.DCFSInvestigation).HasColumnName("DCFSInvestigation");
			Property(t => t.DCFSOpen).HasColumnName("DCFSOpen");
			Property(t => t.SchoolId).HasColumnName("SchoolID");
			Property(t => t.AldermanicWard).HasColumnName("AldermanicWard");
			Property(t => t.LegislativeDistricts).HasColumnName("LegislativeDistricts");
			Property(t => t.RelationSOtoClientId).HasColumnName("RelationSOtoClientID");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");
			Property(t => t.DCFSHotlineDate).HasColumnName("DCFSHotlineDate");
			Property(t => t.DCFSServiceDate).HasColumnName("DCFSServiceDate");
			Property(t => t.InvestigationTypeId).HasColumnName("InvestigationTypeID");
			Property(t => t.PoliceReportDate).HasColumnName("PoliceReportDate");
			Property(t => t.InformationOnlyCase).HasColumnName("InformationOnlyCase");
			Property(t => t.VetStatusId).HasColumnName("VetStatusID");
			Property(t => t.SexualOrientationId).HasColumnName("SexualOrientationID");

			// Relationships
			HasRequired(t => t.Client)
				.WithMany(t => t.ClientCases)
				.HasForeignKey(d => d.ClientId);
		}
	}
}