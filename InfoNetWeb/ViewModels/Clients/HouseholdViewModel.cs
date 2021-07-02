using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Infonet.Web.ViewModels.Shared;

namespace Infonet.Web.ViewModels.Clients {
	public class HouseholdViewModel {
		public HouseholdViewModel() {
			Clients = new List<HouseholdClient>();
			ClientSearchViewModel = new ClientSearchViewModel(true) {
				FCD_StartDate = DateTime.Today.AddMonths(-3).Date,
				FCD_EndDate = DateTime.Today.Date,
				FCDRange = "13",
				FCDDateRangeTooltip = "Search for available client cases to add to this household by selecting First Contact Date ranges. This will narrow your results to display only clients with a First Contact Date within this range."
			};
		}

		public int? ID { get; set; }

		public DateTime CreationDate { get; set; }

		[Required]
		[Display(Name = "Household ID")]
		public string InvestigationID { get; set; }

		public List<HouseholdClient> Clients { get; set; }

		public ClientSearchViewModel ClientSearchViewModel { get; set; }

		public string ReturnURL { get; set; }

		public int saveAddNew { get; set; }

		public class HouseholdClient {
			public HouseholdClient() {
				Cases = new List<SimpleCase>();
			}

			public int? ID { get; set; }

			public int ClientID { get; set; }

			public int CaseID { get; set; }

			public string ClientCode { get; set; }

			public int? T_CACInvestigations_FK { get; set; }

			public List<SimpleCase> Cases { get; set; }

			public class SimpleCase {
				public int? CaseId { get; set; }
			}
		}
	}
}