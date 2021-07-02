using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Infonet.Core.Entity;
using Infonet.Core.Entity.Binding;
using Infonet.Core.Entity.Validation;
using Infonet.Data.Looking;

namespace Infonet.Data.Models.Clients {
	[BindHint(Include = "AppealStatusId,DateReportPolice,TrialTypeId,VWParticipateID,IsDetectiveInterview,IsPatrolInterview,IsSAInterview,IsTrialScheduled,IsVWProgram")]
	[DeleteIfNulled("ClientId,CaseId")]
	public class ClientPoliceProsecution : IRevisable {
		public int? Id { get; set; }

		[Display(Name = "Appeal Status")]
		[Lookup("AppealStatus")]
		public int? AppealStatusId { get; set; }

		public int? CaseId { get; set; }

		public int? ClientId { get; set; }

		[BetweenNineteenSeventyToday]
		[DataType(DataType.Date)]
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
		[Display(Name = "Date Reported to Police")]
		public DateTime? DateReportPolice { get; set; }

		[Display(Name = "Detective Interview")]
		public bool? DetectiveInterview { get; set; }

		[Display(Name = "Patrol Interview")]
		public bool? PatrolInterview { get; set; }

		[Display(Name = "State's Attorney Interview")]
		public bool? SAInterview { get; set; }

		[Lookup("TrialType")]
		[Display(Name = "Trial Type")]
		public int? TrialTypeId { get; set; }

		[Display(Name = "Trial Scheduled?")]
		public bool? TrialScheduled { get; set; }

		[Lookup("VictimWitnessParticipation")]
		[Display(Name = "Victim/Witness Participate?")]
		public int? VWParticipateID { get; set; }

		public bool? VWProgram { get; set; }

		public DateTime? RevisionStamp { get; set; }

		public virtual ClientCase ClientCase { get; set; }

		[Display(Name = "Detective Interview")]
		[NotMapped]
		public bool IsDetectiveInterview {
			get { return DetectiveInterview ?? false; }
			set { DetectiveInterview = value; }
		}

		[Display(Name = "Patrol Interview")]
		[NotMapped]
		public bool IsPatrolInterview {
			get { return PatrolInterview ?? false; }
			set { PatrolInterview = value; }
		}

		[Display(Name = "State's Attorney Interview")]
		[NotMapped]
		public bool IsSAInterview {
			get { return SAInterview ?? false; }
			set { SAInterview = value; }
		}

		[Display(Name = "Trial Scheduled?")]
		[NotMapped]
		public bool IsTrialScheduled {
			get { return TrialScheduled ?? false; }
			set { TrialScheduled = value; }
		}

		[Display(Name = "Victim/Witness Program")]
		[NotMapped]
		public bool IsVWProgram {
			get { return VWProgram ?? false; }
			set { VWProgram = value; }
		}
	}
}