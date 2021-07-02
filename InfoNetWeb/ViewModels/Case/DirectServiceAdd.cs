using System;
using System.ComponentModel.DataAnnotations;
using Infonet.Core.Entity.Validation;
using Infonet.Data.Looking;

namespace Infonet.Web.ViewModels.Case {
	public class DirectServiceAdd {
		public DirectServiceAdd() {
			IsEmpty = true;
		}

		public int? ServiceDetailID { get; set; }

		[Display(Name = "Service")]
		[Lookup("DirectServices")]
		public int? ServiceID { get; set; }

		[Display(Name = "Staff/Volunteer")]
		public int? SVID { get; set; }

		[NotGreaterThanToday]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
		[Display(Name = "Date")]
		public DateTime? ServiceDate { get; set; }

		public int? LocationID { get; set; }

		[QuarterIncrement]
		[Display(Name = "Hours")]
		public double? ReceivedHours { get; set; }

		public DateTime? ShelterBegDate { get; set; }
		public DateTime? ShelterEndDate { get; set; }
		public string StaffName { get; set; }
		public string Service { get; set; }
		public string Location { get; set; }
		public int? ICS_ID { get; set; }
		public int? Index { get; set; }
		public bool IsAdded { get; set; }
		public bool IsDeleted { get; set; }
		public bool IsEmpty { get; set; }
	}
}