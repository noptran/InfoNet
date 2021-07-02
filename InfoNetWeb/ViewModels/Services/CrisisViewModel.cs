using System;
using System.ComponentModel.DataAnnotations;
using Infonet.Core.Entity.Validation;
using Infonet.Data.Looking;

namespace Infonet.Web.ViewModels.Services {
	public class CrisisViewModel {
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

		[Display(Name = "Type of Intervention")]
		[Required]
		[Lookup("HotlineCallType")]
		public int? CallTypeID { get; set; }

		[Display(Name = "Non-Client Type")]
		[Lookup("ClientType")]
		public int? ClientTypeID { get; set; }

		[Display(Name = "Number of Contacts")]
		[WholeNumber]
		[Range(0, 999)]
		[Required]
		public int? NumberOfContacts { get; set; }

		[Display(Name = "Age")]
		//[Age]
		[WholeNumber]
		[Range(-1, 120)]
		public int? Age { get; set; }

		[Display(Name = "Race/Ethnicity")]
		[Lookup("Race")]
		public int? RaceID { get; set; }

		[Display(Name = "Gender Identity")]
		[Lookup("Sex")]
		public int? SexID { get; set; }

		[Display(Name = "Town")]
		public string Town { get; set; }

		[Display(Name = "Township")]
		public string Township { get; set; }

		[Display(Name = "Zip Code")]
		[RegularExpression(@"^\d{5}(?:[-]\d{4})?$", ErrorMessage = "Invalid Zip Code format")]
		public string ZipCode { get; set; }

		[Display(Name = "County")]
		public int? CountyID { get; set; }

		[Display(Name = "Total Time")]
		[Range(0, 999)]
		[QuarterIncrement]
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