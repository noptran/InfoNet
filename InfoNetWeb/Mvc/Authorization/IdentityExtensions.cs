using System;
using System.Security.Claims;
using System.Security.Principal;

namespace Infonet.Web.Mvc.Authorization {
	public static class IdentityExtensions {
		public static int GetCenterId(this IIdentity identity) {
			var claim = ((ClaimsIdentity)identity).FindFirst("CenterId");
			if (claim == null)
				throw new Exception("The user does not have an affiliated CenterID.");
			return Convert.ToInt32(claim.Value);
		}
	}
}