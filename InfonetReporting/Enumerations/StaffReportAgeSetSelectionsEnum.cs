using System.ComponentModel.DataAnnotations;

namespace Infonet.Reporting.Enumerations {
	public enum StaffReportAgeSetSelectionsEnum {
		[Display(Name = "<b>Set 1:</b>&nbsp;&nbsp;&nbsp;< 12; 13-17; 18-29; 30-44; 45-64; 65+")]
		UnderTwelveOverSixtyFive,
		[Display(Name = "<b>Set 2:</b>&nbsp;&nbsp;&nbsp;< 1; 1-3; 4-7; 8-9; 10-14; 15-17; 18-19; 20-29; 30-39; 40-49; 50-59; 60-64; 65+")]
		UnderOneOverSixtyFive
	}
}
