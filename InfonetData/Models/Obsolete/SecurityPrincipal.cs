using System;
using System.Collections.Generic;

namespace Infonet.Data.Models.Obsolete {
	[Obsolete]
	public class SecurityPrincipal {
		public SecurityPrincipal() {
			SecurityIdentities = new List<SecurityIdentity>();
			SecurityPrivileges = new List<SecurityPrivilege>();
			SecurityRoles = new List<SecurityRole>();
		}

		public int PrincipalId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public bool IsActive { get; set; }
		public virtual SecurityPrincipalProperties Properties { get; set; }
		public virtual ICollection<SecurityIdentity> SecurityIdentities { get; set; }
		public virtual ICollection<SecurityPrivilege> SecurityPrivileges { get; set; }
		public virtual ICollection<SecurityRole> SecurityRoles { get; set; }
	}
}