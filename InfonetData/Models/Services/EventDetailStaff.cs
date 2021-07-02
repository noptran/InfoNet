using System;
using System.ComponentModel.DataAnnotations;
using Infonet.Core.Entity;
using Infonet.Data.Models.Centers;

namespace Infonet.Data.Models.Services {
	public class EventDetailStaff : IRevisable {
		public int? ICS_Staff_ID { get; set; }

		public int? ICS_ID { get; set; }

		[Required]
		[Display(Name = "Staff/Volunteer")]
		public int SVID { get; set; }

		[Range(0, 100)]
		[Display(Name = "Conduct Hours")]
		public double? HoursConduct { get; set; }

		[Range(0, 999)]
		[Display(Name = "Prepare Hours")]
		public double? HoursPrep { get; set; }

		[Range(0, 50)]
		[Display(Name = "Travel Hours")]
		public double? HoursTravel { get; set; }

		public DateTime? RevisionStamp { get; set; }

		public virtual StaffVolunteer StaffVolunteer { get; set; }

		public virtual EventDetail EventDetail { get; set; }

		public bool IsUnchanged(EventDetailStaff obj) {
			return obj != null &&
					ICS_Staff_ID == obj.ICS_Staff_ID &&
					ICS_ID == obj.ICS_ID &&
					HoursConduct == obj.HoursConduct &&
					HoursPrep == obj.HoursPrep &&
					HoursTravel == obj.HoursTravel &&
					SVID == obj.SVID;
		}
	}
}