using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Infonet.Core.Entity.Validation;
using Infonet.Data.Looking;
using Infonet.Data.Models._TLU;
using Infonet.Web.ViewModels.Shared;
using PagedList;

namespace Infonet.Web.ViewModels.Services {
	public class ServiceOutcomeViewModel : PagedListPagination {

		public ServiceOutcomeViewModel() {
			Range = "13";
			StartDate = DateTime.Now.AddMonths(-3);
			EndDate = DateTime.Now;
			PageSize = 10;
		}

		public int AddCount { get; set; }

		public IPagedList<ServiceOutcomeSearchResult> OutcomesList { get; set; }
		public List<ServiceOutcomeSearchResult> displayForPaging { get; set; }

		public class ServiceOutcomeSearchResult {
			[Required]
			[Display(Name = "Client Service Group")]
			[Lookup("ServiceCategory")]
			public int? ServiceID { get; set; }

			public string ServiceName { get; set; }
			[Required]
			[BetweenJulyTwoThousandEightToday]
			[DataType(DataType.Date)]
			[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
			[Display(Name = "Date")]
			public DateTime? OutcomeDate { get; set; }

			public string OutcomeName { get; set; }
			public int ID { get; set; }

			[Required]
			[Display(Name = "Survey Question")]
			[Lookup("ServiceOutcome")]
			public int? OutcomeID { get; set; }

			public IList<TLU_Codes_ServiceOutcome> OutcomeList { get; set; }

			[Range(0, 999)]
			[WholeNumber]
			[Display(Name = "No. of YES Responses")]
			public int? ResponseYes { get; set; }


			[WholeNumber]
			[Range(0, 999)]
			[Display(Name = "No. of NO Responses")]
			public int? ResponseNo { get; set; }

			public bool shouldDelete { get; set; }

			public bool shouldEdit { get; set; }

			public bool shouldAdd { get; set; }
		}

	}
}