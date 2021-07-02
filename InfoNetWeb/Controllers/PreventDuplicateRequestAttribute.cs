using System;
using System.Web;
using System.Web.Mvc;

namespace Infonet.Web.Controllers {
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class PreventDuplicateRequestAttribute : ActionFilterAttribute {
		public override void OnActionExecuting(ActionExecutingContext filterContext) {
			if (HttpContext.Current.Request["__RequestVerificationToken"] == null)
				return;

			string currentToken = HttpContext.Current.Request["__RequestVerificationToken"];

			if (HttpContext.Current.Session["LastProcessedToken"] == null) {
				HttpContext.Current.Session["LastProcessedToken"] = currentToken;
				return;
			}

			lock (HttpContext.Current.Session["LastProcessedToken"]) {
				string lastToken = HttpContext.Current.Session["LastProcessedToken"].ToString();

				if (lastToken == currentToken) {
					filterContext.Controller.ViewData.ModelState.AddModelError("", "Looks like you accidentally tried to submit the form more than once.");
					return;
				}

				HttpContext.Current.Session["LastProcessedToken"] = currentToken;
			}
		}
	}
}