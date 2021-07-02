using System;
using System.ComponentModel.DataAnnotations;

namespace Infonet.Web.ViewModels.Shared {
	public interface IDateRange {
		[DataType(DataType.Date)]
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
		DateTime? StartDate { get; set; }

		[DataType(DataType.Date)]
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
		DateTime? EndDate { get; set; }

		string Range { get; set; }
	}
}
