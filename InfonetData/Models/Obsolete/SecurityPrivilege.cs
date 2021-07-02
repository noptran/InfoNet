using System;
using System.Collections.Generic;

namespace Infonet.Data.Models.Obsolete {
	[Obsolete]
	public class SecurityPrivilege {
		public SecurityPrivilege() {
			SecurityPrincipals = new List<SecurityPrincipal>();
			SecurityRoles = new List<SecurityRole>();
		}

		public int PrivilegeId { get; set; }
		public string Code { get; set; }
		public string Description { get; set; }
		public virtual ICollection<SecurityPrincipal> SecurityPrincipals { get; set; }
		public virtual ICollection<SecurityRole> SecurityRoles { get; set; }
	}
}