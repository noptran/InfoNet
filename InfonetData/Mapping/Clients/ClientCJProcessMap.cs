using System.Data.Entity.ModelConfiguration;
using Infonet.Data.Models.Clients;
#pragma warning disable 612

namespace Infonet.Data.Mapping.Clients {
	public class ClientCJProcessMap : EntityTypeConfiguration<ClientCJProcess> {
		public ClientCJProcessMap() {
			// Primary Key
			HasKey(t => t.Med_ID);

			// Properties
			Property(t => t.OtherFamilyProblem)
				.HasMaxLength(100);

			Property(t => t.WherePhotos)
				.HasMaxLength(100);

			Property(t => t.PoliceCharge)
				.HasMaxLength(100);

			Property(t => t.HospitalName)
				.HasMaxLength(150);

			// Table & Column Mappings
			ToTable("Ts_ClientCJProcess");
			Property(t => t.ClientId).HasColumnName("ClientID");
			Property(t => t.CaseId).HasColumnName("CaseID");
			Property(t => t.MedicalVisitId).HasColumnName("MedicalVisit");
			Property(t => t.MedicalTreatmentId).HasColumnName("MedicalTreatment");
			Property(t => t.InjuryId).HasColumnName("InjuryID");
			Property(t => t.EvidKitId).HasColumnName("EvidKit");
			Property(t => t.PhotosTakenId).HasColumnName("PhotosID");
			Property(t => t.MedWhereId).HasColumnName("MedWhereID");
			Property(t => t.OtherFamilyProblem).HasColumnName("OtherFamilyProblem");
			Property(t => t.PatrolInterview).HasColumnName("PatrolInterview");
			Property(t => t.SAInterview).HasColumnName("SAInterview");
			Property(t => t.DetectiveInterview).HasColumnName("DetectiveInterview");
			Property(t => t.SACharge).HasColumnName("SACharge");
			Property(t => t.GoneTrial).HasColumnName("GoneTrial");
			Property(t => t.VictWitPrg).HasColumnName("VictWitPrg");
			Property(t => t.VWParticipateId).HasColumnName("VWParticipateID");
			Property(t => t.DateReportPolice).HasColumnName("DateReportPolice");
			Property(t => t.DefenseContinuances).HasColumnName("DefenseContinuances");
			Property(t => t.ProsecutionContinuances).HasColumnName("ProsecutionContinuances");
			Property(t => t.NoCourtAppearances).HasColumnName("NoCourtAppearances");
			Property(t => t.WherePhotos).HasColumnName("WherePhotos");
			Property(t => t.Warrantlssued).HasColumnName("Warrantlssued");
			Property(t => t.OrderOfProtectionId).HasColumnName("OrderOfProtectionID");
			Property(t => t.OrderTypeId).HasColumnName("OrderTypeID");
			Property(t => t.PoliceCharge).HasColumnName("PoliceCharge");
			Property(t => t.ArrestedDate).HasColumnName("ArrestedDate");
			Property(t => t.AppealStatusId).HasColumnName("AppealStatusID");
			Property(t => t.TrialTypeId).HasColumnName("TrialTypeID");
			Property(t => t.Apprehended).HasColumnName("Apprehended");
			Property(t => t.HospitalName).HasColumnName("HospitalName");
			Property(t => t.RevisionStamp).HasColumnName("RevisionStamp");
			Property(t => t.BeforeAfterId).HasColumnName("BeforeAfterID");
			Property(t => t.ColposcopeUsedId).HasColumnName("ColposcopeUsedID");
			Property(t => t.ExamCompletedId).HasColumnName("ExamCompletedID");
			Property(t => t.ExamDate).HasColumnName("ExamDate");
			Property(t => t.ExamTypeId).HasColumnName("ExamTypeID");
			Property(t => t.FindingId).HasColumnName("FindingID");
			Property(t => t.SiteLocationId).HasColumnName("SiteLocationID");
			Property(t => t.AgencyID).HasColumnName("AgencyID");
			Property(t => t.Med_ID).HasColumnName("Med_ID");
			Property(t => t.CivilNoContactOrderId).HasColumnName("CivilNoContactOrderID");
			Property(t => t.CivilNoContactOrderTypeId).HasColumnName("CivilNoContactOrderTypeID");
			Property(t => t.CivilNoContactOrderRequestId).HasColumnName("CivilNoContactOrderRequestID");
            Property(t => t.SANETreatedId).HasColumnName("SANETreatedId");

			// Relationships
			HasOptional(t => t.Agency)
				.WithMany(t => t.ClientCJProcesses)
				.HasForeignKey(d => d.AgencyID);
			HasRequired(t => t.ClientCase)
				.WithMany(t => t.ClientCJProcesses)
				.HasForeignKey(d => new { d.ClientId, d.CaseId });
		}
	}
}