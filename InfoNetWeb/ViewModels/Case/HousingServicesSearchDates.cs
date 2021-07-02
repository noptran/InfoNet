using System;
using System.ComponentModel.DataAnnotations;

namespace Infonet.Web.ViewModels.Case {
	public class HousingServicesSearchDates {
		[DataType(DataType.Date)]
		[Display(Name = "Start Date")]
		public DateTime? HousingServiceDateRangeStart { get; set; }

		[DataType(DataType.Date)]
		[Display(Name = "End Date")]
		public DateTime? HousingServiceDateRangeEnd { get; set; }

		public int? Page { get; set; }

		public int? PageSize { get; set; }
	}
}