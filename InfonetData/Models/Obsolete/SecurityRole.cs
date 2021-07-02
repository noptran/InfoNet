using System;
using System.Collections.Generic;

namespace Infonet.Data.Models.Obsolete {
	[Obsolete]
	public class SecurityRole {
		public SecurityRole() {
			SecurityIdentities = new List<SecurityIdentity>();
			SecurityPrincipals = new List<SecurityPrincipal>();
			SecurityPrivileges = new List<SecurityPrivilege>();
		}

		public int RoleId { get; set; }
		public string Code { get; set; }
		public string Description { get; set; }
		public virtual ICollection<SecurityIdentity> SecurityIdentities { get; set; }
		public virtual ICollection<SecurityPrincipal> SecurityPrincipals { get; set; }
		public virtual ICollection<SecurityPrivilege> SecurityPrivileges { get; set; }
	}
}