using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Infonet.Data.Helpers;
using Infonet.Data.Models.Reporting;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports;

namespace Infonet.Reporting.ViewModels {
	public class SubmitReportViewModel : StandardReportSpecification, IReportCenterSelectionBase, IDateRangeWithOutputType, ICloneable {
        public SubmitReportViewModel() {
            CityOrTownsDefault = new List<FilterSelection>();
            ClientTypeDefault = new List<FilterSelection>();
            RaceDefault = new List<FilterSelection>();
            IsFilterCollapsed = false;
        }

        public List<ReportEmailList> reportEmailList = new List<ReportEmailList>();

		public string Range { get; set; }

		public ReportOutputType OutputType { get; set; }

		public PdfSize PdfSize { get; set; }

		public PdfOrientation Orientation { get; set; }

		public List<CenterInfo> Centers { get; set; }

		public Dictionary<ReportSelection, List<SubReportSelection>> AvailableSelections { get; set; }

		public List<FundingFilterOptions> FundingFilter { get; set; }

		[Display(Name = "Run Date")]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
		public DateTime RunDate { get; set; }

		public string FundingFilterRadio { get; set; }

		public string CenterSelectionRadio { get; set; }

		public int[] ReportTypes { get; set; }

		public string SubReportGroup1 { get; set; }

		public string SubReportGroup2 { get; set; }

		public string SubReportGroup3 { get; set; }

		public int ApprovalCenterId { get; set; }

        public bool IsFilterCollapsed { get; set; } 

		[Display(Name = "Center Actions")]
		public int CenterAction { get; set; }

		public class FundingFilterOptions {
			public int? CodeId { get; set; }
			public string Description { get; set; }
			public bool IsChecked { get; set; }
		}

		public class ReportEmailList {
			public int?[] CenterIds { get; set; }
			public ReportJob ReportJob { get; set; }
			public string SpecificationJson { get; set; }
		}

        public List<FilterSelection> CityOrTownsDefault { get; set; }
        public List<FilterSelection> ClientTypeDefault { get; set; }
        public List<FilterSelection> RaceDefault { get; set; }

        public object Clone() {
            return MemberwiseClone();
        }
    }
}