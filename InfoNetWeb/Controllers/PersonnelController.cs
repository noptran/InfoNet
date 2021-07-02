using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Mvc;
using Infonet.Data.Models.Centers;
using Infonet.Web.Mvc;
using Infonet.Web.Mvc.Authorization;
using Infonet.Web.ViewModels.Admin;
using PagedList;

namespace Infonet.Web.Controllers {
	[DenyCoalition]
	[Authorize(Roles = "DVADMIN, SAADMIN, CACADMIN")]
	public class PersonnelController : InfonetControllerBase {
		#region Search
		[HttpGet]
		public ActionResult Search(PersonnelSearchViewModel model, int? page) {
			int centerId = Session.Center().Id;
			var results = db.T_StaffVolunteer.Where(sv => sv.CenterId == centerId).Select(sv => new PersonnelSearchViewModel.MyStaffVolunteer {
				SvId = sv.SvId,
				CenterId = sv.CenterId,
				LastName = sv.LastName,
				FirstName = sv.FirstName,
				SexId = sv.SexId,
				RaceId = sv.RaceId,
				PersonnelTypeId = sv.PersonnelTypeId,
				CollegeUnivStudent = sv.CollegeUnivStudent,
				StartDate = sv.StartDate,
				TerminationDate = sv.TerminationDate,
				TypeId = sv.TypeId
			});

			if (model.StartDate != null || model.EndDate != null) {
				DateTime start = model.StartDate ?? DateTime.MinValue;
				DateTime end = model.EndDate ?? DateTime.Today;
				results = results.Where(sv => (sv.TerminationDate ?? DateTime.Today) >= start && (sv.StartDate ?? DateTime.MinValue) <= end);
			}
			if (model.Status != null) {
				if (model.Status == 1)
					results = results.Where(sv => sv.TerminationDate == null);
				else
					results = results.Where(sv => sv.TerminationDate.HasValue);
			}
			if (model.TypeID != null)
				results = results.Where(sv => sv.TypeId == model.TypeID);
			if (model.PersonnelTypeID != null)
				results = results.Where(sv => sv.PersonnelTypeId == model.PersonnelTypeID);
			if (model.CollegeUnivStudent != null)
				results = results.Where(sv => sv.CollegeUnivStudent == (model.CollegeUnivStudent == 1));
			if (model.RaceId != null)
				results = results.Where(sv => sv.RaceId == model.RaceId);
			if (model.SexId != null)
				results = results.Where(sv => sv.SexId == model.SexId);
			if (model.FirstName != null)
				results = results.Where(sv => sv.FirstName.Contains(model.FirstName));
			if (model.LastName != null)
				results = results.Where(sv => sv.LastName.Contains(model.LastName));

			int pageNumber = page ?? 1;
			model.PageNumber = pageNumber;
			model.RecordCount = results.Count(); //KMS DO don't query twice!

			model.staffList = results.OrderBy(p => p.LastName).ThenBy(p => p.FirstName).ToPagedList(pageNumber, model.PageSize == -1 ? results.Count() : model.PageSize);

			ViewBag.Types = new List<SimpleListItem> { new SimpleListItem("1", "Paid Staff"), new SimpleListItem("2", "Volunteer") };

			return View(model);
		}

		[HttpPost]
		public ActionResult Search() {
			ModelState.Clear();
			return RedirectToAction("Search");
		}

		public ActionResult FormRedirect(int? id) {
			if (Request != null && Request.UrlReferrer != null) {
				TempData["PersonnelReturnUrl"] = Request.UrlReferrer.AbsoluteUri;
			}
			return RedirectToAction("Form", new { id });
		}
		#endregion

		#region Form
		[HttpGet]
		public ActionResult Form(int? id) {
			PersonnelViewModel model = new PersonnelViewModel();

			model = LoadOrCreate(id, model);

			if (model == null)
				return RedirectToAction("Search");

			TempData["PersonnelReturnUrl"] = !string.IsNullOrEmpty((string)TempData.Peek("PersonnelReturnUrl")) ? TempData["PersonnelReturnUrl"] : "/Personnel/Search";
			BagLists(model);

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[PreventDuplicateRequest]
		public ActionResult Form(PersonnelViewModel model) {

			TempData["PersonnelReturnUrl"] = model.ReturnURL;
			Validate(model);

			if (ModelState.IsValid) {
				return RedirectToAction("Form", new { id = model.SvId == 0 ? Add(model) : Edit(model) });
			}

			AddErrorMessage("An error occured while saving! Please try again!");

			BagLists(model);

			return View(model);
		}
		#endregion

		#region Private
		private void Validate(PersonnelViewModel model) {
            bool isNewStaff = model.SvId == 0;
            int centerId = Session.Center().Id;
            int matches;
           
            if (isNewStaff) {
                matches = db.Database.SqlQuery<int>("select count(*) from T_StaffVolunteer where CenterId = @p0 and FirstName = @p1 and LastName = @p2", centerId, model.FirstName, model.LastName).Single();
            } else {
                matches = db.Database.SqlQuery<int>("select count(*) from T_StaffVolunteer where CenterId = @p0 and FirstName = @p1 and LastName = @p2 and SVID <> @p3", centerId, model.FirstName, model.LastName, model.SvId).Single();
            }

            if (matches != 0)
                ModelState.AddModelError("FirstName", "This staff name is already in use.");
           
            if (model.StartDate != null && model.TerminationDate != null)
				if (model.StartDate > model.TerminationDate)
					ModelState.AddModelError("TerminationDate", "Termination Date must be between the Start Date and today.");
		}

		private PersonnelViewModel LoadOrCreate(int? id, PersonnelViewModel model) {
			if (id == null)
				return model;

			int centerId = Session.Center().Id;
			var staff = db.T_StaffVolunteer.SingleOrDefault(i => i.SvId == id && i.CenterId == centerId);
			if (staff == null) {
				AddErrorMessage($"The Personnel record with an ID of <b>{id}</b> is not a member of the current center.");
				return null;
			}

			model.SvId = staff.SvId;
			model.CenterId = staff.CenterId;
			model.CollegeUnivStudent = staff.CollegeUnivStudent;
			model.Department = staff.Department;
			model.Email = staff.Email;
			model.FirstName = staff.FirstName;
			model.LastName = staff.LastName;
			model.PersonnelTypeId = staff.PersonnelTypeId;
			model.RaceId = staff.RaceId;
			model.SexId = staff.SexId;
			model.StartDate = staff.StartDate;
			model.SupervisorId = staff.SupervisorId;
			model.TerminationDate = staff.TerminationDate;
			model.Title = staff.Title;
			model.TypeId = staff.TypeId;
			model.WorkPhone = staff.WorkPhone;
			return model;
		}

		private int? Add(PersonnelViewModel model) {
			bool isNewStaff = model.SvId == 0;
			try {
				StaffVolunteer staff = new StaffVolunteer();
				if (isNewStaff) {
					staff.SvId = model.SvId;
					staff.CenterId = Session.Center().Id;
					staff.LastName = model.LastName;
					staff.FirstName = model.FirstName;
					staff.SexId = model.SexId;
					staff.RaceId = model.RaceId;
					staff.PersonnelTypeId = model.PersonnelTypeId;
					staff.Title = model.Title;
					staff.CollegeUnivStudent = model.CollegeUnivStudent;
					staff.Department = model.Department;
					staff.WorkPhone = model.WorkPhone;
					staff.Email = model.Email;
					staff.StartDate = model.StartDate;
					staff.TerminationDate = model.TerminationDate;
					staff.SupervisorId = model.SupervisorId;
					staff.Type = model.Type;
					staff.TypeId = model.TypeId;
					db.T_StaffVolunteer.Add(staff);
				}
				db.SaveChanges();
				AddSuccessMessage("You have successfully added a new Personnel record!");
				return model.saveAddNew == 0 ? (int?)staff.SvId : null;
			} catch (RetryLimitExceededException) {
				AddErrorMessage("Unable to save changes. Try again, and if the problem persists, see your system administrator.");
				//ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
				return null;
			}
		}

		private int? Edit(PersonnelViewModel model) {
			model.CenterId = Session.Center().Id;
			var original = db.T_StaffVolunteer.FirstOrDefault(sv => sv.SvId == model.SvId);
			var thisStaff = db.Entry(original);
			thisStaff.CurrentValues.SetValues(model);
			db.SaveChanges();
			AddSuccessMessage("You have successfully modified a Personnel record and your changes have been saved!");
			return model.saveAddNew == 0 ? (int?)model.SvId : null;
		}

		private void BagLists(PersonnelViewModel model) {
			ViewBag.StaffPersonnelTypes = db.Database.SqlQuery<PersonnelType>("SELECT CodeID, Description FROM TLU_Codes_PersonnelType WHERE isStaff = 1").ToList();
			ViewBag.VolunteerPersonnelTypes = db.Database.SqlQuery<PersonnelType>("SELECT CodeID, Description FROM TLU_Codes_PersonnelType WHERE isVolunteer = 1").ToList();
			var supervisorList = db.Helpers.Center.GetPaidStaffForCenterAndDate(DateTime.Now, Session.Center().Id);
			if (model.SupervisorId != null && supervisorList.All(s => s.SVID != model.SupervisorId))
				supervisorList.Add(db.Helpers.Center.GetStaffFromSvId((int)model.SupervisorId));
			ViewBag.Supervisors = supervisorList.OrderBy(s => s.EmployeeName).ToList();
			ViewBag.Types = new List<SimpleListItem> { new SimpleListItem("1", "Paid Staff"), new SimpleListItem("2", "Volunteer") };
		}
		#endregion

		#region Inner Classes
		public class PersonnelType {
			public int CodeID { get; set; }
			public string Description { get; set; }
		}
		#endregion
	}
}