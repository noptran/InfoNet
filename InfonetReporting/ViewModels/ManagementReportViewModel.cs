using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Infonet.Data.Helpers;
using Infonet.Data.Looking;
using Infonet.Reporting.Core;
using Infonet.Reporting.Enumerations;
using Infonet.Reporting.ViewModels.Validation;

namespace Infonet.Reporting.ViewModels {
	public class ManagementReportViewModel : IReportCenterSelectionBase, IDateRangeWithOutputType {
		public ManagementReportViewModel() {
			PdfSize = PdfSize.Letter;
			Orientation = PdfOrientation.Portrait;

			AvailableSelections = new Dictionary<ReportSelection, List<SubReportSelection>>();
			StaffDefault = new List<StaffModel>();
			FundingSourcesDefault = new List<ReportLookup>();
			GenderDefault = new List<LookupCode>();
			EthnicityDefault = new List<LookupCode>();
			RaceDefault = new List<LookupCode>();
			CityOrTownsDefault = new List<ReportLookup>();
			TownshipsDefault = new List<ReportLookup>();
			CountiesDefault = new List<ReportLookup>();
			ZipcodesDefault = new List<ReportLookup>();
			StatesDefault = new List<ReportLookup>();
			ServiceNameDefault = new List<ReportLookup>();
			ClientTypeDefault = new List<ReportLookup>();
			ShelterTypeDefault = new List<ReportLookup>();
			AgencyDefault = new List<ReportLookup>();
			ServiceLocationDefault = new List<ReportLookup>();
			MediaPubTypeDefault = new List<ReportLookup>();
			HotlineTypeDefault = new List<ReportLookup>();
			ActivitiesDefault = new List<LookupCode>();
			ReferralMadeDefault = new List<LookupCode>();
			ClientDetailAvailableColumnSelections = new List<ReportColumnSelectionsEnum>();
			ClientDetailAvailableOrderSelections = new List<ReportOrderSelectionsEnum>();
			StaffClientServiceAvailableColumnSelections = new List<ReportColumnSelectionsEnum>();
			StaffClientServiceAvailableOrderSelections = new List<ReportOrderSelectionsEnum>();
			StaffHotlineAvailableColumnSelections = new List<ReportColumnSelectionsEnum>();
			StaffHotlineAvailableOrderSelections = new List<ReportOrderSelectionsEnum>();
			StaffReportAgeSetAvailableSelections = new List<StaffReportAgeSetSelectionsEnum>();
			OtherStaffActivityAvailableColumnSelections = new List<ReportColumnSelectionsEnum>();
			CancellationAvailableColumnSelections = new List<ReportColumnSelectionsEnum>();
		}

		public Provider Provider { get; set; }

		// Date Range Properties
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public string Range { get; set; }

		// Output Type
		public ReportOutputType OutputType { get; set; }
		public PdfSize PdfSize { get; set; }
		public PdfOrientation Orientation { get; set; }

		// Center Slection Properties
		public List<CenterInfo> Centers { get; set; }
		public int?[] CenterIds { get; set; }

		// Report Selection Properties
		[Required]		
		public SubReportSelection ReportSelection { get; set; }
		public Dictionary<ReportSelection, List<SubReportSelection>> AvailableSelections { get; set; }

		////////////////////////
		/// Filter Properties //
		////////////////////////

		// LISTS //

		// Client Filters
		public List<ReportLookup> ClientTypeDefault { get; set; }
		public List<ReportLookup> ShelterTypeDefault { get; set; }
		public List<LookupCode> GenderDefault { get; set; }
		public List<LookupCode> EthnicityDefault { get; set; }
		public List<LookupCode> RaceDefault { get; set; }
		public List<ReportLookup> CityOrTownsDefault { get; set; }
		public List<ReportLookup> TownshipsDefault { get; set; }
		public List<ReportLookup> CountiesDefault { get; set; }
		public List<ReportLookup> ZipcodesDefault { get; set; }
		public List<ReportLookup> StatesDefault { get; set; }

		// Service Filters
		public List<ReportLookup> ServiceNameDefault { get; set; }
		public List<StaffModel> StaffDefault { get; set; }
		public List<ReportLookup> FundingSourcesDefault { get; set; }
		public List<ReportLookup> AgencyDefault { get; set; }
		public List<ReportLookup> ServiceLocationDefault { get; set; }
		public List<ReportLookup> MediaPubTypeDefault { get; set; }
		public List<ReportLookup> HotlineTypeDefault { get; set; }

		// Other Filters
		public List<LookupCode> ActivitiesDefault { get; set; }
		public List<LookupCode> ReferralMadeDefault { get; set; }

		// SELECTIONS //

		////////
		// Client Detail Information
		////////

		public List<ReportColumnSelectionsEnum> ClientDetailAvailableColumnSelections { get; set; }
		public ReportColumnSelectionsEnum[] ClientDetailColumnSelections { get; set; }
		public List<ReportOrderSelectionsEnum> ClientDetailAvailableOrderSelections { get; set; }
		public ReportOrderSelectionsEnum ClientDetailOrderSelection { get; set; }
		[Display(Name = "Client ID")]
		public string ClientDetailClientCodeSelection { get; set; }
		[Display(Name = "Client Type")]
		public int?[] ClientDetailClientTypeSelections { get; set; }
		[Display(Name = "Service Name")]
		public int?[] ClientDetailServiceSelections { get; set; }
		[Display(Name = "Age Range")]
		public int? ClientDetailMinimumAge { get; set; }
		public int? ClientDetailMaximumAge { get; set; }
		[Display(Name = "Gender")]
		public int?[] ClientDetailGenderSelections { get; set; }
		[Display(Name = "Ethnicity")]
		public int?[] ClientDetailEthnicitySelections { get; set; }
		[Display(Name = "Race")]
		public int?[] ClientDetailRaceSelections { get; set; }
		[Display(Name = "City or Town")]
		public string[] ClientDetailCityOrTownSelections { get; set; }
		[Display(Name = "Township")]
		public string[] ClientDetailTownshipSelections { get; set; }
		[Display(Name = "County")]
		public int?[] ClientDetailCountySelections { get; set; }
		[Display(Name = "Zip Code")]
		public string[] ClientDetailZipCodeSelections { get; set; }
		[Display(Name = "State")]
		public int?[] ClientDetailStateSelections { get; set; }

		////////
		// Income Source Management
		////////
		[Display(Name = "Client Type")]
		public int?[] IncomeSourceShelterTypeSelections { get; set; }
		public IncomeSourceIncomeRange[] IncomeSourceIncomeRanges { get; set; }

		public class IncomeSourceIncomeRange
		{
			public decimal? LowerBound { get; set; }

			public decimal? UpperBound { get; set; }
		}

		////////
		// Staff/Client Service Information 
		////////

		public List<ReportColumnSelectionsEnum> StaffClientServiceAvailableColumnSelections { get; set; }
		[ReportSpecificRequirement(SubReportSelection.MngRptStaffServiceServiceInformation, "Must select a column to display")]
		public ReportColumnSelectionsEnum[] StaffClientServiceColumnSelections { get; set; }
		public List<ReportOrderSelectionsEnum> StaffClientServiceAvailableOrderSelections { get; set; }
		public RecordDetailOrderSelectionsEnum StaffClientServiceRecordDetailOrderSelection { get;set;}
		[Display(Name = "Staff Name")]
		public int?[] StaffClientServiceStaffSelections { get; set; }
		[Display(Name = "Service Name")]
		public int?[] StaffClientServiceServiceNameSelections { get; set; }
		[Display(Name = "Funding Source")]
		public int?[] StaffClientServiceFundingSourceSelections { get; set; }

		////////
		// Staff/Community, Institutional & Group Services 
		////////
		[Display(Name = "Staff Name")]
		public int?[] StaffCommGroupServiceStaffSelections { get; set; }
		[Display(Name = "Service Name")]
		public int?[] StaffCommGroupServiceServiceNameSelections { get; set; }
		[Display(Name = "Agency")]
		public int?[] StaffCommGroupServiceAgencySelections { get; set; }
		[Display(Name = "Location")]
		public string[] StaffCommGroupServiceLocationSelections { get; set; }

		////////
		// Staff/Media/Publication
		////////
		[Display(Name = "Staff Name")]
		public int?[] StaffMediaPublicationStaffSelections { get; set; }
		[Display(Name = "Media/Publication Type")]
		public int?[] StaffMediaPublicationPublicationTypeSelections { get; set; }
		[Display(Name = "Location")]
		public string[] StaffMediaPublicationLocationSelections { get; set; }

		////////
		// Staff/Hotline Call
		////////
		public List<ReportColumnSelectionsEnum> StaffHotlineAvailableColumnSelections { get; set; }
		public ReportColumnSelectionsEnum[] StaffHotlineColumnSelections { get; set; }
		public List<ReportOrderSelectionsEnum> StaffHotlineAvailableOrderSelections { get; set; }
		public ReportOrderSelectionsEnum StaffHotlineOrderSelection { get; set; }
		[Display(Name = "Staff Name")]
		public int?[] StaffHotlineStaffSelections { get; set; }
		[Display(Name = "Hotline Type")]
		public int?[] StaffHotlineHotlineTypeSelections { get; set; }
		[Display(Name = "City or Town")]
		public string[] StaffHotlineCitySelections { get; set; }
		[Display(Name = "Township")]
		public string[] StaffHotlineTownshipSelections { get; set; }
		[Display(Name = "County")]
		public int?[] StaffHotlineCountySelections { get; set; }
		[Display(Name = "Zip Code")]
		public string[] StaffHotlineZipCodeSelections { get; set; }

		////////
		// Staff Report
		////////
		public List<StaffReportAgeSetSelectionsEnum> StaffReportAgeSetAvailableSelections { get; set; }
		public StaffReportAgeSetSelectionsEnum StaffReportAgeSetSelection { get; set; }
		[Display(Name = "Staff Name")]
		public int?[] StaffReportStaffSelections { get; set; }

		////////
		// Other Staff Activity
		////////

		public List<ReportColumnSelectionsEnum> OtherStaffActivityAvailableColumnSelections { get; set; }
		public ReportColumnSelectionsEnum[] OtherStaffActivityColumnSelections { get; set; }
		public RecordDetailOrderSelectionsEnum OtherStaffActivityRecordDetailOrderSelection { get; set; }
		[Display(Name = "Staff Name")]
		public int?[] OtherStaffActivityStaffSelections { get; set; }
		[Display(Name = "Activity Name")]
		public int?[] OtherStaffActivityActivitySelections { get; set; }

		////////
		// Turn Away
		////////

		[Display(Name = "Referral Made to Another Shelter")]
		public int?[] TurnAwayReferralSelections { get; set; }

		////////
		// Other Staff Activity
		////////
		public List<ReportColumnSelectionsEnum> CancellationAvailableColumnSelections { get; set; }
		public ReportColumnSelectionsEnum[] CancellationColumnSelections { get; set; }
		[Display(Name = "Staff Name")]
		public int?[] CancellationStaffSelections { get; set; }
		[Display(Name = "Service Name")]
		public int?[] CancellationServiceSelections { get; set; }
		[Display(Name = "Reason Name")]
		public int?[] CancellationReasonSelections { get; set; }

		////////
		// Order of Protection
		////////

		public OrderOfProtectionIssuedOrExpiredSelectionsEnum OrderOfProtectionDateSelection { get; set; }		
	}
}