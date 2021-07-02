using System;
using System.ComponentModel.DataAnnotations;
using Infonet.Core.Entity.Validation;
using Infonet.Data.Looking;

namespace Infonet.Web.ViewModels.Case {
	public class ReferralEdit {
		public int ReferralDetailID { get; set; }

		[Required]
		[NotGreaterThanToday]
		[Display(Name = "Referral Date")]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
		public DateTime? ReferralDate { get; set; }

		[Required]
		[Display(Name = "Referral Type")]
		[Lookup("ReferralType")]
		public int? ReferralTypeID { get; set; }

		[Required]
		[Display(Name = "Agency")]
		public int? AgencyID { get; set; }

		[Required]
		[Display(Name = "Response")]
		[Lookup("ReferralResponse")]
		public int? ResponseID { get; set; }

		public int? LocationID { get; set; }

		[Display(Name = "Edit")]
		public bool IsEdited { get; set; }

		public bool IsDeleted { get; set; }
		public int? Index { get; set; }
	}
}