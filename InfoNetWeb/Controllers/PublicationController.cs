using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Mvc;
using Infonet.Core;
using Infonet.Data.Helpers;
using Infonet.Data.Looking;
using Infonet.Data.Models.Services;
using Infonet.Web.Mvc;
using Infonet.Web.Mvc.Authorization;
using Infonet.Web.ViewModels.Services;
using PagedList;

namespace Infonet.Web.Controllers {
	[DenyCoalition]
	[Authorize(Roles = "DVADMIN, DVDATAENTRY, SAADMIN, SADATAENTRY, CACADMIN, CACDATAENTRY")]
	public class PublicationController : InfonetControllerBase {
		#region Search
		public ActionResult Search(PublicationSearchViewModel model, int? page) {
			int centerId = Session.Center().Id;
			var results = from ciServices in db.Tl_PublicationDetail
				join programsAndServices in db.TLU_Codes_ProgramsAndServices on ciServices.ProgramID equals programsAndServices.CodeID
				join lookupAssignment in db.LookupList_ItemAssignment on programsAndServices.CodeID equals lookupAssignment.CodeId
				join lookupTable in db.LookupList_Tables on lookupAssignment.TableId equals lookupTable.TableId
				where lookupAssignment.ProviderId == 1 && lookupAssignment.TableId == 30 && programsAndServices.IsPublication == true && ciServices.CenterID == centerId
				orderby ciServices.PDate descending
				select new PublicationSearchViewModel.MediaPubSearchResult {
					PDate = ciServices.PDate,
					Type = programsAndServices.Description,
					Title = ciServices.Title,
					NumOfPubs = ciServices.NumOfBrochure ?? 0,
					ICS_ID = ciServices.ICS_ID
				};

			if (model.StartDate != null && model.EndDate != null)
				results = results.Where(c => c.PDate >= model.StartDate && c.PDate <= model.EndDate);

			else if (model.StartDate != null)
				results = results.Where(c => c.PDate >= model.StartDate);

			else if (model.EndDate != null)
				results = results.Where(c => c.PDate <= model.EndDate);

			int pageNumber = page ?? 1;
			model.PageNumber = pageNumber;
			model.RecordCount = results.Count();

			model.MediaPubServiceList = results.ToPagedList(pageNumber, model.PageSize == -1 ? (int)model.RecordCount : model.PageSize);

			return View(model);
		}

		[HttpPost]
		public ActionResult Search() {
			ModelState.Clear();
			return RedirectToAction("Search");
		}

		public ActionResult FormRedirect(int? id) {
			if (Request != null && Request.UrlReferrer != null)
				TempData["PublicationReturnUrl"] = Request.UrlReferrer.AbsoluteUri;
			return RedirectToAction("Form", new { id });
		}
		#endregion

		#region Form
		public ActionResult Form(int? id) {
			var model = LoadOrCreate(id, new PublicationViewModel());
			if (model == null)
				return RedirectToAction("Search");

			TempData["PublicationReturnUrl"] = !string.IsNullOrEmpty((string)TempData.Peek("PublicationReturnUrl")) ? TempData["PublicationReturnUrl"] : "/Publication/Search";
			BagPublications();

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[PreventDuplicateRequest]
		public ActionResult Form(PublicationViewModel outputModel) {
			//delete the rows with no data
			((List<PublicationDetailStaff>)outputModel.PublicationDetailStaff)?.RemoveAll(m => m.SVID == 0);

			TempData["PublicationReturnUrl"] = outputModel.ReturnURL;
			Validate(outputModel);

			if (ModelState.IsValid)
				return RedirectToAction("Form", "Publication", new { id = outputModel.ICS_ID == null ? Add(LoadOrCreate(outputModel.ICS_ID, outputModel)) : Edit(outputModel) });

			AddErrorMessage("An error occured while saving! Please try again!");
			BagPublications();

			return View(outputModel);
		}

		public ActionResult Delete(int? id) {
			if (id == null)
				return RedirectToAction("Search");

			int locationId = Session.Center().Id;
			try {
				db.Tl_PublicationDetail.Remove(db.Tl_PublicationDetail.Single(p => p.ICS_ID == id && p.CenterID == locationId));
				db.SaveChanges();
				AddSuccessMessage("You have successfully deleted a Non-Client Crisis Intervention record!");
			} catch (Exception) {
				AddErrorMessage("Unable to delete record. Try again, and if the problem persists, see your system administrator.");
			}
            if (string.IsNullOrEmpty((string)TempData.Peek("PublicationReturnUrl")))
                return RedirectToAction("Search");
            else
                return Redirect(TempData.Peek("PublicationReturnUrl").ToString());
        }

		private void Validate(PublicationViewModel model) {
			var nonActiveStaff = new List<Staff>();
			var validateDic = new Dictionary<int?, string>();

			for (int i = 0; i < model.PublicationDetailStaff.Count; i++) {
				int svId = (int)model.PublicationDetailStaff[i].SVID;
				var employee = db.T_StaffVolunteer.SingleOrDefault(s => s.SvId == svId);

				if (employee.StartDate != null && employee.StartDate > model.PDate || employee.TerminationDate != null && employee.TerminationDate <= model.PDate) {
					ModelState.AddModelError("PublicationDetailStaff[" + i + "].SVID", "Staff/Volunteer was not active during the time of the service.");
					var nonActive = new Staff();
					nonActive.EmployeeName = employee.LastName + ", " + employee.FirstName;
					nonActive.SVID = employee.SvId;
					nonActiveStaff.Add(nonActive);
				}
			}

			for (int i = 0; i < model.PublicationDetailStaff.Count; i++) {
				string added;
				bool isPresent = validateDic.TryGetValue(model.PublicationDetailStaff[i].SVID, out added);
				if (isPresent) {
					ModelState.AddModelError(validateDic[model.PublicationDetailStaff[i].SVID], "Employee added more than once");
					ModelState.AddModelError("PublicationDetailStaff[" + i + "].SVID", "Employee added more than once");
				} else {
					validateDic.Add(model.PublicationDetailStaff[i].SVID, "PublicationDetailStaff[" + i + "].SVID");
				}
			}

			TempData["NonActiveStaff"] = nonActiveStaff;
		}

		private int? Add(PublicationViewModel model) {
			bool isNewGroupService = model.ICS_ID == null;
			int locationId = Session.Center().Id;
			try {
				var service = new PublicationDetail();
				if (isNewGroupService) {
					service.ProgramID = model.ProgramID;
					service.CenterID = Session.Center().Id;
					service.PDate = model.PDate;
					service.FundDateID = model.PDate.NotNull(d => Data.FundingForStaff.GetFundDateId((DateTime)d, locationId));
					service.Title = model.Title;
					service.PrepareHours = model.PrepareHours;
					service.NumOfBrochure = model.NumOfBrochure;
					service.Comment_Pub = model.Comment_Pub;
					service.PublicationDetailStaff = model.PublicationDetailStaff;
					db.Tl_PublicationDetail.Add(service);
				}
				db.SaveChanges();
				model.ICS_ID = service.ICS_ID;
				AddSuccessMessage("You have successfully added a new Media/Publication record!");
				return model.saveAddNew == 0 ? model.ICS_ID : null;
			} catch (RetryLimitExceededException) {
				if (isNewGroupService)
					model.ICS_ID = null;
				AddErrorMessage("Unable to save changes. Try again, and if the problem persists, see your system administrator.");
				return 0;
			}
		}

		private int? Edit(PublicationViewModel model) {
			int locationId = Session.Center().Id;
			var original = db.Tl_PublicationDetail.Single(p => p.ICS_ID == model.ICS_ID && p.CenterID == locationId);

			var thisGroupService = db.Entry(original);
			thisGroupService.CurrentValues.SetValues(model);

			if ((DateTime)thisGroupService.OriginalValues["PDate"] != (DateTime)thisGroupService.CurrentValues["PDate"])
				thisGroupService.CurrentValues["FundDateID"] = model.PDate.NotNull(d => Data.FundingForStaff.GetFundDateId((DateTime)d, locationId));

			foreach (var each in model.PublicationDetailStaff) {
				var originalStaff = original.PublicationDetailStaff.SingleOrDefault(pds => pds.ICS_Staff_ID == each.ICS_Staff_ID && pds.ICS_Staff_ID != null);

				if (originalStaff != null) {
					db.Entry(originalStaff).State = EntityState.Detached;
					db.Ts_PublicationDetail_Staffs.Attach(each);
					db.Entry(each).State = originalStaff.IsUnchanged(each) ? EntityState.Unchanged : EntityState.Modified;
				} else {
					each.ICS_ID = model.ICS_ID ?? 0;
					db.Ts_PublicationDetail_Staffs.Add(each);
					db.Entry(each).State = EntityState.Added;
				}
			}
			foreach (var originalPublicationDetailStaff in original.PublicationDetailStaff.ToList())
				if (model.PublicationDetailStaff.All(p => p.ICS_Staff_ID != originalPublicationDetailStaff.ICS_Staff_ID))
					db.Ts_PublicationDetail_Staffs.Remove(originalPublicationDetailStaff);

			db.SaveChanges();
			AddSuccessMessage("You have successfully modified a Media/Publication record and your changes have been saved!");
			return model.saveAddNew == 0 ? model.ICS_ID : null;
		}

		private PublicationViewModel LoadOrCreate(int? icsId, PublicationViewModel model) {
			if (icsId == null)
				return model;

			int locationId = Session.Center().Id;
			var publicationDetail = db.Tl_PublicationDetail.FirstOrDefault(c => c.ICS_ID == icsId && c.CenterID == locationId);
			if (publicationDetail == null) {
				AddErrorMessage("The service with an ID of " + icsId + " is not a Publication.");
				return null;
			}

			model.ProgramID = publicationDetail.ProgramID;
			model.PDate = publicationDetail.PDate;
			model.Title = publicationDetail.Title;
			model.PrepareHours = publicationDetail.PrepareHours;
			model.NumOfBrochure = publicationDetail.NumOfBrochure;
			model.Comment_Pub = publicationDetail.Comment_Pub;
			model.PublicationDetailStaff = (List<PublicationDetailStaff>)publicationDetail.PublicationDetailStaff;
			model.ICS_ID = publicationDetail.ICS_ID;
			return model;
		}

		private void BagPublications() {
			ViewBag.Publications = Lookups.PublicationTypes[Session.Center().Provider];
		}		
		#endregion

		#region ajax
		public ActionResult AddStaff() {
			return PartialView("_Staff", new PublicationViewModel {
				PublicationDetailStaff = new List<PublicationDetailStaff> { new PublicationDetailStaff() }
			});
		}
		#endregion
	}
}