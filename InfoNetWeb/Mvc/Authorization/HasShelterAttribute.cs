using System;
using System.Web;
using System.Web.Mvc;

namespace Infonet.Web.Mvc.Authorization {
	public class HasShelterAttribute : AuthorizeAttribute {
		protected override bool AuthorizeCore(HttpContextBase httpContext) {
			if (httpContext == null)
				throw new ArgumentNullException(nameof(httpContext));

			return httpContext.User.Identity.IsAuthenticated && httpContext.Session.Center().HasShelter;
		}
	}
}