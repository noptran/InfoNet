using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Mvc;
using Infonet.Core;
using Infonet.Core.Collections;
using Infonet.Data;
using Infonet.Data.Looking;
using Infonet.Data.Models.Clients;
using Infonet.Data.Models.Offenders;
using Infonet.Data.Models.Services;
using Infonet.Web.Mvc;
using Infonet.Web.Mvc.Authorization;
using Infonet.Web.Mvc.Collections;
using Infonet.Web.ViewModels.Case;
using PagedList;

namespace Infonet.Web.Controllers {
	[DenyCoalition]
	[Authorize(Roles = "DVADMIN, DVDATAENTRY, SAADMIN, SADATAENTRY, CACADMIN, CACDATAENTRY")]
	public class CaseController : InfonetControllerBase {
		private const int SERVICES_PAGINATION_PAGE_SIZE = 5;

		public CaseController() {
			db.Configuration.ValidateOnSaveEnabled = false;
		}

		#region Intake
		public ActionResult Edit(int? clientId, int? caseId) {
			var model = LoadOrCreate(clientId, caseId);
			if (model == null)
				return HttpNotFound();

			if (clientId != null && caseId == null)
				if (db.T_ClientCases.Where(cc => cc.ClientId == clientId).Max(cc => cc.FirstContactDate).Value >= DateTime.Today) {
					AddErrorMessage("Unable to add a new case. No valid First Contact Date available.");
					return Redirect(Request.UrlReferrer?.AbsoluteUri);
				}

			foreach (var each in model.FinancialResources) {
				if (each.IncomeSource2ID == -1)
					model.IsUnknownIncomeSourceSelected = true;
				if (each.IncomeSource2ID == -2)
					model.IsNoneIncomeSourceSelected = true;
			}

			PopulateEmptyIncomeModels(model);

			BagAgency();
			BagOtherCases(clientId);
			ConstructWarnings(model);

			TempData["ReturnUrl"] = clientId != null && !string.IsNullOrEmpty((string)TempData.Peek("ReturnUrl")) && ((string)TempData["ReturnUrl"]).Contains(model.ClientId.ToString()) ? TempData["ReturnUrl"] : "/Client/Search";
			ViewBag.UrlReferrer = TempData.Peek("ReturnUrl");

			return View(model);
		}

		[HttpPost, ActionName("Edit")]
		[ValidateAntiForgeryToken]
		[PreventDuplicateRequest]
		public ActionResult EditPost(int? clientId, int? caseId, string hash) {
			var model = LoadOrCreate(clientId, caseId);

			//when no races (checkboxes) selected, we must clear them
			if (Request.Params["Client.RaceHudIds"] == null)
				model.Client.RaceHudIds = new int[0];

			TryUpdateModel(model);
			ClearModelStateErrorsForKeysWithoutIncomingValues("Client.RaceHudIds");

			ValidateIntake(model);

			DeleteEmptyIncomeModels(model);

			if (ModelState.IsValid && !string.IsNullOrEmpty(Request.Params["ClientReferralSource.AgencyID"]))
				model.ClientReferralSource.AgencyName = db.Database.SqlQuery<string>("select AgencyName from T_Agency where AgencyId = @p0", Request.Params["ClientReferralSource.AgencyID"]).Single();

			var dbPresentingIssues = db.Ts_ClientPresentingIssue.AsNoTracking().SingleOrDefault(pi => pi.ClientId == model.ClientId && pi.CaseId == model.CaseId);
			if (model.PresentingIssues != null && model.PresentingIssues.IsEqualTo(dbPresentingIssues))
				db.Entry(model.PresentingIssues).State = EntityState.Unchanged;

			if (model.Provider == Provider.CAC && model.PresentingIssues?.StartDateOfAbuse > model.FirstContactDate)
				ModelState.AddModelError("PresentingIssues.StartDateOfAbuse", $"Approximate Abuse/Offense Date (or start of abuse) cannot be later than CAC Case Open Date. ({model.FirstContactDate:MM/dd/yyyy})");

			if (model.Provider == Provider.DV) {
				if (model.PresentingIssues?.DateOfPrimOffense < DateTime.Parse("01/01/1950"))
					ModelState.AddModelError("PresentingIssues.DateOfPrimOffense", "The field Primary Offense Date must be between 1/1/1950 and today.");
			} else {
				if (model.PresentingIssues?.StartDateOfAbuse < DateTime.Parse(model.Provider == Provider.SA ? "01/01/1930" : "01/01/1970"))
					ModelState.AddModelError("PresentingIssues.StartDateOfAbuse", "The field Approximate Abuse/Offense Date (or start of abuse) must be between " + (model.Provider == Provider.SA ? "1/1/1930" : "1/1/1970") + " and today.");
				if (model.PresentingIssues?.EndDateOfAbuse < DateTime.Parse(model.Provider == Provider.SA ? "01/01/1930" : "01/01/1970"))
					ModelState.AddModelError("PresentingIssues.EndDateOfAbuse", "The field End of Abuse/Offense Date (if applicable) must be between " + (model.Provider == Provider.SA ? "1/1/1930" : "1/1/1970") + " and today.");
			}

			if (model.NumberOfChildren != null && (model.Provider == Provider.CAC && !(model.NumberOfChildren >= 0 && model.NumberOfChildren <= 25) || model.Provider != Provider.CAC && !(model.NumberOfChildren >= -1 && model.NumberOfChildren <= 20)))
				ModelState.AddModelError("NumberOfChildren", "Number of Children must be between " + (model.Provider == Provider.CAC ? "0 and 25." : "-1 and 20."));

			if (model.FirstContactDate != null && model.Provider == Provider.CAC && model.FirstContactDate < DateTime.Parse("01/01/1990"))
				ModelState.AddModelError("FirstContactDate", "The field CAC Case Open Date must be between 1/1/1990 and today.");

			if (ModelState.IsValid && Save(model, ProcessFinancialResources, ProcessResidences))
				return Redirect(Url.Action("Edit", "Case", new { clientId = model.ClientId, caseId = model.CaseId }) + hash);

			AddErrorMessage("Errors have prevented the form being saved.");
			PopulateEmptyIncomeModels(model);

			BagAgency();
			BagOtherCases(model.Client.ClientId);
			ConstructWarnings(model);

			return View(model);
		}

		public ActionResult EditRedirect(int? clientId) {
			if (Request != null && Request.UrlReferrer != null)
				TempData["ReturnUrl"] = Request.UrlReferrer.AbsoluteUri;
			else
				TempData["ReturnUrl"] = "/Client/Search";
			return RedirectToAction("Edit", new { clientId });
		}

		public ActionResult RefreshPage(string url, string hash) {
			return Redirect(url + (hash == "" ? "" : "#" + hash));
		}

		private ClientCase LoadOrCreate(int? clientId, int? caseId) {
			int centerId = Session.Center().Top.Id;

			if (caseId != null)
				return db.T_ClientCases.SingleOrDefault(c => c.ClientId == clientId && c.CaseId == caseId && c.Client.CenterId == centerId);

			if (clientId != null) {
				var client = db.T_Client.SingleOrDefault(c => c.ClientId == clientId && c.CenterId == centerId);
				return client == null ? null : new ClientCase { Client = client };
			}

			int? clientTypeId = ConvertNull.ToInt32(Request.Params["clientType"]);
			if (clientTypeId != null)
				return (CaseTypes.For(clientTypeId) & CaseTypes.For(Session.Center().Provider)) == CaseType.None ? null : new ClientCase { Client = new Client { ClientTypeId = clientTypeId, CenterId = centerId } };

			return null;
		}

		private void ValidateIntake(ClientCase model) {
			bool isNewClient = model.Client.ClientId == null;
			bool isNewCase = model.CaseId == null;

			//validate ClientCode against database
			if (isNewClient && ModelState["Client.ClientCode"].Errors.Count == 0) {
				int matches = db.Database.SqlQuery<int>("select count(*) from T_Client where CenterId = @p0 and ClientCode = @p1", Session.Center().Top.Id, model.Client.ClientCode).Single();
				if (matches != 0)
					ModelState.AddModelError("Client.ClientCode", "This Client ID is already in use.");
			}

			//validate FirstContactDate against database
			if (ModelState["FirstContactDate"].Errors.Count == 0)
				if (!isNewCase) {
					var maxOlder = db.Database.SqlQuery<DateTime?>("select max(FirstContactDate) from T_ClientCases where ClientId = @p0 and CaseId < @p1", model.ClientId, model.CaseId).Single();
					var minNewer = db.Database.SqlQuery<DateTime?>("select min(FirstContactDate) from T_ClientCases where ClientId = @p0 and CaseId > @p1", model.ClientId, model.CaseId).Single();
					if (maxOlder != null && minNewer == null && model.FirstContactDate <= maxOlder)
						ModelState.AddModelError("FirstContactDate", $"First Contact Date must be after {maxOlder:M/d/yyyy}, the latest prior case.");
					else if (maxOlder == null && minNewer != null && model.FirstContactDate >= minNewer)
						ModelState.AddModelError("FirstContactDate", $"First Contact Date must be before {minNewer:M/d/yyyy}, the earliest following case.");
					else if (maxOlder != null && minNewer != null && (model.FirstContactDate <= maxOlder || model.FirstContactDate >= minNewer))
						ModelState.AddModelError("FirstContactDate", $"First Contact Date must be after {maxOlder:M/d/yyyy} but before {minNewer:M/d/yyyy}, the latest prior and earliest following cases.");
				} else if (!isNewClient) {
					var maxExisting = db.Database.SqlQuery<DateTime?>("select max(FirstContactDate) from T_ClientCases where ClientId = @p0", model.Client.ClientId).Single();
					if (maxExisting != null && model.FirstContactDate <= maxExisting)
						ModelState.AddModelError("FirstContactDate", $"First Contact Date must be after latest prior case ({maxExisting:M/d/yyyy}).");
				}

			var twntshipcounties = model.Client.TwnTshipCountyById;
			Key prevKey = null;
			foreach (var each in twntshipcounties.KeysFor(twntshipcounties.Values.OrderBy(t => t.LocID, true).ThenBy(t => twntshipcounties.KeyFor(t).Occurrence))) {
				string zipcode = model.Client.TwnTshipCountyById[each].Zipcode;
				if (zipcode != null && zipcode != "-1" && zipcode != "-2" && ModelState[$"Client.TwnTshipCountyById[{each}].Zipcode"].Errors.Count == 0)
					if (!Data.Usps.IsValidZip(zipcode, model.Client.TwnTshipCountyById[each].CountyID, model.Client.TwnTshipCountyById[each].StateID))
						ModelState.AddModelError($"Client.TwnTshipCountyById[{each}].Zipcode", "Invalid Zip Code for State/County");

				if (ModelState[$"Client.TwnTshipCountyById[{each}].MoveDate"].Errors.Count == 0) {
					var firstCase = isNewClient && isNewCase ? model : db.T_ClientCases.FirstOrDefault(cc => cc.ClientId == model.Client.ClientId && cc.CaseId == 1);
					bool isValidDate, isSamePriorDate;
					if (prevKey == null) {
						isSamePriorDate = false;
						isValidDate = (firstCase.FirstContactDate == null || model.Client.TwnTshipCountyById[each].MoveDate >= firstCase.FirstContactDate) && model.Client.TwnTshipCountyById[each].MoveDate <= DateTime.Now;
					} else {
						isSamePriorDate = model.Client.TwnTshipCountyById[each].MoveDate == model.Client.TwnTshipCountyById[prevKey].MoveDate;
						isValidDate = model.Client.TwnTshipCountyById[each].MoveDate > model.Client.TwnTshipCountyById[prevKey].MoveDate && model.Client.TwnTshipCountyById[each].MoveDate <= DateTime.Now;
					}
					if (!isValidDate && !isSamePriorDate)
						ModelState.AddModelError($"Client.TwnTshipCountyById[{each}].MoveDate", "Effective Date cannot be older than the First Contact Date (or effective date of previous residence), and can be no later than today's date (or the effective date of the next completed residence)");
					else if (isSamePriorDate)
						ModelState.AddModelError($"Client.TwnTshipCountyById[{each}].MoveDate", "Effective Date cannot be the same as the effective date of previous residence");

				}
				prevKey = each;
			}
		}

		//KMS DO is this in use?
		public JsonResult IsResidenceValid(int? clientId, int? caseId, string hash) {
			var model = LoadOrCreate(clientId, caseId);

			//when no races (checkboxes) selected, we must clear them
			if (Request.Params["Client.RaceHudIds"] == null)
				model.Client.RaceHudIds = new int[0];

			TryUpdateModel(model);
			//KMS DO need this here?: ClearModelStateErrorsForKeysWithoutIncomingValues("Client.RaceHudIds");

			JsonResult result = new JsonResult();
			if (ModelState.Any())
				result.Data = false;
			else
				result.Data = true;

			return result;
		}

		private bool Save(ClientCase model, params Action<ClientCase>[] preSaveActions) {
			bool isNewClient = model.Client.ClientId == null;
			bool isNewCase = model.CaseId == null;
			try {
				if (isNewCase) {
					model.CaseId = isNewClient ? 1 : db.Database.SqlQuery<int>("select coalesce(max(CaseId),0) + 1 from T_ClientCases where ClientId = @p0", model.Client.ClientId).Single();
					db.T_ClientCases.Add(model);
				}

                var cntResidencesChanged = 0;

                foreach (var each in preSaveActions)
                {
                    if(each.Method.Name == "ProcessResidences")
                    {
                        cntResidencesChanged = db.ChangeTracker.Entries<TwnTshipCounty>().Where(o => o.State != EntityState.Unchanged).Count();
                    }
                    each(model);
                }

                db.SaveChanges();

                if (preSaveActions.Where(x => x.Method.Name == "ProcessResidences").Count() > 0 && cntResidencesChanged > 0)
                {
                    UpdateServiceDetailTownTownshipCountyId(model.ClientId);
                }

                AddSuccessMessage("Your changes have been successfully saved.");
				return true;
			} catch (RetryLimitExceededException) {
				if (isNewCase)
					model.CaseId = null;
				ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
				return false;
			}
		}

        private void UpdateServiceDetailTownTownshipCountyId (int? clientId)
        {
            using (var transaction = db.Database.BeginTransaction())
                try
                {
                        db.Database.ExecuteSqlCommand(
                                    @"UPDATE Tl_ServiceDetailOfClient
                                         SET CityTownTownshpID =
                                         (
                                            SELECT      LocID
                                            FROM        Ts_TwnTshipCounty
                                            WHERE       ClientID = Tl_ServiceDetailOfClient.ClientID
                                            AND         MoveDate = (    SELECT      MAX(MoveDate)
                                                                        FROM        Ts_TwnTshipCounty t
                                                                        WHERE       t.ClientID = Tl_ServiceDetailOfClient.ClientID
                                                                        AND(Tl_ServiceDetailOfClient.ServiceDate >= t.MoveDate OR Tl_ServiceDetailOfClient.ShelterBegDate >= t.MoveDate)
                                                                    )
		                                 )
		                                 WHERE ClientID = @ClientID",
                                    new SqlParameter("ClientID", clientId)
                                );

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
        }

		private void ProcessFinancialResources(ClientCase model) {
			var originalFinancialResources = db.Ts_ClientFinancialResources.Where(m => m.CaseID == model.CaseId && m.ClientID == model.ClientId).ToList();

			//RP Modified -- CODE DOES Delete the garbage objects that were null and add new records in DB (solving foreign key issues FOR FinancialResource) 

			foreach (var postedFinancialResource in model.FinancialResources.ToList()) {
				var originalFinRes = originalFinancialResources.SingleOrDefault(x => x.ID == postedFinancialResource.ID);
				if (originalFinRes != null) {
					db.Entry(originalFinRes).State = EntityState.Detached;
					db.Ts_ClientFinancialResources.Attach(postedFinancialResource);
					db.Entry(postedFinancialResource).State = originalFinRes.IsUnchanged(postedFinancialResource) ? EntityState.Unchanged : EntityState.Modified;
				} else {
					db.Ts_ClientFinancialResources.Add(postedFinancialResource);
					db.Entry(postedFinancialResource).State = EntityState.Added;
				}
			}

			foreach (var originalFinRes in originalFinancialResources)
				if (model.FinancialResources.All(c => c.ID != originalFinRes.ID))
					db.Ts_ClientFinancialResources.Remove(originalFinRes);

			//RP Modified -- CODE DOES update financial resource table in database to add records if unknown or no income source is chosen
			if (model.IsUnknownIncomeSourceSelected || model.IsNoneIncomeSourceSelected) {
				var clientFinRes = new FinancialResource {
					Amount = model.IsNoneIncomeSourceSelected ? 0 : -1,
					CaseID = model.CaseId.GetValueOrDefault(),
					ClientID = model.ClientId.GetValueOrDefault(),
					ClientCase = model,
					IncomeSource2ID = model.IsUnknownIncomeSourceSelected ? -1 : -2
				};

				foreach (var originalFinRes in originalFinancialResources)
					if (model.FinancialResources.Any(c => c.ID == originalFinRes.ID))
						db.Ts_ClientFinancialResources.Remove(originalFinRes);

				db.Ts_ClientFinancialResources.Add(clientFinRes);
			}
		}

		private void BagOtherCases(int? clientId) {
			if (clientId == null)
				ViewBag.OtherCases = null;
			else
				ViewBag.OtherCases = db.Database.SqlQuery<OtherCase>("select CaseId, FirstContactDate, cast(coalesce(caseclosed, 0) as bit) as IsClosed from T_ClientCases where ClientId = @p0 order by 1", clientId);
		}

		private void BagAgency() {
			ViewBag.AgencyLookup = db.Database.SqlQuery<ListAgency>("SELECT dbo.T_Agency.AgencyID, dbo.T_Agency.AgencyName FROM dbo.LOOKUPLIST_Tables INNER JOIN dbo.LOOKUPLIST_ItemAssignment ON dbo.LOOKUPLIST_Tables.TableID = dbo.LOOKUPLIST_ItemAssignment.TableID INNER JOIN dbo.T_Agency ON dbo.LOOKUPLIST_ItemAssignment.CodeID = dbo.T_Agency.AgencyID WHERE dbo.LOOKUPLIST_ItemAssignment.ProviderID = @p0 AND dbo.LOOKUPLIST_ItemAssignment.IsActive = 1 AND dbo.LOOKUPLIST_ItemAssignment.TableID = 48 AND(dbo.T_Agency.Centerid = 0 OR dbo.T_Agency.CenterID in(@p1)) ORDER BY dbo.LOOKUPLIST_ItemAssignment.DisplayOrder, dbo.T_Agency.AgencyName", Session.Center().Provider.Id(), Session.Center().Id).ToList();
		}

		private void BagContacts(int? centerId, int? providerId) {
			ViewBag.ContactLookup = db.Database.SqlQuery<SimpleListItem>(@"	SELECT		CONVERT(varchar(10), dbo.T_Contact.ContactID) AS ID, 
                                                                                        dbo.T_Contact.ContactName AS Name 
                                                                            FROM		dbo.LOOKUPLIST_Tables 
                                                                            JOIN		dbo.LOOKUPLIST_ItemAssignment 
                                                                            ON			dbo.LOOKUPLIST_Tables.TableID = dbo.LOOKUPLIST_ItemAssignment.TableID 
                                                                            JOIN		dbo.T_Contact 
                                                                            ON			dbo.LOOKUPLIST_ItemAssignment.CodeID = dbo.T_Contact.ContactID 
                                                                            WHERE		dbo.LOOKUPLIST_ItemAssignment.ProviderID = 6 
                                                                            AND			dbo.LOOKUPLIST_ItemAssignment.IsActive = 1 
                                                                            AND			dbo.LOOKUPLIST_ItemAssignment.TableID = 49 
                                                                            AND			(dbo.T_Contact.Centerid = 0 OR dbo.T_Contact.CenterID in(@p1)) 
                                                                            ORDER BY	dbo.LOOKUPLIST_ItemAssignment.DisplayOrder, dbo.T_Contact.ContactName",
				providerId, centerId).ToList();
		}

		private void PopulateEmptyIncomeModels(ClientCase caze) {
			var listOfFinancialResources = new List<FinancialResource>();
			foreach (var i in Lookups.IncomeSource2[Provider.DV]) {
				var financialresource = new FinancialResource { IncomeSource2ID = i.CodeId };
				listOfFinancialResources.Add(financialresource);
				var dbValue = caze.FinancialResources.FirstOrDefault(x => financialresource.IncomeSource2ID == x.IncomeSource2ID);
				if (dbValue != null) {
					financialresource.Amount = dbValue.Amount;
					financialresource.CaseID = dbValue.CaseID;
					financialresource.ClientCase = dbValue.ClientCase;
					financialresource.ClientID = dbValue.ClientID;
					financialresource.ID = dbValue.ID;
				}
			}
			caze.FinancialResources = listOfFinancialResources;
		}

		private void DeleteEmptyIncomeModels(ClientCase model) {
			((List<FinancialResource>)model.FinancialResources).RemoveAll(m => m.Amount == null);
			foreach (var currentResource in model.FinancialResources) {
				currentResource.ClientID = model.ClientId.GetValueOrDefault();
				currentResource.CaseID = model.CaseId.GetValueOrDefault();
				currentResource.ClientCase = model;
			}
		}

		private void ConstructWarnings(ClientCase model) {
			if (model.IsClosed)
				if (model.ClosedDate != null)
					AddWarningMessage("Case " + model.CaseId + " was closed for client " + model.Client.ClientCode + " on " + model.ClosedDate.Value.ToShortDateString());
				else
					AddWarningMessage("Case " + model.CaseId + " was closed for client " + model.Client.ClientCode);

			ViewBag.WarningMessage = "";
			ViewBag.ShowWarning = false;
			ViewBag.NoDatesAvail = false;
			var sb = new StringBuilder();
			sb.Append("<div>Please review the following warnings before proceeding.</div>");
			sb.Append("<br><ul class='text-warning' style='font-weight:bold'>");
			var caseId = model.CaseId - 1;
			if (caseId == 0)
				caseId = model.CaseId;
			var cases = db.T_ClientCases.Where(cc => cc.ClientId == model.ClientId && cc.CaseId == caseId && !cc.ServiceDetailsOfClient.Any()).ToList();
			if (cases.Any()) {
				ViewBag.ShowWarning = true;
				sb.Append("<li><div>Client " + model.Client.ClientCode + " does not have any services entered for the following cases:</div>");
				sb.Append("<ul>");
				foreach (var each in cases)
					sb.Append("<li>" + each.CaseId + " - " + string.Format("{0:MM/dd/yyyy}", each.FirstContactDate) + "</li>");
				sb.Append("</ul></li>");
			}
			var minusOneYear = DateTime.Today.AddYears(-1);
			var prevCase = db.T_ClientCases.SingleOrDefault(cc => cc.ClientId == model.ClientId && cc.CaseId == db.T_ClientCases.Where(cc2 => cc2.ClientId == model.ClientId).Max(cc2 => cc2.CaseId) && cc.FirstContactDate == db.T_ClientCases.Where(cc2 => cc2.ClientId == model.ClientId).Max(cc2 => cc2.FirstContactDate) && cc.ServiceDetailsOfClient.Any(s => s.ServiceDate >= minusOneYear));
			if (prevCase != null) {
				ViewBag.ShowWarning = true;
				sb.Append(string.Format("<li><div>Client {0} has a service date within the last year on {1} for Case {2}.</div></li>", model.Client.ClientCode, string.Format("{0:MM/dd/yyyy}", prevCase.ServiceDetailsOfClient.Max(s => s.ServiceDate).Value), prevCase.CaseId));
			}
			sb.Append("</ul>");
			sb.Append("<div>Are you sure you want to add a new case for this client?</div>");
			ViewBag.WarningMessage = sb.ToString();

			prevCase = db.T_ClientCases.SingleOrDefault(cc => cc.ClientId == model.ClientId && cc.CaseId == db.T_ClientCases.Where(cc2 => cc2.ClientId == model.ClientId).Max(cc2 => cc2.CaseId));
			if (prevCase != null)
				ViewBag.NoDatesAvail = prevCase.FirstContactDate >= DateTime.Today;
		}
		#endregion

		#region Medical
		public ActionResult Medical(int clientId, int caseId) {
			var model = LoadOrCreate(clientId, caseId);

			if (model == null)
				return HttpNotFound();

			//this isn't very efficient, but because we used lazy loading, I'm not sure it's any less efficient
			if (((CaseType.Adult | CaseType.Victim) & model.CaseType) != CaseType.None)
				db.Entry(model).Collection(o => o.ClientCourtAppearances).Query().OrderBy(a => a.CourtDate.HasValue).ThenBy(a => a.CourtDate).Load();

			BagOtherCases(clientId);
			ConstructWarnings(model);

			return View("Edit", model);
		}

		[HttpPost, ActionName("Medical")]
		[ValidateAntiForgeryToken]
		[PreventDuplicateRequest]
		public ActionResult MedicalPost(int clientId, int caseId, string hash) {
			var model = LoadOrCreate(clientId, caseId);

			//copied from corresponding Medical(int,int)
			if (((CaseType.Adult | CaseType.Victim) & model.CaseType) != CaseType.None)
				db.Entry(model).Collection(o => o.ClientCourtAppearances).Query().OrderBy(a => a.CourtDate.HasValue).ThenBy(a => a.CourtDate).Load();

			TryUpdateModel(model);
			ClearModelStateErrorsForKeysWithoutIncomingValues();

			if (ModelState.IsValid && Save(model))
				return Redirect(Url.Action("Medical", "Case", new { clientId = model.ClientId, caseId = model.CaseId }) + hash);

			ConstructWarnings(model);
			BagOtherCases(model.Client.ClientId);
			return View("Edit", model);
		}
		#endregion

		#region Offenders
		public ActionResult Offenders(int clientId, int caseId) {
			var model = LoadOrCreate(clientId, caseId);
			if (model == null)
				return HttpNotFound();

			BagOtherCases(clientId);
			ConstructWarnings(model);

			return View("Edit", model);
		}

		[HttpPost, ActionName("Offenders")]
		[ValidateAntiForgeryToken]
		[PreventDuplicateRequest]
		public ActionResult OffendersPost(int clientId, int caseId, string hash) {
			var model = LoadOrCreate(clientId, caseId);

			TryUpdateModel(model);
			ValidateOffenders(model);
			ClearModelStateErrorsForKeysWithoutIncomingValues();

			if (ModelState.IsValid) {
				foreach (var each in model.Offenders) {
					if (each.StateId == Data.Usps.OutOfCountry.ID)
						each.CountyId = null;

					if (model.Provider == Provider.CAC) {
						int topCenterId = Session.Center().Top.Id;
						if (each.OffenderListingId != null && each.OffenderListing.OffenderListingId == null)
							each.OffenderListing = db.T_OffenderList.Single(o => o.OffenderListingId == each.OffenderListingId && o.ParentCenterId == topCenterId);
						each.OffenderListing.ParentCenterId = topCenterId;
						if (each.OffenderListing.RaceId != each.RaceId)
							each.OffenderListing.RaceId = each.RaceId;
						if (each.OffenderListing.SexId != each.SexId)
							each.OffenderListing.SexId = each.SexId;
						if (each.Age != -1 && each.OffenderListing.BirthYear != model.FirstContactDate.Value.Year - each.Age)
							each.OffenderListing.BirthYear = model.FirstContactDate.Value.Year - each.Age;
						else if (each.Age == -1)
							each.OffenderListing.BirthYear = 9999;
					}
				}

				Offender hashOffender = null;
				if (hash.StartsWith("#offender"))
					hashOffender = model.OffendersById[Key.Parse(hash.Substring(9).Replace('_', ':'))];

				if (Save(model)) {
					if (hashOffender != null)
						hash = !model.Offenders.Contains(hashOffender) ? "" : ("#offender" + model.OffendersById.KeyFor(hashOffender)).Replace(':', '_');
					return Redirect(Url.Action("Offenders", "Case", new { clientId = model.ClientId, caseId = model.CaseId }) + hash);
				}
			}
			BagOtherCases(model.Client.ClientId);
			ConstructWarnings(model);

			return View("Edit", model);
		}

		private void ValidateOffenders(ClientCase model) {
			if (model.Provider == Provider.CAC)
				foreach (var offender in model.OffendersById) {
					bool isNewOffender = offender.Value.OffenderListingId == null;
					if (isNewOffender && ModelState["OffendersById[" + offender.Key + "].OffenderListing.OffenderCode"].Errors.Count == 0) {
						int matches = db.Database.SqlQuery<int>("SELECT COUNT(*) FROM T_OffenderList WHERE ParentCenterID = @p0 AND OffenderCode = @p1", Session.Center().Top.Id, offender.Value.OffenderListing.OffenderCode).Single();
						if (0 != matches)
							ModelState.AddModelError("OffendersById[" + offender.Key + "].OffenderListing.OffenderCode", "This Offender ID is already in use.");
					}
				}
		}
		#endregion

		#region Investigation
		public ActionResult Investigation(int clientId, int caseId) {
			var caze = LoadOrCreate(clientId, caseId);
			if (caze == null)
				return HttpNotFound();

			BagOtherCases(clientId);
			BagAgency();
			BagContacts(Session.Center().Id, caze.Provider.Id());
			InitializeDcfsAllegations(caze);
			InitializePetitions(caze);
			BagRespondents(caze);
			ViewBag.HasRelatedClientsOrOffenders = HasRelatedClientsOrOffenders(caze);

			ConstructWarnings(caze);

			return View("Edit", caze);
		}

		[HttpPost, ActionName("Investigation")]
		[ValidateAntiForgeryToken]
		[PreventDuplicateRequest]
		public ActionResult InvestigationPost(int clientId, int caseId, string hash) {
			var caze = LoadOrCreate(clientId, caseId);

			TryUpdateModel(caze);
			ClearModelStateErrorsForKeysWithoutIncomingValues();
			ProcessAllegations(caze);
			ProcessPetitions(caze);

			ValidateInvestigation(caze);

			if (ModelState.IsValid && Save(caze, ProcessClientMDT))
				return Redirect(Url.Action("Investigation", "Case", new { clientId = caze.ClientId, caseId = caze.CaseId }) + hash);

			BagOtherCases(caze.Client.ClientId);
			BagAgency();
			BagContacts(Session.Center().Id, caze.Provider.Id());
			InitializeDcfsAllegations(caze);
			InitializePetitions(caze);
			BagRespondents(caze);
			ViewBag.HasRelatedClientsOrOffenders = HasRelatedClientsOrOffenders(caze);

			ConstructWarnings(caze);

			return View("Edit", caze);
		}

		private void InitializeDcfsAllegations(ClientCase caze) {
			if (HasRelatedClientsOrOffenders(caze)) {
				var exisitingRespondentsPerAllegation = new Dictionary<Key, List<string>>();
				var dcfsAllegations = caze.DCFSAllegationsById;
				foreach (var each in dcfsAllegations.KeysFor(dcfsAllegations.Values.Concat(dcfsAllegations.Values.Restorable).OrderByDescending(a => a.Id.HasValue).ThenBy(a => a.Id).ThenBy(a => dcfsAllegations.KeyFor(a).Occurrence))) {
					List<string> respondents = caze.DCFSAllegationsById[each].Respondents.Select(r => string.Format("{0}:{1}", r.RespondentType, r.RespondentId)).ToList();
					exisitingRespondentsPerAllegation.Add(each, respondents);
				}
				ViewBag.AllegationRespondentsDictionary = exisitingRespondentsPerAllegation;
			}
		}

		private void InitializePetitions(ClientCase caze) {
			if (HasRelatedClientsOrOffenders(caze)) {
				var exisitingRespondentsPerPetition = new Dictionary<Key, List<string>>();
				var petitions = caze.AbuseNeglectPetitionsById;
				foreach (var each in petitions.KeysFor(petitions.Values.Concat(petitions.Values.Restorable).OrderByDescending(a => a.Id.HasValue).ThenBy(a => a.Id).ThenBy(a => petitions.KeyFor(a).Occurrence))) {
					List<string> respondents = caze.AbuseNeglectPetitionsById[each].Respondents.Select(r => string.Format("{0}:{1}", r.RespondentType, r.RespondentId)).ToList();
					exisitingRespondentsPerPetition.Add(each, respondents);
				}
				ViewBag.PetitionRespondentsDictionary = exisitingRespondentsPerPetition;
			}
		}

		private void BagRespondents(ClientCase caze) {
			List<AllegationRespondent> clients = new List<AllegationRespondent>();
			List<AllegationRespondent> offenders = new List<AllegationRespondent>();
			var investigationClient = caze.InvestigationClients.FirstOrDefault();
			if (investigationClient != null)
				clients = investigationClient.Investigation.InvestigationClient.Where(ic => ic.ClientCase.Client.ClientTypeId == 8).Select(ic => new AllegationRespondent { ID = "1:" + ic.ClientID, Display = ic.ClientCase.Client.ClientCode }).ToList();
			var offenderList = caze.Offenders;
			if (offenderList != null)
				offenders = offenderList.Select(o => new AllegationRespondent { ID = "2:" + ((int)o.OffenderListingId).ToString(), Display = o.OffenderListing.OffenderCode }).ToList();
			clients = clients.Concat(offenders).ToList();
			ViewBag.Respondents = clients;
		}

		private bool HasRelatedClientsOrOffenders(ClientCase caze) {
			bool result = false;
			var investigationClient = caze.InvestigationClients.FirstOrDefault();
			if (investigationClient != null)
				result = investigationClient.Investigation.InvestigationClient.Any(ic => ic.ClientCase.Client.ClientTypeId == 8 && ic.ClientID != caze.ClientId);
			return result || caze.Offenders.Count > 0;
		}

		private void ProcessAllegations(ClientCase caze) {
			foreach (var each in caze.DCFSAllegationsById.KeysFor(caze.DCFSAllegationsById.Values.Concat(caze.DCFSAllegationsById.Values.Restorable).OrderByDescending(t => t.Id.HasValue).ThenBy(t => t.Id).ThenBy(t => caze.DCFSAllegationsById.KeyFor(t).Occurrence))) {
				var allegation = caze.DCFSAllegationsById[each];
				string[] respondents = allegation.RespondentArray;
				if (respondents != null) {
					foreach (string respKey in respondents) {
						string[] values = respKey.Split(':');
						int type = Convert.ToInt32(values[0]);
						int id = Convert.ToInt32(values[1]);
						if (!allegation.Respondents.Any(r => r.RespondentId == id && r.RespondentType == type))
							allegation.Respondents.Add(new DCFSAllegationRespondent { RespondentId = id, RespondentType = type });
					}
					var currentRespondents = allegation.Respondents.ToList();
					foreach (var existingRespondent in currentRespondents) {
						bool isDeleted = true;
						foreach (string respKey in respondents)
							if (respKey == existingRespondent.RespondentType + ":" + existingRespondent.RespondentId) {
								isDeleted = false;
								break;
							}
						if (isDeleted)
							allegation.Respondents.Remove(existingRespondent);
					}
				} else {
					if (allegation.Id != null)
						ModelState.AddModelError("DCFSAllegationsById[" + each + "].RespondentArray", "The Respondents field is required.");
				}
			}
			foreach (var each in caze.DCFSAllegationsById.KeysFor(caze.DCFSAllegationsById.Values.Restorable.OrderByDescending(t => t.Id.HasValue).ThenBy(t => t.Id).ThenBy(t => caze.DCFSAllegationsById.KeyFor(t).Occurrence))) {
				var allegation = caze.DCFSAllegationsById[each];
				if (allegation.Id != null)
					db.Ts_DCFSAllegations.Remove(allegation);
			}
			TryValidateModel(caze);
		}

		private void ProcessPetitions(ClientCase caze) {
			foreach (var each in caze.AbuseNeglectPetitionsById.KeysFor(caze.AbuseNeglectPetitionsById.Values.Concat(caze.AbuseNeglectPetitionsById.Values.Restorable).OrderByDescending(t => t.Id.HasValue).ThenBy(t => t.Id).ThenBy(t => caze.AbuseNeglectPetitionsById.KeyFor(t).Occurrence))) {
				var petition = caze.AbuseNeglectPetitionsById[each];
				string[] respondents = petition.RespondentArray;
				if (respondents != null) {
					foreach (string respKey in respondents) {
						string[] values = respKey.Split(':');
						int type = Convert.ToInt32(values[0]);
						int id = Convert.ToInt32(values[1]);
						if (!petition.Respondents.Any(r => r.RespondentId == id && r.RespondentType == type))
							petition.Respondents.Add(new AbuseNeglectPetitionRespondent { RespondentId = id, RespondentType = type });
					}
					List<AbuseNeglectPetitionRespondent> currentRespondents = petition.Respondents.ToList();
					foreach (var existingRespondent in currentRespondents) {
						bool isDeleted = true;
						foreach (string respKey in respondents)
							if (respKey == existingRespondent.RespondentType + ":" + existingRespondent.RespondentId)
								isDeleted = false;
						if (isDeleted)
							petition.Respondents.Remove(existingRespondent);
					}
				} else {
					if (petition.Id != null)
						ModelState.AddModelError("AbuseNeglectPetitionsById[" + each + "].RespondentArray", "The Respondents field is required.");
				}
			}
			foreach (var each in caze.AbuseNeglectPetitionsById.KeysFor(caze.AbuseNeglectPetitionsById.Values.Restorable.OrderByDescending(t => t.Id.HasValue).ThenBy(t => t.Id).ThenBy(t => caze.AbuseNeglectPetitionsById.KeyFor(t).Occurrence))) {
				var petition = caze.AbuseNeglectPetitionsById[each];
				if (petition.Id != null)
					db.Ts_AbuseNeglectPetitions.Remove(petition);
			}
			TryValidateModel(caze);
		}

		private void ProcessClientMDT(ClientCase caze) {
			foreach (var each in caze.ClientMDTById.Values.Restorable)
				if (each.MDT_ID != null)
					db.Entry(each).State = EntityState.Deleted;
		}

		private void ValidateInvestigation(ClientCase caze) {
			// Validate DCFS Allegation finding dates are not earlier than the First Contact Date of the current case
			foreach (var each in caze.DCFSAllegationsById.KeysFor(caze.DCFSAllegationsById.Values.OrderByDescending(t => t.Id.HasValue).ThenBy(t => t.Id).ThenBy(t => caze.DCFSAllegationsById.KeyFor(t).Occurrence)))
				if (caze.DCFSAllegationsById[each].FindingDate.HasValue && caze.FirstContactDate.HasValue && caze.FirstContactDate.Value > caze.DCFSAllegationsById[each].FindingDate.Value)
					ModelState.AddModelError("DCFSAllegationsById[" + each + "].FindingDate", "The Finding Date must be on or after the case's First Contact Date (" + string.Format("{0:MM/dd/yyyy}", caze.FirstContactDate.Value) + ").");

			// Validation for Exam Date for CAC Medical Visit
			var medicalVisits = caze.ClientCJProcessesById;
			foreach (var each in medicalVisits.KeysFor(medicalVisits.Values.Concat(medicalVisits.Values.Restorable).OrderByDescending(c => c.Med_ID.HasValue).ThenBy(c => c.Med_ID).ThenBy(c => medicalVisits.KeyFor(c).Occurrence)))
				if (medicalVisits[each].ExamDate.HasValue && caze.FirstContactDate.HasValue && medicalVisits[each].ExamDate.Value < caze.FirstContactDate.Value.AddYears(-1))
					ModelState.AddModelError("ClientCJProcessesById[" + each + "].ExamDate", "Cannot be older than one year prior to the First Contact Date. (" + string.Format("{0:MM/dd/yyyy}", caze.FirstContactDate.Value) + ").");

			// Validate Abuse Neglect Petition dates are not earlier than the First Contact Date of the current case
			foreach (var each in caze.AbuseNeglectPetitionsById.KeysFor(caze.AbuseNeglectPetitionsById.Values.OrderByDescending(t => t.Id.HasValue).ThenBy(t => t.Id).ThenBy(t => caze.AbuseNeglectPetitionsById.KeyFor(t).Occurrence)))
				if (caze.AbuseNeglectPetitionsById[each].PetitionDate.HasValue && caze.AbuseNeglectPetitionsById[each].AdjudicatedDate.HasValue && caze.AbuseNeglectPetitionsById[each].PetitionDate.Value > caze.AbuseNeglectPetitionsById[each].AdjudicatedDate.Value)
					ModelState.AddModelError("AbuseNeglectPetitionsById[" + each + "].AdjudicatedDate", "The Adjudicated Date must be on or after the Date of Petition.");
		}
		#endregion

		#region Services
		public ActionResult Services(int clientId, int caseId) {
			var caze = LoadOrCreate(clientId, caseId);
			if (caze == null)
				return HttpNotFound();
			var provider = Session.Center().Provider;
			Bag_OtherCases(clientId);

			BagDirectServices(new List<DirectServiceAdd> { new DirectServiceAdd() }, DirectServicesSearchPagedList(clientId, caseId, DateTime.Today.AddMonths(-3).Date, DateTime.Today.Date, SERVICES_PAGINATION_PAGE_SIZE, 1), new List<DirectServicesSearchDates> { new DirectServicesSearchDates { DirectServicesDateRangeStart = DateTime.Today.AddMonths(-3).Date, DirectServicesDateRangeEnd = DateTime.Today.Date } });

			if (provider == Provider.DV && Session.Center().AllRelated.Count(c => c.HasShelter) > 0)
				BagGroup_Departures_HousingServices(new List<DepartureAdd>(), new List<HousingServicesAdd> { new HousingServicesAdd() }, HousingServicesSearchPagedList(clientId, caseId, DateTime.Today.AddMonths(-3).Date, DateTime.Today.Date, SERVICES_PAGINATION_PAGE_SIZE, 1), new List<HousingServicesSearchDates> { new HousingServicesSearchDates { HousingServiceDateRangeStart = DateTime.Today.AddMonths(-3).Date, HousingServiceDateRangeEnd = DateTime.Today.Date } });

			if (provider != Provider.CAC)
				BagCancellations(new List<CancellationsAdd> { new CancellationsAdd() }, CancellationsSearchPagedList(clientId, caseId, DateTime.Today.AddMonths(-3).Date, DateTime.Today.Date, SERVICES_PAGINATION_PAGE_SIZE, 1), new List<CancellationsSearchDates> { new CancellationsSearchDates { CancellationsDateRangeStart = DateTime.Today.AddMonths(-3).Date, CancellationsDateRangeEnd = DateTime.Today.Date } });

			if (caze.Provider == Provider.CAC)
				BagReferral(new List<ReferralAdd> { new ReferralAdd() }, ReferralsSearchPagedList(clientId, caseId, DateTime.Today.AddMonths(-3).Date, DateTime.Today.Date, SERVICES_PAGINATION_PAGE_SIZE, 1), new List<ReferralSearchDates> { new ReferralSearchDates { ReferralDateRangeStart = DateTime.Today.AddMonths(-3).Date, ReferralDateRangeEnd = DateTime.Today.Date } });

			ConstructWarnings(caze);

			return View("Edit", caze);
		}

		[HttpPost, ActionName("Services")]
		[ValidateAntiForgeryToken]
		[PreventDuplicateRequest]
		public ActionResult ServicesPost(
			[Bind(Prefix = "DirectServicesEdit")] IList<DirectServiceEdit> directServiceModelEdit, [Bind(Prefix = "DirectServicesAdd")] IList<DirectServiceAdd> directServiceModelAdd, [Bind(Prefix = "DirectServiceSearchDates")]
			IList<DirectServicesSearchDates> directServiceSearchDates,
			[Bind(Prefix = "HousingServicesEdit")] IList<HousingServicesModify> housingServiceModelEdit, [Bind(Prefix = "HousingServicesAdd")] IList<HousingServicesAdd> housingServiceModelAdd, [Bind(Prefix = "HousingServiceSearchDates")]
			IList<HousingServicesSearchDates> housingServiceSearchDates,
			[Bind(Prefix = "DepartureAdd")] IList<DepartureAdd> departuresAdd, [Bind(Prefix = "CancellationsAdd")] IList<CancellationsAdd> cancellationsModelAdd, [Bind(Prefix = "CancellationsEdit")] IList<CancellationsEdit> cancellationsModelModify, [Bind(Prefix = "CancellationsSearchDates")]
			IList<CancellationsSearchDates> cancellationsSearchDates,
			[Bind(Prefix = "ReferralEdit")] IList<ReferralEdit> referralModelEdit, [Bind(Prefix = "ReferralAdd")] IList<ReferralAdd> referralModelAdd, [Bind(Prefix = "ReferralSearchDates")] IList<ReferralSearchDates> referralSearchDates,
			int clientId, int caseId, string hash) {
			var caze = LoadOrCreate(clientId, caseId);
			var provider = Session.Center().Provider;

			TryUpdateModel(caze);
			ClearModelStateErrorsForKeysWithoutIncomingValues();

			ModelStateCancellations(caze, cancellationsModelAdd, cancellationsModelModify);

			if (Session.Center().HasShelter) {
				ModelStateDepartureServices(caze, departuresAdd, housingServiceModelAdd, housingServiceModelEdit);
				ModelStateHousingServices(caze, housingServiceModelAdd, housingServiceModelEdit);
			}

			ModelStateDirectServices(caze, directServiceModelAdd, directServiceModelEdit);
			ModelStateReferral(caze, referralModelAdd, referralModelEdit);

			if (ModelState.IsValid) {
				if (SaveServices(caze, caseId, clientId, departuresAdd, cancellationsModelAdd, cancellationsModelModify, directServiceModelAdd, directServiceModelEdit, housingServiceModelAdd, housingServiceModelEdit, referralModelAdd, referralModelEdit))
					return Redirect(Url.Action("Services", "Case", new { clientId = caze.ClientId, caseId = caze.CaseId }) + hash);

				BagDirectServices(directServiceModelAdd, RecreateDirectServicesPagedList(clientId, caseId, directServiceModelEdit, directServiceSearchDates), directServiceSearchDates);
				Bag_OtherCases(caze.Client.ClientId);

				if (provider == Provider.DV && Session.Center().HasShelter)
					BagGroup_Departures_HousingServices(departuresAdd, housingServiceModelAdd, RecreateHousingServicesPagedList(clientId, caseId, housingServiceModelEdit, housingServiceSearchDates), housingServiceSearchDates);

				if (provider != Provider.CAC)
					BagCancellations(cancellationsModelAdd, RecreateCancellationsPagedList(clientId, caseId, cancellationsModelModify, cancellationsSearchDates), cancellationsSearchDates);
				if (caze.Provider == Provider.CAC)
					BagReferral(referralModelAdd, RecreateReferralPagedList(clientId, caseId, referralModelEdit, referralSearchDates), referralSearchDates);

				ConstructWarnings(caze);

				return View("Edit", caze);
			}

			if (ModelState[""] != null && ModelState[""].Errors.Count > 0) {
				Thread.Sleep(500);
				return Redirect(Url.Action("Services", "Case", new { clientId = caze.ClientId, caseId = caze.CaseId }) + hash);
			}

			BagDirectServices(directServiceModelAdd, RecreateDirectServicesPagedList(clientId, caseId, directServiceModelEdit, directServiceSearchDates), directServiceSearchDates);
			Bag_OtherCases(caze.Client.ClientId);

			if (provider == Provider.DV && Session.Center().HasShelter)
				BagGroup_Departures_HousingServices(departuresAdd, housingServiceModelAdd, RecreateHousingServicesPagedList(clientId, caseId, housingServiceModelEdit, housingServiceSearchDates), housingServiceSearchDates);

			if (provider != Provider.CAC)
				BagCancellations(cancellationsModelAdd, RecreateCancellationsPagedList(clientId, caseId, cancellationsModelModify, cancellationsSearchDates), cancellationsSearchDates);

			if (caze.Provider == Provider.CAC)
				BagReferral(referralModelAdd, RecreateReferralPagedList(clientId, caseId, referralModelEdit, referralSearchDates), referralSearchDates);

			ConstructWarnings(caze);

			return View("Edit", caze);
		}

		public IPagedList<DirectServiceEdit> RecreateDirectServicesPagedList(int clientId, int caseId, IList<DirectServiceEdit> directServiceModelEdit, IList<DirectServicesSearchDates> directServiceSearchDates) {
			if (directServiceModelEdit == null || directServiceSearchDates == null)
				return new PagedList<DirectServiceEdit>(null, 1, 1);

			var recreateDSPL = DirectServicesSearchPagedList(clientId, caseId, directServiceSearchDates[0].DirectServicesDateRangeStart, directServiceSearchDates[0].DirectServicesDateRangeEnd, directServiceSearchDates[0].PageSize ?? SERVICES_PAGINATION_PAGE_SIZE, directServiceSearchDates[0].Page ?? 1);

			for (int i = 0; i <= recreateDSPL.Count - 1; i++) {
				recreateDSPL[i].ICS_ID = directServiceModelEdit[i].ICS_ID;
				recreateDSPL[i].Index = directServiceModelEdit[i].Index;
				recreateDSPL[i].IsDeleted = directServiceModelEdit[i].IsDeleted;
				recreateDSPL[i].IsEdited = directServiceModelEdit[i].IsEdited;
				recreateDSPL[i].Location = directServiceModelEdit[i].Location;
				recreateDSPL[i].LocationID = directServiceModelEdit[i].LocationID;
				recreateDSPL[i].ReceivedHours = directServiceModelEdit[i].ReceivedHours;
				recreateDSPL[i].Service = directServiceModelEdit[i].Service;
				recreateDSPL[i].ServiceDate = directServiceModelEdit[i].ServiceDate;
				recreateDSPL[i].ServiceDetailID = directServiceModelEdit[i].ServiceDetailID;
				recreateDSPL[i].ServiceID = directServiceModelEdit[i].ServiceID;
				recreateDSPL[i].ShelterBegDate = directServiceModelEdit[i].ShelterBegDate;
				recreateDSPL[i].ShelterEndDate = directServiceModelEdit[i].ShelterEndDate;
				recreateDSPL[i].StaffName = directServiceModelEdit[i].StaffName;
				recreateDSPL[i].SVID = directServiceModelEdit[i].SVID;
			}
			return recreateDSPL;
		}
		public ActionResult DepartureAdd(int index, DepartureAdd departure) {
			departure.Index = index;

			ViewData.TemplateInfo.HtmlFieldPrefix = $"DepartureAdd[{departure.Index}]";
			return PartialView("_DeparturesAdd", departure);
		}

		public ActionResult DirectServicesAdd(int index, DirectServiceAdd directService) {
			directService.Index = index;

			ViewData.TemplateInfo.HtmlFieldPrefix = $"DirectServicesAdd[{directService.Index}]";
			return PartialView("_DirectServicesAdd", directService);
		}

		public PartialViewResult DirectServicesSearch(int? clientId, int? caseId, DateTime? fromDate, DateTime? toDate, int? pageSize, int? page) {
			return PartialView("_DirectServicesEdit", DirectServicesSearchPagedList(clientId, caseId, fromDate, toDate, pageSize, page));
		}


		public IPagedList<DirectServiceEdit> DirectServicesSearchPagedList(int? clientId, int? caseId, DateTime? fromDate, DateTime? toDate, int? pageSize, int? page) {
			return db.Tl_ServiceDetailOfClient
				.Where(s => s.ClientID == clientId && s.CaseID == caseId)
				.Where(ServiceDetailOfClient.IsNotShelter())
				.Where(ServiceDetailOfClient.ServiceDateBetween(fromDate, toDate))
				.OrderByDescending(s => s.ServiceBegDate)
				.Select(s => new DirectServiceEdit {
					ICS_ID = s.ICS_ID,
					Location = s.Center.CenterName,
					LocationID = s.LocationID,
					ReceivedHours = s.ReceivedHours,
					Service = s.TLU_Codes_ProgramsAndServices.Description,
					ServiceID = s.ServiceID,
					ServiceDate = s.ServiceDate,
					ServiceDetailID = s.ServiceDetailID,
					StaffName = s.StaffVolunteer.LastName + ", " + s.StaffVolunteer.FirstName,
					SVID = s.SVID,
					ShelterBegDate = s.ShelterBegDate,
					ShelterEndDate = s.ShelterEndDate
				}).ToPagedList(page ?? 1, pageSize ?? SERVICES_PAGINATION_PAGE_SIZE);
		}


		public PartialViewResult DirectServicesSearchDates(IList<DirectServicesSearchDates> directServiceSearchDates) {
			var searchDates = new DirectServicesSearchDates();

			if (directServiceSearchDates.Count > 0) {
				searchDates.DirectServicesDateRangeStart = directServiceSearchDates[0].DirectServicesDateRangeStart;
				searchDates.DirectServicesDateRangeEnd = directServiceSearchDates[0].DirectServicesDateRangeEnd;
			} else {
				searchDates.DirectServicesDateRangeStart = null;
				searchDates.DirectServicesDateRangeEnd = null;
			}

			ViewData.TemplateInfo.HtmlFieldPrefix = "directServiceSearchDates[0]";
			return PartialView("_DirectServicesSearch", searchDates);
		}

		public ActionResult HousingServicesAdd(int index, HousingServicesAdd housingService) {
			housingService.Index = index;
			ViewData.TemplateInfo.HtmlFieldPrefix = $"HousingServicesAdd[{housingService.Index}]";
			return PartialView("_HousingServicesAdd", housingService);
		}

		public PartialViewResult HousingServicesSearch(int? clientId, int? caseId, DateTime? fromDate, DateTime? toDate, int? pageSize, int? page) {
			return PartialView("_HousingServicesEdit", HousingServicesSearchPagedList(clientId, caseId, fromDate, toDate, pageSize, page));
		}

		public IPagedList<HousingServicesModify> HousingServicesSearchPagedList(int? clientId, int? caseId, DateTime? fromDate, DateTime? toDate, int? pageSize, int? page) {
			return db.Tl_ServiceDetailOfClient
				.Where(s => s.ClientID == clientId && s.CaseID == caseId)
				.Where(ServiceDetailOfClient.IsShelter())
				.Where(ServiceDetailOfClient.ShelterDatesIntersect(fromDate, toDate))
				.OrderByDescending(s => s.ShelterBegDate)
				.Select(s => new HousingServicesModify {
					ServiceDetailID = s.ServiceDetailID,
					ServiceID = s.ServiceID,
					SVID = s.SVID,
					ServiceDate = s.ServiceDate,
					LocationID = s.LocationID,
					ReceivedHours = s.ReceivedHours,
					ShelterBegDate = s.ShelterBegDate,
					ShelterEndDate = s.ShelterEndDate,
					StaffName = s.StaffVolunteer.LastName + ", " + s.StaffVolunteer.FirstName,
					Service = s.TLU_Codes_ProgramsAndServices.Description,
					Location = s.Center.CenterName,
					ICS_ID = s.ICS_ID
				}).ToPagedList(page ?? 1, pageSize ?? SERVICES_PAGINATION_PAGE_SIZE);
		}

		public PartialViewResult HousingServicesSearchDates(IList<HousingServicesSearchDates> housingServiceSearchDates) {
			var searchDates = new HousingServicesSearchDates();

			if (housingServiceSearchDates.Count > 0) {
				searchDates.HousingServiceDateRangeStart = housingServiceSearchDates[0].HousingServiceDateRangeStart;
				searchDates.HousingServiceDateRangeEnd = housingServiceSearchDates[0].HousingServiceDateRangeEnd;
			} else {
				searchDates.HousingServiceDateRangeStart = null;
				searchDates.HousingServiceDateRangeEnd = null;
			}

			ViewData.TemplateInfo.HtmlFieldPrefix = "housingServiceSearchDates[0]";
			return PartialView("_HousingServicesSearch", searchDates);
		}

		public IPagedList<HousingServicesModify> RecreateHousingServicesPagedList(int clientId, int caseId, IList<HousingServicesModify> housingServicesEdit, IList<HousingServicesSearchDates> housingServicesSearchDates) {
			if (housingServicesEdit == null || housingServicesSearchDates == null)
				return new PagedList<HousingServicesModify>(null, 1, 1);

			var recreateDSPL = HousingServicesSearchPagedList(clientId, caseId, housingServicesSearchDates[0].HousingServiceDateRangeStart, housingServicesSearchDates[0].HousingServiceDateRangeEnd, housingServicesSearchDates[0].PageSize ?? SERVICES_PAGINATION_PAGE_SIZE, housingServicesSearchDates[0].Page ?? 1);

			for (int i = 0; i <= recreateDSPL.Count - 1; i++) {
				recreateDSPL[i].ICS_ID = housingServicesEdit[i].ICS_ID;
				recreateDSPL[i].Index = housingServicesEdit[i].Index;
				recreateDSPL[i].IsDeleted = housingServicesEdit[i].IsDeleted;
				recreateDSPL[i].IsEdited = housingServicesEdit[i].IsEdited;
				recreateDSPL[i].Location = housingServicesEdit[i].Location;
				recreateDSPL[i].LocationID = housingServicesEdit[i].LocationID;
				recreateDSPL[i].ReceivedHours = housingServicesEdit[i].ReceivedHours;
				recreateDSPL[i].Service = housingServicesEdit[i].Service;
				recreateDSPL[i].ServiceDate = housingServicesEdit[i].ServiceDate;
				recreateDSPL[i].ServiceDetailID = housingServicesEdit[i].ServiceDetailID;
				recreateDSPL[i].ServiceID = housingServicesEdit[i].ServiceID;
				recreateDSPL[i].ShelterBegDate = housingServicesEdit[i].ShelterBegDate;
				recreateDSPL[i].ShelterEndDate = housingServicesEdit[i].ShelterEndDate;
				recreateDSPL[i].StaffName = housingServicesEdit[i].StaffName;
				recreateDSPL[i].SVID = housingServicesEdit[i].SVID;
			}
			return recreateDSPL;
		}

		public ActionResult CancellationsAdd(int index, CancellationsAdd cancellations) {
			cancellations.Index = index;

			ViewData.TemplateInfo.HtmlFieldPrefix = $"cancellationsAdd[{cancellations.Index}]";
			return PartialView("_CancellationsAdd", cancellations);
		}

		public PartialViewResult CancellationsSearch(int? clientId, int? caseId, DateTime? fromDate, DateTime? toDate, int? pageSize, int? page) {
			return PartialView("_CancellationsEdit", CancellationsSearchPagedList(clientId, caseId, fromDate, toDate, pageSize, page));
		}

		public IPagedList<CancellationsEdit> CancellationsSearchPagedList(int? clientId, int? caseId, DateTime? fromDate, DateTime? toDate, int? pageSize, int? page) {
			return db.Tl_Cancellations
				.Where(c => c.ClientID == clientId && c.CaseID == caseId)
				.Where(Cancellation.DateBetween(fromDate, toDate))
				.OrderByDescending(c => c.Date)
				.Select(c => new CancellationsEdit {
					CaseID = c.CaseID,
					ClientID = c.ClientID,
					Date = c.Date,
					ID = c.ID,
					LocationID = c.LocationID,
					ServiceCancellationID = c.ID,
					ReasonID = c.ReasonID,
					ServiceID = c.ServiceID,
					SVID = c.SVID
				}).ToPagedList(page ?? 1, pageSize ?? SERVICES_PAGINATION_PAGE_SIZE);
		}


		public PartialViewResult CancellationsSearchDates(IList<CancellationsSearchDates> cancellationsSearchDates) {
			var searchDates = new CancellationsSearchDates();

			if (cancellationsSearchDates.Count > 0) {
				searchDates.CancellationsDateRangeStart = cancellationsSearchDates[0].CancellationsDateRangeStart;
				searchDates.CancellationsDateRangeEnd = cancellationsSearchDates[0].CancellationsDateRangeEnd;
			} else {
				searchDates.CancellationsDateRangeStart = null;
				searchDates.CancellationsDateRangeEnd = null;
			}

			ViewData.TemplateInfo.HtmlFieldPrefix = "cancellationsSearchDates[0]";
			return PartialView("_CancellationsSearch", searchDates);
		}

		public IPagedList<CancellationsEdit> RecreateCancellationsPagedList(int clientId, int caseId, IList<CancellationsEdit> cancellationsEdit, IList<CancellationsSearchDates> cancellationsSearchDates) {
			if (cancellationsEdit == null || cancellationsSearchDates == null)
				return new PagedList<CancellationsEdit>(null, 1, 1);

			var recreateDSPL = CancellationsSearchPagedList(clientId, caseId, cancellationsSearchDates[0].CancellationsDateRangeStart, cancellationsSearchDates[0].CancellationsDateRangeEnd, cancellationsSearchDates[0].PageSize ?? SERVICES_PAGINATION_PAGE_SIZE, cancellationsSearchDates[0].Page ?? 1);

			for (int i = 0; i <= recreateDSPL.Count - 1; i++) {
				recreateDSPL[i].CaseID = cancellationsEdit[i].CaseID;
				recreateDSPL[i].ClientID = cancellationsEdit[i].ClientID;
				recreateDSPL[i].Date = cancellationsEdit[i].Date;
				recreateDSPL[i].ID = cancellationsEdit[i].ID;
				recreateDSPL[i].Index = cancellationsEdit[i].Index;
				recreateDSPL[i].IsDeleted = cancellationsEdit[i].IsDeleted;
				recreateDSPL[i].IsEdited = cancellationsEdit[i].IsEdited;
				recreateDSPL[i].LocationID = cancellationsEdit[i].LocationID;
				recreateDSPL[i].ReasonID = cancellationsEdit[i].ReasonID;
				recreateDSPL[i].ServiceCancellationID = cancellationsEdit[i].ServiceCancellationID;
				recreateDSPL[i].ServiceID = cancellationsEdit[i].ServiceID;
				recreateDSPL[i].SVID = cancellationsEdit[i].SVID;
			}
			return recreateDSPL;
		}

		public ActionResult ReferralAdd(int index, ReferralAdd referral) {
			BagAgency();

			referral.Index = index;
			ViewData.TemplateInfo.HtmlFieldPrefix = string.Format("ReferralAdd[{0}]", referral.Index);
			return PartialView("_ReferralAdd", referral);
		}

		public PartialViewResult ReferralSearch(int? clientId, int? caseId, DateTime? fromDate, DateTime? toDate, int? pageSize, int? page) {
			return PartialView("_ReferralEdit", ReferralsSearchPagedList(clientId, caseId, fromDate, toDate, pageSize, page));
		}

		public IPagedList<ReferralEdit> ReferralsSearchPagedList(int? clientId, int? caseId, DateTime? fromDate, DateTime? toDate, int? pageSize, int? page) {
			BagAgency();
			return db.Ts_ClientReferralDetail
				.Where(r => r.ClientID == clientId && r.CaseID == caseId)
				.Where(ClientReferralDetail.ReferralDateBetween(fromDate, toDate))
				.OrderByDescending(r => r.ReferralDate)
				.Select(r => new ReferralEdit {
					AgencyID = r.AgencyID,
					LocationID = r.LocationID,
					ReferralDate = r.ReferralDate,
					ReferralDetailID = r.ReferralDetailID,
					ReferralTypeID = r.ReferralTypeID,
					ResponseID = r.ResponseID,
				}).ToPagedList(page ?? 1, pageSize ?? SERVICES_PAGINATION_PAGE_SIZE);
		}

		public PartialViewResult ReferralSearchDates(IList<ReferralSearchDates> referralSearchDates) {
			var referral = new ReferralSearchDates();

			if (referralSearchDates.Count > 0) {
				referral.ReferralDateRangeStart = referralSearchDates[0].ReferralDateRangeStart;
				referral.ReferralDateRangeEnd = referralSearchDates[0].ReferralDateRangeEnd;
			} else {
				referral.ReferralDateRangeStart = null;
				referral.ReferralDateRangeEnd = null;
			}

			ViewData.TemplateInfo.HtmlFieldPrefix = "referralSearchDates[0]";
			return PartialView("_ReferralSearch", referral);
		}

		public IPagedList<ReferralEdit> RecreateReferralPagedList(int clientId, int caseId, IList<ReferralEdit> referralEdit, IList<ReferralSearchDates> referralSearchDates) {
			if (referralEdit == null || referralSearchDates == null)
				return new PagedList<ReferralEdit>(null, 1, 1);

			var recreateDSPL = ReferralsSearchPagedList(clientId, caseId, referralSearchDates[0].ReferralDateRangeStart, referralSearchDates[0].ReferralDateRangeEnd, referralSearchDates[0].PageSize ?? SERVICES_PAGINATION_PAGE_SIZE, referralSearchDates[0].Page ?? 1);

			for (int i = 0; i <= recreateDSPL.Count - 1; i++) {
				recreateDSPL[i].AgencyID = referralEdit[i].AgencyID;
				recreateDSPL[i].Index = referralEdit[i].Index;
				recreateDSPL[i].IsDeleted = referralEdit[i].IsDeleted;
				recreateDSPL[i].IsEdited = referralEdit[i].IsEdited;
				recreateDSPL[i].LocationID = referralEdit[i].LocationID;
				recreateDSPL[i].ReferralDate = referralEdit[i].ReferralDate;
				recreateDSPL[i].ReferralDetailID = referralEdit[i].ReferralDetailID;
				recreateDSPL[i].ReferralTypeID = referralEdit[i].ReferralTypeID;
				recreateDSPL[i].ResponseID = referralEdit[i].ResponseID;
			}
			return recreateDSPL;
		}

		private void Bag_OtherCases(int? clientId) {
			BagOtherCases(clientId);
		}

		private void BagGroup_Departures_HousingServices(IList<DepartureAdd> departuresAdd, IList<HousingServicesAdd> housingServiceModelAdd, IPagedList<HousingServicesModify> housingServiceModelEdit, IList<HousingServicesSearchDates> housingServiceSearchDates) {
			ViewBag.DeparturesAdd = departuresAdd;
			ViewBag.HousingServiceSearchDates = housingServiceSearchDates;
			ViewBag.HousingServicesEdit = housingServiceModelEdit;
			ViewBag.HousingServicesAdd = housingServiceModelAdd;
		}

		private void BagDirectServices(IList<DirectServiceAdd> directServiceModelAdd, IPagedList<DirectServiceEdit> directServiceModelEdit, IList<DirectServicesSearchDates> directServiceSearchDates) {
			ViewBag.DirectServiceSearchDates = directServiceSearchDates;
			ViewBag.DirectServicesEdit = directServiceModelEdit;
			ViewBag.DirectServicesAdd = directServiceModelAdd;
		}

		private void BagReferral(IList<ReferralAdd> referralModelAdd, IPagedList<ReferralEdit> referralModelEdit, IList<ReferralSearchDates> referralSearchDates) {
			BagAgency();
			ViewBag.ReferralSearchDates = referralSearchDates;
			ViewBag.ReferralEdit = referralModelEdit;
			ViewBag.ReferralAdd = referralModelAdd;
		}

		private void BagCancellations(IList<CancellationsAdd> cancellationsModelAdd, IPagedList<CancellationsEdit> cancellationsModelModify, IList<CancellationsSearchDates> cancellationsSearchDates) {
			ViewBag.CancellationsAdd = cancellationsModelAdd;
			ViewBag.CancellationsEdit = cancellationsModelModify;
			ViewBag.CancellationsSearchDates = cancellationsSearchDates;
		}


		private void ModelStateDepartureServices(ClientCase caze, IList<DepartureAdd> departuresAdd, IList<HousingServicesAdd> housingServiceModelAdd, IList<HousingServicesModify> housingServiceModelEdit) {
			var newHousingServices = housingServiceModelAdd.Where(x => x.ShelterEndDate != null && x.IsAdded);

			var existingHousingServices = caze.ServiceDetailsOfClient.Where(x => Array.AsReadOnly(new[] { 65, 66, 118 }).Contains(x.ServiceID));
			var existingHousingServicesEndDates = existingHousingServices.Where(x => x.ShelterEndDate != null).OrderBy(x => x.ShelterEndDate).Select(x => x.ShelterEndDate);
			var earliestExistingShelterEndDate = existingHousingServicesEndDates.FirstOrDefault();

			if (departuresAdd != null)
				for (int i = 0; i < departuresAdd.Count; i++)
					if (departuresAdd[i].IsDeleted) {
						ModelState.RemoveWithPrefix("DepartureAdd[" + i + "]");
					} else {
                        if (earliestExistingShelterEndDate == null && ((housingServiceModelAdd == null ? 0 : housingServiceModelAdd.Where(x => x.ShelterEndDate != null && x.ShelterEndDate == departuresAdd[i].DepartureDate && x.IsAdded).Count()) + (housingServiceModelEdit == null ? 0 : housingServiceModelEdit.Where(x => x.ShelterEndDate != null && x.ShelterEndDate == departuresAdd[i].DepartureDate && x.IsEdited).Count()) == 0)) {
							ModelState.AddModelError("DepartureAdd[" + i + "].DepartureDate", "Must match a Shelter End Date in client’s saved service records.");
						} else {
							if (departuresAdd[i].DepartureDate < earliestExistingShelterEndDate) {
								ModelState.AddModelError("DepartureAdd[" + i + "].DepartureDate", "Cannot be older than the client’s earliest Shelter End Date among saved service records.");
							} else if (!existingHousingServicesEndDates.Contains(departuresAdd[i].DepartureDate) && ((housingServiceModelAdd == null ? 0 : housingServiceModelAdd.Where(x => x.ShelterEndDate != null && x.ShelterEndDate == departuresAdd[i].DepartureDate && x.IsAdded).Count()) + (housingServiceModelEdit == null ? 0 : housingServiceModelEdit.Where(x => x.ShelterEndDate != null && x.ShelterEndDate == departuresAdd[i].DepartureDate && x.IsEdited).Count()) == 0)) {
								ModelState.AddModelError("DepartureAdd[" + i + "].DepartureDate", "Must match a Shelter End Date in client’s saved service records.");
							}
						}
					}

			if (caze.ClientDepartures != null)
				for (int i = 0; i < caze.ClientDepartures.Count; i++)

					if (caze.ClientDepartures[i].IsDeleted) {
						ModelState.RemoveWithPrefix("ClientDepartures[" + i + "]");
					} else {
						var originalDepartures = db.Ts_ClientDeparture.Where(t => t.ClientID == caze.ClientId).ToList();
						var origDeparture = originalDepartures.SingleOrDefault(r => r.DepartureID == caze.ClientDepartures[i].DepartureID);

						if (!origDeparture.IsUnchanged(caze.ClientDepartures[i])) {
							if (earliestExistingShelterEndDate == null && ((housingServiceModelAdd == null ? 0 : housingServiceModelAdd.Where(x => x.ShelterEndDate != null && x.ShelterEndDate == caze.ClientDepartures[i].DepartureDate && x.IsAdded).Count()) + (housingServiceModelEdit == null ? 0 : housingServiceModelEdit.Where(x => x.ShelterEndDate != null && x.ShelterEndDate == caze.ClientDepartures[i].DepartureDate && x.IsEdited).Count()) == 0)) {
								ModelState.AddModelError("ClientDepartures[" + i + "].DepartureDate", "Must match a Shelter End Date in client’s saved service records.");
							} else {
								if (caze.ClientDepartures[i].DepartureDate < earliestExistingShelterEndDate) {
									ModelState.AddModelError("ClientDepartures[" + i + "].DepartureDate", "Cannot be older than the client’s earliest Shelter End Date among saved service records.");
								} else if (!existingHousingServicesEndDates.Contains(caze.ClientDepartures[i].DepartureDate) && ((housingServiceModelAdd == null ? 0 : housingServiceModelAdd.Where(x => x.ShelterEndDate != null && x.ShelterEndDate == caze.ClientDepartures[i].DepartureDate && x.IsAdded).Count()) + (housingServiceModelEdit == null ? 0 : housingServiceModelEdit.Where(x => x.ShelterEndDate != null && x.ShelterEndDate == caze.ClientDepartures[i].DepartureDate && x.IsEdited).Count()) == 0)) {
									ModelState.AddModelError("ClientDepartures[" + i + "].DepartureDate", "Must match a Shelter End Date in client’s saved service records.");
								}
							}
						}
					}
		}

		private void ModelStateDirectServices(ClientCase caze, IList<DirectServiceAdd> directServiceModelAdd, IList<DirectServiceEdit> directServiceModelEdit) {
			int centerId = Session.Center().Id;

			ModelState.RemoveWithPrefix("directServiceSearchDates[0]");
			if (directServiceModelAdd != null)
				for (int i = 0; i < directServiceModelAdd.Count; i++)
					if (directServiceModelAdd[i].IsDeleted) {
						ModelState.RemoveWithPrefix("DirectServicesAdd[" + i + "]");
					} else {
						if (!(directServiceModelAdd[i].ServiceID == null && directServiceModelAdd[i].SVID == null && directServiceModelAdd[i].ServiceDate == null && directServiceModelAdd[i].ReceivedHours == null)) {
							if (directServiceModelAdd[i].ServiceID == null)
								ModelState.AddModelError("DirectServicesAdd[" + i + "].ServiceID", "The Service field is required.");
							if (directServiceModelAdd[i].SVID == null)
								ModelState.AddModelError("DirectServicesAdd[" + i + "].SVID", "The Staff/Volunteer field is required.");
							if (directServiceModelAdd[i].ServiceDate == null)
								ModelState.AddModelError("DirectServicesAdd[" + i + "].ServiceDate", "The Date field is required.");
							if (directServiceModelAdd[i].ReceivedHours == null)
								ModelState.AddModelError("DirectServicesAdd[" + i + "].ReceivedHours", "The Hours field is required.");
							if (directServiceModelAdd[i].ServiceDate < caze.FirstContactDate)
								ModelState.AddModelError("DirectServicesAdd[" + i + "].ServiceDate", "Cannot be earlier than the case's First Contact Date.");
						}
						var addServiceDate = directServiceModelAdd[i].ServiceDate;
						var addSvid = directServiceModelAdd[i].SVID;
						if (addServiceDate != null && addSvid != null && db.T_StaffVolunteer.Count(sv => sv.CenterId == centerId && sv.SvId == addSvid && (sv.StartDate == null || sv.StartDate <= addServiceDate) && (sv.TerminationDate == null || sv.TerminationDate > addServiceDate)) == 0)
							ModelState.AddModelError("DirectServicesAdd[" + i + "].SVID", Resource.StringErrorMessageStaffNotActiveDuringService);
					}

			if (directServiceModelEdit != null)
				for (int i = 0; i < directServiceModelEdit.Count; i++)
					if (directServiceModelEdit[i].IsEdited == false || directServiceModelEdit[i].IsDeleted) {
						ModelState.RemoveWithPrefix("DirectServicesEdit[" + i + "]");
					} else {
						if (directServiceModelEdit[i].ServiceDate < caze.FirstContactDate)
							ModelState.AddModelError("DirectServicesEdit[" + i + "].ServiceDate", "Cannot be earlier than the case's First Contact Date.");
						var addServiceDate = directServiceModelEdit[i].ServiceDate;
						var addSvid = directServiceModelEdit[i].SVID;
						if (addServiceDate != null && addSvid != null && db.T_StaffVolunteer.Count(sv => sv.CenterId == centerId && sv.SvId == addSvid && (sv.StartDate == null || sv.StartDate <= addServiceDate) && (sv.TerminationDate == null || sv.TerminationDate > addServiceDate)) == 0)
							ModelState.AddModelError("DirectServicesEdit[" + i + "].SVID", Resource.StringErrorMessageStaffNotActiveDuringService);
					}

			if (directServiceModelEdit != null) {
				var serviceDetailDelete = directServiceModelEdit.Where(m => m.IsDeleted);

				foreach (var serviceDelete in serviceDetailDelete) {
					var currentServiceDetailRec = caze.ServiceDetailsOfClient.FirstOrDefault(m => m.ServiceDetailID == serviceDelete.ServiceDetailID);
					if (currentServiceDetailRec != null)
						db.Entry(currentServiceDetailRec).State = EntityState.Deleted;
				}
			}
		}

		private void ModelStateHousingServices(ClientCase caze, IList<HousingServicesAdd> housingServiceModelAdd, IList<HousingServicesModify> housingServiceModelEdit) {
			ModelState.RemoveWithPrefix("housingServiceSearchDates[0]");
			var tranHousing = new List<int?> { 65, 66, 118 };
			var tranHousingDetails = caze.ServiceDetailsOfClient.ToList();
			tranHousingDetails.RemoveAll(x => !tranHousing.Contains(x.ServiceID));
			if (housingServiceModelEdit != null)
				tranHousingDetails.RemoveAll(thd => housingServiceModelEdit.Select(s => s.ServiceDetailID).ToArray().Contains(thd.ServiceDetailID));

			int emptyTranHousingEndDates = tranHousingDetails.Count(x => x.ShelterEndDate == null);
			int emptyAddTranHousingEndDates = housingServiceModelAdd.Count(x => x.ShelterEndDate == null && x.IsAdded);
			int emptyHousingServiceModelEdit = housingServiceModelEdit == null ? 0 : housingServiceModelEdit.Count(x => x.ShelterEndDate == null);

			if (Session.Center().HasShelter && housingServiceModelAdd != null)
				for (int i = 0; i < housingServiceModelAdd.Count; i++)
					if (housingServiceModelAdd[i].IsDeleted) {
						ModelState.RemoveWithPrefix("HousingServicesAdd[" + i + "]");
					} else {
						if (!(housingServiceModelAdd[i].ServiceID == null && housingServiceModelAdd[i].ShelterBegDate == null && housingServiceModelAdd[i].ShelterEndDate == null)) {
							if (housingServiceModelAdd[i].ServiceID == null)
								ModelState.AddModelError("HousingServicesAdd[" + i + "].ServiceID", "The Service field is required.");
							if (housingServiceModelAdd[i].ShelterBegDate == null)
								ModelState.AddModelError("HousingServicesAdd[" + i + "].ShelterBegDate", "The Shelter Begin field is required.");
						}
						if (housingServiceModelAdd[i].ShelterBegDate < caze.FirstContactDate)
							ModelState.AddModelError("HousingServicesAdd[" + i + "].ShelterBegDate", "Shelter Begin cannot be earlier than the case's First Contact Date.");
						if (housingServiceModelAdd[i].ShelterEndDate < caze.FirstContactDate)
							ModelState.AddModelError("HousingServicesAdd[" + i + "].ShelterEndDate", "Shelter End cannot be earlier than the case's First Contact Date.");
						if (housingServiceModelAdd[i].ShelterBegDate > housingServiceModelAdd[i].ShelterEndDate)
							ModelState.AddModelError("HousingServicesAdd[" + i + "].ShelterBegDate", "Shelter/Tran. Housing Begin cannot be greater than the Shelter/Tran. Housing End.");
						if (housingServiceModelAdd[i].ShelterEndDate < housingServiceModelAdd[i].ShelterBegDate)
							ModelState.AddModelError("HousingServicesAdd[" + i + "].ShelterEndDate", "Shelter/Tran. Housing End cannot be less than Shelter/Tran. Housing Begin.");

						var hsAdds = housingServiceModelAdd.ToList();
						hsAdds.RemoveAt(i);
						hsAdds.RemoveAll(x => x.IsAdded == false);

						foreach (var hsAdd in hsAdds) {
							if (hsAdd.ShelterBegDate != null && hsAdd.ShelterEndDate != null) {
								if (housingServiceModelAdd[i].ShelterBegDate > hsAdd.ShelterBegDate && housingServiceModelAdd[i].ShelterBegDate < hsAdd.ShelterEndDate)
									ModelState.AddModelError("HousingServicesAdd[" + i + "].ShelterBegDate", "Shelter/Tran. Housing Begin cannot conflict with an existing Shelter/Trans. Housing service period that exists for this case.");
								if (housingServiceModelAdd[i].ShelterEndDate > hsAdd.ShelterBegDate && housingServiceModelAdd[i].ShelterEndDate < hsAdd.ShelterEndDate)
									ModelState.AddModelError("HousingServicesAdd[" + i + "].ShelterEndDate", "Shelter/Tran. Housing End cannot conflict with an existing Shelter/Trans. Housing service period that exists for this case.");
							}
							if (hsAdd.ShelterEndDate == null)
								if (hsAdd.ShelterBegDate <= housingServiceModelAdd[i].ShelterBegDate)
									ModelState.AddModelError("HousingServicesAdd[" + i + "].ShelterBegDate", "Shelter/Tran. Housing Begin cannot conflict with an existing Shelter/Trans. Housing service period that exists for this case.");
						}

						foreach (var tranHousingDetail in tranHousingDetails) {
							if (emptyTranHousingEndDates + emptyAddTranHousingEndDates + emptyHousingServiceModelEdit > 1) {
								if (housingServiceModelAdd[i].ShelterBegDate != null)
									ModelState.AddModelError("HousingServicesAdd[" + i + "].ShelterBegDate", "Shelter/Tran. Housing Begin cannot conflict with an existing Shelter/Trans. Housing service period that exists for this case.");
								if (housingServiceModelAdd[i].ShelterEndDate != null)
									ModelState.AddModelError("HousingServicesAdd[" + i + "].ShelterEndDate", "Shelter/Tran. Housing End cannot conflict with an existing Shelter/Trans. Housing service period that exists for this case.");
								continue;
							}

							if (tranHousingDetail.ShelterBegDate != null)
								if (housingServiceModelAdd[i].ShelterBegDate >= tranHousingDetail.ShelterBegDate && housingServiceModelAdd[i].ShelterBegDate < tranHousingDetail.ShelterEndDate)
									ModelState.AddModelError("HousingServicesAdd[" + i + "].ShelterBegDate", "Shelter/Tran. Housing Begin cannot conflict with an existing Shelter/Trans. Housing service period that exists for this case.");

							if (tranHousingDetail.ShelterEndDate != null)
								if (housingServiceModelAdd[i].ShelterEndDate > tranHousingDetail.ShelterBegDate && housingServiceModelAdd[i].ShelterEndDate <= tranHousingDetail.ShelterEndDate)
									ModelState.AddModelError("HousingServicesAdd[" + i + "].ShelterEndDate", "Shelter/Tran. Housing End cannot conflict with an existing Shelter/Trans. Housing service period that exists for this case.");
						}
						if(housingServiceModelEdit != null)
							foreach (var tranHousingDetail in housingServiceModelEdit) {
								if (emptyTranHousingEndDates + emptyAddTranHousingEndDates + emptyHousingServiceModelEdit > 1) {
									if (housingServiceModelAdd[i].ShelterBegDate != null)
										ModelState.AddModelError("HousingServicesAdd[" + i + "].ShelterBegDate", "Shelter/Tran. Housing Begin cannot conflict with an existing Shelter/Trans. Housing service period that exists for this case.");
									if (housingServiceModelAdd[i].ShelterEndDate != null)
										ModelState.AddModelError("HousingServicesAdd[" + i + "].ShelterEndDate", "Shelter/Tran. Housing End cannot conflict with an existing Shelter/Trans. Housing service period that exists for this case.");
									continue;
								}

								if (tranHousingDetail.ShelterBegDate != null)
									if (housingServiceModelAdd[i].ShelterBegDate >= tranHousingDetail.ShelterBegDate && housingServiceModelAdd[i].ShelterBegDate < tranHousingDetail.ShelterEndDate)
										ModelState.AddModelError("HousingServicesAdd[" + i + "].ShelterBegDate", "Shelter/Tran. Housing Begin cannot conflict with an existing Shelter/Trans. Housing service period that exists for this case.");

								if (tranHousingDetail.ShelterEndDate != null)
									if (housingServiceModelAdd[i].ShelterEndDate > tranHousingDetail.ShelterBegDate && housingServiceModelAdd[i].ShelterEndDate <= tranHousingDetail.ShelterEndDate)
										ModelState.AddModelError("HousingServicesAdd[" + i + "].ShelterEndDate", "Shelter/Tran. Housing End cannot conflict with an existing Shelter/Trans. Housing service period that exists for this case.");
							}
					}

			if (housingServiceModelEdit != null)
				for (int i = 0; i < housingServiceModelEdit.Count; i++) {
					if (emptyTranHousingEndDates + emptyAddTranHousingEndDates + emptyHousingServiceModelEdit > 1) {
						if (housingServiceModelEdit[i].ShelterBegDate != null)
							ModelState.AddModelError("HousingServicesEdit[" + i + "].ShelterBegDate", "Shelter/Tran. Housing Begin cannot conflict with an existing Shelter/Trans. Housing service period that exists for this case.");
						if (housingServiceModelEdit[i].ShelterEndDate != null)
							ModelState.AddModelError("HousingServicesEdit[" + i + "].ShelterEndDate", "Shelter/Tran. Housing End cannot conflict with an existing Shelter/Trans. Housing service period that exists for this case.");
						continue;
					}
					if (housingServiceModelEdit[i].IsEdited == false || housingServiceModelEdit[i].IsDeleted) {
						ModelState.RemoveWithPrefix("HousingServicesEdit[" + i + "]");
					} else {
						if (housingServiceModelEdit[i].ShelterBegDate < caze.FirstContactDate)
							ModelState.AddModelError("HousingServicesEdit[" + i + "].ShelterBegDate", "Shelter begin cannot be earlier than the case's First Contact Date.");
						if (housingServiceModelEdit[i].ShelterEndDate < caze.FirstContactDate)
							ModelState.AddModelError("HousingServicesEdit[" + i + "].ShelterEndDate", "Shelter End cannot be earlier than the case's First Contact Date.");
						if (housingServiceModelEdit[i].ShelterBegDate > housingServiceModelEdit[i].ShelterEndDate)
							ModelState.AddModelError("HousingServicesEdit[" + i + "].ShelterBegDate", "Shelter Begin cannot be greater than Shelter End.");
						if (housingServiceModelEdit[i].ShelterEndDate < housingServiceModelEdit[i].ShelterBegDate)
							ModelState.AddModelError("HousingServicesEdit[" + i + "].ShelterEndDate", "Shelter End cannot be less than Shelter Begin");
					}
				}
			if (housingServiceModelEdit != null) {
				var housingServiceDetailDelete = housingServiceModelEdit.Where(m => m.IsDeleted);

				foreach (var housingServiceDelete in housingServiceDetailDelete) {
					var currentServiceDetailRec = caze.ServiceDetailsOfClient.FirstOrDefault(m => m.ServiceDetailID == housingServiceDelete.ServiceDetailID);
					if (currentServiceDetailRec != null)
						db.Entry(currentServiceDetailRec).State = EntityState.Deleted;
				}
			}
		}

		private void ModelStateReferral(ClientCase caze, IList<ReferralAdd> referralModelAdd, IList<ReferralEdit> referralModelEdit) {
			if (referralModelAdd != null)
				for (int i = 0; i < referralModelAdd.Count; i++)
					if (referralModelAdd[i].IsDeleted) {
						ModelState.RemoveWithPrefix("ReferralAdd[" + i + "]");
					} else {
						if (!(referralModelAdd[i].ReferralTypeID == null && referralModelAdd[i].ReferralDate == null && referralModelAdd[i].AgencyID == null && referralModelAdd[i].ResponseID == null)) {
							if (referralModelAdd[i].ReferralTypeID == null)
								ModelState.AddModelError("ReferralAdd[" + i + "].ReferralTypeID", "The Referral Type field is required.");
							if (referralModelAdd[i].ReferralDate == null)
								ModelState.AddModelError("ReferralAdd[" + i + "].ReferralDate", "The Referral Date field is required.");
							if (referralModelAdd[i].AgencyID == null)
								ModelState.AddModelError("ReferralAdd[" + i + "].AgencyID", "The Agency field is required.");
							if (referralModelAdd[i].ResponseID == null)
								ModelState.AddModelError("ReferralAdd[" + i + "].ResponseID", "The Response field is required.");
						}
						if (referralModelAdd[i].ReferralDate < caze.FirstContactDate)
							ModelState.AddModelError("ReferralAdd[" + i + "].ReferralDate", "Cannot be earlier than the case's First Contact Date.");
					}

			if (referralModelEdit != null)
				for (int i = 0; i < referralModelEdit.Count; i++)
					if (referralModelEdit[i].IsEdited == false || referralModelEdit[i].IsDeleted) {
						ModelState.RemoveWithPrefix("ReferralEdit[" + i + "]");
					} else {
						if (referralModelEdit[i].ReferralDate < caze.FirstContactDate)
							ModelState.AddModelError("ReferralEdit[" + i + "].ReferralDate", "Cannot be earlier than the case's First Contact Date.");
					}

			if (referralModelEdit != null) {
				var serviceDetailDelete = referralModelEdit.Where(m => m.IsDeleted);

				foreach (var serviceDelete in serviceDetailDelete) {
					var currentServiceDetailRec = caze.ClientReferralDetail.FirstOrDefault(m => m.ReferralDetailID == serviceDelete.ReferralDetailID);
					if (currentServiceDetailRec != null)
						db.Entry(currentServiceDetailRec).State = EntityState.Deleted;
				}
			}
		}

		private void ModelStateCancellations(ClientCase caze, IList<CancellationsAdd> cancellationsModelAdd, IList<CancellationsEdit> cancellationsModelEdit) {
			int centerId = Session.Center().Id;
			if (cancellationsModelAdd != null)
				for (int i = 0; i < cancellationsModelAdd.Count; i++)
					if (cancellationsModelAdd[i].IsDeleted) {
						ModelState.RemoveWithPrefix("CancellationsAdd[" + i + "]");
					} else {
						if (!(cancellationsModelAdd[i].ServiceID == null && cancellationsModelAdd[i].ReasonID == null && cancellationsModelAdd[i].SVID == null && cancellationsModelAdd[i].Date == null)) {
							if (cancellationsModelAdd[i].ServiceID == null)
								ModelState.AddModelError("CancellationsAdd[" + i + "].ServiceID", "The Service field is required.");
							if (cancellationsModelAdd[i].ReasonID == null)
								ModelState.AddModelError("CancellationsAdd[" + i + "].ReasonID", "The Reason field is required.");
							if (cancellationsModelAdd[i].SVID == null)
								ModelState.AddModelError("CancellationsAdd[" + i + "].SVID", "The Staff field is required.");
							if (cancellationsModelAdd[i].Date == null)
								ModelState.AddModelError("CancellationsAdd[" + i + "].Date", "The Date field is required.");
						}

						if (cancellationsModelAdd[i].Date < caze.FirstContactDate)
							ModelState.AddModelError("CancellationsAdd[" + i + "].Date", "Cannot be earlier than the case's First Contact Date.");
						var addServiceDate = cancellationsModelAdd[i].Date;
						var addSvid = cancellationsModelAdd[i].SVID;
						if (addServiceDate != null && addSvid != null && db.T_StaffVolunteer.Count(sv => sv.CenterId == centerId && sv.SvId == addSvid && (sv.StartDate == null || sv.StartDate <= addServiceDate) && (sv.TerminationDate == null || sv.TerminationDate > addServiceDate)) == 0)
							ModelState.AddModelError("CancellationsAdd[" + i + "].SVID", Resource.StringErrorMessageStaffNotActiveDuringService);
					}

			if (cancellationsModelEdit != null)
				for (int i = 0; i < cancellationsModelEdit.Count; i++)
					if (cancellationsModelEdit[i].IsEdited == false || cancellationsModelEdit[i].IsDeleted) {
						ModelState.RemoveWithPrefix("CancellationsEdit[" + i + "]");
					} else {
						if (cancellationsModelEdit[i].Date < caze.FirstContactDate)
							ModelState.AddModelError("CancellationsEdit[" + i + "].Date", "Cannot be earlier than the case's First Contact Date.");
						var addServiceDate = cancellationsModelEdit[i].Date;
						var addSvid = cancellationsModelEdit[i].SVID;
						if (addServiceDate != null && addSvid != null && db.T_StaffVolunteer.Count(sv => sv.CenterId == centerId && sv.SvId == addSvid && (sv.StartDate == null || sv.StartDate <= addServiceDate) && (sv.TerminationDate == null || sv.TerminationDate > addServiceDate)) == 0)
							ModelState.AddModelError("CancellationsEdit[" + i + "].SVID", Resource.StringErrorMessageStaffNotActiveDuringService);
					}

			if (cancellationsModelEdit != null) {
				var serviceDetailDelete = cancellationsModelEdit.Where(m => m.IsDeleted);

				foreach (var serviceDelete in serviceDetailDelete) {
					var currentServiceDetailRec = caze.Cancellations.FirstOrDefault(m => m.ID == serviceDelete.ID);
					if (currentServiceDetailRec != null)
						db.Entry(currentServiceDetailRec).State = EntityState.Deleted;
				}
			}
		}

		private void ProcessDepartures(ClientCase model) {
			if (model.ClientId == null)
				return;
			var originalDepartures = db.Ts_ClientDeparture.Where(t => t.ClientID == model.ClientId && t.CaseID == model.CaseId).ToList();

			foreach (var departure in model.ClientDepartures.ToList()) {
				var origDeparture = originalDepartures.SingleOrDefault(r => r.DepartureID == departure.DepartureID);

				if (origDeparture != null)
					if (departure.IsDeleted) {
						db.Entry(departure).State = EntityState.Detached;
						db.Ts_ClientDeparture.Remove(origDeparture);
					} else {
						db.Entry(origDeparture).State = EntityState.Detached;
						db.Ts_ClientDeparture.Attach(departure);
						db.Entry(departure).State = origDeparture.IsUnchanged(departure) ? EntityState.Unchanged : EntityState.Modified;
					}
			}
		}

		private bool SaveServices(ClientCase caze, int caseId, int clientId, IList<DepartureAdd> departuresAdd, IList<CancellationsAdd> cancellationsAdd, IList<CancellationsEdit> cancellationsEdit,
			IList<DirectServiceAdd> directServiceModelAdd, IList<DirectServiceEdit> directServiceModelEdit,
			IList<HousingServicesAdd> housingServiceModelAdd, IList<HousingServicesModify> housingServiceModelEdit,
			IList<ReferralAdd> referralModelAdd, IList<ReferralEdit> referralModelEdit) {
			try {
				using (var transaction = db.Database.BeginTransaction())
					try {
						if (departuresAdd != null)
							foreach (var departureAdd in departuresAdd)
								if (departureAdd.IsDeleted == false) {
									var clientDeparture = new ClientDeparture {
										CaseID = caze.CaseId.GetValueOrDefault(),
										ClientID = caze.ClientId.GetValueOrDefault(),
										DepartureDate = departureAdd.DepartureDate,
										DestinationID = departureAdd.DestinationID,
										DestinationTenureID = departureAdd.DestinationTenureID,
										DestinationSubsidyID = departureAdd.DestinationSubsidyID,
										ReasonForLeavingID = departureAdd.ReasonForLeavingID
									};
									db.Ts_ClientDeparture.Add(clientDeparture);
								}

						if (cancellationsAdd != null)
							foreach (var cancellationAdd in cancellationsAdd)
								if (cancellationAdd.IsDeleted == false && cancellationAdd.IsAdded) {
									db.Database.ExecuteSqlCommand(
										"CLT_ServiceCancellation_Add @ClientID, @CaseID, @ServiceID, @SVID, @ServiceDate, @ReasonID, @LocationID",
										new SqlParameter("ClientID", clientId),
										new SqlParameter("CaseID", caseId),
										new SqlParameter("ServiceID", cancellationAdd.ServiceID ?? Convert.DBNull),
										new SqlParameter("SVID", cancellationAdd.SVID ?? Convert.DBNull),
										new SqlParameter("ServiceDate", cancellationAdd.Date ?? Convert.DBNull),
										new SqlParameter("ReasonID", cancellationAdd.ReasonID ?? Convert.DBNull),
										new SqlParameter("LocationID", Session.Center().Id)
									);
								}

						ProcessDepartures(caze);

						if (cancellationsEdit != null) {
							var cancellationsDetailEdit = cancellationsEdit.Where(m => m.IsEdited);

							foreach (var cancellationEdit in cancellationsDetailEdit)
								db.Database.ExecuteSqlCommand(
									"CLT_ServiceCancellation_Update @ServiceCancellationID, @ServiceID, @SVID,  @ServiceDate, @ReasonID",
									new SqlParameter("@ServiceCancellationID", cancellationEdit.ID),
									new SqlParameter("@ServiceID", cancellationEdit.ServiceID),
									new SqlParameter("@SVID", cancellationEdit.SVID),
									new SqlParameter("@ServiceDate", cancellationEdit.Date),
									new SqlParameter("@ReasonID", cancellationEdit.ReasonID)
								);
						}

						if (directServiceModelAdd != null) {
							var serviceDetailAdd = directServiceModelAdd.Where(m => m.IsAdded);

							foreach (var serviceAdd in serviceDetailAdd)
								db.Database.ExecuteSqlCommand(
									"CLT_ServiceDetail_ADD @ClientID, @CaseID, @ServiceID, @SVID, @ServiceDate, @LocationID, @ReceivedHours, @FundDateID, @ShelterBegDate, @ShelterEndDate, @ICS_ID, @ServiceDetailID",
									new SqlParameter("ClientID", clientId),
									new SqlParameter("CaseID", caseId),
									new SqlParameter("ServiceID", serviceAdd.ServiceID ?? Convert.DBNull),
									new SqlParameter("SVID", serviceAdd.SVID ?? Convert.DBNull),
									new SqlParameter("ServiceDate", serviceAdd.ServiceDate ?? Convert.DBNull),
									new SqlParameter("LocationID", Session.Center().Id),
									new SqlParameter("ReceivedHours", serviceAdd.ReceivedHours ?? 0),
									new SqlParameter("FundDateID", Convert.DBNull),
									new SqlParameter("ShelterBegDate", serviceAdd.ShelterBegDate ?? Convert.DBNull),
									new SqlParameter("ShelterEndDate", serviceAdd.ShelterEndDate ?? Convert.DBNull),
									new SqlParameter("ICS_ID", Convert.DBNull),
									new SqlParameter("ServiceDetailID", serviceAdd.ServiceDetailID ?? Convert.DBNull)
								);
						}

						if (directServiceModelEdit != null) {
							var serviceDetailEdit = directServiceModelEdit.Where(m => m.IsEdited);

							foreach (var serviceEdit in serviceDetailEdit) {
								var currentServiceDetailRec = caze.ServiceDetailsOfClient.FirstOrDefault(m => m.ServiceDetailID == serviceEdit.ServiceDetailID);

								int sqlRet = db.Database.ExecuteSqlCommand(
									"CLT_ServiceDetail_Update @ServiceDetailID, @ClientID, @CaseID, @ServiceID, @SVID, @ServiceDate, @ReceivedHours, @LocationID, @FundDateID, @ShelterBegDate, @ShelterEndDate",
									new SqlParameter("ServiceDetailID", serviceEdit.ServiceDetailID),
									new SqlParameter("ClientID", clientId),
									new SqlParameter("CaseID", caseId),
									new SqlParameter("ServiceID", serviceEdit.ServiceID ?? Convert.DBNull),
									new SqlParameter("SVID", serviceEdit.SVID ?? Convert.DBNull),
									new SqlParameter("ServiceDate", serviceEdit.ServiceDate ?? Convert.DBNull),
									new SqlParameter("ReceivedHours", serviceEdit.ReceivedHours ?? 0),
									new SqlParameter("LocationID", Session.Center().Id),
									new SqlParameter("FundDateID", currentServiceDetailRec.FundDateID ?? Convert.DBNull),
									new SqlParameter("ShelterBegDate", serviceEdit.ShelterBegDate ?? Convert.DBNull),
									new SqlParameter("ShelterEndDate", serviceEdit.ShelterEndDate ?? Convert.DBNull)
								);
							}
						}

						if (housingServiceModelAdd != null) {
							var housingServiceDetailAdd = housingServiceModelAdd.Where(m => m.IsAdded);

							foreach (var housingServiceAdd in housingServiceDetailAdd)
								db.Database.ExecuteSqlCommand(
									"CLT_ServiceDetail_ADD @ClientID, @CaseID, @ServiceID, @SVID, @ServiceDate, @LocationID, @ReceivedHours, @FundDateID, @ShelterBegDate, @ShelterEndDate, @ICS_ID, @ServiceDetailID",
									new SqlParameter("ClientID", clientId),
									new SqlParameter("CaseID", caseId),
									new SqlParameter("ServiceID", housingServiceAdd.ServiceID ?? Convert.DBNull),
									new SqlParameter("SVID", Convert.DBNull),
									new SqlParameter("ServiceDate", Convert.DBNull),
									new SqlParameter("LocationID", Session.Center().Id),
									new SqlParameter("ReceivedHours", housingServiceAdd.ReceivedHours ?? 0),
									new SqlParameter("FundDateID", Convert.DBNull),
									new SqlParameter("ShelterBegDate", housingServiceAdd.ShelterBegDate ?? Convert.DBNull),
									new SqlParameter("ShelterEndDate", housingServiceAdd.ShelterEndDate ?? Convert.DBNull),
									new SqlParameter("ICS_ID", Convert.DBNull),
									new SqlParameter("ServiceDetailID", housingServiceAdd.ServiceDetailID ?? Convert.DBNull)
								);
						}

						if (housingServiceModelEdit != null) {
							var housingServiceDetailEdit = housingServiceModelEdit.Where(m => m.IsEdited);

							foreach (var housingServiceEdit in housingServiceDetailEdit) {
								var currentServiceDetailRec = caze.ServiceDetailsOfClient.FirstOrDefault(m => m.ServiceDetailID == housingServiceEdit.ServiceDetailID);

								db.Database.ExecuteSqlCommand(
									"CLT_ServiceDetail_Update @ServiceDetailID, @ClientID, @CaseID, @ServiceID, @SVID, @ServiceDate, @ReceivedHours, @LocationID, @FundDateID, @ShelterBegDate, @ShelterEndDate",
									new SqlParameter("ServiceDetailID", housingServiceEdit.ServiceDetailID),
									new SqlParameter("ClientID", clientId),
									new SqlParameter("CaseID", caseId),
									new SqlParameter("ServiceID", housingServiceEdit.ServiceID ?? Convert.DBNull),
									new SqlParameter("SVID", housingServiceEdit.SVID ?? Convert.DBNull),
									new SqlParameter("ServiceDate", housingServiceEdit.ServiceDate ?? Convert.DBNull),
									new SqlParameter("ReceivedHours", housingServiceEdit.ReceivedHours ?? 0),
									new SqlParameter("LocationID", Session.Center().Id),
									new SqlParameter("FundDateID", currentServiceDetailRec.FundDateID ?? Convert.DBNull),
									new SqlParameter("ShelterBegDate", housingServiceEdit.ShelterBegDate ?? Convert.DBNull),
									new SqlParameter("ShelterEndDate", housingServiceEdit.ShelterEndDate ?? Convert.DBNull)
								);
							}
						}

						if (caze.Provider == Provider.CAC) {
							if (referralModelAdd != null) {
								var referralDetailAdd = referralModelAdd.Where(m => m.IsAdded);

								foreach (var referralAdd in referralDetailAdd)
									db.Database.ExecuteSqlCommand(
										"CLT_ReferralDetail_Add @ClientID, @CaseID, @ReferralDate, @AgencyID, @ResponseID, @ReferralTypeID, @LocationID",
										new SqlParameter("ClientID", clientId),
										new SqlParameter("CaseID", caseId),
										new SqlParameter("ReferralDate", referralAdd.ReferralDate ?? Convert.DBNull),
										new SqlParameter("AgencyID", referralAdd.AgencyID ?? Convert.DBNull),
										new SqlParameter("ResponseID", referralAdd.ResponseID ?? Convert.DBNull),
										new SqlParameter("ReferralTypeID", referralAdd.ReferralTypeID ?? Convert.DBNull),
										new SqlParameter("LocationID", Session.Center().Id)
									);
							}

							if (referralModelEdit != null) {
								var referralDetailEdit = referralModelEdit.Where(m => m.IsEdited);

								foreach (var referralEdit in referralDetailEdit)
									db.Database.ExecuteSqlCommand(
										"CLT_ReferralDetail_Update @ReferralDetailID, @ReferralDate, @AgencyID, @ResponseID, @ReferralTypeID",
										new SqlParameter("ReferralDetailID", referralEdit.ReferralDetailID),
										new SqlParameter("ReferralDate", referralEdit.ReferralDate ?? Convert.DBNull),
										new SqlParameter("AgencyID", referralEdit.AgencyID ?? Convert.DBNull),
										new SqlParameter("ResponseID", referralEdit.ResponseID ?? Convert.DBNull),
										new SqlParameter("ReferralTypeID", referralEdit.ReferralTypeID ?? Convert.DBNull)
									);
							}
						}

						db.SaveChanges();
						transaction.Commit();
						AddSuccessMessage("Your changes have been successfully saved.");
					} catch {
						transaction.Rollback();
						throw;
					}
				return true;
			} catch (RetryLimitExceededException) {
				ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
				return false;
			}
		}
		#endregion

		#region Closeout
		public ActionResult Closeout(int clientId, int caseId) {
			var caze = LoadOrCreate(clientId, caseId);
			if (caze == null)
				return HttpNotFound();

			BagOtherCases(clientId);
			BagAgency();
			ConstructWarnings(caze);

			return View("Edit", caze);
		}

		[HttpPost, ActionName("Closeout")]
		[ValidateAntiForgeryToken]
		[PreventDuplicateRequest]
		public ActionResult CloseoutPost(int clientId, int caseId, string hash) {
			var caze = LoadOrCreate(clientId, caseId);

			TryUpdateModel(caze);
			ClearModelStateErrorsForKeysWithoutIncomingValues();

			if (ModelState.IsValid && Save(caze))
				return Redirect(Url.Action("Closeout", "Case", new { clientId = caze.ClientId, caseId = caze.CaseId }) + hash);

			BagOtherCases(caze.Client.ClientId);
			BagAgency();
			ConstructWarnings(caze);

			return View("Edit", caze);
		}
		#endregion

		#region Residence
		private void ProcessResidences(ClientCase caze) {
			foreach (var each in caze.Client.TwnTshipCounty)
				if (each.CaseID == null && db.Entry(each).State == EntityState.Added)
					each.CaseID = caze.CaseId ?? 1;

			foreach (var each in caze.Client.TwnTshipCountyById.Values.Restorable)
				if (each.LocID != null) {
					db.Database.ExecuteSqlCommand("EXEC [dbo].[P_deleteLocation] @LocID", new SqlParameter("LocID", each.LocID));
					db.Entry(each).State = EntityState.Detached;
				}
		}
		#endregion
	}
}