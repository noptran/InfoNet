using System;
using System.ComponentModel.DataAnnotations;

namespace Infonet.Web.ViewModels.Case {
	public class ReferralSearchDates {
		[DataType(DataType.Date)]
		[Display(Name = "Start Date")]
		public DateTime? ReferralDateRangeStart { get; set; }

		[DataType(DataType.Date)]
		[Display(Name = "End Date")]
		public DateTime? ReferralDateRangeEnd { get; set; }

		public int? Page { get; set; }

		public int? PageSize { get; set; }
	}
}