using System.Web.Mvc;
using System.Web.Routing;

namespace Infonet.Web {
	public class RouteConfig {
		public static void RegisterRoutes(RouteCollection routes) {
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				name: "Case",
				url: "{controller}/{action}/{clientId}c{caseId}/{id}",
				defaults: new { id = UrlParameter.Optional },
				constraints: new { clientId = @"\d+", caseId = @"\d+", id = @"\d*" }
			);

			routes.MapRoute(
				name: "CaseOptional",
				url: "{controller}/{action}/{clientId}c",
				defaults: new { },
				constraints: new { clientId = @"\d+" }
			);

			routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
				constraints: new { id = @"\d*" }
			);
		}
	}
}