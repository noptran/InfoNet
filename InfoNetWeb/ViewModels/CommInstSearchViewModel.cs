using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Infonet.Data.Models.Services;
using Infonet.Web.ViewModels.Shared;
using PagedList;

namespace Infonet.Web.ViewModels {
	public class CommInstSearchViewModel : PagedListPagination {

		public CommInstSearchViewModel() {
			StartDate = DateTime.Today.AddMonths(-3).Date;
			EndDate = DateTime.Today.Date;
			Range = "13";
			PageSize = 10;
		}

		public IPagedList<CommInstSearchResult> CommInstServiceList { get; set; }

		public class CommInstSearchResult {
			public int? ICS_ID { get; set; }
			[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
			public DateTime? PDate { get; set; }
			public double? Hours { get; set; }
			public int? NumOfSession { get; set; }
			public int? ParticipantsNum { get; set; }
			public string ProgramName { get; set; }
			public string emName { get; set; }
			public int ProgramID { get; set; }
			public virtual IList<ProgramDetailStaff> ProgramDetailStaff { get; set; }
		}
	}


}