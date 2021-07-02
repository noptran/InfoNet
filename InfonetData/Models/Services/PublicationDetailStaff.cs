using System;
using System.ComponentModel.DataAnnotations;
using Infonet.Core.Entity;
using Infonet.Core.Entity.Validation;
using Infonet.Data.Models.Centers;

namespace Infonet.Data.Models.Services {
	public class PublicationDetailStaff : IRevisable {
		public int? ICS_Staff_ID { get; set; }

		public int? ICS_ID { get; set; }

		[Required]
		[Display(Name = "Staff/Volunteer")]
		public int? SVID { get; set; }

		[Display(Name = "Preparing Hours")]
		[Range(0, 99999)]
		[QuarterIncrement]
		public double? HoursPrep { get; set; }

		public DateTime? RevisionStamp { get; set; }

		public virtual StaffVolunteer StaffVolunteer { get; set; }

		public virtual PublicationDetail PublicationDetail { get; set; }

		public bool IsUnchanged(PublicationDetailStaff obj) {
			return obj != null &&
					ICS_Staff_ID == obj.ICS_Staff_ID &&
					ICS_ID == obj.ICS_ID &&
					HoursPrep == obj.HoursPrep &&
					SVID == obj.SVID;
		}
	}
}