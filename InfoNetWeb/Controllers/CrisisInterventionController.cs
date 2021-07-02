using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Mvc;
using Infonet.Core;
using Infonet.Data.Helpers;
using Infonet.Data.Models.Services;
using Infonet.Web.Mvc;
using Infonet.Web.Mvc.Authorization;
using Infonet.Web.ViewModels.Services;
using PagedList;

namespace Infonet.Web.Controllers {
	[DenyCoalition]
	[Authorize(Roles = "SAADMIN, SADATAENTRY")]
	public class CrisisInterventionController : InfonetControllerBase {
		#region Search
		public ActionResult Search(CrisisSearchViewModel model, int? page) {
			int centerId = Session.Center().Id;
			var results = from hotlines in db.T_PhoneHotline
				join staffVolunteer in db.T_StaffVolunteer on hotlines.SVID equals staffVolunteer.SvId into staff
				from s in staff.DefaultIfEmpty()
				where hotlines.CenterID == centerId
				orderby hotlines.Date descending
				select new CrisisSearchViewModel.CrisisSearchResult {
					Date = hotlines.Date,
					SVID = s.SvId,
					Staff = string.IsNullOrEmpty(s.LastName) ? "" : s.LastName + ", " + s.FirstName,
					CallTypeID = hotlines.CallTypeID,
					PH_ID = hotlines.PH_ID
				};

			if (model.StartDate != null)
				results = results.Where(h => h.Date >= model.StartDate);
			if (model.EndDate != null)
				results = results.Where(h => h.Date <= model.EndDate);
			if (model.CallTypeID != null)
				results = results.Where(h => h.CallTypeID == model.CallTypeID);
			if (model.SVID != null)
				results = results.Where(h => h.SVID == model.SVID);

			int pageNumber = page ?? 1;
			model.PageNumber = pageNumber;
			model.RecordCount = results.Count();

			model.CrisisList = results.ToPagedList(pageNumber, model.PageSize == -1 ? (int)model.RecordCount : model.PageSize);


			return View(model);
		}

		[HttpPost]
		public ActionResult Search() {
			ModelState.Clear();
			return RedirectToAction("Search");
		}

		public ActionResult FormRedirect(int? id) {
			if (Request != null && Request.UrlReferrer != null)
				TempData["CrisisReturnUrl"] = Request.UrlReferrer.AbsoluteUri;
			return RedirectToAction("Form", new { id });
		}
		#endregion

		#region Form
		public ActionResult Form(int? id) {
			var model = LoadorCreate(id, new CrisisViewModel());
			if (model == null)
				return IcjiaNotFound();

			TempData["CrisisReturnUrl"] = !string.IsNullOrEmpty((string)TempData.Peek("CrisisReturnUrl")) ? TempData["CrisisReturnUrl"] : "/CrisisIntervention/Search";
			
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[PreventDuplicateRequest]
		public ActionResult Form(CrisisViewModel outputModel) {
			TempData["CrisisReturnUrl"] = outputModel.ReturnURL;
			Validate(outputModel);

			if (ModelState.IsValid)
				return RedirectToAction("Form", new { id = outputModel.PH_ID == null ? Add(LoadorCreate(outputModel.PH_ID, outputModel)) : Edit(outputModel) });

			AddErrorMessage("An error occured while saving! Please try again!");

			return View(outputModel);
		}

		public ActionResult Delete(int? id) {
			if (id == null)
				return RedirectToAction("Search");

			int centerId = Session.Center().Id;
			try {
				db.T_PhoneHotline.Remove(db.T_PhoneHotline.Single(h => h.PH_ID == id && h.CenterID == centerId));
				db.SaveChanges();
				AddSuccessMessage("You have successfully deleted a Non-Client Crisis Intervention record!");
			} catch (Exception) {
				AddErrorMessage("Unable to delete record. Try again, and if the problem persists, see your system administrator.");
			}

            if (string.IsNullOrEmpty((string)TempData.Peek("CrisisReturnUrl")))
                return RedirectToAction("Search");
            else
                return Redirect(TempData.Peek("CrisisReturnUrl").ToString());
		}
		#endregion

		#region Private
		private CrisisViewModel LoadorCreate(int? phId, CrisisViewModel model) {
			if (phId == null)
				return model;

			int centerId = Session.Center().Id;
			var crisis = db.T_PhoneHotline.Single(h => h.PH_ID == phId && h.CenterID == centerId);
			if (crisis == null)
				return null;

			model.PH_ID = crisis.PH_ID;
			model.Age = crisis.Age;
			model.NumberOfContacts = crisis.NumberOfContacts;
			model.CallTypeID = crisis.CallTypeID;
			model.ClientTypeID = crisis.ClientTypeID;
			model.CountyID = crisis.CountyID;
			model.Date = crisis.Date;
			model.RaceID = crisis.RaceID;
			model.ReferralFromID = crisis.ReferralFromID;
			model.ReferralToID = crisis.ReferralToID;
			model.SexID = crisis.SexID;
			model.SVID = crisis.SVID;
			model.TotalTime = crisis.TotalTime;
			model.Town = crisis.Town;
			model.Township = crisis.Township;
			model.ZipCode = crisis.ZipCode;
			return model;
		}

		private void Validate(CrisisViewModel model) {
			int? svId = model.SVID;
			if (svId != null) {
				var employee = db.T_StaffVolunteer.Single(s => s.SvId == svId);
				if (employee.StartDate != null && employee.StartDate > model.Date || employee.TerminationDate != null && employee.TerminationDate <= model.Date) {
					ModelState.AddModelError("SVID", "Staff/Volunteer was not active during the time of the call");
					var nonActive = new Staff();
					nonActive.EmployeeName = employee.LastName + ", " + employee.FirstName;
					nonActive.SVID = employee.SvId;
					TempData["NonActiveStaff"] = nonActive;
				}
			}
			int countyId = model.CountyID ?? 0;
			string zipcode = model.ZipCode;
			if (countyId > 0 && countyId < 1936 && !string.IsNullOrEmpty(zipcode))
				if (!Data.Usps.IsValidZip(zipcode, countyId, null))
					ModelState.AddModelError("ZipCode", "Invalid Zip Code for County");
		}

		private int? Add(CrisisViewModel model) {
			bool isNewHotline = model.PH_ID == null;
			int centerId = Session.Center().Id;
			try {
				var crisis = new PhoneHotline();
				if (isNewHotline) {
					crisis.Age = model.Age;
					crisis.CallTypeID = model.CallTypeID;
					crisis.ClientTypeID = model.ClientTypeID;
					crisis.CountyID = model.CountyID;
					crisis.Date = model.Date;
					crisis.NumberOfContacts = model.NumberOfContacts;
					crisis.RaceID = model.RaceID;
					crisis.ReferralFromID = model.ReferralFromID;
					crisis.ReferralToID = model.ReferralToID;
					crisis.SexID = model.SexID;
					crisis.SVID = model.SVID;
					crisis.TotalTime = model.TotalTime;
					crisis.Town = model.Town;
					crisis.Township = model.Township;
					crisis.ZipCode = model.ZipCode;
					crisis.CenterID = centerId;
					crisis.FundDateID = model.Date.NotNull(d => Data.FundingForStaff.GetFundDateId((DateTime)d, centerId));
					db.T_PhoneHotline.Add(crisis);
				}
				db.SaveChanges();
				AddSuccessMessage("You have successfully added a new Non-Client Crisis Intervention record!");
				return model.saveAddNew == 0 ? crisis.PH_ID : null;
			} catch (RetryLimitExceededException) {
				if (isNewHotline)
					model.PH_ID = null;
				AddErrorMessage("Unable to save changes. Try again, and if the problem persists, see your system administrator.");
				return 0;
			}
		}

		private int? Edit(CrisisViewModel model) {
			int centerId = Session.Center().Id;
			var original = db.T_PhoneHotline.Single(h => h.PH_ID == model.PH_ID && h.CenterID == centerId);
			var thisHotline = db.Entry(original);
			thisHotline.CurrentValues.SetValues(model);

			if ((DateTime)thisHotline.OriginalValues["Date"] != model.Date)
				thisHotline.CurrentValues["FundDateID"] = model.Date.NotNull(d => Data.FundingForStaff.GetFundDateId((DateTime)d, Session.Center().Id));

			db.SaveChanges();
			AddSuccessMessage("You have successfully modified a Non-Client Crisis Intervention record and your changes have been saved!");
			return model.saveAddNew == 0 ? model.PH_ID : null;
		}
		#endregion
	}
}