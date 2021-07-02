using System;
using System.ComponentModel.DataAnnotations;
using Infonet.Web.ViewModels.Shared;
using PagedList;

namespace Infonet.Web.ViewModels.Services {
	public class PublicationSearchViewModel : PagedListPagination {

		public PublicationSearchViewModel() {
			StartDate = DateTime.Today.AddMonths(-3).Date;
			EndDate = DateTime.Today.Date;
			Range = "13";
			PageSize = 10;
		}

		public IPagedList<MediaPubSearchResult> MediaPubServiceList { get; set; }

		public class MediaPubSearchResult {
			public int? ICS_ID { get; set; }
			[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
			public string Type { get; set; }
			[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
			public DateTime? PDate { get; set; }
			[Display(Name = "Media/Publication Title")]
			public string Title { get; set; }
			[Display(Name = "Number of Publications or Media Segments")]
			public int NumOfPubs { get; set; }
		}
	}
}