using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Infonet.Data.Models.Services;
using Infonet.Data.Models._TLU;
using Infonet.Web.Mvc;
using Infonet.Web.Mvc.Authorization;
using Infonet.Web.ViewModels.Services;
using PagedList;
using Infonet.Web.Mvc.Collections;

namespace Infonet.Web.Controllers {
	[DenyCoalition]
	[Authorize(Roles = "DVADMIN, DVDATAENTRY")]
	public class ServiceOutcomeController : InfonetControllerBase {

		#region ActionMethods

		// GET: ServiceOutcome
		public ActionResult Search_Edit(ServiceOutcomeViewModel searchModel, int? page) {
			int centerId = Session.Center().Id;
			var outcomes = (from outcome in db.Ts_ServiceOutcome
							where outcome.LocationID == centerId
							orderby outcome.OutcomeDate descending
							select new ServiceOutcomeViewModel.ServiceOutcomeSearchResult() {
								ID = outcome.ID,
								OutcomeDate = outcome.OutcomeDate,
								OutcomeName = outcome.TLU_Codes_ServiceOutcome.Description,
								ServiceID = outcome.ServiceID,
								ServiceName = outcome.TLU_Codes_ServiceCategory.Description,
								OutcomeID = outcome.OutcomeID,
								ResponseYes = outcome.ResponseYes,
								ResponseNo = outcome.ResponseNo,
							});

			if (searchModel.StartDate != null && searchModel.EndDate != null)
				outcomes = outcomes.Where(c => (c.OutcomeDate >= searchModel.StartDate
				&& c.OutcomeDate <= searchModel.EndDate));

			else if (searchModel.StartDate != null)
				outcomes = outcomes.Where(c => c.OutcomeDate >= searchModel.StartDate);

			else if (searchModel.EndDate != null)
				outcomes = outcomes.Where(c => c.OutcomeDate <= searchModel.EndDate);

			int pageNumber = page ?? (searchModel.PageNumber ?? 1);
			searchModel.PageNumber = pageNumber;

			pageNumber = (outcomes.Count() > 0 && outcomes.Count() < (((pageNumber - 1) * searchModel.PageSize) + 1)) ? pageNumber - 1 : pageNumber;
			searchModel.RecordCount = outcomes.Count();

			searchModel.OutcomesList = outcomes.OrderByDescending(o => o.OutcomeDate).ThenBy(o => o.ServiceName).ThenBy(o => o.OutcomeName).ToPagedList(pageNumber, searchModel.PageSize == -1 ? outcomes.Count() : searchModel.PageSize);
			searchModel.displayForPaging = searchModel.OutcomesList.ToList();

			foreach (ServiceOutcomeViewModel.ServiceOutcomeSearchResult result in searchModel.displayForPaging) {
				result.OutcomeList = db.Database.SqlQuery<TLU_Codes_ServiceOutcome>(@"
																					SELECT		so.CodeID, 
																								so.Description 
																					FROM		TLU_Codes_ServiceOutcome so

																					JOIN		TLU_Codes_ServiceCategoryXServiceOutcome x 
																					ON			x.OutcomeID = so.CodeID 
																					AND			x.ServiceID = @p0

																					JOIN		LOOKUPLIST_ItemAssignment ia
																					ON			ia.CodeID = so.CodeID 
																					AND			ia.ProviderID = @p1 
																					AND			ia.IsActive = 1

																					JOIN		LOOKUPLIST_Tables tbl 
																					ON			tbl.TableID = 90 
																					AND			tbl.TableID = ia.TableID

																					ORDER BY DisplayOrder",
																					result.ServiceID,
																					Session.Center().Provider)
																					.ToList();
			}

			return View(searchModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[PreventDuplicateRequest]
		public ActionResult Search_Edit_Post(ServiceOutcomeViewModel model) {
			if (model.displayForPaging == null || model.displayForPaging.Count == model.displayForPaging.Count(dfp => !dfp.shouldAdd && !dfp.shouldDelete && !dfp.shouldEdit)) {
				AddInfoMessage("No changes were made to the form. Nothing was saved to the database.");
				return RedirectToAction("Search_Edit", "ServiceOutcome", model);
			}

			if (model.displayForPaging != null)
				foreach (var record in model.displayForPaging)
					if (record.shouldEdit == false && record.shouldAdd == false && record.shouldDelete == false)
						ModelState.RemoveWithPrefix("displayForPaging[" + model.displayForPaging.IndexOf(record) + "]");

			model.displayForPaging?.RemoveAll(m => (m.OutcomeDate == null || (m.ResponseNo == null && m.ResponseYes == null)) && m.shouldDelete == false);

			foreach (var isEdit in model.displayForPaging.Where(dfp => dfp.shouldEdit)) {
				if (model.displayForPaging[model.displayForPaging.IndexOf(isEdit)].ResponseNo == null)
					ModelState.AddModelError("displayForPaging[" + model.displayForPaging.IndexOf(isEdit) + "].ResponseNo", "Cannot be empty.");
				if (model.displayForPaging[model.displayForPaging.IndexOf(isEdit)].ResponseYes == null)
					ModelState.AddModelError("displayForPaging[" + model.displayForPaging.IndexOf(isEdit) + "].ResponseYes", "Cannot be empty.");
			}

			if (ModelState.IsValid) {
				if (model.displayForPaging != null) {
					bool saved = UpdateEntities(model.displayForPaging);
					if (saved) {
						AddSuccessMessage("Your changes have been successfully saved.");
					} else {
						AddErrorMessage("Unable to save the changes.");
					}
				} else {
					AddInfoMessage("No changes were made to the form. Nothing was saved to the database.");
				}
				return RedirectToAction("Search_Edit", "ServiceOutcome", model);
			}

			AddErrorMessage("Unable to save the changes.");

			return RedirectToAction("Search_Edit", "ServiceOutcome", model);
		}

		[HttpPost]
		public ActionResult Search_Edit() {
			ModelState.Clear();
			return RedirectToAction("Search_Edit");
		}

		public ActionResult AddNewRecord() {
			ServiceOutcomeViewModel model = new ServiceOutcomeViewModel();
			return PartialView("_NewOutcomePartial", model);
		}

		public ActionResult AddNewRecordFromDropdownChange(int count, DateTime outcomeDate) {
			ServiceOutcomeViewModel model = new ServiceOutcomeViewModel();
			model.AddCount = count;
			ServiceOutcomeViewModel.ServiceOutcomeSearchResult result = new ServiceOutcomeViewModel.ServiceOutcomeSearchResult();
			result.OutcomeDate = outcomeDate;
			model.displayForPaging.Add(result);
			return PartialView("_NewOutcomePartial", model);
		}


		public JsonResult GetQuestionListForServiceID(string serviceID) {
			return Json(db.Database.SqlQuery<TLU_Codes_ServiceOutcome>(@"
																		SELECT		so.CodeID, 
																					so.Description 
																		FROM		TLU_Codes_ServiceOutcome so

																		JOIN		TLU_Codes_ServiceCategoryXServiceOutcome x 
																		ON			x.OutcomeID = so.CodeID 
																		AND			x.ServiceID = @p0

																		JOIN		LOOKUPLIST_ItemAssignment ia 
																		ON			ia.CodeID = so.CodeID 
																		AND			ia.ProviderID = @p1 
																		AND			ia.IsActive = 1

																		JOIN		LOOKUPLIST_Tables tbl 
																		ON			tbl.TableID = 90 
																		AND			tbl.TableID = ia.TableID

																		ORDER BY	DisplayOrder",
																		serviceID,
																		Session.Center().Provider)
																		.ToList(),
																		JsonRequestBehavior.AllowGet);
		}

		public ActionResult GetQuestionsPartial(string serviceID, string date, int addCount) {
			int srvId = 0;
			if (!string.IsNullOrEmpty(serviceID)) {
				srvId = int.Parse(serviceID);
			}

			var model = new ServiceOutcomeViewModel {
				displayForPaging = new List<ServiceOutcomeViewModel.ServiceOutcomeSearchResult>(),
				AddCount = addCount
			};

			var outcomeList = db.Database.SqlQuery<TLU_Codes_ServiceOutcome>(@"
																										SELECT		so.CodeID, 
																													so.Description 
																										FROM		TLU_Codes_ServiceOutcome so

																										JOIN		TLU_Codes_ServiceCategoryXServiceOutcome x 
																										ON			x.OutcomeID = so.CodeID 
																										AND			x.ServiceID = @p0

																										JOIN		LOOKUPLIST_ItemAssignment ia 
																										ON			ia.CodeID = so.CodeID 
																										AND			ia.ProviderID = @p1 
																										AND			ia.IsActive = 1

																										JOIN		LOOKUPLIST_Tables tbl 
																										ON			tbl.TableID = 90 
																										AND			tbl.TableID = ia.TableID

																										ORDER BY	DisplayOrder",
																										serviceID,
																										Session.Center().Provider)
																										.ToList();

			foreach (TLU_Codes_ServiceOutcome outcome in outcomeList) {
				ServiceOutcomeViewModel.ServiceOutcomeSearchResult result = new ServiceOutcomeViewModel.ServiceOutcomeSearchResult();
				result.ServiceID = srvId;
				result.ServiceName = db.TLU_Codes_ServiceCategory.Where(s => s.CodeID == srvId).SingleOrDefault().Description;
				result.OutcomeDate = DateTime.Parse(date);
				result.OutcomeList = outcomeList;
				model.displayForPaging.Add(result);
			}

			return PartialView("_NewQuestionPartial", model);
		}

		#endregion

		#region Private

		private bool UpdateEntities(List<ServiceOutcomeViewModel.ServiceOutcomeSearchResult> records) {
			try {
				foreach (ServiceOutcomeViewModel.ServiceOutcomeSearchResult record in records) {
					var originalRecord = db.Ts_ServiceOutcome.Where(o => o.ID == record.ID).FirstOrDefault();
					ServiceOutcome current = createOutcomeEntity(record);

					if (record.shouldEdit) {
						db.Entry(originalRecord).State = EntityState.Detached;
						db.Ts_ServiceOutcome.Attach(current);
						db.Entry(current).State = (originalRecord.IsUnchanged(current)) ? EntityState.Unchanged : EntityState.Modified;
					}

					if (record.shouldAdd) {
						db.Ts_ServiceOutcome.Add(current);
						db.Entry(current).State = EntityState.Added;
					}

					if (record.shouldDelete) {						
                       db.Ts_ServiceOutcome.Remove(originalRecord);
                    }
				}

				db.SaveChanges();

				return true;
			} catch {
				return false;
			}

		}

		private ServiceOutcome createOutcomeEntity(ServiceOutcomeViewModel.ServiceOutcomeSearchResult record) {
			ServiceOutcome current = new ServiceOutcome();

			current.ID = record.ID;
			current.LocationID = Session.Center().Id;
			current.ServiceID = record.ServiceID;
			current.OutcomeDate = record.OutcomeDate;
			current.OutcomeID = record.OutcomeID;
			current.ResponseYes = record.ResponseYes;
			current.ResponseNo = record.ResponseNo;

			return current;
		}
		#endregion
	}
}