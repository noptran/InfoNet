using System.ComponentModel.DataAnnotations;

namespace Infonet.Reporting.Enumerations {
	public enum ReportOrderSelectionsEnum {
		[Display(Name = "Town")]
		Town = 1,
		[Display(Name = "Township")]
		Township,
		[Display(Name = "County")]
		County,
		[Display(Name = "Zip Code")]
		ZipCode,
		[Display(Name = "State")]
		State,
		[Display(Name = "Record Detail")]
		RecordDetail,
		[Display(Name = "Group By")]
		GroupBy,
		[Display(Name = "Date Issued")]
		DateIssued,
		[Display(Name = "Date Expired")]
		DateExpired,
		[Display(Name = "Staff Name")]
		Staff,
		[Display(Name = "Client")]
		Client,
		[Display(Name = "Service Name")]
		Service,
		[Display(Name = "Referral Made")]
		ReferralMade,
		[Display(Name = "Activity Name")]
		Activity
	}
}
