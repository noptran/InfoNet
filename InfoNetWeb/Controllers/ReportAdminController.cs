using System.Web.Mvc;
using Infonet.Web.Utilities;

namespace Infonet.Web.Controllers {
	[Authorize(Roles = "SYSADMIN")]
	public class ReportAdminController : Controller {
		[HttpGet]
		public ActionResult CenterActivity() {
			return View(UsersActivityList.GetCurrentActiveUsers());
		}
	}
}