using System;
using System.ComponentModel.DataAnnotations;
using Infonet.Core.Entity;
using Infonet.Core.Entity.Binding;
using Infonet.Core.Entity.Validation;
using Infonet.Data.Looking;
using Infonet.Data.Models.Centers;

namespace Infonet.Data.Models.Clients {
    [BindHint(Include = "MedicalVisitId, MedicalTreatmentId, InjuryId, EvidKitId, PhotosTakenId, MedWhereId, OtherFamilyProblem, WherePhotos, AppealStatusId, DateReportPolice, PatrolInterview, DetectiveInterview, SAInterview, VictWitPrg, GoneTrial, TrialTypeId, VWParticipateId, HospitalName, OrderOfProtectionId, OrderTypeId, CivilNoContactOrderId, CivilNoContactOrderTypeId, CivilNoContactOrderRequestId, AgencyID, ExamCompletedId, BeforeAfterId, ExamDate, ExamTypeId, SiteLocationId, ColposcopeUsedId, FindingId,SANETreatedID")]
    [DeleteIfNulled("ClientId,CaseId")]
    public class ClientCJProcess : IRevisable {
        public int? Med_ID { get; set; }

        public int? ClientId { get; set; }

        public int? CaseId { get; set; }

        [Lookup("YesNo")]
        [Display(Name = "Visited Medical Facility?")]
        public int? MedicalVisitId { get; set; }

        [Lookup("YesNo")]
        [Display(Name = "Treated for Injuries?")]
        public int? MedicalTreatmentId { get; set; }

        [Lookup("InjurySeverity")]
        [Display(Name = "Seriousness of Injury")]
        public int? InjuryId { get; set; }

        [Lookup("YesNo")]
        [Display(Name = "Evidence Kit Used?")]
        public int? EvidKitId { get; set; }

        /* Has 2 FK constraints: TLU_Codes_YesNo and TLU_Codes_PhotoTaken */
        [Lookup("PhotosTaken")]
        [Display(Name = "Photos Taken?")]
        public int? PhotosTakenId { get; set; }

        [Lookup("MedicalTreatmentLocation")]
        [Display(Name = "Type of Medical Facility")]
        public int? MedWhereId { get; set; }

        [MaxLength(100, ErrorMessageResourceName = "StringMaxLengthMessage", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Other Problem")]
        public string OtherFamilyProblem { get; set; }

        #region obsolete
        [Obsolete]
        [Display(Name = "Patrol Interview")]
        public bool PatrolInterview { get; set; }

        [Obsolete]
        [Display(Name = "State's Attorney Interview")]
        public bool SAInterview { get; set; }

        [Obsolete]
        [Display(Name = "Detective Interview")]
        public bool DetectiveInterview { get; set; }

        [Obsolete]
        public bool SACharge { get; set; }

        [Obsolete]
        [Display(Name = "Trial Scheduled?")]
        public bool GoneTrial { get; set; }

        [Obsolete]
        [Display(Name = "Victim/Witness Program")]
        public bool VictWitPrg { get; set; }

        [Obsolete]
        [Lookup("VictimWitnessParticipation")]
        [Display(Name = "Victim/Witness Participate?")]
        public int? VWParticipateId { get; set; }

        [Obsolete]
        [BetweenNineteenSeventyToday]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Date Reported to Police")]
        public DateTime? DateReportPolice { get; set; }

        [Obsolete]
        public short? DefenseContinuances { get; set; }

        [Obsolete]
        public short? ProsecutionContinuances { get; set; }

        [Obsolete]
        public short? NoCourtAppearances { get; set; }
        #endregion

        [MaxLength(100, ErrorMessageResourceName = "StringMaxLengthMessage", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Where are the photos?")]
        public string WherePhotos { get; set; }

        #region obsolete
        [Obsolete]
        public bool Warrantlssued { get; set; }

        [Obsolete]
        [Lookup("OrderOfProtectionForum")]
        [Display(Name = "Order of Protection")]
        public int? OrderOfProtectionId { get; set; }

        [Obsolete]
        [Lookup("OrderOfProtectionType")]
        [Display(Name = "Order of Protection Type")]
        public int? OrderTypeId { get; set; }

        [Obsolete]
        public string PoliceCharge { get; set; }

        [Obsolete]
        public DateTime? ArrestedDate { get; set; }

        [Obsolete]
        [Display(Name = "Appeal Status")]
        [Lookup("AppealStatus")]
        public int? AppealStatusId { get; set; }

        [Obsolete]
        [Lookup("TrialType")]
        [Display(Name = "Trial Type")]
        public int? TrialTypeId { get; set; }

        [Obsolete]
        public bool Apprehended { get; set; }
        #endregion

        [MaxLength(100, ErrorMessageResourceName = "StringMaxLengthMessage", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Hospital/Medical Facility Visited")]
        public string HospitalName { get; set; }

        public DateTime? RevisionStamp { get; set; }

        [Display(Name = "Before or After VSI")]
        [Lookup("BeforeAfter")]
        public int? BeforeAfterId { get; set; }

        [Display(Name = "Colposcope Used")]
        [Lookup("YesNo")]
        public int? ColposcopeUsedId { get; set; }

        [Display(Name = "Exam Completed")]
        [Lookup("YesNo")]
        public int? ExamCompletedId { get; set; }

        [Display(Name = "Treated by SANE?")]
        [Lookup("YesNo")]
        public int? SANETreatedId { get; set; }

        [NotLessThanNineteenSeventy]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Exam Date")]
        public DateTime? ExamDate { get; set; }

        [Display(Name = "Type of Exam")]
        [Lookup("MedicalExamType")]
        public int? ExamTypeId { get; set; }

        [Display(Name = "Finding")]
        [Lookup("MedicalExamFinding")]
        public int? FindingId { get; set; }

        [Display(Name = "Location")]
        [Lookup("SiteLocation")]
        public int? SiteLocationId { get; set; }

        [Display(Name = "Facility Name")]
        public int? AgencyID { get; set; }

        #region obsolete
        [Obsolete]
        [Lookup("OrderOfProtectionForum")]
        [Display(Name = "Civil No Contact Order")]
        public int? CivilNoContactOrderId { get; set; }

        [Obsolete]
        [Lookup("OrderOfProtectionType")]
        [Display(Name = "Civil No Contact Order Type")]
        public int? CivilNoContactOrderTypeId { get; set; }

        [Obsolete]
        [Lookup("OrderOfProtectionStatus")]
        [Display(Name = "Civil No Contact Order Request")]
        public int? CivilNoContactOrderRequestId { get; set; }
        #endregion

        public virtual Agency Agency { get; set; }

        public virtual ClientCase ClientCase { get; set; }
    }
}