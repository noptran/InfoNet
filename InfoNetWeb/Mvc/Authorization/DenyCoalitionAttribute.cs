using System;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace Infonet.Web.Mvc.Authorization {
	public class DenyCoalitionAttribute : AuthorizeAttribute {
		protected override bool AuthorizeCore(HttpContextBase httpContext) {
			if (httpContext == null)
				throw new ArgumentNullException(nameof(httpContext));
			IPrincipal user = httpContext.User;
			if (!user.Identity.IsAuthenticated)
				return false;
			if (user.IsInRole("DVCOALITIONADMIN") || user.IsInRole("CACCOALITIONADMIN") || user.IsInRole("SACOALITIONADMIN") || user.IsInRole("DHSCOALITIONADMIN"))
				return false;
			return true;
		}
	}
}