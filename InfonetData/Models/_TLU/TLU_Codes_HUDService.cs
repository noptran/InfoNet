using System.Collections.Generic;

namespace Infonet.Data.Models._TLU {
	public class TLU_Codes_HUDService {
		public int CodeId { get; set; }
		public string Description { get; set; }
		public virtual ICollection<HudServiceMapping> Tl_InfoNetHUDServiceMappings { get; set; }
	}
}