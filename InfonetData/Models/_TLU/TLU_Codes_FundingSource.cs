using System;
using System.Collections.Generic;
using Infonet.Data.Models.Centers;

namespace Infonet.Data.Models._TLU {
	public class TLU_Codes_FundingSource {
		public TLU_Codes_FundingSource() {
			FundServiceProgramsOfStaff = new List<FundServiceProgramOfStaff>();
			CenterAdminFundingSources = new List<CenterAdminFundingSources>();
		}

		public int? CodeID { get; set; }
		public string Description { get; set; }
		public int? CenterID { get; set; }
		public bool? ICADVAdmin { get; set; }
		public bool? ICASAAdmin { get; set; }
		public DateTime? BeginDate { get; set; }
		public DateTime? EndDate { get; set; }
		public virtual Center Center { get; set; }
		public virtual ICollection<FundServiceProgramOfStaff> FundServiceProgramsOfStaff { get; set; }
		public virtual ICollection<CenterAdminFundingSources> CenterAdminFundingSources { get; set; }
	}
}