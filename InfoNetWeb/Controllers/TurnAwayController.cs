using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Infonet.Data.Models.Services;
using Infonet.Web.Mvc;
using Infonet.Web.Mvc.Authorization;
using Infonet.Web.ViewModels.Services;
using PagedList;

namespace Infonet.Web.Controllers {
	[DenyCoalition]
	[HasShelter]
	[Authorize(Roles = "DVADMIN, DVDATAENTRY")]
	public class TurnAwayController : InfonetControllerBase {

		public ActionResult AddNewRecord() {
			return PartialView("_NewTurnAwayPartial");
		}

		#region Search/Edit
		public ActionResult Search_Edit(TurnAwayViewModel model, int? page) {
			int centerId = Session.Center().Id;
			var results = from turnAway in db.Ts_TurnAwayServices
						  where turnAway.LocationId == centerId
						  select new TurnAwayViewModel.TurnAwaysSearchResult {
							  TurnAwayDate = turnAway.TurnAwayDate, AdultsNo = turnAway.AdultsNo,
							  ChildrenNo = turnAway.ChildrenNo, Id = turnAway.Id, ReferralMadeId = turnAway.ReferralMadeId
						  };

			if (model.StartDate != null && model.EndDate != null)
				results = results.Where(c => (c.TurnAwayDate >= model.StartDate
				&& c.TurnAwayDate <= model.EndDate));

			else if (model.StartDate != null)
				results = results.Where(c => c.TurnAwayDate >= model.StartDate);

			else if (model.EndDate != null)
				results = results.Where(c => c.TurnAwayDate <= model.EndDate);

			int pageNumber = page ?? (model.PageNumber ?? 1);
			model.PageNumber = pageNumber;
			model.RecordCount = results.Count();

			model.TurnAwaysList = results.OrderByDescending(t => t.TurnAwayDate).ThenBy(t => t.AdultsNo).ThenBy(t => t.ChildrenNo).ThenBy(t => t.ReferralMadeId).ToPagedList(pageNumber, model.PageSize == -1 ? results.Count() : model.PageSize);
			model.displayForPaging = model.TurnAwaysList.ToList();

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[PreventDuplicateRequest]
		public ActionResult Search_Edit_Post(TurnAwayViewModel model) {
			model.displayForPaging?.RemoveAll(m => m.TurnAwayDate == null || m.shouldDelete == false && m.shouldEdit == false && m.Id != null);

			if (ModelState.IsValid) {
				if (model.displayForPaging != null && model.displayForPaging.Count != 0) {
					bool saved = UpdateEntities(model.displayForPaging);
					if (saved) {
						AddSuccessMessage("Changes have been successfully saved.");
					} else {
						AddErrorMessage("Unable to save the changes.");
					}
				} else {
					AddInfoMessage("No changes were made to the form. Nothing was saved to the database.");
				}
				return RedirectToAction("Search_Edit", "TurnAway", model);
			} else {
				AddErrorMessage("Unable to save the changes.");

				return RedirectToAction("Search_Edit", "TurnAway", model);
			}

		}

		[HttpPost]
		public ActionResult Search_Edit() {
			ModelState.Clear();
			return RedirectToAction("Search_Edit");
		}

		#endregion

		#region Private

		private bool UpdateEntities(List<TurnAwayViewModel.TurnAwaysSearchResult> turnAwayRecords) {
			try {
				foreach (var record in turnAwayRecords) {
					var originalturnAwayRecord = db.Ts_TurnAwayServices.FirstOrDefault(t => t.Id == record.Id);
					TurnAwayService currentTurnAway = createNewturnAwayEntity(record);

					if (originalturnAwayRecord != null) {
						if (record.shouldEdit) {
							db.Entry(originalturnAwayRecord).State = EntityState.Detached;
							db.Ts_TurnAwayServices.Attach(currentTurnAway);
							db.Entry(currentTurnAway).State = originalturnAwayRecord.IsUnchanged(currentTurnAway) ? EntityState.Unchanged : EntityState.Modified;
						}

					} else {
						db.Ts_TurnAwayServices.Add(currentTurnAway);
						db.Entry(currentTurnAway).State = EntityState.Added;
					}

					if (record.shouldDelete) {
						db.Ts_TurnAwayServices.Remove(originalturnAwayRecord);
					}
				}
				db.SaveChanges();
				return true;
			} catch {
				return false;
			}
		}

		private TurnAwayService createNewturnAwayEntity(TurnAwayViewModel.TurnAwaysSearchResult record) {
			TurnAwayService currentTurnAway = new TurnAwayService();

			currentTurnAway.Id = record.Id;
			currentTurnAway.LocationId = Session.Center().Id;
			currentTurnAway.ReferralMadeId = record.ReferralMadeId;
			currentTurnAway.TurnAwayDate = record.TurnAwayDate;
			currentTurnAway.AdultsNo = record.AdultsNo;
			currentTurnAway.ChildrenNo = record.ChildrenNo;

			return currentTurnAway;
		}
		#endregion
	}
}