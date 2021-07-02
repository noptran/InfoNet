using System.ComponentModel.DataAnnotations;

namespace Infonet.Data.Models.Investigations {
	public class InvestigationHouseHold {
		public InvestigationHouseHold() {
			HouseHoldID = 1;
		}

		public int? ID { get; set; }

		public int? T_CACInvestigations_FK { get; set; }

		public int? TS_CACInvestigationClients_FK { get; set; }

		[Required]
		[Display(Name = "Household ID")]
        [Range(1, 9999)]
        public int HouseHoldID { get; set; }

		public virtual Investigation Investigation { get; set; }

		public virtual InvestigationClient InvestigationClient { get; set; }

		public bool IsUnchanged(InvestigationHouseHold household) {
			return HouseHoldID == household.HouseHoldID;
		}
	}
}