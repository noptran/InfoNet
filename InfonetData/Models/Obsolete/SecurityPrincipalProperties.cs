using System;
using Infonet.Data.Models.Centers;

namespace Infonet.Data.Models.Obsolete {
	[Obsolete]
	public class SecurityPrincipalProperties {
		public int PrincipalId { get; set; }
		public int CenterId { get; set; }
		public int ProviderId { get; set; }
		public string CenterName { get; set; }
		public virtual SecurityPrincipal SecurityPrincipal { get; set; }
		public virtual Center Center { get; set; }
	}
}