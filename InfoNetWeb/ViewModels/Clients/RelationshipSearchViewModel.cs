using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Infonet.Data.Models.Investigations;
using Infonet.Web.ViewModels.Shared;
using PagedList;

namespace Infonet.Web.ViewModels.Clients {
	public class RelationshipSearchViewModel : PagedListPagination {
		public RelationshipSearchViewModel() {
			StartDate = DateTime.Today.AddMonths(-3).Date;
			EndDate = DateTime.Today.Date;
			Range = "13";
			PageSize = 10;
		}

		[Display(Name = "Relationship ID")]
		public string InvestigationID { get; set; }

		[Display(Name = "Client ID")]
		public string ClientID { get; set; }

		public IPagedList<InvestigationSearchResult> InvestigationList { get; set; }

		public class InvestigationSearchResult {
			public InvestigationSearchResult() {
				ClientList = new List<InvestigationClient>();
			}

			public int? ID { get; set; }
			[DataType(DataType.DateTime)]
			[Display(Name = "Creation Date")]
			[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
			public DateTime CreationDate { get; set; }
			[Display(Name = "Household ID")]
			public string InvestigationID { get; set; }
			public string Clients { get; set; }
			public List<InvestigationClient> ClientList { get; set; }
		}
	}
}