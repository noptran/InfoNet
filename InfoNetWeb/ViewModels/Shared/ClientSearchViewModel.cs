using System;
using System.ComponentModel.DataAnnotations;

namespace Infonet.Web.ViewModels.Shared {
	public class ClientSearchViewModel {

		public ClientSearchViewModel(bool useClientSearchOptions, bool useServiceSearchOptions = false) {
			useServiceSearch = useServiceSearchOptions;
			useClientSearch = useClientSearchOptions;
			ServiceDateRangeTooltip = "If a client has attended a Group Service in the past, enter the Group Service date range here to find potential clients. Remember, a client's ID number will appear in the search results only if they received a GROUP service in the past, not just any service.";
			FCDDateRangeTooltip = "If this was a client's first group service ever, and has not attended a group service prior to this session, you may search their First Contact Date range to find the client. Make sure you remove the Group Service Date search criteria if you only want to search clients by their First Contact Date.";
			ClientIDTooltip = "Enter all or part of a Client ID to search for available clients according to the characters in their Client ID number.";
			ClientTypeTooltip = "Select the client type from the drop-down menu to search for clients by type.";
			RangeTooltip = "Select a date range to change the values of the date ranges.";
		}

		#region Service Search Options
		public bool useServiceSearch { get; set; }

		[DataType(DataType.Date)]
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
		[Display(Name = "Service Date Range")]
		public DateTime? ServiceStart { get; set; }

		[DataType(DataType.Date)]
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
		public DateTime? ServiceEnd { get; set; }
		[Display(Name = "Range")]
		public string ServiceRange { get; set; }

		public string ServiceDateRangeTooltip { get; set; }
		#endregion

		#region Client Search Options

		public bool useClientSearch { get; set; }

		[DataType(DataType.Date)]
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
		[Display(Name = "First Contact Date Range")]
		public DateTime? FCD_StartDate { get; set; }

		[DataType(DataType.Date)]
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
		public DateTime? FCD_EndDate { get; set; }
		[Display(Name = "Range")]
		public string FCDRange { get; set; }

		public string FCDDateRangeTooltip { get; set; }
		[Display(Name = "Client ID")]
		public int? ClientID { get; set; }

		public string ClientIDTooltip { get; set; }
		[Display(Name = "Client Type")]
		public int? ClientType { get; set; }

		public string ClientTypeTooltip { get; set; }

		public string RangeTooltip { get; set; }
		#endregion

	}
}