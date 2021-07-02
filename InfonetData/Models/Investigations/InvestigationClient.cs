using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Infonet.Data.Models.Clients;

namespace Infonet.Data.Models.Investigations {
	public class InvestigationClient {
		public InvestigationClient() {
			Households = new List<InvestigationHouseHold>();
		}

		public int? ID { get; set; }

		public int? T_CACInvestigations_FK { get; set; }

		[Display(Name = "Client ID")]
		[Required]
		public int ClientID { get; set; }

		[Display(Name = "Case ID")]
		[Required]
		public int CaseID { get; set; }

		[NotMapped]
		public InvestigationHouseHold Household {
			get { return Households[0] ?? new InvestigationHouseHold(); }
		}

		public virtual ClientCase ClientCase { get; set; }

		public virtual Investigation Investigation { get; set; }

		/* 
		 * This is not a list of households. There should be only one household per client.
		 * This was modeled this way to match the database schema.
		 */
		public virtual IList<InvestigationHouseHold> Households { get; set; }

		public bool IsUnchanged(InvestigationClient client) {
			return ID == client.ID && ClientID == client.ClientID && CaseID == client.CaseID;
		}
	}
}