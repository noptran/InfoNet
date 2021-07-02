using System.ComponentModel.DataAnnotations;

namespace Infonet.Reporting.Enumerations {
	public enum OrderOfProtectionIssuedOrExpiredSelectionsEnum {
		[Display(Name = "Date Issued")]
		DateIssued,
		[Display(Name = "Date Expired")]
		DateExpired
	}
}
