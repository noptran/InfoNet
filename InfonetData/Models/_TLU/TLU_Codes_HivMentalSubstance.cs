using System;
using System.Collections.Generic;
using Infonet.Data.Models.Services;

namespace Infonet.Data.Models._TLU {
	[Obsolete]
	public class TLU_Codes_HivMentalSubstance {
		public TLU_Codes_HivMentalSubstance() {
			HivMentalSubstance = new List<HivMentalSubstance>();
		}

		public int CodeID { get; set; }
		public string Description { get; set; }
		public virtual ICollection<HivMentalSubstance> HivMentalSubstance { get; set; }
	}
}