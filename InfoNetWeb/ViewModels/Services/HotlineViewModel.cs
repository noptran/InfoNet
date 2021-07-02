using System;
using System.ComponentModel.DataAnnotations;
using Infonet.Core.Entity.Validation;
using Infonet.Data.Looking;

namespace Infonet.Web.ViewModels.Services {
	public class HotlineViewModel {
		// Model Properties
		public int? PH_ID { get; set; }

		[Display(Name = "Staff/Volunteer")]
		public int? SVID { get; set; }

		[Required]
		[BetweenNineteenSeventyToday]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
		[Display(Name = "Date")]
		public DateTime? Date { get; set; }

		[Display(Name = "Call Type")]
		[Lookup("HotlineCallType")]
		[Required]
		public int? CallTypeID { get; set; }

		[Display(Name = "Number of Contacts")]
		[WholeNumber]
		[Range(1, 999)]
		[Required]
		public int? NumberOfContacts { get; set; }

		public string Town { get; set; }

		public string Township { get; set; }

		[Display(Name = "Zip Code")]
		[RegularExpression(@"^\d{5}(?:[-]\d{4})?$", ErrorMessage = "Invalid Zip Code format")]
		public string ZipCode { get; set; }

		[Display(Name = "County")]
		public int? CountyID { get; set; }

		[Display(Name = "Time of Day")]
		[DataType(DataType.Time)]
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
		public DateTime? TimeOfDay { get; set; }

		[Display(Name = "Total Time (minutes)")]
		[Range(0, 99999)]
		[WholeNumber]
		public double? TotalTime { get; set; }

		[Display(Name = "Referred From")]
		[Lookup("ReferralSource")]
		public int? ReferralFromID { get; set; }

		[Display(Name = "Referred To")]
		[Lookup("ReferralSource")]
		public int? ReferralToID { get; set; }

		// View Specific Properties
		public int saveAddNew { get; set; }

		public string ReturnURL { get; set; }
	}
}