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
	[Authorize(Roles = "DVADMIN, DVDATAENTRY, DVSPECIALENTRY")]
	public class HotlineController : InfonetControllerBase {
		#region Search
		public ActionResult Search(HotlineSearchViewModel model, int? page) {
			int centerId = Session.Center().Id;
			var results = from hotlines in db.T_PhoneHotline
				join staffVolunteer in db.T_StaffVolunteer on hotlines.SVID equals staffVolunteer.SvId into staff
				from s in staff.DefaultIfEmpty()
				where hotlines.CenterID == centerId
				orderby hotlines.Date descending
				select new HotlineSearchViewModel.HotlineSearchResult {
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

			model.HotlineList = results.ToPagedList(pageNumber, model.PageSize == -1 ? (int)model.RecordCount : model.PageSize);

			return View(model);
		}

		[HttpPost]
		public ActionResult Search() {
			ModelState.Clear();
			return RedirectToAction("Search");
		}

		public ActionResult FormRedirect(int? id) {
			if (Request != null && Request.UrlReferrer != null)
				TempData["HotlineReturnUrl"] = Request.UrlReferrer.AbsoluteUri;
			return RedirectToAction("Form", new { id });
		}
		#endregion

		#region Action Methods
		public ActionResult Form(int? id) {
			var model = LoadOrCreate(id, new HotlineViewModel());
			if (model == null)
				return IcjiaNotFound();

			TempData["HotlineReturnUrl"] = !string.IsNullOrEmpty((string)TempData.Peek("HotlineReturnUrl")) ? TempData["HotlineReturnUrl"] : "/Hotline/Search";
			
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[PreventDuplicateRequest]
		public ActionResult Form(HotlineViewModel outputModel) {
			TempData["HotlineReturnUrl"] = outputModel.ReturnURL;
			Validate(outputModel);

			if (ModelState.IsValid)
				return RedirectToAction("Form", new { id = outputModel.PH_ID == null ? Add(LoadOrCreate(outputModel.PH_ID, outputModel)) : Edit(outputModel) });

			AddErrorMessage("An error occured while saving! Please try again!");

			return View(outputModel);
		}

		public ActionResult Delete(int? id, string r) {
            if (id == null)
                return Redirect(!string.IsNullOrEmpty(r) ? r : string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, "/Hotline/Search"));

            int centerId = Session.Center().Id;
			try {
				db.T_PhoneHotline.Remove(db.T_PhoneHotline.Single(h => h.PH_ID == id && h.CenterID == centerId));
				db.SaveChanges();
				AddSuccessMessage("You have successfully deleted a Hotline record!");
			} catch (Exception) {
				AddErrorMessage("Unable to delete record. Try again, and if the problem persists, see your system administrator.");
			}
            if (string.IsNullOrEmpty((string)TempData.Peek("HotlineReturnUrl")))
                return RedirectToAction("Search");
            else
                return Redirect(TempData.Peek("HotlineReturnUrl").ToString());
        }
		#endregion

		#region Private
		private HotlineViewModel LoadOrCreate(int? phId, HotlineViewModel model) {
			if (phId == null)
				return model;

			int centerId = Session.Center().Id;
			var hotline = db.T_PhoneHotline.FirstOrDefault(h => h.PH_ID == phId && h.CenterID == centerId);
			if (hotline == null)
				return null;

			model.PH_ID = hotline.PH_ID;
			model.NumberOfContacts = hotline.NumberOfContacts;
			model.CallTypeID = hotline.CallTypeID;
			model.CountyID = hotline.CountyID;
			model.Date = hotline.Date;
			model.ReferralFromID = hotline.ReferralFromID;
			model.ReferralToID = hotline.ReferralToID;
			model.SVID = hotline.SVID;
			model.TimeOfDay = hotline.TimeOfDay;
			model.TotalTime = hotline.TotalTime;
			model.Town = hotline.Town;
			model.Township = hotline.Township;
			model.ZipCode = hotline.ZipCode;
			return model;
		}

		private void Validate(HotlineViewModel model) {
			int? svId = model.SVID;
			if (svId != null) {
				var employee = db.T_StaffVolunteer.FirstOrDefault(s => s.SvId == svId);
				if (employee.StartDate != null && employee.StartDate > model.Date || employee.TerminationDate != null && employee.TerminationDate <= model.Date) {
					ModelState.AddModelError("SVID", "Staff/Volunteer was not active during the time of the hotline call");
					var nonActive = new Staff {
						EmployeeName = employee.LastName + ", " + employee.FirstName,
						SVID = employee.SvId
					};
					TempData["NonActiveStaff"] = nonActive;
				}
			}
			int countyId = model.CountyID ?? 0;
			if (model.ZipCode != null) {
				string zipcode = model.ZipCode.TrimStart().Substring(0, 5);
				if (countyId > 0 && countyId < 1936 && !string.IsNullOrEmpty(zipcode))
					if (!Data.Usps.IsValidZip(zipcode, countyId, null))
						ModelState.AddModelError("ZipCode", "Invalid Zip Code for County");
			}
		}

		private int? Add(HotlineViewModel model) {
			bool isNewHotline = model.PH_ID == null;
			int centerId = Session.Center().Id;
			try {
				var hotline = new PhoneHotline();
				if (isNewHotline) {
					hotline.CallTypeID = model.CallTypeID;
					hotline.CountyID = model.CountyID;
					hotline.Date = model.Date;
					hotline.NumberOfContacts = model.NumberOfContacts;
					hotline.ReferralFromID = model.ReferralFromID;
					hotline.ReferralToID = model.ReferralToID;
					hotline.SVID = model.SVID;
					hotline.TimeOfDay = model.TimeOfDay;
					hotline.TotalTime = model.TotalTime;
					hotline.Town = model.Town;
					hotline.Township = model.Township;
					hotline.ZipCode = model.ZipCode;
					hotline.CenterID = centerId;
					hotline.FundDateID = model.Date.NotNull(d => Data.FundingForStaff.GetFundDateId((DateTime)d, centerId));
					db.T_PhoneHotline.Add(hotline);
				}
				db.SaveChanges();
				AddSuccessMessage("You have successfully added a new Hotline record!");
				return model.saveAddNew == 0 ? hotline.PH_ID : null;
			} catch (RetryLimitExceededException) {
				if (isNewHotline)
					model.PH_ID = null;
				AddErrorMessage("Unable to save changes. Try again, and if the problem persists, see your system administrator.");
				return 0;
			}
		}

		private int? Edit(HotlineViewModel model) {
			int centerId = Session.Center().Id;
			var original = db.T_PhoneHotline.Single(h => h.PH_ID == model.PH_ID && h.CenterID == centerId);
			var thisHotline = db.Entry(original);
			thisHotline.CurrentValues.SetValues(model);

			if ((DateTime)thisHotline.OriginalValues["Date"] != model.Date)
				thisHotline.CurrentValues["FundDateID"] = model.Date.NotNull(d => Data.FundingForStaff.GetFundDateId((DateTime)d, Session.Center().Id));

			db.SaveChanges();
			AddSuccessMessage("You have successfully modified a Hotline record and your changes have been saved!");
			return model.saveAddNew == 0 ? model.PH_ID : null;
		}
		#endregion
	}
}