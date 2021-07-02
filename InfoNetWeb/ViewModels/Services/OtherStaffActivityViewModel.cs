using System;
using System.ComponentModel.DataAnnotations;
using Infonet.Core.Entity;
using Infonet.Core.Entity.Validation;

namespace Infonet.Web.ViewModels.Services {
	public class OtherStaffActivityViewModel : IRevisable {
		public int? OsaID { get; set; }

		[Display(Name = "Staff/Volunteer")]
		[Required]
		public int? SVID { get; set; }

		[Required]
		[Display(Name = "Other Staff Activity")]
		public int? OtherStaffActivityID { get; set; }

		[Display(Name = "Conduct Hours")]
		[Range(0, 999)]
		[QuarterIncrement]
		[Required]
		public float? ConductingHours { get; set; }

		[Display(Name = "Travel Hours")]
		[Range(0, 99)]
		[QuarterIncrement]
		public float? TravelHours { get; set; }

		[Display(Name = "Prepare Hours")]
		[Range(0, 999)]
		[QuarterIncrement]
		public float? PrepareHours { get; set; }

		[Required]
		[Display(Name = "Date")]
		[DataType(DataType.Date)]
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
		[BetweenNineteenSeventyToday]
		public DateTime? OsaDate { get; set; }

		public DateTime? RevisionStamp { get; set; }

		// View Specific Properties
		public int saveAddNew { get; set; }

		public string ReturnURL { get; set; }
	}
}