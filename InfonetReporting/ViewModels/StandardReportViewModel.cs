using System.Collections.Generic;
using Infonet.Data.Helpers;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.StandardReports;

namespace Infonet.Reporting.ViewModels {
	public class StandardReportViewModel : StandardReportSpecification, IReportCenterSelectionBase, IDateRangeWithOutputType {
		public StandardReportViewModel() {
			PdfSize = PdfSize.Letter;
			Orientation = PdfOrientation.Portrait;

			StaffDefault = new List<FilterSelection>();
			OffenderRelationshipDefault = new List<FilterSelection>();
			FundingSourcesDefault = new List<FilterSelection>();
			GenderDefault = new List<FilterSelection>();
			EthnicityDefault = new List<FilterSelection>();
			RaceDefault = new List<FilterSelection>();
			CityOrTownsDefault = new List<FilterSelection>();
			TownshipsDefault = new List<FilterSelection>();
			CountiesDefault = new List<FilterSelection>();
			ZipcodesDefault = new List<FilterSelection>();
			StatesDefault = new List<FilterSelection>();
			ServiceDefault = new List<FilterSelection>();
		}

		// Date Range
		public string Range { get; set; }

		public ReportOutputType OutputType { get; set; }

		public PdfSize PdfSize { get; set; }  //KMS DO why not just set defaults here and get rid of javascript?

		public PdfOrientation Orientation { get; set; }

		public List<CenterInfo> Centers { get; set; }

		public List<ReportSelection> AvailableSelectionTypes { get; set; }

		public Dictionary<ReportSelection, List<SubReportSelection>> AvailableSelections { get; set; }

		public List<FilterSelection> StaffDefault { get; set; }

		public List<FilterSelection> ServiceDefault { get; set; }

		public List<FilterSelection> OffenderRelationshipDefault { get; set; }

		public List<FilterSelection> FundingSourcesDefault { get; set; }

		public List<FilterSelection> GenderDefault { get; set; }

		public List<FilterSelection> EthnicityDefault { get; set; }

		public List<FilterSelection> RaceDefault { get; set; }

		public List<FilterSelection> CityOrTownsDefault { get; set; }

		public List<FilterSelection> TownshipsDefault { get; set; }

		public List<FilterSelection> CountiesDefault { get; set; }

		public List<FilterSelection> ZipcodesDefault { get; set; }

		public List<FilterSelection> StatesDefault { get; set; }

		public List<FilterSelection> ClientTypeDefault { get; set; }
	}
}