using System;
using System.ComponentModel.DataAnnotations;
using Infonet.Web.ViewModels.Shared;
using PagedList;

namespace Infonet.Web.ViewModels.Services {
	public class OtherStaffActivitySearchViewModel : PagedListPagination {

		public OtherStaffActivitySearchViewModel() {
			StartDate = DateTime.Today.AddMonths(-3).Date;
			EndDate = DateTime.Today.Date;
			Range = "13";
			PageSize = 10;
		}

		[Display(Name = "Staff/Volunteer")]
		public int? SVID { get; set; }

		[Display(Name = "Other Staff Activity")]
		public int? OtherStaffActivityID { get; set; }

		public IPagedList<OtherStaffActivitySearchResult> OSAList { get; set; }

		public class OtherStaffActivitySearchResult {
			public int? OsaID { get; set; }
			public int? SVID { get; set; }
			public string Staff { get; set; }
			public string Activity { get; set; }
			public int? OtherStaffActivityID { get; set; }
			[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
			public DateTime? OsaDate { get; set; }
		}
	}
}