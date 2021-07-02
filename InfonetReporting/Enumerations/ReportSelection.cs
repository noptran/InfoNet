using System.ComponentModel.DataAnnotations;

namespace Infonet.Reporting.Enumerations { 
	public enum ReportSelection { 
		[Display(Name = "Client Information")]
		StdRptClientInformation = 1,
		[Display(Name = "Medical/CJ Process")]
		StdRptMedicalCjProcess = 2,
		[Display(Name = "Service/Programs")]
		StdRptServicePrograms = 3,
		[Display(Name = "Investigation")]
		StdRptInvestigationInformation = 4,
		[Display(Name = "Client Reports")]
		MngRptClient = 5,
		[Display(Name = "Staff & Service Reports")]
		MngRptStaffService = 6,
		[Display(Name = "Other Reports")]
		MngRptOther = 7
	}	
}