using System;
using System.ComponentModel.DataAnnotations;

namespace Infonet.Web.ViewModels.Case {
	public class CancellationsSearchDates {
		[DataType(DataType.Date)]
		[Display(Name = "Start Date")]
		public DateTime? CancellationsDateRangeStart { get; set; }

		[DataType(DataType.Date)]
		[Display(Name = "End Date")]
		public DateTime? CancellationsDateRangeEnd { get; set; }

		public int? Page { get; set; }

		public int? PageSize { get; set; }
	}
}