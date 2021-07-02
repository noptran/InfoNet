using System;
using System.ComponentModel.DataAnnotations;
using Infonet.Core.Entity.Validation;
using Infonet.Data.Looking;

namespace Infonet.Web.ViewModels.Case {
	public class HousingServicesAdd {
		public HousingServicesAdd() {
			IsEmpty = true;
		}

		public int? ServiceDetailID { get; set; }
		[Lookup("HousingServices")]
		public int? ServiceID { get; set; }
		public int? SVID { get; set; }
		public DateTime? ServiceDate { get; set; }
		public int? LocationID { get; set; }
		public double? ReceivedHours { get; set; }

		[NotGreaterThanToday]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
		[Display(Name = "Shelter/Tran. Housing Begin")]
		public DateTime? ShelterBegDate { get; set; }

		[NotGreaterThanToday]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
		[Display(Name = "Shelter/Tran Housing End")]
		public DateTime? ShelterEndDate { get; set; }

		[Display(Name = "Staff")]
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