using System;
using System.ComponentModel.DataAnnotations;

namespace Infonet.Web.ViewModels.Case {
	public class DirectServicesSearchDates {
		[DataType(DataType.Date)]
		[Display(Name = "Start Date")]
		public DateTime? DirectServicesDateRangeStart { get; set; }

		[DataType(DataType.Date)]
		[Display(Name = "End Date")]
		public DateTime? DirectServicesDateRangeEnd { get; set; }

		public int? Page { get; set; }

		public int? PageSize { get; set; }
	}
}