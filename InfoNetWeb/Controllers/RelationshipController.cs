using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Infonet.Data.Models.Investigations;
using Infonet.Web.Mvc;
using Infonet.Web.Mvc.Authorization;
using Infonet.Web.ViewModels.Clients;
using PagedList;

namespace Infonet.Web.Controllers {
	[DenyCoalition]
	[Authorize(Roles = "CACADMIN, CACDATAENTRY")]
	public class RelationshipController : InfonetControllerBase {
		#region Search
		[HttpGet]
		public ActionResult Search(RelationshipSearchViewModel model, int? page) {
			int centerId = Session.Center().Id;
			var results = from investigation in db.T_Investigations
				where investigation.CenterID == centerId
				orderby investigation.CreationDate descending
				select new RelationshipSearchViewModel.InvestigationSearchResult {
					ID = investigation.ID,
					CreationDate = investigation.CreationDate,
					InvestigationID = investigation.InvestigationID,
					ClientList = investigation.InvestigationClient.ToList()
				};

			if (model.StartDate != null)
				results = results.Where(i => i.CreationDate >= model.StartDate);
			if (model.EndDate != null) {
				var endDate = model.EndDate.Value.AddDays(1);
				results = results.Where(i => i.CreationDate < endDate);
			}
			if (model.InvestigationID != null)
				results = results.Where(i => i.InvestigationID.Contains(model.InvestigationID));
			if (model.ClientID != null)
				results = results.Where(i => i.ClientList.Any(cl => cl.ClientCase.Client.ClientCode.Contains(model.ClientID)));

			int pageNumber = page ?? 1;
			model.PageNumber = pageNumber;
			model.RecordCount = results.Count();

			model.InvestigationList = results.OrderByDescending(i => i.CreationDate).ThenBy(i => i.InvestigationID).ToPagedList(pageNumber, model.PageSize);

			foreach (var result in model.InvestigationList)
				result.Clients = db.Database.SqlQuery<string>("SELECT STUFF((SELECT ', ' + c.ClientCode FROM T_ClientCases cc JOIN T_Client c ON c.ClientID = cc.ClientID JOIN Ts_InvestigationClients ic ON ic.ClientID = cc.ClientID AND ic.CaseID = cc.CaseID JOIN T_Investigations i ON i.ID = ic.T_CACInvestigations_FK WHERE i.InvestigationID = @p0 FOR XML PATH('')), 1, 1, '')", result.InvestigationID).SingleOrDefault();

			return View(model);
		}

		[HttpPost]
		public ActionResult Search() {
			ModelState.Clear();
			return RedirectToAction("Search");
		}

		public ActionResult FormRedirect(int? id) {
			if (Request != null && Request.UrlReferrer != null)
				TempData["RelationshipReturnUrl"] = Request.UrlReferrer.AbsoluteUri;
			return RedirectToAction("Form", new { id });
		}
		#endregion

		#region Form
		[HttpGet]
		public ActionResult Form(int? id) {
			var model = LoadOrCreate(id, new RelationshipViewModel());
			if (model == null)
				return RedirectToAction("Search");

			TempData["RelationshipReturnUrl"] = !string.IsNullOrEmpty((string)TempData.Peek("RelationshipReturnUrl")) ? TempData["RelationshipReturnUrl"] : "/Relationship/Search";

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[PreventDuplicateRequest]
		public ActionResult Form(RelationshipViewModel model) {
			model.Clients?.RemoveAll(m => m.InvestigationClient.ClientID == 0 && string.IsNullOrEmpty(m.ClientCode));

			TempData["RelationshipReturnUrl"] = model.ReturnURL;
			Validate(model);

			if (ModelState.IsValid) {
				if (model.ID == null) {
					int? id = Add(LoadOrCreate(null, model));
					if (id.GetValueOrDefault() > -1)
						return RedirectToAction("Form", new { id });
				} else {
					int? id = Edit(model);
					if (id.GetValueOrDefault() > -1)
						return RedirectToAction("Form", new { id });
				}
			}

			AddErrorMessage("An error occured while saving! Please try again!");
			if (model.Clients != null)
				foreach (var client in model.Clients)
					client.Cases = db.T_ClientCases.Where(cc => cc.ClientId == client.InvestigationClient.ClientID && !cc.InvestigationClients.Any(ic => ic.ClientID == cc.ClientId && ic.CaseID == cc.CaseId && ic.T_CACInvestigations_FK != model.ID)).Select(cc => new RelationshipViewModel.RelationshipClient.SimpleCase { CaseId = cc.CaseId }).OrderBy(cc => cc.CaseId).ToList();
			return View(model);
		}

		public ActionResult AddNewClient(int? clientId, int? caseId, int count) {
			var model = new RelationshipViewModel();
			var client = new RelationshipViewModel.RelationshipClient { InvestigationClient = new InvestigationClient() };
			if (clientId != null && clientId > 0) {
				var origClient = db.T_Client.SingleOrDefault(c => c.ClientId == clientId);
				if (origClient != null) {
					client.InvestigationClient.ClientID = (int)origClient.ClientId;
					client.ClientCode = origClient.ClientCode;
					client.Cases = db.T_ClientCases.Where(cc => cc.ClientId == origClient.ClientId && !cc.InvestigationClients.Any()).Select(cc => new RelationshipViewModel.RelationshipClient.SimpleCase { CaseId = cc.CaseId }).ToList();
					client.InvestigationClient.CaseID = caseId ?? 1;
				}
			}
			client.InvestigationClient.Households.Add(new InvestigationHouseHold());
			model.Clients.Add(client);
			model.isNewClient = true;
			return PartialView("_NewClientPartial", model);
		}

		public ActionResult Delete(int? id) {
			if (id == null)
				return RedirectToAction("Search");

			try {
				int centerId = Session.Center().Id;
				var investigation = db.T_Investigations.Single(i => i.ID == id && i.CenterID == centerId);
				db.Ts_InvestigationClients.Where(ic => ic.T_CACInvestigations_FK == investigation.ID).ToList().ForEach(c => db.Ts_InvestigationClients.Remove(c));
				db.T_Investigations.Remove(investigation);
				db.SaveChanges();
				AddSuccessMessage("You have successfully deleted an Investigation!");
			} catch (Exception) {
				ModelState.AddModelError("", "Unable to delete record. Try again, and if the problem persists, see your system administrator.");
			}
			return RedirectToAction("Search");
		}
		#endregion

		#region Private
		private RelationshipViewModel LoadOrCreate(int? id, RelationshipViewModel model) {
			if (id == null)
				return model;

			int centerId = Session.Center().Id;
			var investigation = db.T_Investigations.SingleOrDefault(i => i.ID == id && i.CenterID == centerId);
			if (investigation == null) {
				AddErrorMessage("The record with an ID of " + id + " is not an Investigation for the current center.");
				return null;
			}
			model.ID = investigation.ID;
			model.CreationDate = investigation.CreationDate;
			model.InvestigationID = investigation.InvestigationID;
			model.Clients = investigation.InvestigationClient.Select(ic => new RelationshipViewModel.RelationshipClient {
				InvestigationClient = ic,
				ClientCode = ic.ClientCase.Client.ClientCode,
				Cases = ic.ClientCase.Client.ClientCases.Where(cc => cc.InvestigationClients.All(i => i.T_CACInvestigations_FK == Convert.ToInt32(model.ID))).Select(cc => new RelationshipViewModel.RelationshipClient.SimpleCase { CaseId = cc.CaseId }).OrderBy(cc => cc.CaseId).ToList()
			}).ToList();
			return model;
		}

		private void Validate(RelationshipViewModel model) {			
            int topCenterId = Session.Center().Top.Id;

            for (int i = 0; i < model.Clients.Count; i++)
            {
                var each = model.Clients[i];
                if (each.ClientCode != null)
                    if (!db.T_Client.Any(m => m.ClientCode == each.ClientCode && m.CenterId == topCenterId))
                        ModelState.AddModelError("Clients[" + i + "].ClientCode", "The Client ID entered is not valid.");
            }


            var household = db.T_Investigations.SingleOrDefault(i => i.InvestigationID.ToLower() == model.InvestigationID.Trim().ToLower() && i.ID != model.ID);
			if (household != null)
				ModelState.AddModelError("InvestigationID", $"A Client Relationship with ID of {model.InvestigationID} already exists. The Client Relationship ID must be unique.");

			for (int i = 0; i < model.Clients.Count; i++) {
				int clientId = model.Clients[i].InvestigationClient.ClientID;
				int caseId = model.Clients[i].InvestigationClient.CaseID;
				int id = model.Clients[i].InvestigationClient.ID ?? -1;

				var originalClient = db.Ts_InvestigationClients.SingleOrDefault(ic => ic.ClientID == clientId && ic.CaseID == caseId && ic.ID != id);

				if (originalClient != null)
					ModelState.AddModelError("Clients[" + i + "].InvestigationClient.CaseID", "Case is already assigned to a household.");
			}

            var validateDic = new Dictionary<string, string>();
            for (int i = 0; i < model.Clients.Count; i++) {
				string added;
				string identifier = model.Clients[i].InvestigationClient.ClientID + " " + model.Clients[i].InvestigationClient.CaseID;
				bool isPresent = validateDic.TryGetValue(identifier, out added);
				if (isPresent) {
					ModelState.AddModelError(validateDic[identifier], "Case added more than once");
					ModelState.AddModelError("Clients[" + i + "].InvestigationClient.CaseID", "Case added more than once");
				} else {
					validateDic.Add(identifier, "Clients[" + i + "].InvestigationClient.CaseID");
				}
			}
		}

		private int? Add(RelationshipViewModel model) {
			using (var transaction = db.Database.BeginTransaction())
				try {
					bool isNewInvest = model.ID == null;
					int centerId = Session.Center().Id;
					var investigation = new Investigation();
					if (isNewInvest) {
						investigation.CenterID = centerId;
						investigation.CreationDate = DateTime.Now;
						investigation.InvestigationID = model.InvestigationID;
					}

					db.T_Investigations.Add(investigation);
					db.SaveChanges();

					foreach (var client in model.Clients) {
						var newClient = new InvestigationClient {
							T_CACInvestigations_FK = investigation.ID,
							ClientID = client.InvestigationClient.ClientID,
							CaseID = client.InvestigationClient.CaseID
						};
						db.Ts_InvestigationClients.Add(newClient);
						db.SaveChanges();

						var originalHousehold = db.Ts_InvestigationHouseHolds.Single(h => h.TS_CACInvestigationClients_FK == newClient.ID);
						originalHousehold.HouseHoldID = client.InvestigationClient.Household.HouseHoldID;
						db.Entry(originalHousehold).State = EntityState.Modified;
						db.SaveChanges();
					}

					transaction.Commit();
					AddSuccessMessage("You have succesfully added a new Investigation record!");
					return model.saveAddNew == 0 ? investigation.ID : null;
				} catch (Exception) {
					transaction.Rollback();
					AddErrorMessage("Unable to save changes. Try again, and if the problem persists, see your system administrator.");
					return -1;
				}
		}

		private int? Edit(RelationshipViewModel model) {
			using (var transaction = db.Database.BeginTransaction())
				try {
					int centerId = Session.Center().Id;
					var original = db.T_Investigations.Single(i => i.ID == model.ID && i.CenterID == centerId);
					var thisInvestigation = db.Entry(original);
					thisInvestigation.CurrentValues.SetValues(model);

					db.SaveChanges();

					foreach (var client in model.Clients.ToList()) {
						client.InvestigationClient.T_CACInvestigations_FK = thisInvestigation.CurrentValues.GetValue<int?>("ID");
						var originalClient = db.Ts_InvestigationClients.SingleOrDefault(ic => ic.ID == client.InvestigationClient.ID);
						if (originalClient != null) {
							originalClient.Households = new List<InvestigationHouseHold>();
							originalClient.CaseID = client.InvestigationClient.CaseID;
							originalClient.ClientID = client.InvestigationClient.ClientID;
							db.Entry(originalClient).State = EntityState.Modified;
						} else {
							var newClient = new InvestigationClient {
								T_CACInvestigations_FK = client.InvestigationClient.T_CACInvestigations_FK,
								ClientID = client.InvestigationClient.ClientID,
								CaseID = client.InvestigationClient.CaseID,
								Households = new List<InvestigationHouseHold>()
							};
							db.Ts_InvestigationClients.Add(newClient);
							db.Entry(newClient).State = EntityState.Added;

							db.SaveChanges();

							client.InvestigationClient.ID = newClient.ID;
						}
					}

					db.SaveChanges();

					// Update Clients
					foreach (var client in model.Clients) {
						var originalHousehold = db.Ts_InvestigationHouseHolds.SingleOrDefault(h => h.TS_CACInvestigationClients_FK == client.InvestigationClient.ID);
						if (originalHousehold != null) {
							originalHousehold.HouseHoldID = client.InvestigationClient.Household.HouseHoldID;
							db.Entry(originalHousehold).State = EntityState.Modified;
						}
					}

					// Remove Deleted Clients
					foreach (var originalClient in original.InvestigationClient.ToList())
						if (model.Clients.All(p => p.InvestigationClient.ID != originalClient.ID)) {
							var household = db.Ts_InvestigationHouseHolds.Single(h => h.TS_CACInvestigationClients_FK == originalClient.ID);
							db.Ts_InvestigationHouseHolds.Remove(household);
							db.Ts_InvestigationClients.Remove(originalClient);
						}

					db.SaveChanges();

					transaction.Commit();

					AddSuccessMessage("You have successfully modified an Investigation!");
					return model.saveAddNew == 0 ? model.ID : null;
				} catch (Exception) {
					transaction.Rollback();
					AddErrorMessage("UNABLE TO SAVE CHANGES. PLEASE TRY AGAIN.");
					return -1;
				}
		}
		#endregion

		#region AJAX
		public JsonResult SearchClientCases(string clientCode, DateTime? fStartDate, DateTime? fEndDate, int? clientTypeId, int? skip, IList<int?> existingClientIds) {
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

		public JsonResult GetClients(string clientCode) {
			int centerId = Session.Center().Top.Id;
			var clients = db.T_Client.Where(m => m.ClientCode.Contains(clientCode) && m.CenterId == centerId && !m.ClientCases.All(cc => cc.InvestigationClients.Any(ic => ic.ClientID == cc.ClientId && ic.CaseID == cc.CaseId))).Select(m => new { m.ClientCode }).Take(8).ToList();
			return Json(clients, JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetCases(string clientCode, int? householdId) {
			int id = householdId ?? 0;
			int centerId = Session.Center().Top.Id;
			var cases = db.T_ClientCases.Where(cc => !cc.InvestigationClients.Any(ic => ic.ClientID == cc.ClientId && ic.CaseID == cc.CaseId && ic.T_CACInvestigations_FK != id) && cc.Client.CenterId == centerId && cc.Client.ClientCode == clientCode).Select(cc => new { caseId = cc.CaseId, clientId = cc.ClientId }).ToList();
			return Json(cases, JsonRequestBehavior.AllowGet);
		}
		#endregion
	}
}