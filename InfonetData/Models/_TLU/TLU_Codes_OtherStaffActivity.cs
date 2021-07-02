using System.Collections.Generic;
using Infonet.Data.Models.Centers;
using Infonet.Data.Models.Services;

namespace Infonet.Data.Models._TLU {
	public class TLU_Codes_OtherStaffActivity {
		public TLU_Codes_OtherStaffActivity() {
			OtherStaffActivities = new List<OtherStaffActivity>();
		}

		public int? CodeID { get; set; }
		public string Description { get; set; }
		public int? CenterID { get; set; }
		public virtual Center Center { get; set; }
		public virtual ICollection<OtherStaffActivity> OtherStaffActivities { get; set; }
	}
}