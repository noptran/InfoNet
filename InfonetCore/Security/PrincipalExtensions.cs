using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace Infonet.Core.Security {
	public static class PrincipalExtensions {
		// ReSharper disable once InconsistentNaming
		private static readonly char[] SEPARATORS = { ',', ' ' };

		public static bool IsInAnyOf(this IPrincipal principal, string commaOrSpaceSeparatedRoles) {
			return principal.IsInAnyOf(commaOrSpaceSeparatedRoles.Split(SEPARATORS, StringSplitOptions.RemoveEmptyEntries));
		}

		public static bool IsInAnyOf(this IPrincipal principal, params string[] roles) {
			return principal.IsInAnyOf((IEnumerable<string>)roles);
		}

		public static bool IsInAnyOf(this IPrincipal principal, IEnumerable<string> roles) {
			if (roles == null)
				throw new ArgumentNullException(nameof(roles));
			return roles.Any(principal.IsInRole);
		}
	}
}