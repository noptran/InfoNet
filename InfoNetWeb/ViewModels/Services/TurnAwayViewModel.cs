using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Infonet.Core.Entity;
using Infonet.Core.Entity.Validation;
using Infonet.Data.Looking;
using Infonet.Web.ViewModels.Shared;
using PagedList;

namespace Infonet.Web.ViewModels.Services {
	public class TurnAwayViewModel : PagedListPagination {

		public TurnAwayViewModel() {
			StartDate = DateTime.Today.AddMonths(-1).Date;
			EndDate = DateTime.Today.Date;
			Range = "20";
			PageSize = 10;
		}

		public IPagedList<TurnAwaysSearchResult> TurnAwaysList { get; set; }
		public List<TurnAwaysSearchResult> displayForPaging { get; set; }

		public class TurnAwaysSearchResult : IRevisable {
			public int? Id { get; set; }
			[Required]
			[DataType(DataType.Date)]
			[BetweenNineteenSeventyToday]
			[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
			[Display(Name = "Date")]
			public DateTime? TurnAwayDate { get; set; }

			[Required]
			[Range(0, 300, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
			[Display(Name = "Number of Adults")]
			public int? AdultsNo { get; set; }

			[Required]
			[Range(0, 300, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
			[Display(Name = "Number of Children")]
			public int? ChildrenNo { get; set; }

			/* FK actually to TLU_Codes_YesNo (of which YesNo2 is subset) */
			[Required]
			[Lookup("YesNo2")]
			[Display(Name = "Referral Made to Another Shelter")]
			public int? ReferralMadeId { get; set; }

			public bool shouldDelete { get; set; }

			public bool shouldEdit { get; set; }

			public DateTime? RevisionStamp { get; set; }
		}
	}
}