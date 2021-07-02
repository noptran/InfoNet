using System;
using System.ComponentModel.DataAnnotations;
using Infonet.Data.Looking;
using Infonet.Web.ViewModels.Shared;
using PagedList;

namespace Infonet.Web.ViewModels.Services {
	public class CrisisSearchViewModel : PagedListPagination {

		public CrisisSearchViewModel() {
			StartDate = DateTime.Today.AddDays(-10).Date;
			EndDate = DateTime.Today.Date;
			Range = "0";
			PageSize = 10;
		}
		[Display(Name = "Staff/Volunteer")]
		public int? SVID { get; set; }
		[Display(Name = "Intervention Type")]
		[Lookup("HotlineCallType")]
		public int? CallTypeID { get; set; }

		public IPagedList<CrisisSearchResult> CrisisList { get; set; }

		public class CrisisSearchResult {
			public int? PH_ID { get; set; }
			[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
			public DateTime? Date { get; set; }
			public int? SVID { get; set; }
			public string Staff { get; set; }
			public int? CallTypeID { get; set; }
		}
	}
}