using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Infonet.Core.Entity.Validation;
using Infonet.Data.Looking;
using Infonet.Web.ViewModels.Shared;
using PagedList;

namespace Infonet.Web.ViewModels.Clients {
	public class AggregateInformationViewModel : PagedListPagination {
		public AggregateInformationViewModel() {
			Range = "20";
			StartDate = DateTime.Now.AddDays(-30);
			EndDate = DateTime.Now;
			PageSize = 10;
		}

		[Display(Name = "Information Type")]
		public int? TypeID { get; set; }

		public IPagedList<AggregateInformationSearchResult> AggregateList { get; set; }

		public List<AggregateInformationSearchResult> displayForPaging { get; set; }

		public class AggregateInformationSearchResult {
			[Required]
			[Display(Name = "Type of Information")]
			[Lookup("HivMentalSubstance")]
			public int TypeID { get; set; }

			public string TypeName { get; set; }

			[Required]
			[BetweenJulyTwoThousandEightToday]
			[DataType(DataType.Date)]
			[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
			[Display(Name = "Date")]
			public DateTime HMSDate { get; set; }

			[Required]
			[Display(Name = "No. of Adults")]
			[Range(0, 15, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
			public int? AdultsNo { get; set; }

			[Required]
			[Display(Name = "No. of Children")]
			[Range(0, 15, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
			public int? ChildrenNo { get; set; }

			public int? ID { get; set; }

			public bool shouldDelete { get; set; }

		}
	}
}