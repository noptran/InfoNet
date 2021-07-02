using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Mvc;
using Infonet.Data.Models.Investigations;
using Infonet.Web.Mvc;
using Infonet.Web.Mvc.Authorization;
using Infonet.Web.ViewModels.Clients;
using PagedList;

namespace Infonet.Web.Controllers {
	[DenyCoalition]
	[Authorize(Roles = "DVADMIN, DVDATAENTRY, SAADMIN, SADATAENTRY")]
	public class HouseholdsController : InfonetControllerBase {
		#region Search
		[HttpGet]
		public ActionResult Search(HouseholdSearchViewModel model, int? page) {
			int centerId = Session.Center().Id;
			var results = from household in db.T_Investigations
				where household.CenterID == centerId
				orderby household.CreationDate descending
				select new HouseholdSearchViewModel.HouseholdSearchResult {
					ID = household.ID,
					CreationDate = household.CreationDate,
					InvestigationID = household.InvestigationID,
					ClientList = household.InvestigationClient.ToList()
				};

			if (model.StartDate != null)
				results = results.Where(c => c.CreationDate >= model.StartDate);
			if (model.EndDate != null) {
				var endDate = model.EndDate.Value.AddDays(1).AddMilliseconds(-1);
				results = results.Where(c => c.CreationDate <= endDate);
			}

			if (model.HouseholdID != null)
				results = results.Where(c => c.InvestigationID.Contains(model.HouseholdID));
			if (model.ClientID != null)
				results = results.Where(c => c.ClientList.Any(cl => cl.ClientCase.Client.ClientCode.Contains(model.ClientID)));

			int pageNumber = page ?? 1;
			model.PageNumber = pageNumber;
			model.RecordCount = results.Count();

			model.HouseholdList = results.OrderByDescending(h => h.CreationDate).ToPagedList(pageNumber, model.PageSize == -1 ? (int)model.RecordCount : model.PageSize);

			foreach (var result in model.HouseholdList)
				result.Clients = db.Database.SqlQuery<string>("SELECT STUFF((SELECT DISTINCT ', ' + c.ClientCode FROM Ts_InvestigationClients ic JOIN T_Client c on c.ClientId = ic.ClientId WHERE ic.T_CACInvestigations_FK = @p0 ORDER BY 1 FOR XML PATH('')), 1, 2, '')", result.ID).Single();

			return View(model);
		}

		public ActionResult Search() {
			ModelState.Clear();
			return RedirectToAction("Search");
		}

		public ActionResult FormRedirect(int? id) {
			if (Request != null && Request.UrlReferrer != null)
				TempData["HouseholdReturnUrl"] = Request.UrlReferrer.AbsoluteUri;
			return RedirectToAction("Form", new { id });
		}
		#endregion

		#region Form
		[HttpGet]
		public ActionResult Form(int? id) {
			var model = LoadOrCreate(id, new HouseholdViewModel());
			if (model == null)
				return IcjiaNotFound();

			TempData["HouseholdReturnUrl"] = !string.IsNullOrEmpty((string)TempData.Peek("HouseholdReturnUrl")) ? TempData["HouseholdReturnUrl"] : "/Households/Search";

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[PreventDuplicateRequest]
		public ActionResult Form(HouseholdViewModel model) {
			model.Clients?.RemoveAll(m => m.ClientID == 0 && string.IsNullOrEmpty(m.ClientCode));

			TempData["HouseholdReturnUrl"] = model.ReturnURL;

			Validate(model);

			if (ModelState.IsValid)
				return RedirectToAction("Form", new { id = model.ID == null ? Add(LoadOrCreate(null, model)) : Edit(model) });

			AddErrorMessage("An error occured while saving! Please try again!");

			if (model.Clients != null)
				foreach (var client in model.Clients)
					client.Cases = db.T_ClientCases.Where(cc => cc.ClientId == client.ClientID && !cc.InvestigationClients.Any(ic => ic.ClientID == cc.ClientId && ic.CaseID == cc.CaseId && ic.T_CACInvestigations_FK != model.ID)).Select(cc => new HouseholdViewModel.HouseholdClient.SimpleCase { CaseId = cc.CaseId }).OrderBy(cc => cc.CaseId).ToList();

			return View(model);
		}

		public ActionResult AddNewClient(int? clientId, int? caseId) {
			var model = new HouseholdViewModel { Clients = new List<HouseholdViewModel.HouseholdClient>() };
			var client = new HouseholdViewModel.HouseholdClient { Cases = new List<HouseholdViewModel.HouseholdClient.SimpleCase>() };

			if (clientId != null && clientId > 0) {
				var origClient = db.T_Client.SingleOrDefault(c => c.ClientId == clientId);
				if (origClient != null) {
					client.ClientID = (int)origClient.ClientId;
					client.ClientCode = origClient.ClientCode;
					client.Cases = db.T_ClientCases.Where(cc => cc.ClientId == origClient.ClientId).Select(cc => new HouseholdViewModel.HouseholdClient.SimpleCase { CaseId = cc.CaseId }).ToList();
					client.CaseID = caseId ?? 1;
				}
			}

			model.Clients.Add(client);

			return PartialView("_NewClientPartial", model);
		}

		public ActionResult Delete(int? id) {
			if (id == null)
				return RedirectToAction("Search");

			int centerId = Session.Center().Id;
			try {
				var household = db.T_Investigations.Single(i => i.ID == id && i.CenterID == centerId);
				db.Ts_InvestigationClients.Where(ic => ic.T_CACInvestigations_FK == household.ID).ToList().ForEach(c => db.Ts_InvestigationClients.Remove(c));
				db.T_Investigations.Remove(household);
				db.SaveChanges();
				AddSuccessMessage("You have successfully deleted a Household!");
			} catch (Exception) {
				ModelState.AddModelError("", "Unable to delete record. Try again, and if the problem persists, see your system administrator.");
			}
			return RedirectToAction("Search");
		}
		#endregion

		#region Private
		private HouseholdViewModel LoadOrCreate(int? id, HouseholdViewModel model) {
			if (id == null)
				return model;

			int centerId = Session.Center().Id;
			var household = db.T_Investigations.FirstOrDefault(i => i.ID == id && i.CenterID == centerId);
			if (household == null)
				return null;

			model.ID = household.ID;
			model.CreationDate = household.CreationDate;
			model.InvestigationID = household.InvestigationID;
			model.Clients = household.InvestigationClient.Select(ic => new HouseholdViewModel.HouseholdClient {
				ID = ic.ID ?? 0,
				ClientID = ic.ClientID,
				CaseID = ic.CaseID,
				ClientCode = ic.ClientCase.Client.ClientCode,
				T_CACInvestigations_FK = ic.T_CACInvestigations_FK,
				Cases = ic.ClientCase.Client.ClientCases.Where(cc => cc.InvestigationClients.All(i => i.T_CACInvestigations_FK == Convert.ToInt32(model.ID))).Select(cc => new HouseholdViewModel.HouseholdClient.SimpleCase { CaseId = cc.CaseId }).OrderBy(cc => cc.CaseId).ToList()
			}).ToList();
			return model;
		}

		private void Validate(HouseholdViewModel model) {
			int centerId = Session.Center().Id;
			int topCenterId = Session.Center().Top.Id;

			for (int i = 0; i < model.Clients.Count; i++) {
				var each = model.Clients[i];
				if (each.ClientCode != null)
					if (!db.T_Client.Any(m => m.ClientCode == each.ClientCode && m.CenterId == topCenterId))
						ModelState.AddModelError("Clients[" + i + "].ClientCode", "The Client ID entered is not valid.");
			}

			var household = db.T_Investigations.SingleOrDefault(i => i.InvestigationID.ToLower() == model.InvestigationID.Trim().ToLower() && i.ID != model.ID && i.CenterID == centerId);
			if (household != null)
				ModelState.AddModelError("InvestigationID", string.Format("A Household with ID of {0} already exists. The Household ID must be unique.", model.InvestigationID));

			if (model.Clients != null) {
				for (int i = 0; i < model.Clients.Count; i++) {
					int clientId = model.Clients[i].ClientID;
					int caseId = model.Clients[i].CaseID;
					int id = model.Clients[i].ID ?? -1;

					var originalClient = db.Ts_InvestigationClients.SingleOrDefault(ic => ic.ClientID == clientId && ic.CaseID == caseId && ic.ID != id);

					if (originalClient != null) {
						if (Session.Center().IsSA)
							ModelState.AddModelError("Clients[" + i + "].ClientCode", "Case is already assigned to a household");
						else
							ModelState.AddModelError("Clients[" + i + "].CaseID", "Case is already assigned to a household");
					}
				}

				var validateDic = new Dictionary<string, string>();
				for (int i = 0; i < model.Clients.Count; i++) {
					string added;
					string identifier = model.Clients[i].ClientID + " " + model.Clients[i].CaseID;
					bool isPresent = validateDic.TryGetValue(identifier, out added);
					if (isPresent) {
						ModelState.AddModelError(validateDic[identifier], "Case added more than once");
						if (Session.Center().IsSA)
							ModelState.AddModelError("Clients[" + i + "].ClientCode", "Case added more than once");
						else
							ModelState.AddModelError("Clients[" + i + "].CaseID", "Case added more than once");
					} else {
						if (Session.Center().IsSA)
							validateDic.Add(identifier, "Clients[" + i + "].ClientCode");
						else
							validateDic.Add(identifier, "Clients[" + i + "].CaseID");
					}
				}
			}
		}

		private int? Add(HouseholdViewModel model) {
			bool isNewHousehold = model.ID == null;
			int centerId = Session.Center().Id;
			ICollection<InvestigationClient> clients = new Collection<InvestigationClient>();
			foreach (var client in model.Clients)
				clients.Add(new InvestigationClient {
					ID = client.ID,
					ClientID = client.ClientID,
					CaseID = client.CaseID,
					T_CACInvestigations_FK = client.T_CACInvestigations_FK
				});
			try {
				var household = new Investigation();
				if (isNewHousehold) {
					household.CenterID = centerId;
					household.CreationDate = DateTime.Now;
					household.InvestigationID = model.InvestigationID;
					household.InvestigationClient = clients;
					db.T_Investigations.Add(household);
				}
				db.SaveChanges();
				AddSuccessMessage("You have successfully added a new Household!");
				return model.saveAddNew == 0 ? household.ID : null;
			} catch (RetryLimitExceededException) {
				if (isNewHousehold)
					model.ID = null;
				AddErrorMessage("Unable to save changes. Try again, and if the problem persists, see your system administrator.");
				return 0;
			}
		}

		private int? Edit(HouseholdViewModel model) {
			int centerId = Session.Center().Id;
			var original = db.T_Investigations.Single(i => i.ID == model.ID && i.CenterID == centerId);
			var thisHousehold = db.Entry(original);
			thisHousehold.CurrentValues.SetValues(model);

			if (model.Clients != null)
				foreach (var client in model.Clients.ToList()) {
					var originalClient = db.Ts_InvestigationClients.SingleOrDefault(ic => ic.ID == client.ID);

					var newClient = new InvestigationClient {
						ID = client.ID,
						ClientID = client.ClientID,
						CaseID = client.CaseID,
						T_CACInvestigations_FK = (int)model.ID
					};

					if (originalClient != null) {
						db.Entry(originalClient).State = EntityState.Detached;
						db.Ts_InvestigationClients.Attach(newClient);
						db.Entry(newClient).State = originalClient.IsUnchanged(newClient) ? EntityState.Unchanged : EntityState.Modified;
					} else {
						db.Ts_InvestigationClients.Add(newClient);
						db.Entry(newClient).State = EntityState.Added;
					}
				}

			foreach (var originalClient in original.InvestigationClient.ToList())
				if (model.Clients == null || model.Clients.All(p => p.ID != originalClient.ID))
					db.Ts_InvestigationClients.Remove(originalClient);

			db.SaveChanges();
			AddSuccessMessage("You have successfully modified a Household. Your changes have been saved!");
			return model.saveAddNew == 0 ? model.ID : null;
		}
		#endregion

		#region AJAX
		public JsonResult SearchCasesHousehold(string clientCode, DateTime? fStartDate, DateTime? fEndDate, int? clientTypeId, int? skip, IList<int?> existingClientIds) {
			int centerId = Session.Center().Top.Id;
			int skipAmount = skip ?? 0;

			var cases = db.T_ClientCases.Where(cc => cc.Client.CenterId == centerId && !cc.InvestigationClients.Any(ic => ic.ClientID == cc.ClientId && ic.CaseID == cc.CaseId))
				.Select(m => new {
					clientCode = m.Client.ClientCode,
					caseId = m.CaseId,
					clientId = m.ClientId,
					m.FirstContactDate,
					closed = m.CaseClosed,
					clientTypeId = m.Client.ClientTypeId
				});

			if (existingClientIds != null)
				cases = cases.Where(c => !existingClientIds.Contains(c.clientId));

			if (fStartDate != null)
				cases = cases.Where(c => c.FirstContactDate >= fStartDate);

			if (fEndDate != null)
				cases = cases.Where(c => c.FirstContactDate <= fEndDate);

			if (clientTypeId != null)
				cases = cases.Where(c => c.clientTypeId == clientTypeId);

			if (!string.IsNullOrEmpty(clientCode) && !string.IsNullOrWhiteSpace(clientCode))
				cases = cases.Where(c => c.clientCode.Contains(clientCode));

			var caseList = cases.Distinct().ToList();

			int total = caseList.Count;

			var results = caseList.Select(c => new { c.clientId, c.clientCode, c.caseId, c.FirstContactDate, c.closed, total }).Distinct();

			return Json(results.OrderBy(c => c.clientCode).Skip(skipAmount).Take(50), JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetClientsHousehold(string clientCode) {
			int centerId = Session.Center().Top.Id;
			var clients = db.T_Client.Where(m => m.ClientCode.Contains(clientCode) && m.CenterId == centerId && !m.ClientCases.All(cc => cc.InvestigationClients.Any(ic => ic.ClientID == cc.ClientId && ic.CaseID == cc.CaseId))).Select(m => new { m.ClientCode }).Take(8).ToList();

			return Json(clients, JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetCasesForHousehold(string clientCode, int? householdId) {
			int id = householdId ?? 0;
			int centerId = Session.Center().Top.Id;
			var cases = db.T_ClientCases.Where(cc => !cc.InvestigationClients.Any(ic => ic.ClientID == cc.ClientId && ic.CaseID == cc.CaseId && ic.T_CACInvestigations_FK != id) && cc.Client.CenterId == centerId && cc.Client.ClientCode == clientCode).Select(cc => new { caseId = cc.CaseId, clientId = cc.ClientId }).ToList();
			return Json(cases, JsonRequestBehavior.AllowGet);
		}
		#endregion
	}
}