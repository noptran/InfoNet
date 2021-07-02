using System.Web.Mvc;

namespace Infonet.Web.Controllers {
	public class ErrorController : Controller {
		public ActionResult PageNotFound() {
			return View();
		}
	}
}