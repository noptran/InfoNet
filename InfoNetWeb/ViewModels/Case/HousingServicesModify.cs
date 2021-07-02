using System;
using System.ComponentModel.DataAnnotations;
using Infonet.Core.Entity.Validation;
using Infonet.Data.Looking;

namespace Infonet.Web.ViewModels.Case {
	public class HousingServicesModify {
		public int? ServiceDetailID { get; set; }

		[Required]
		[Lookup("HousingServices")]
		public int? ServiceID { get; set; }

		public int? SVID { get; set; }

		public DateTime? ServiceDate { get; set; }

		public int? LocationID { get; set; }

		public double? ReceivedHours { get; set; }

		[Required]
		[NotGreaterThanToday]
		[Display(Name = "Shelter Begin")]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
		public DateTime? ShelterBegDate { get; set; }

		[NotGreaterThanToday]
		[Display(Name = "Shelter End")]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
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