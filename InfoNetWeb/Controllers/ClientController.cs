using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using Infonet.Data;
using Infonet.Web.Mvc;
using Infonet.Web.Mvc.Authorization;
using Infonet.Web.ViewModels;
using Infonet.Web.ViewModels.Case;
using PagedList;

namespace Infonet.Web.Controllers {
	[DenyCoalition]
	[Authorize(Roles = "DVADMIN, DVDATAENTRY, SAADMIN, SADATAENTRY, CACADMIN, CACDATAENTRY, DHSADATAENTRY")]
	public class ClientController : InfonetControllerBase {
		#region Index
		public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page) {
			if (string.IsNullOrEmpty(sortOrder))
				sortOrder = "RevisionStamp_desc";

			ViewBag.CurrentSort = sortOrder;
			ViewBag.DateSortParm = sortOrder == "RevisionStamp_desc" ? "RevisionStamp_asc" : "RevisionStamp_desc";
			ViewBag.NameSortParm = sortOrder == "ClientCode_desc" ? "ClientCode_asc" : "ClientCode_desc";

			if (searchString != null)
				page = 1;
			else
				searchString = currentFilter;

			ViewBag.CurrentFilter = searchString;

			var clients = from c in db.T_Client
				select c;
			if (!string.IsNullOrEmpty(searchString))
				clients = clients.Where(c => c.ClientCode.Contains(searchString));
			switch (sortOrder) {
				case "ClientCode_desc":
					clients = clients.OrderByDescending(c => c.ClientCode);
					break;
				case "ClientCode_asc":
					clients = clients.OrderBy(c => c.ClientCode);
					break;
				case "RevisionStamp_desc":
					clients = clients.OrderByDescending(c => c.RevisionStamp);
					break;
				case "RevisionStamp_asc":
					clients = clients.OrderBy(c => c.RevisionStamp);
					break;
				default:
					return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			int pageSize = 10;
			int pageNumber = page ?? 1;
			var x = clients.ToPagedList(pageNumber, pageSize);
			return View(x);
		}
		#endregion

		#region Quick Search
		public ActionResult QuickSearch(string clientCode) {
			if (string.IsNullOrEmpty(clientCode))
				return RedirectToAction("Search", "Client");

			var redirectClient = db.Database.SqlQuery<QuickSearchClient>("SELECT clientId, typeId FROM T_Client WHERE centerId = @p0 AND clientCode = @p1", Session.Center().Top.Id, clientCode).SingleOrDefault();
			if (redirectClient == null)
				return RedirectToAction("Search", "Client", new { clientCode, startDate = " ", endDate = " " });

			var redirectCaseId = db.Database.SqlQuery<int?>("SELECT TOP 1 caseId FROM T_ClientCases WHERE clientId = @p0 ORDER BY COALESCE(caseClosed, 0), caseId DESC", redirectClient.ClientId).SingleOrDefault();
			if (redirectCaseId == null)
				return RedirectToAction("Edit", "Client", new { id = redirectClient.ClientId });

			var redirectOutline = CaseOutline.CreateFor(null, Session.Center().HasShelter, Session.Center().AllRelated.Any(c => c.HasShelter));
			var redirectCaseType = CaseTypes.For(redirectClient.TypeId);
			string redirectAction = Session.RecentClients().FirstOrDefault()?.Action;
			if (redirectAction == null || !redirectOutline.Pages.Any(p => p.Action == redirectAction && (p.Visibility & redirectCaseType) != CaseType.None))
				redirectAction = redirectOutline.Pages.First().Action;

			return RedirectToAction(redirectAction, "Case", new { clientId = redirectClient.ClientId, caseId = redirectCaseId });
		}

		private class QuickSearchClient {
			public int ClientId { get; set; }
			public int? TypeId { get; set; }
		}
		#endregion

		#region Search
		public ActionResult Search(CaseSearchViewModel model, int? page) {
			int centerId = Session.Center().Top.Id;
			var results = from cc in db.T_ClientCases
				join c in db.T_Client on cc.ClientId equals c.ClientId
				where c.CenterId == centerId
				orderby c.ClientCode, cc.CaseId descending
				select new CaseSearchViewModel.SearchResults {
					ClientCode = c.ClientCode,
					CaseId = cc.CaseId,
					CaseClosed = cc.CaseClosed,
					FirstContactDate = cc.FirstContactDate,
					Age = cc.Age,
					GenderIdentityId = c.GenderIdentityId,
					ClientTypeId = c.ClientTypeId,
					ClientId = c.ClientId
				};

			if (model.StartDate != null)
				results = results.Where(c => c.FirstContactDate >= model.StartDate);

			if (model.EndDate != null)
				results = results.Where(c => c.FirstContactDate <= model.EndDate);

			if (model.Age != null)
				results = results.Where(c => c.Age == model.Age);

			if (model.ClientCode != null)
				results = results.Where(c => c.ClientCode.Contains(model.ClientCode));

			if (model.ClientTypeId != null)
				results = results.Where(c => c.ClientTypeId == model.ClientTypeId);

			int pageNumber = page ?? 1;
			model.PageNumber = pageNumber;
			model.RecordCount = results.Count();

			if (model.StartDate == null && model.EndDate == null)
				model.Range = "0";

			model.SearchList = results.ToPagedList(pageNumber, model.PageSize == -1 ? results.Count() : model.PageSize);

			return View(model);
		}

		[HttpPost]
		public ActionResult Search() {
			ModelState.Clear();

			return RedirectToAction("Search");
		}
		#endregion

		#region Edit
		public ActionResult Edit(int id) {
			int centerId = Session.Center().Top.Id;
			var model = db.T_Client.SingleOrDefault(c => c.ClientId == id && c.CenterId == centerId);
			if (model == null)
				return HttpNotFound();

			ConstructWarnings(id);
			return View(model);
		}

		[HttpPost]
		[ActionName("Edit")]
		[ValidateAntiForgeryToken]
		[PreventDuplicateRequest]
		public ActionResult EditPost(int id) {
			int centerId = Session.Center().Top.Id;
			var model = db.T_Client.Single(c => c.ClientId == id && c.CenterId == centerId);

			//hack for when no races (checkboxes) selected
			if (Request.Params["Client.RaceHudIds"] == null)
				model.RaceHudIds = new int[0];

			var existingCaseType = model.CaseType;
			string existingClientCode = model.ClientCode;

			TryUpdateModel(model);

			if (existingCaseType != model.CaseType && (existingCaseType != CaseType.ChildNonVictim || model.CaseType != CaseType.ChildVictim))
				ModelState.AddModelError("ClientTypeId", "Type may not be changed.");

			if (existingClientCode != model.ClientCode && ModelState["ClientCode"].Errors.Count == 0) {
				int matches = db.Database.SqlQuery<int>("select count(*) from T_Client where CenterId = @p0 and ClientCode = @p1", Session.Center().Top.Id, model.ClientCode).Single();
				if (matches != 0)
					ModelState.AddModelError("ClientCode", "This Client ID is already in use.");
			}

			if (ModelState.IsValid && Save())
				return RedirectToAction("Edit", new { id = model.ClientId });

			AddErrorMessage("Errors have prevented the changes from being saved.");
			ConstructWarnings(id);
			return View(model);
		}

		private bool Save() {
			try {
				db.SaveChanges();
				AddSuccessMessage("Your changes have been successfully saved.");
				return true;
			} catch (RetryLimitExceededException) {
				ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
				return false;
			}
		}
		#endregion

		#region Private
		private void ConstructWarnings(int? clientId) {
			if (clientId != null) {
				string clientCode = db.T_Client.Where(c => c.ClientId == clientId).Select(c => c.ClientCode).SingleOrDefault();
				ViewBag.WarningMessage = "";
				ViewBag.ShowWarning = false;
				ViewBag.NoDatesAvail = false;
				var sb = new StringBuilder();
				sb.Append("<div>Please review the following warnings before proceeding.</div>");
				sb.Append("<br><ul class='text-warning' style='font-weight:bold'>");
				var cases = db.T_ClientCases.Where(cc => cc.ClientId == clientId && (!cc.CaseClosed.HasValue || cc.CaseClosed.Value == 0) && cc.ServiceDetailsOfClient.Count == 0).ToList();
				if (cases.Count > 0) {
					ViewBag.ShowWarning = true;
					sb.Append("<li><div>Client " + clientCode + " does not have any services entered for the following cases:</div>");
					sb.Append("<ul>");
					foreach (var casse in cases)
						sb.Append("<li>" + casse.CaseId + " - " + string.Format("{0:MM/dd/yyyy}", casse.FirstContactDate) + "</li>");
					sb.Append("</ul></li>");
				}
				var minusOneYear = DateTime.Today.AddYears(-1);
				var prevCase = db.T_ClientCases.SingleOrDefault(cc => cc.ClientId == clientId && cc.CaseId == db.T_ClientCases.Where(cc2 => cc2.ClientId == clientId).Max(cc2 => cc2.CaseId) && cc.FirstContactDate == db.T_ClientCases.Where(cc2 => cc2.ClientId == clientId).Max(cc2 => cc2.FirstContactDate) && cc.ServiceDetailsOfClient.Any(s => s.ServiceDate >= minusOneYear));
				if (prevCase != null) {
					ViewBag.ShowWarning = true;
					sb.Append(string.Format("<li><div>Client {0} has a service date within the last year on {1} for Case {2}.</div></li>", clientCode, string.Format("{0:MM/dd/yyyy}", prevCase.ServiceDetailsOfClient.Max(s => s.ServiceDate).Value), prevCase.CaseId));
				}
				sb.Append("</ul>");
				sb.Append("<div>Are you sure you want to add a new case for this client?</div>");
				ViewBag.WarningMessage = sb.ToString();

				prevCase = db.T_ClientCases.SingleOrDefault(cc => cc.ClientId == clientId && cc.CaseId == db.T_ClientCases.Where(cc2 => cc2.ClientId == clientId).Max(cc2 => cc2.CaseId));
				if (prevCase != null)
					ViewBag.NoDatesAvail = prevCase.FirstContactDate >= DateTime.Today;
			}
		}
		#endregion

		#region AJAX Methods
		public JsonResult SearchCases(string clientCode, DateTime? FStartDate, DateTime? FEndDate, DateTime? SStartDate, DateTime? SEndDate, int? clientTypeId, int? skip) {
			int centerId = Session.Center().Top.Id;
			int skipAmount = skip ?? 0;

			var cases = from cc in db.T_ClientCases
				join c in db.T_Client on cc.ClientId equals c.ClientId
				where (c.ClientTypeId == 1 || c.ClientTypeId == 2) && c.CenterId == centerId
				orderby c.ClientCode, cc.CaseId descending
				select new {
					clientCode = c.ClientCode,
					caseId = cc.CaseId,
					clientId = c.ClientId,
					cc.FirstContactDate,
					ServiceDate = (DateTime?)DateTime.Now,
					clientTypeId = c.ClientTypeId
				};

			if (FStartDate != null)
				cases = cases.Where(c => c.FirstContactDate >= FStartDate);

			if (FEndDate != null)
				cases = cases.Where(c => c.FirstContactDate <= FEndDate);

			if (clientTypeId != null)
				cases = cases.Where(c => c.clientTypeId == clientTypeId);

			if (!string.IsNullOrEmpty(clientCode) && !string.IsNullOrWhiteSpace(clientCode))
				cases = cases.Where(c => c.clientCode.Contains(clientCode));

			if (SStartDate != null || SEndDate != null) {
				cases = from item in cases
					join sdc in db.Tl_ServiceDetailOfClient on item.clientId equals sdc.ClientID
					join plu in db.TLU_Codes_ProgramsAndServices on sdc.ServiceID equals plu.CodeID
					join cc in db.T_ClientCases on sdc.ClientID equals cc.ClientId
					join c in db.T_Client on cc.ClientId equals c.ClientId
					where c.CenterId == centerId
					select new {
						clientCode = c.ClientCode,
						caseId = sdc.CaseID,
						clientId = sdc.ClientID,
						cc.FirstContactDate,
						sdc.ServiceDate,
						clientTypeId = c.ClientTypeId
					};

				if (SStartDate != null)
					cases = cases.Where(c => c.ServiceDate >= SStartDate);
				if (SEndDate != null)
					cases = cases.Where(c => c.ServiceDate <= SEndDate);
			}

			int total = cases.Distinct().Count();

			var results = cases.Select(c => new { c.clientId, c.clientCode, c.caseId, total }).Distinct();

			return Json(results.OrderBy(c => c.clientCode).Skip(skipAmount).Take(50), JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetClients(string clientCode) {
			int centerId = Session.Center().Top.Id;
			var clients = db.T_Client.Where(m => m.ClientCode.Contains(clientCode) && m.CenterId == centerId).Select(m => new { m.ClientCode }).Take(5).ToList();

			return Json(clients, JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetCases(string clientCode) {
			int centerId = Session.Center().Top.Id;
			var cases = from cc in db.T_ClientCases
				join c in db.T_Client on cc.ClientId equals c.ClientId
				where c.ClientCode == clientCode && c.CenterId == centerId
				select new { caseId = cc.CaseId, clientId = cc.ClientId };

			cases = cases.OrderBy(c => c.caseId);
			return Json(cases, JsonRequestBehavior.AllowGet);
		}
		#endregion
	}
}