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
	[Authorize(Roles = "SAADMIN, SADATAENTRY")]
	public class EventController : InfonetControllerBase {
		#region Search
		[HttpGet]
		public ActionResult Search(EventSearchViewModel model, int? page) {
			int centerId = Session.Center().Id;
			int providerId = Session.Center().ProviderId;
			var results = from eventDetails in db.Tl_EventDetail
				join programsAndServices in db.TLU_Codes_ProgramsAndServices on eventDetails.ProgramID equals programsAndServices.CodeID
				join lookupAssignment in db.LookupList_ItemAssignment on programsAndServices.CodeID equals lookupAssignment.CodeId
				join lookupTable in db.LookupList_Tables on lookupAssignment.TableId equals lookupTable.TableId
				where lookupAssignment.ProviderId == providerId && lookupAssignment.TableId == 30 && programsAndServices.IsEvent == true && eventDetails.CenterID == centerId
				orderby eventDetails.EventDate descending
				select new EventSearchViewModel.EventSearchResult {
					EventDate = eventDetails.EventDate,
					EventHours = eventDetails.EventHours,
					NumOfPeopleReached = eventDetails.NumPeopleReached,
					ProgramID = eventDetails.ProgramID,
					EventType = db.TLU_Codes_ProgramsAndServices.Where(p => p.CodeID == eventDetails.ProgramID).Select(p => p.Description).FirstOrDefault(),
					ICS_ID = eventDetails.ICS_ID
				};

			if (model.StartDate != null)
				results = results.Where(e => e.EventDate >= model.StartDate);
			if (model.EndDate != null)
				results = results.Where(e => e.EventDate <= model.EndDate);

			int pageNumber = page ?? 1;
			model.PageNumber = pageNumber;
			model.RecordCount = results.Count();

			model.EventList = results.ToPagedList(pageNumber, model.PageSize == -1 ? (int)model.RecordCount : model.PageSize);

			foreach (var each in model.EventList)
				each.Staff = LoadEventStaffNamesAsString(each.ICS_ID);

			return View(model);
		}

		[HttpPost]
		public ActionResult Search() {
			ModelState.Clear();
			return RedirectToAction("Search");
		}

		public ActionResult FormRedirect(int? id) {
			if (Request != null && Request.UrlReferrer != null)
				TempData["EventReturnUrl"] = Request.UrlReferrer.AbsoluteUri;
			return RedirectToAction("Form", new { id });
		}

		private string LoadEventStaffNamesAsString(int? icsId) {
			return db.Database.SqlQuery<string>("SELECT STUFF((SELECT ', ' + T_StaffVolunteer.LastName + ', ' + T_StaffVolunteer.FirstName FROM Tl_EventDetail join Ts_EventDetail_Staffs on Tl_EventDetail.ICS_ID = Ts_EventDetail_Staffs.ICS_ID join T_StaffVolunteer on Ts_EventDetail_Staffs.SVID = T_StaffVolunteer.SVID where Tl_EventDetail.ICS_ID = @p0 FOR XML PATH('')), 1, 1, '')", icsId).SingleOrDefault();
		}
		#endregion

		#region Form
		[HttpGet]
		public ActionResult Form(int? id) {
			var model = LoadOrCreate(id, new EventViewModel());
			if (model == null)
				return IcjiaNotFound();

			TempData["EventReturnUrl"] = !string.IsNullOrEmpty((string)TempData.Peek("EventReturnUrl")) ? TempData["EventReturnUrl"] : "/Event/Search";
            BagEventTypes();

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[PreventDuplicateRequest]
		public ActionResult Form(EventViewModel model) {
			//delete the rows with no data
			((List<EventDetailStaff>)model.EventDetailStaff)?.RemoveAll(m => m.SVID == 0);

			TempData["EventReturnUrl"] = model.ReturnURL;
			Validate(model);

			if (ModelState.IsValid)
				return RedirectToAction("Form", new { id = model.ICS_ID == null ? Add(LoadOrCreate(model.ICS_ID, model)) : Edit(model) });

			AddErrorMessage("An error occured while saving! Please try again!");
			BagEventTypes();

			return View(model);
		}

		public ActionResult Delete(int? id) {
			if (id == null)
				return RedirectToAction("Search");

			int centerId = Session.Center().Id;
			try {
                var _event = db.Tl_EventDetail.Single(c => c.ICS_ID == id && c.CenterID == centerId);
				db.Tl_EventDetail.Remove(_event);
				db.SaveChanges();
				AddSuccessMessage("You have successfully deleted an Event record!");
			} catch (Exception) {
				ModelState.AddModelError("", "Unable to delete record. Try again, and if the problem persists, see your system administrator.");
			}

            if (string.IsNullOrEmpty((string)TempData.Peek("EventReturnUrl")))
                return RedirectToAction("Search");
            else
                return Redirect(TempData.Peek("EventReturnUrl").ToString());
        }

		private EventViewModel LoadOrCreate(int? icsId, EventViewModel model) {
			if (icsId == null)
				return model;

			int centerId = Session.Center().Id;
			var eventDetail = db.Tl_EventDetail.SingleOrDefault(c => c.ICS_ID == icsId && c.CenterID == centerId);
			if (eventDetail == null)
				return null;

			model.ProgramID = eventDetail.ProgramID;
			model.EventDate = eventDetail.EventDate;
			model.NumPeopleReached = eventDetail.NumPeopleReached;
			model.EventHours = eventDetail.EventHours;
			model.EventName = eventDetail.EventName;
			model.Location = eventDetail.Location;
			model.StateID = eventDetail.StateID;
			model.CountyID = eventDetail.CountyID;
			model.Comment = eventDetail.Comment;
			model.EventDetailStaff = eventDetail.EventDetailStaff.ToList();
			model.ICS_ID = eventDetail.ICS_ID;
			return model;
		}

		private void Validate(EventViewModel model) {
			var nonActiveStaff = new List<Staff>();
			var validateDic = new Dictionary<int, string>();

			if (model.EventDetailStaff != null) {
				for (int i = 0; i < model.EventDetailStaff.Count; i++) {
					int svId = model.EventDetailStaff[i].SVID;
					var employee = db.T_StaffVolunteer.FirstOrDefault(s => s.SvId == svId);

					if (employee.StartDate != null && employee.StartDate > model.EventDate || employee.TerminationDate != null && employee.TerminationDate <= model.EventDate) {
						ModelState.AddModelError("EventDetailStaff[" + i + "].SVID", "Staff/Volunteer was not active during the time of the service.");
						nonActiveStaff.Add(new Staff {
							EmployeeName = employee.LastName + ", " + employee.FirstName,
							SVID = employee.SvId
						});
					}
				}

				for (int i = 0; i < model.EventDetailStaff.Count; i++) {
					string added;
					bool isPresent = validateDic.TryGetValue(model.EventDetailStaff[i].SVID, out added);
					if (isPresent) {
						ModelState.AddModelError(validateDic[model.EventDetailStaff[i].SVID], "Employee added more than once");
						ModelState.AddModelError("EventDetailStaff[" + i + "].SVID", "Employee added more than once");
					} else {
						validateDic.Add(model.EventDetailStaff[i].SVID, "EventDetailStaff[" + i + "].SVID");
					}
				}
			} else {
				ModelState.AddModelError("", "Event must have at least one staff member");
			}

			TempData["NonActiveStaff"] = nonActiveStaff;
		}

		private int? Add(EventViewModel model) {
			bool isNewEvent = model.ICS_ID == null;
			int centerId = Session.Center().Id;
			try {
				var ev = new EventDetail();
				if (isNewEvent) {
					ev.ProgramID = model.ProgramID;
					ev.EventDate = model.EventDate;
					ev.EventName = model.EventName;
					ev.EventHours = model.EventHours;
					ev.NumPeopleReached = model.NumPeopleReached;
					ev.Comment = model.Comment;
					ev.Location = model.Location;
					ev.StateID = model.StateID;
					ev.CountyID = model.CountyID;
					ev.EventDetailStaff = model.EventDetailStaff;
					ev.CenterID = centerId;
					ev.FundDateID = model.EventDate.NotNull(d => Data.FundingForStaff.GetFundDateId((DateTime)d, centerId));
					db.Tl_EventDetail.Add(ev);
				}
				db.SaveChanges();
				AddSuccessMessage("You have successfully added a new Event record!");
				return model.saveAddNew == 0 ? (int?)ev.ICS_ID : null;
			} catch (RetryLimitExceededException) {
				if (isNewEvent)
					model.ICS_ID = null;
				AddErrorMessage("Unable to save changes. Try again, and if the problem persists, see your system administrator.");
				return 0;
			}
		}

		private int? Edit(EventViewModel model) {
			int locationId = Session.Center().Id;
			var original = db.Tl_EventDetail.Single(p => p.ICS_ID == model.ICS_ID && p.CenterID == locationId);
			var thisEvent = db.Entry(original);
			thisEvent.CurrentValues.SetValues(model);

			if ((DateTime)thisEvent.OriginalValues["EventDate"] != model.EventDate)
				thisEvent.CurrentValues["FundDateID"] = model.EventDate.NotNull(d => Data.FundingForStaff.GetFundDateId((DateTime)d, locationId));

			foreach (var each in model.EventDetailStaff) {
				var originalEventDetailStaff = original.EventDetailStaff.SingleOrDefault(pds => pds.ICS_Staff_ID == each.ICS_Staff_ID && pds.ICS_Staff_ID != null);

				if (originalEventDetailStaff != null) {
					db.Entry(originalEventDetailStaff).State = EntityState.Detached;
					db.Ts_EventDetail_Staffs.Attach(each);
					db.Entry(each).State = originalEventDetailStaff.IsUnchanged(each) ? EntityState.Unchanged : EntityState.Modified;
				} else {
					each.ICS_ID = model.ICS_ID;
					db.Ts_EventDetail_Staffs.Add(each);
					db.Entry(each).State = EntityState.Added;
				}
			}

			foreach (var originalEventDetailStaff in original.EventDetailStaff.ToList())
				if (model.EventDetailStaff.All(p => p.ICS_Staff_ID != originalEventDetailStaff.ICS_Staff_ID))
					db.Ts_EventDetail_Staffs.Remove(originalEventDetailStaff);

			db.SaveChanges();
			AddSuccessMessage("You have successfully modified an Event record and your changes have been saved!");
			return model.saveAddNew == 0 ? model.ICS_ID : null;
		}

		private void BagEventTypes() {
			ViewBag.EventTypes = Lookups.EventTypes[Session.Center().Provider];
		}
		#endregion

		#region ajax
		public ActionResult AddNewStaff(int? ICS_ID, DateTime? date) {
			var model = new EventViewModel {
				EventDate = date,
				ICS_ID = ICS_ID,
				EventDetailStaff = new List<EventDetailStaff> { new EventDetailStaff() }
			};
			return PartialView("_Staff", model);
		}
		#endregion
	}
}