using System;
using System.ComponentModel.DataAnnotations;
using Infonet.Core.Entity;
using Infonet.Core.Entity.Validation;
using Infonet.Data.Models.Centers;

namespace Infonet.Data.Models.Services {
	public class ProgramDetailStaff : IRevisable {
		public int? ICS_Staff_ID { get; set; }

		public int? ICS_ID { get; set; }

		[Required]
		[Display(Name = "Staff/Volunteer")]
		public int SVID { get; set; }

		[Required]
		[Display(Name = "Conduct Hours")]
		[Range(0, 999)]
		[QuarterIncrement]
		public double? ConductHours { get; set; }

		[Display(Name = "Prepare Hours")]
		[Range(0, 200)]
		[QuarterIncrement]
		public double? HoursPrep { get; set; }

		[Display(Name = "Travel Hours")]
		[Range(0, 200)]
		[QuarterIncrement]
		public double? HoursTravel { get; set; }

		public DateTime? RevisionStamp { get; set; }

		public virtual StaffVolunteer StaffVolunteer { get; set; }

		public virtual ProgramDetail ProgramDetail { get; set; }

		public bool IsUnchanged(ProgramDetailStaff obj) {
			return obj != null &&
					ICS_Staff_ID == obj.ICS_Staff_ID &&
					ICS_ID == obj.ICS_ID &&
					ConductHours == obj.ConductHours &&
					HoursPrep == obj.HoursPrep &&
					HoursTravel == obj.HoursTravel &&
					SVID == obj.SVID;
		}
	}
}