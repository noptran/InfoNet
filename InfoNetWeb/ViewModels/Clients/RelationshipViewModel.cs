using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Infonet.Data.Models.Investigations;
using Infonet.Web.ViewModels.Shared;

namespace Infonet.Web.ViewModels.Clients {
	public class RelationshipViewModel {
		public RelationshipViewModel() {
			Clients = new List<RelationshipClient>();
			ClientSearchViewModel = new ClientSearchViewModel(true) {
				FCD_StartDate = DateTime.Today.AddMonths(-3).Date,
				FCD_EndDate = DateTime.Today.Date,
				FCDRange = "13",
				FCDDateRangeTooltip = "Search for available client cases to add to this relationship by selecting first contact date ranges. This will narrow your results to display only clients with a first contact date within this range."
			};
		}

		public int? ID { get; set; }

		public DateTime CreationDate { get; set; }

		[Required]
		[Display(Name = "Relationship ID")]
		public string InvestigationID { get; set; }

		public List<RelationshipClient> Clients { get; set; }

		public ClientSearchViewModel ClientSearchViewModel { get; set; }

		public string ReturnURL { get; set; }

		public int saveAddNew { get; set; }

		public bool isNewClient { get; set; }

		public class RelationshipClient {
			public RelationshipClient() {
				Cases = new List<SimpleCase>();
			}

			public int? CenterId { get; set; }
			public InvestigationClient InvestigationClient { get; set; }

			[Required]
			[Display(Name = "Client ID")]
			public string ClientCode { get; set; }

			public List<SimpleCase> Cases { get; set; }

			public class SimpleCase {
				public int? CaseId { get; set; }
			}
		}
	}
}