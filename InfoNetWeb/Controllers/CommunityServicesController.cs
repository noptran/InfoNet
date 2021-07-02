using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Mvc;
using Infonet.Core;
using Infonet.Data.Helpers;
using Infonet.Data.Models.Services;
using Infonet.Web.Mvc;
using Infonet.Web.Mvc.Authorization;
using Infonet.Web.ViewModels;
using PagedList;

namespace Infonet.Web.Controllers {
	[DenyCoalition]
	[Authorize(Roles = "DVADMIN, DVDATAENTRY, SAADMIN, SADATAENTRY, CACADMIN, CACDATAENTRY")]
	public class CommunityServicesController : InfonetControllerBase {
		private const int AGENCY_TABLE_ID = 48;

		#region Search
		public ActionResult Search(CommInstSearchViewModel model, int? page) {
			int centerId = Session.Center().Id;
			int providerId = Session.Center().ProviderId;
			var results = from ciServices in db.Tl_ProgramDetail
						  join programsAndServices in db.TLU_Codes_ProgramsAndServices on ciServices.ProgramID equals programsAndServices.CodeID
						  join lookupAssignment in db.LookupList_ItemAssignment on programsAndServices.CodeID equals lookupAssignment.CodeId
						  join lookupTable in db.LookupList_Tables on lookupAssignment.TableId equals lookupTable.TableId
						  where lookupAssignment.ProviderId == providerId && lookupAssignment.TableId == 30 && programsAndServices.IsCommInst == true && ciServices.CenterID == centerId
						  orderby ciServices.PDate descending
						  select new CommInstSearchViewModel.CommInstSearchResult {
							  PDate = ciServices.PDate,
							  Hours = ciServices.Hours,
							  NumOfSession = ciServices.NumOfSession,
							  ParticipantsNum = ciServices.ParticipantsNum,
							  ProgramID = ciServices.ProgramID,
							  ProgramName = db.TLU_Codes_ProgramsAndServices.Where(p => p.CodeID == ciServices.ProgramID).Select(p => p.Description).FirstOrDefault(),
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

			model.CommInstServiceList = results.ToPagedList(pageNumber, model.PageSize == -1 ? (int)model.RecordCount : model.PageSize);

			foreach (var service in model.CommInstServiceList) {
				service.emName = GroupServiceController.GetProgramDetailStaffNamesAsString(db, service.ICS_ID);
			}

			return View(model);
		}

		[HttpPost]
		public ActionResult Search() {
			ModelState.Clear();
			return RedirectToAction("Search");
		}

		public ActionResult FormRedirect(int? id) {
			if (Request != null && Request.UrlReferrer != null) {
				TempData["CommServReturnUrl"] = Request.UrlReferrer.AbsoluteUri;
			}
			return RedirectToAction("Form", new { id });
		}

		#endregion

		#region Form
		public ActionResult Form(int? id) {
			var model = LoadOrCreate(id, new CommunityServicesViewModel());

			if (model == null)
				return IcjiaNotFound();

			TempData["CommServReturnUrl"] = !string.IsNullOrEmpty((string)TempData.Peek("CommServReturnUrl")) ? TempData["CommServReturnUrl"] : "/CommunityServices/Search";
			BagAgencies();

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[PreventDuplicateRequest]
		public ActionResult Form(CommunityServicesViewModel outputModel) {
			//delete the rows with no data
			((List<ProgramDetailStaff>)outputModel.ProgramDetailStaff)?.RemoveAll(m => m.SVID == 0);

			TempData["CommServReturnUrl"] = outputModel.ReturnURL;
			Validate(outputModel);

			if (ModelState.IsValid)
				return RedirectToAction("Form", new { id = outputModel.ICS_ID == null ? Add(LoadOrCreate(null, outputModel)) : Edit(outputModel) });

			AddErrorMessage("An error occured while saving! Please try again!");
			BagAgencies();

			return View(outputModel);
		}

		public ActionResult Delete(int? id) {
			if (id == null)
				return RedirectToAction("Search");

			int centerId = Session.Center().Id;
			try {
				db.Tl_ProgramDetail.Remove(db.Tl_ProgramDetail.Single(p => p.ICS_ID == id && p.CenterID == centerId));
				db.SaveChanges();
				AddSuccessMessage("You have successfully deleted a Community and Institutional service record!");
			} catch (Exception) {
				ModelState.AddModelError("", "Unable to delete record. Try again, and if the problem persists, see your system administrator.");
			}
             if (string.IsNullOrEmpty((string)TempData.Peek("CommServReturnUrl")))
                return RedirectToAction("Search");
            else
                return Redirect(TempData.Peek("CommServReturnUrl").ToString());
		}

		#endregion

		#region Private
		private CommunityServicesViewModel LoadOrCreate(int? icsId, CommunityServicesViewModel model) {
			if (icsId == null) {
				model.StateID = Data.Usps.Illinois.ID;
				return model;
			}

			int centerId = Session.Center().Id;
			var progDetail = db.Tl_ProgramDetail.FirstOrDefault(c => c.ICS_ID == icsId && c.CenterID == centerId);
			if (progDetail == null) {
				return null;
			}
			model.ProgramID = progDetail.ProgramID;
			model.ProgramName = db.TLU_Codes_ProgramsAndServices.Where(p => p.CodeID == progDetail.ProgramID).Select(p => p.Description).First();
			model.PDate = progDetail.PDate;
			model.NumOfSession = progDetail.NumOfSession;
			model.Hours = progDetail.Hours;
			model.ParticipantsNum = progDetail.ParticipantsNum;
			model.AgencyID = progDetail.AgencyID;
			model.Location = progDetail.Location;
			model.StateID = progDetail.StateID;
			model.CountyID = progDetail.CountyID;
			model.Comment_Act = progDetail.Comment_Act;
			model.ProgramDetailStaff = progDetail.ProgramDetailStaff;
			model.ICS_ID = progDetail.ICS_ID;
			return model;
		}

		private void Validate(CommunityServicesViewModel model) {

			var nonActiveStaff = new List<Staff>();
			var validateDic = new Dictionary<int, string>();

			for (int i = 0; i < model.ProgramDetailStaff.Count; i++) {
				int svid = model.ProgramDetailStaff[i].SVID;
				var employee = db.T_StaffVolunteer.FirstOrDefault(s => s.SvId == svid);

				if (employee.StartDate != null && employee.StartDate > model.PDate || employee.TerminationDate != null && employee.TerminationDate <= model.PDate) {
					ModelState.AddModelError("ProgramDetailStaff[" + i + "].SVID", "Staff/Volunteer was not active during the time of the service.");
					var nonActive = new Staff();
					nonActive.EmployeeName = employee.LastName + ", " + employee.FirstName;
					nonActive.SVID = employee.SvId;
					nonActiveStaff.Add(nonActive);
				}
			}

			for (int i = 0; i < model.ProgramDetailStaff.Count; i++) {
				string added;
				bool isPresent = validateDic.TryGetValue(model.ProgramDetailStaff[i].SVID, out added);
				if (isPresent) {
					ModelState.AddModelError(validateDic[model.ProgramDetailStaff[i].SVID], "Employee added more than once");
					ModelState.AddModelError("ProgramDetailStaff[" + i + "].SVID", "Employee added more than once");
				} else {
					validateDic.Add(model.ProgramDetailStaff[i].SVID, "ProgramDetailStaff[" + i + "].SVID");
				}
			}

			TempData["NonActiveStaff"] = nonActiveStaff;
		}

		private int? Add(CommunityServicesViewModel model) {
			bool isNewGroupService = model.ICS_ID == null;
			int centerId = Session.Center().Id;
			try {
				var service = new ProgramDetail();
				if (isNewGroupService) {
					service.ProgramID = model.ProgramID;
					service.NumOfSession = model.NumOfSession;
					service.Hours = model.Hours;
					service.ParticipantsNum = model.ParticipantsNum;
					service.PDate = model.PDate;
					service.ProgramDetailStaff = model.ProgramDetailStaff;
					service.StateID = model.StateID;
					service.CountyID = model.CountyID;
					service.AgencyID = model.AgencyID;
					service.CenterID = centerId;
					service.FundDateID = model.PDate.NotNull(d => Data.FundingForStaff.GetFundDateId((DateTime)d, centerId));
					service.Location = model.Location;
					service.Comment_Act = model.Comment_Act;
					db.Tl_ProgramDetail.Add(service);
				}
				db.SaveChanges();
				AddSuccessMessage("You have successfully added a new Community and Institutional service record!");
				return model.saveAddNew == 0 ? service.ICS_ID : null;
			} catch (RetryLimitExceededException) {
				if (isNewGroupService)
					model.ICS_ID = null;
				AddErrorMessage("Unable to save changes. Try again, and if the problem persists, see your system administrator.");
				return 0;
			}
		}

		private int? Edit(CommunityServicesViewModel model) {
			int centerId = Session.Center().Id;
			var original = db.Tl_ProgramDetail.Single(p => p.ICS_ID == model.ICS_ID && p.CenterID == centerId);
			var thisGroupService = db.Entry(original);
			thisGroupService.CurrentValues.SetValues(model);

			if ((DateTime)thisGroupService.OriginalValues["PDate"] != model.PDate)
				thisGroupService.CurrentValues["FundDateID"] = model.PDate.NotNull(d => Data.FundingForStaff.GetFundDateId((DateTime)d, Session.Center().Id));

			foreach (var each in model.ProgramDetailStaff) {
				var originalProgramDetailStaff = original.ProgramDetailStaff.SingleOrDefault(pds => pds.ICS_Staff_ID == each.ICS_Staff_ID && pds.ICS_Staff_ID != null);

				if (originalProgramDetailStaff != null) {
					db.Entry(originalProgramDetailStaff).State = EntityState.Detached;
					db.Ts_ProgramDetail_Staffs.Attach(each);
					db.Entry(each).State = originalProgramDetailStaff.IsUnchanged(each) ? EntityState.Unchanged : EntityState.Modified;
				} else {
					each.ICS_ID = model.ICS_ID;
					db.Ts_ProgramDetail_Staffs.Add(each);
					db.Entry(each).State = EntityState.Added;
				}
			}

			foreach (var each in original.ProgramDetailStaff.ToList())
				if (model.ProgramDetailStaff.All(p => p.ICS_Staff_ID != each.ICS_Staff_ID))
					db.Ts_ProgramDetail_Staffs.Remove(each);

			foreach (var each in model.ProgramDetailStaff.Where(pd => !pd.HoursPrep.HasValue || !pd.HoursTravel.HasValue)) {
				each.HoursPrep = each.HoursPrep ?? 0.0;
				each.HoursTravel = each.HoursTravel ?? 0.0;
			}

			db.SaveChanges();
			AddSuccessMessage("You have successfully modified a Community and Institutional service record!");
			return model.saveAddNew == 0 ? model.ICS_ID : null;
		}

		private void BagAgencies() {
			int centerId = Session.Center().Id;
			int providerId = Session.Center().ProviderId;
			ViewBag.AgencyList = db.T_Agency
				.Join(db.LookupList_ItemAssignment, src => src.AgencyID, des => des.CodeId,
					(src, des) => new { src, des })
				.Where(d => d.des.TableId == AGENCY_TABLE_ID && d.des.ProviderId == providerId
				&& (d.src.CenterID == centerId || d.src.CenterID == 0) && d.des.IsActive).Distinct().Select(a => new AgencyViewModel {
					AgencyID = a.src.AgencyID,
					AgencyName = a.src.AgencyName,
					CenterID = a.src.CenterID,
					DisplayOrder = a.des.DisplayOrder
				}).OrderBy(o => o.DisplayOrder).ThenBy(o => o.AgencyName).ToList();
		}
		#endregion

		#region ajax
		public ActionResult AddStaff(int? icsId, DateTime? date) {
			var model = new CommunityServicesViewModel {
				PDate = date,
				ICS_ID = icsId,
				ProgramDetailStaff = new List<ProgramDetailStaff> { new ProgramDetailStaff() }
			};
			return PartialView("_Staff", model);
		}
		#endregion
	}

	public class AgencyViewModel {
		public int? AgencyID { get; set; }
		public string AgencyName { get; set; }
		public int? CenterID { get; set; }
		public int? DisplayOrder { get; set; }
	}
}