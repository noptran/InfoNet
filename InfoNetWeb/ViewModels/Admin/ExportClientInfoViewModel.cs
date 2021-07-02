using System;
using System.ComponentModel.DataAnnotations;
using Infonet.Data.Models.Clients;
using Infonet.Web.ViewModels.Shared;
using PagedList;

namespace Infonet.Web.ViewModels.Admin {
	public class ExportClientInfoViewModel : PagedListPagination {
		public ExportClientInfoViewModel() {
			StartDate = DateTime.Today.AddMonths(-3).Date;
			EndDate = DateTime.Today.Date;
			Range = "13";
			PageSize = 10;
		}

        [MaxLength(50, ErrorMessageResourceName = "StringMaxLengthMessage", ErrorMessageResourceType = typeof(Resource))]
        [Display(Name = "Client Code")]
		public string ClientCode { get; set; }

		public IPagedList<ClientCase> SearchResults { get; set; }
	}
}