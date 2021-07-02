using System.Collections.Generic;
using Infonet.Data.Models.Services;

namespace Infonet.Data.Models._TLU {
	public class TLU_Codes_ServiceCategory {
		public TLU_Codes_ServiceCategory() {
			ServiceOutcome = new List<ServiceOutcome>();
		}

		public int CodeID { get; set; }
		public string Description { get; set; }
		public virtual ICollection<ServiceOutcome> ServiceOutcome { get; set; }
	}
}