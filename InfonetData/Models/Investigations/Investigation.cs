using System;
using System.Collections.Generic;
using Infonet.Data.Models.Centers;

namespace Infonet.Data.Models.Investigations {
	public class Investigation {
		public Investigation() {
			InvestigationHouseHold = new List<InvestigationHouseHold>();
			InvestigationClient = new List<InvestigationClient>();
		}

		public int? ID { get; set; }
		public string InvestigationID { get; set; }
		public int CenterID { get; set; }
		public DateTime CreationDate { get; set; }
		public virtual Center Center { get; set; }
		public virtual ICollection<InvestigationHouseHold> InvestigationHouseHold { get; set; }
		public virtual ICollection<InvestigationClient> InvestigationClient { get; set; }
	}
}