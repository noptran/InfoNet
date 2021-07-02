using System;
using System.ComponentModel.DataAnnotations;
using Infonet.Core.Entity.Validation;

namespace Infonet.Web.ViewModels.Case {
	public class DepartureAdd {
		[Required]
		[NotGreaterThanToday]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
		[Display(Name = "Departure Date")]
		public DateTime? DepartureDate { get; set; }

		[Required]
		[Display(Name = "Destination")]
		public int? DestinationID { get; set; }

		[Display(Name = "Destination Tenure")]
		public int? DestinationTenureID { get; set; }

		[Display(Name = "Destination Subsidy")]
		public int? DestinationSubsidyID { get; set; }

		[Display(Name = "Reason for Leaving")]
		public int? ReasonForLeavingID { get; set; }

		public int DepartureID { get; set; }
		public int? Index { get; set; }
		public bool IsAdded { get; set; }
		public bool IsDeleted { get; set; }
	}
}