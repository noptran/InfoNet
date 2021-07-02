using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Infonet.Core.Entity.Validation;
using Infonet.Data.Helpers;

namespace Infonet.Web.ViewModels.Admin {
	//**************************************************************************//
	//Acronym Definitions
	//AFS = Assign Funding Source
	//FFS = Funding For Staff
	//MFS = Multi Fund Assignment
	//**************************************************************************//

	public class FundingForStaff {
		[Display(Name = "New Date")]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
		public IEnumerable<SelectListItem> AssignedStaffList { get; set; }
		public int? CurrentServiceProgramID { get; set; }
		public IEnumerable<SelectListItem> FundIssueDatesList { get; set; }
		[Display(Name = "New Date")]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
		public string NewFundIssueDate { get; set; }
		[Display(Name = "Last Date")]
		public DateTime? SelectedFundIssueDate { get; set; }
		public int SelectedFundIssueDateId { get; set; }
		public int SelectedStaffSVID { get; set; }
		public List<FFSStaffFundedSources> StaffFundedSources { get; set; }
		public FFSAdd FFSAdd { get; set; }
		public FFSEditStaffList FFSEditStaffList { get; set; }
		public FFSAssignServices FFSAssignServices { get; set; }
		public FFSMFA FFSMFA { get; set; }
		public FFSAFS FFSAFS { get; set; }
		public FFSReports FFSReports { get; set; }
	}

	public class FFSAdd {
		public IEnumerable<SelectListItem> FundIssueDatesList { get; set; }
		[Required]
		[BetweenNineteenSeventyToday]
		[Display(Name = "New Date")]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
		public DateTime? NewFundIssueDate { get; set; }
		[Display(Name = "Last Date")]
		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
		public DateTime? SelectedFundIssueDate { get; set; }
		public int SelectedFundIssueDateId { get; set; }
	}

	public class FFSEditStaffList {
		public MultiSelectList AvailableAndAssignedStaff { get; set; }
		[Display(Name = "Assigned/Available Staff")]
		public int?[] AvailableAndAssignedStaffSVIDs { get; set; }
	}

	public class FFSAssignServices {
		public List<FFSAssignedServices> AssignedServices { get; set; }
		public MultiSelectList AvailableAndAssignedServices { get; set; }
		[Display(Name = "Assigned/Available Programs/Services")]
		public int?[] AvailableAndAssignedServiceIDs { get; set; }
		public IEnumerable<SelectListItem> EmployeeToDuplicateID { get; set; }
		[Display(Name = "Match Services For")]
		public int SelectedEmployeeToDuplicateID { get; set; }
	}

	public class FFSMFA {
		public List<FundingSelection> FundingSelectionList { get; set; }
		public List<ServicesSelection> ServicesSelectionList { get; set; }
	}

	public class FFSAFS {
		public List<FundingSelection> FundingSelectionList { get; set; }
	}

	public class FFSReports {
		public MultiSelectList FundingDates { get; set; }
		[Display(Name = "Available Statements")]
		public int?[] FundingDateIDs { get; set; }
		public List<FFSReportFundingHistory> ReportDetails { get; set; }
		public string PDFAction { get; set; }
		public string ReportName { get; set; }
		public bool UseDefaultMasterPage { get; set; }
	}

	public class EmployeeToDuplicateID {
		public string Name { get; set; }
		public int? ProgramServiceID { get; set; }
	}

	public class FundingSelection : FFSStaffFundingSources {
		public bool IsChecked { get; set; }
	}

	public class ServicesSelection : FFSAvailableAndAssignedServices {

		public bool IsChecked { get; set; }
	}
}