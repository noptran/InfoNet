using System;
using System.Collections.Generic;

namespace Infonet.Data.Models.Obsolete {
	[Obsolete]
	public class SecurityIdentity {
		public SecurityIdentity() {
			SecurityRoles = new List<SecurityRole>();
		}

		public int IdentityId { get; set; }
		public string SigninName { get; set; }
		public string Pwd { get; set; }
		public int? PrincipalId { get; set; }
		public virtual SecurityPrincipal SecurityPrincipal { get; set; }
		public virtual ICollection<SecurityRole> SecurityRoles { get; set; }
	}
}