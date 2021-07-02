using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Mvc;
using Infonet.Data.Helpers;
using Infonet.Data.Models.Services;
using Infonet.Web.Mvc;
using Infonet.Web.Mvc.Authorization;
using Infonet.Web.ViewModels.Services;
using PagedList;

namespace Infonet.Web.Controllers {
	[DenyCoalition]
	[Authorize(Roles = "DVADMIN, DVDATAENTRY, SAADMIN, SADATAENTRY, CACADMIN, CACDATAENTRY")]
	public class OtherStaffActivityController : InfonetControllerBase {
		#region Search
		public ActionResult Search(OtherStaffActivitySearchViewModel model, int? page) {
			int centerId = Session.Center().Id;
			var results = from otherStaffActivity in db.Ts_OtherStaffActivity
				join staffVolunteer in db.T_StaffVolunteer on otherStaffActivity.SVID equals staffVolunteer.SvId
				join activities in db.TLU_Codes_OtherStaffActivity on otherStaffActivity.OtherStaffActivityID equals activities.CodeID
				where staffVolunteer.CenterId == centerId
				orderby otherStaffActivity.OsaDate descending
				select new OtherStaffActivitySearchViewModel.OtherStaffActivitySearchResult {
					OsaID = otherStaffActivity.OsaID,
					OsaDate = otherStaffActivity.OsaDate,
					SVID = otherStaffActivity.SVID,
					Staff = staffVolunteer.LastName + ", " + staffVolunteer.FirstName,
					Activity = activities.Description,
					OtherStaffActivityID = otherStaffActivity.OtherStaffActivityID
				};

			if (model.StartDate != null && model.EndDate != null)
				results = results.Where(osa => osa.OsaDate >= model.StartDate && osa.OsaDate <= model.EndDate);
			else if (model.StartDate != null)
				results = results.Where(osa => osa.OsaDate >= model.StartDate);
			else if (model.EndDate != null)
				results = results.Where(osa => osa.OsaDate <= model.EndDate);
			if (model.OtherStaffActivityID != null)
				results = results.Where(osa => osa.OtherStaffActivityID == model.OtherStaffActivityID);
			if (model.SVID != null)
				results = results.Where(osa => osa.SVID == model.SVID);

			int pageNumber = page ?? 1;
			model.PageNumber = pageNumber;
			model.RecordCount = results.Count();
			model.OSAList = results.ToPagedList(pageNumber, model.PageSize == -1 ? (int)model.RecordCount : model.PageSize);

			BagStaffActivities();

			return View(model);
		}

		[HttpPost]
		public ActionResult Search() {
			ModelState.Clear();
			return RedirectToAction("Search");
		}

		public ActionResult FormRedirect(int? id) {
			TempData["URL"] = Request.UrlReferrer.AbsoluteUri;
			return RedirectToAction("Form", new { id });
		}
		#endregion

		#region Form
		public ActionResult Form(int? id) {
			var model = Load(id, new OtherStaffActivityViewModel());
			if (model == null)
				return RedirectToAction("Search");

			TempData["URL"] = !string.IsNullOrEmpty((string)TempData.Peek("URL")) ? TempData["URL"] : "/OtherStaffActivity/Search";
			
			BagStaffActivities();

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[PreventDuplicateRequest]
		public ActionResult Form(OtherStaffActivityViewModel outputModel) {
			TempData["URL"] = outputModel.ReturnURL;
			Validate(outputModel);

			if (ModelState.IsValid)
				return RedirectToAction("Form", new { id = outputModel.OsaID == null ? Add(Load(null, outputModel)) : Edit(outputModel) });

			AddErrorMessage("An error occured while saving! Please try again!");
            BagStaffActivities();
            return View(outputModel);
		}

		public ActionResult Delete(int? id) {
			if (id == null)
				return RedirectToAction("Search");

			int centerId = Session.Center().Id;
			try {
				var activity = db.Ts_OtherStaffActivity.Single(osa => osa.OsaID == id && osa.StaffVolunteer.CenterId == centerId);
				db.Ts_OtherStaffActivity.Remove(activity);
				db.SaveChanges();
				AddSuccessMessage("You have successfully deleted a Staff Activity record!");
			} catch (Exception) {
				AddErrorMessage("Unable to delete record. Try again, and if the problem persists, see your system administrator.");
			}

            if (string.IsNullOrEmpty((string)TempData.Peek("URL")))
                return RedirectToAction("Search");
            else
                return Redirect(TempData.Peek("URL").ToString());
		}
		#endregion

		#region Private
		private void Validate(OtherStaffActivityViewModel model) {
			int? svId = model.SVID;
			if (svId == null)
				return;

			var employee = db.T_StaffVolunteer.Single(s => s.SvId == svId);
			if (employee.StartDate != null && employee.StartDate > model.OsaDate || employee.TerminationDate != null && employee.TerminationDate <= model.OsaDate) {
				ModelState.AddModelError("SVID", "Staff/Volunteer was not active during the time of the activity.");
				TempData["NonActiveStaff"] = new Staff {
					EmployeeName = employee.LastName + ", " + employee.FirstName,
					SVID = employee.SvId
				};
			}
		}

		private int? Add(OtherStaffActivityViewModel model) {
			bool isNewActivity = model.OsaID == null;
			try {
				var activity = new OtherStaffActivity();
				if (isNewActivity) {
					activity.OtherStaffActivityID = model.OtherStaffActivityID;
					activity.OsaDate = model.OsaDate;
					activity.SVID = model.SVID;
					activity.ConductingHours = model.ConductingHours;
					activity.TravelHours = model.TravelHours ?? 0;
					activity.PrepareHours = model.PrepareHours ?? 0;
					db.Ts_OtherStaffActivity.Add(activity);
				}
				db.SaveChanges();
				AddSuccessMessage("You have successfully added a new Other Staff Activity record!");
				return model.saveAddNew == 0 ? activity.OsaID : null;
			} catch (RetryLimitExceededException) {
				if (isNewActivity)
					model.OsaID = null;
				AddErrorMessage("Unable to save changes. Try again, and if the problem persists, see your system administrator.");
				return 0;
			}
		}

		private int? Edit(OtherStaffActivityViewModel model) {
			int centerId = Session.Center().Id;
			var original = db.Ts_OtherStaffActivity.Single(m => m.OsaID == model.OsaID && m.StaffVolunteer.CenterId == centerId);

			model.TravelHours = model.TravelHours ?? 0;
			model.PrepareHours = model.PrepareHours ?? 0;

			var thisActivity = db.Entry(original);
			thisActivity.CurrentValues.SetValues(model);

			db.SaveChanges();
			AddSuccessMessage("You have successfully modified an Other Staff Activity record and your changes have been saved!");
			return model.saveAddNew == 0 ? model.OsaID : null;
		}

		private OtherStaffActivityViewModel Load(int? osaId, OtherStaffActivityViewModel model) {
			if (osaId == null)
				return model;

			int centerId = Session.Center().Id;
			var currentActivity = db.Ts_OtherStaffActivity.SingleOrDefault(m => m.OsaID == osaId && m.StaffVolunteer.CenterId == centerId);
			if (currentActivity == null) {
				AddErrorMessage("The record with an ID of " + osaId + " does not exist.");
				return null;
			}

			model.OsaID = currentActivity.OsaID;
			model.SVID = currentActivity.SVID;
			model.OsaDate = currentActivity.OsaDate;
			model.OtherStaffActivityID = currentActivity.OtherStaffActivityID;
			model.ConductingHours = currentActivity.ConductingHours;
			model.PrepareHours = currentActivity.PrepareHours ?? 0;
			model.TravelHours = currentActivity.TravelHours ?? 0;
			return model;
		}

		private void BagStaffActivities() {
			ViewBag.StaffActivities = db.Database.SqlQuery<Activity>("SELECT [ItemAssignment].CodeID, Description FROM dbo.TLU_Codes_OtherStaffActivity [OtherStaffActivity] INNER JOIN dbo.LOOKUPLIST_ItemAssignment [ItemAssignment] ON [OtherStaffActivity].CodeID = [ItemAssignment].CodeID WHERE(CenterID = @p0 or CenterID = 0) and ProviderID = @p1 and TableID = 26 and IsActive = 1 ORDER BY CenterID, DisplayOrder, Description", Session.Center().Id, Session.Center().ProviderId).ToList(); //Center ID = 0 is the admin
		}

		public class Activity {
			public int CodeID { get; set; }
			public string Description { get; set; }
		}
		#endregion
	}
}