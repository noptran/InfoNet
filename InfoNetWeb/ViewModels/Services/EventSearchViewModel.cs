using System;
using System.ComponentModel.DataAnnotations;
using Infonet.Web.ViewModels.Shared;
using PagedList;

namespace Infonet.Web.ViewModels.Services {
	public class EventSearchViewModel : PagedListPagination {

		public EventSearchViewModel() {
			StartDate = DateTime.Today.AddMonths(-3).Date;
			EndDate = DateTime.Today.Date;
			Range = "13";
			PageSize = 10;
		}

		public IPagedList<EventSearchResult> EventList { get; set; }

		public class EventSearchResult {
			public int? ICS_ID { get; set; }
			[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
			[Display(Name = "Event Date")]
			public DateTime? EventDate { get; set; }
			public int ProgramID { get; set; }
			[Display(Name = "Event Name")]
			public string EventType { get; set; }
			public int? SVID { get; set; }
			public string Staff { get; set; }
			public double? EventHours { get; set; }
			public int? NumOfPeopleReached { get; set; }
		}
	}
}