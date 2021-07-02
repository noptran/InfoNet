using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Infonet.Core.Collections;
using Infonet.Core.Entity;
using Infonet.Core.Entity.Binding;
using Infonet.Core.Entity.Validation;
using Infonet.Data.Looking;
using Infonet.Data.Models.Centers;

namespace Infonet.Data.Models.Clients {
	[BindHint(Include = "InterviewDate, InterviewerID, SiteLocationId, LocationID, RecordTypeID, IsCourtesyInterview, VSIObserversById")]
	[DeleteIfNulled("ClientId,CaseId")]
	public class VictimSensitiveInterview : IRevisable {
		public VictimSensitiveInterview() {
			VSIObservers = new List<VSIObserver>();
			VSIObserversById = new DerivedDictionary<VSIObserver>(() => VSIObservers, true, e => e.ID?.ToString()) { Template = () => new VSIObserver() };
		}

		public int? VSI_ID { get; set; }

		public int? ClientId { get; set; }

		public int? CaseId { get; set; }

		[Required]
		[NotLessThanNineteenSeventy]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
		[Display(Name = "Interview Date")]
		public DateTime? InterviewDate { get; set; }

		[Display(Name = "Interviewer")]
		public int? InterviewerID { get; set; }

		public int? LocationID { get; set; }

		[Display(Name = "Location")]
		[Lookup("SiteLocation")]
		public int? SiteLocationId { get; set; }

		[Display(Name = "Recorded")]
		[Lookup("RecordingType")]
		public int? RecordTypeID { get; set; }

		[Display(Name = "Courtesy Interview")]
		public bool? CourtesyInterview { get; set; }

		[Display(Name = "Courtesy Interview")]
		[NotMapped]
		public bool IsCourtesyInterview {
			get { return CourtesyInterview ?? false; }
			set { CourtesyInterview = value; }
		}

		public DateTime? RevisionStamp { get; set; }

		public virtual DerivedDictionary<VSIObserver> VSIObserversById { get; }

		public virtual Center Center { get; set; }

		public virtual ClientCase ClientCase { get; set; }

		public virtual Contact Interviewer { get; set; }

		public virtual ICollection<VSIObserver> VSIObservers { get; set; }

		//KMS DO clear VSIObserversById?
	}
}