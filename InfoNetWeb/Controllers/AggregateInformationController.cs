using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Infonet.Data.Models.Services;
using Infonet.Web.Mvc;
using Infonet.Web.Mvc.Authorization;
using Infonet.Web.ViewModels.Clients;
using PagedList;

namespace Infonet.Web.Controllers {
	[DenyCoalition]
	[HasShelter]
	[Authorize(Roles = "DVADMIN, DVDATAENTRY")]
	public class AggregateInformationController : InfonetControllerBase {

		// GET: AggregateInformation/Search_Edit
		public ActionResult Search_Edit(AggregateInformationViewModel model, int? page) {
			int centerId = Session.Center().Id;

			var results = from aggregate in db.Ts_HivMentalSubstance
				where aggregate.LocationID == centerId
				orderby aggregate.HMSDate descending
				select new AggregateInformationViewModel.AggregateInformationSearchResult {
					ID = aggregate.ID,
					HMSDate = aggregate.HMSDate,
					TypeID = aggregate.TypeID,
					TypeName = aggregate.TLU_Codes_HivMentalSubstance.Description,
					AdultsNo = aggregate.AdultsNo,
					ChildrenNo = aggregate.ChildrenNo
				};

			if (model.StartDate != null && model.EndDate != null)
				results = results.Where(c => c.HMSDate >= model.StartDate
				                             && c.HMSDate <= model.EndDate);

			else if (model.StartDate != null)
				results = results.Where(c => c.HMSDate >= model.StartDate);

			else if (model.EndDate != null)
				results = results.Where(c => c.HMSDate <= model.EndDate);

			if (model.TypeID != null)
				results = results.Where(c => c.TypeID == model.TypeID);

			int cntAggregates = results.Count();

			int pageNumber = page ?? (model.PageNumber ?? 1);
			pageNumber = cntAggregates > 0 && cntAggregates < (pageNumber - 1) * model.PageSize + 1 ? pageNumber - 1 : pageNumber;

			model.PageNumber = pageNumber;
			model.RecordCount = cntAggregates;

			model.AggregateList = results.OrderByDescending(o => o.HMSDate).ThenBy(o => o.TypeName).ThenBy(o => o.AdultsNo).ThenBy(o => o.ChildrenNo).ToPagedList(pageNumber, model.PageSize == -1 ? cntAggregates : model.PageSize);
			model.displayForPaging = model.AggregateList.ToList();

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[PreventDuplicateRequest]
		public ActionResult Search_Edit_Post(AggregateInformationViewModel model) {
			model.displayForPaging?.RemoveAll(m => m.AdultsNo == null && m.ChildrenNo == null && !m.shouldDelete);

			if (ModelState.IsValid) {
				if (model.displayForPaging != null) {
					if (UpdateEntities(model.displayForPaging))
						AddSuccessMessage("Your changes have been successfully saved.");
					else
						AddErrorMessage("Unable to save the changes.");
				} else {
					AddInfoMessage("No changes were made to the form. Nothing was saved to the database.");
				}
				return RedirectToAction("Search_Edit", "AggregateInformation", model);
			}

			AddErrorMessage("Unable to save the changes.");
			return RedirectToAction("Search_Edit", "AggregateInformation", model);
		}

		// Reset Page
		[HttpPost]
		public ActionResult Search_Edit() {
			ModelState.Clear();
			return RedirectToAction("Search_Edit");
		}

		// Calls Ajax that connects to PartialView, adding a new row of inputs.
		public ActionResult AddNewRecord(int addCount) {
			AggregateInformationViewModel model = new AggregateInformationViewModel();
			return PartialView("_NewAggregateInformationPartial", model);
		}

		#region Private

		private bool UpdateEntities(List<AggregateInformationViewModel.AggregateInformationSearchResult> records) {
			try {
				foreach (AggregateInformationViewModel.AggregateInformationSearchResult record in records) {
					var originalRecord = db.Ts_HivMentalSubstance.Where(o => o.ID == record.ID).FirstOrDefault();
					HivMentalSubstance current = createAggregateEntity(record);

					if (originalRecord != null) {
						db.Entry(originalRecord).State = EntityState.Detached;
						db.Ts_HivMentalSubstance.Attach(current);
						db.Entry(current).State = originalRecord.IsUnchanged(current) ? EntityState.Unchanged : EntityState.Modified;
					} else {
						db.Ts_HivMentalSubstance.Add(current);
						db.Entry(current).State = EntityState.Added;
					}

					if (record.shouldDelete) {
						db.Ts_HivMentalSubstance.Remove(current);
					}
				}

				db.SaveChanges();

				return true;
			} catch {
				return false;
			}
		}

		private HivMentalSubstance createAggregateEntity(AggregateInformationViewModel.AggregateInformationSearchResult record) {
			HivMentalSubstance current = new HivMentalSubstance();

			current.ID = record.ID ?? 0;
			current.LocationID = Session.Center().Id;
			current.HMSDate = record.HMSDate;
			current.TypeID = record.TypeID;
			current.AdultsNo = record.AdultsNo;
			current.ChildrenNo = record.ChildrenNo;

			return current;
		}
		#endregion
	}
}
