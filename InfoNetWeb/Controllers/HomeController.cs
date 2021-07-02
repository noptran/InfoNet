using System.Linq;
using System.Web.Mvc;
using Infonet.Data.Models.Centers;
using Infonet.Web.Mvc;

namespace Infonet.Web.Controllers {
	[Authorize]
	public class HomeController : InfonetControllerBase {
		private const int PAGE_SIZE = 9;

		public ActionResult Index() {
			ViewBag.TotalAvailable = GetMessages(SystemMessage.Mode.Card).Count();
			return View(SystemMessage.OrderForDisplay(GetMessages(SystemMessage.Mode.Carousel)).ToList());
		}

		public ActionResult GetMoreMessages(int pageIndex) {
			return PartialView("_Cards", SystemMessage.OrderForDisplay(GetMessages(SystemMessage.Mode.Card)).Skip(pageIndex * PAGE_SIZE).Take(PAGE_SIZE));
		}

		private IQueryable<SystemMessage> GetMessages(SystemMessage.Mode modeId) {
			return SystemMessage.WhereAvailable(db.T_SystemMessages.Where(m => m.ModeId == (int)modeId), Session.Center().ProviderId, Session.Center().Top.Id, Session.Center().Id);
		}
	}
}