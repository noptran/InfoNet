using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Infonet.Core.Entity.Validation;
using Infonet.Data.Helpers;
using Infonet.Reporting.Enumerations;
using Infonet.Data.Looking;

namespace Infonet.Reporting.ViewModels {
	public class ExceptionReportViewModel : IReportCenterSelectionBase {
		public ExceptionReportViewModel() {
			PdfSize = PdfSize.Letter;
			Orientation = PdfOrientation.Portrait;
			OpenCases = 60;
			ShelterDaysExceed = 60;
			AvailableSelectionTypes = new List<SubReportSelection>();
		}

		public Provider Provider { get; set; }

		// Dates
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }

		// Output Type
		[Display(Name = "Output Type")]
		public ReportOutputType OutputType { get; set; }
		public PdfSize PdfSize { get; set; }
		public PdfOrientation Orientation { get; set; }

		// Center Slection Properties
		public List<CenterInfo> Centers { get; set; }
		public int?[] CenterIds { get; set; }

		// Report Selection Properties
		public SubReportSelection ReportSelectionType { get; set; }
		public List<SubReportSelection> AvailableSelectionTypes { get; set; }

		// Lengthy Shelter
		[Display(Name = "Shelter Days Exceed")]
		[Range(1,9999)]
		public int? ShelterDaysExceed { get; set; }

		// Open Cient Cases
		[Display(Name = "Last Date of Service Exceeds (Days)")]
		[Range(1, 9999)]
		public int? OpenCases { get; set; }

		// Unknown, Not Reported, Unassigned
		public UNRUDataFieldsEnum[] UNRUDataFieldsSelections { get; set; }
		public List<UNRUDataFieldsEnum> AvailableUNRUDataFieldsSelections { get; set; }

		// Date Range Properties
		[BetweenNineteenSeventyToday]
		[Display(Name="Start Date")]
		public DateTime? UNRUStartDate { get; set; }
		[BetweenNineteenSeventyToday]
		[Display(Name="End Date")]
		public DateTime? UNRUEndDate { get; set; }
		public string UNRURange { get; set; }

		// Date Range Properties
		[BetweenNineteenSeventyToday]
		[Display(Name="Start Date")]
		public DateTime? OffenderStartDate { get; set; }
		[BetweenNineteenSeventyToday]
		[Display(Name="End Date")]
		public DateTime? OffenderEndDate { get; set; }
		public string OffenderRange { get; set; }
	}
}