using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Infonet.Data.Looking;
using Infonet.Data.Models.Centers;
using PagedList;

namespace Infonet.Web.Controllers {
	[Authorize(Roles = "SYSADMIN")]
	public class MessageAdminController : InfonetControllerBase {
		public ActionResult Index(int? page, int? pageSize) {
			int pageNumber = page ?? 1;
			return View(
				SystemMessage.OrderForDisplay(
					db.T_SystemMessages.Where(m => m.CenterIdsString == null && m.LocationIdsString == null)
				).ToPagedList(pageNumber, pageSize ?? 50));
		}

		#region Edit
		public ActionResult Edit(int? id) {
			var model = id != null ? db.T_SystemMessages.SingleOrDefault(m => m.Id == id) : new SystemMessage();
			if (model == null)
				return HttpNotFound();

			return View(model);
		}

		[HttpPost, ActionName("Edit")]
		[ValidateInput(false)]
		[ValidateAntiForgeryToken]
		[PreventDuplicateRequest]
		public ActionResult EditPost(int? id) {
			var model = id != null ? db.T_SystemMessages.SingleOrDefault(m => m.Id == id) : new SystemMessage();
			if (model == null)
				return HttpNotFound();

			TryUpdateModel(model);
			if (Request.Unvalidated.Form["ProviderIds"] == null)
				model.ProviderIds = Enumerable.Empty<int>();

			if (string.IsNullOrWhiteSpace(model.Title))
				ModelState.AddModelError("Title", "The Title field is required.");

			if (!ModelState.IsValid) {
				AddErrorMessage("Message could not be saved.");
				return View(model);
			}

			if (!Save(model))
				return View(model);

			Session["_Marquee"] = null;
			return RedirectToAction("Edit", new { id = model.Id });
		}

		private bool Save(SystemMessage model) {
			try {
				/* for ProviderIds, null is equivalent to (and more efficient than) listing all of them */
				if (model.ProviderIds != null && !ProviderEnum.All.Except(model.ProviderIds.Cast<Provider>()).Any())
					model.ProviderIds = null;

				if (model.Id == null)
					db.T_SystemMessages.Add(model);

				db.SaveChanges();

				AddSuccessMessage("Message saved successfully.");
				return true;
			} catch (RetryLimitExceededException) {
				AddErrorMessage("Unable to save message. Try again, and if the problem persists, see your system administrator.");
				return false;
			}
		}
		#endregion

		[HttpPost, ActionName("Delete")]
		[PreventDuplicateRequest]
		public JsonResult Delete(int id) {
			bool success = false;
			string errorMessage = string.Empty;

			if (ModelState.IsValid)
				try {
					var message = db.T_SystemMessages.Find(id);
					if (message == null)
						errorMessage = "Entity not found";

					db.T_SystemMessages.Remove(message);
					db.SaveChanges();
					success = true;
				} catch (Exception ex) {
					success = false;
					errorMessage = ex.Message;
				}

			return Json(new {
				Success = success,
				ErrorMessage = errorMessage,
				__RequestVerificationToken = UpdateRequestVerificationToken(Request)
			}, JsonRequestBehavior.AllowGet);
		}

		private string UpdateRequestVerificationToken(HttpRequestBase Request) {
			string formToken;
			string cookieToken;
			const string __RequestVerificationToken = "__RequestVerificationToken";
			AntiForgery.GetTokens(Request.Form[__RequestVerificationToken], out cookieToken, out formToken);
			if (Request.Cookies.AllKeys.Contains(__RequestVerificationToken)) {
				HttpCookie cookie = Request.Cookies[__RequestVerificationToken];
				cookie.HttpOnly = true;
				cookie.Name = __RequestVerificationToken;
				cookie.Value = cookieToken;
				Response.Cookies.Add(cookie);
			}
			return formToken;
		}

	}
}