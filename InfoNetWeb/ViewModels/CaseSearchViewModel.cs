using System;
using System.ComponentModel.DataAnnotations;
using Infonet.Data.Looking;
using Infonet.Web.ViewModels.Shared;
using PagedList;

namespace Infonet.Web.ViewModels {
	public class CaseSearchViewModel : PagedListPagination {
		public CaseSearchViewModel() {
			StartDate = DateTime.Today.AddMonths(-3).Date;
			EndDate = DateTime.Today.Date;
			Range = "13";
			PageSize = 10;
		}

        [MaxLength(50, ErrorMessageResourceName = "StringMaxLengthMessage", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Client ID")]
		public string ClientCode { get; set; }

		[Range(-1, 120)]
		[Display(Name = "Age at First Contact")]
		public int? Age { get; set; }

		[Lookup("ClientType")]
		public int? ClientTypeId { get; set; }

		public IPagedList<SearchResults> SearchList { get; set; }
		public class SearchResults {
			// From ClientCase.cs
			public int? ClientId { get; set; }
			public int? CaseId { get; set; }
			public int? Age { get; set; }

			[DataType(DataType.Date)]
			[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
			[Display(Name = "First Contact")]
			public DateTime? FirstContactDate { get; set; }

			public int? CaseClosed { get; set; }

			// From Client.cs 
			[Display(Name = "Type")]
			[Required]
			public int? ClientTypeId { get; set; }

            [MaxLength(50, ErrorMessageResourceName = "StringMaxLengthMessage", ErrorMessageResourceType = typeof(Resource))]
            [Display(Name = "Client ID")]
			public string ClientCode { get; set; }

			[Display(Name = "Gender")]
			public int? GenderIdentityId { get; set; }
		}
	}
}