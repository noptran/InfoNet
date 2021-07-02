using System;
using System.ComponentModel.DataAnnotations;
using Infonet.Core.Entity.Validation;
using Infonet.Data.Looking;

namespace Infonet.Web.ViewModels.Case {
	public class DirectServiceEdit {
		public int? ServiceDetailID { get; set; }

		[Required]
		[Display(Name = "Service")]
		[Lookup("DirectServices")]
		public int? ServiceID { get; set; }

		[Required]
		[Display(Name = "Staff/Volunteer")]
		public int? SVID { get; set; }

		[Required]
		[NotGreaterThanToday]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
		[Display(Name = "Date")]
		public DateTime? ServiceDate { get; set; }

		public int? LocationID { get; set; }

		[Required]
		[QuarterIncrement]
		[Display(Name = "Hours")]
		public double? ReceivedHours { get; set; }

		public DateTime? ShelterBegDate { get; set; }
		public DateTime? ShelterEndDate { get; set; }
		public string StaffName { get; set; }
		public string Service { get; set; }
		public string Location { get; set; }
		public int? ICS_ID { get; set; }

		[Display(Name = "Delete")]
		public bool IsDeleted { get; set; }

		[Display(Name = "Edit")]
		public bool IsEdited { get; set; }

		public int? Index { get; set; }
	}
}