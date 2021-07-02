using System;
using System.ComponentModel.DataAnnotations;
using Infonet.Core.Entity.Validation;
using Infonet.Data.Looking;

namespace Infonet.Web.ViewModels.Case {
	public class ReferralAdd {
		public ReferralAdd() {
			IsEmpty = true;
		}

		public int ReferralDetailID { get; set; }

		[NotGreaterThanToday]
		[Display(Name = "Referral Date")]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
		public DateTime? ReferralDate { get; set; }

		[Display(Name = "Referral Type")]
		[Lookup("ReferralType")]
		public int? ReferralTypeID { get; set; }

		[Display(Name = "Agency")]
		public int? AgencyID { get; set; }

		[Display(Name = "Response")]
		[Lookup("ReferralResponse")]
		public int? ResponseID { get; set; }

		public int? LocationID { get; set; }
		public int? Index { get; set; }
		public bool IsAdded { get; set; }
		public bool IsDeleted { get; set; }
		public bool IsEmpty { get; set; }
	}
}