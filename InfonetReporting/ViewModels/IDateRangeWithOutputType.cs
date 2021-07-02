using System;
using System.ComponentModel.DataAnnotations;
using Infonet.Reporting.Enumerations;

namespace Infonet.Reporting.ViewModels {
	public interface IDateRangeWithOutputType {
		[Display(Name = "Output Type")]
		ReportOutputType OutputType { get; set; }

		[Required]
		[DataType(DataType.Date)]
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
		[Display(Name = "Start Date")]
		DateTime? StartDate { get; set; }

		[Required]
		[DataType(DataType.Date)]
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
		[Display(Name = "End Date")]
		DateTime? EndDate { get; set; }

		string Range { get; set; }

		[Display(Name = "Page Size")]
		PdfSize PdfSize { get; set; }

		PdfOrientation Orientation { get; set; }
	}
}