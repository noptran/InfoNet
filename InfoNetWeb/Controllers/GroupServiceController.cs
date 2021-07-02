using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Mvc;
using Infonet.Core;
using Infonet.Data;
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
	public class GroupServiceController : InfonetControllerBase {
		#region Ajax Calls
		public JsonResult SearchCases(string clientCode, DateTime? FStartDate, DateTime? FEndDate, DateTime? SStartDate, DateTime? SEndDate, int? clientTypeId, int? skip, IList<int?> existingClientIds) {
			int centerId = Session.Center().Top.Id;
			int skipAmount = skip ?? 0;
			int total;

			if ((FStartDate != null || FEndDate != null) && SStartDate == null && SEndDate == null) {
				var cases = from cc in db.T_ClientCases
							join c in db.T_Client on cc.ClientId equals c.ClientId
							where c.CenterId == centerId
							orderby c.ClientCode, cc.CaseId descending
							select new {
								clientCode = c.ClientCode, caseId = cc.CaseId, clientId = c.ClientId,
								firstContactDate = cc.FirstContactDate, clientTypeId = c.ClientTypeId
							};

				if (existingClientIds != null)
					cases = cases.Where(c => !existingClientIds.Contains(c.clientId));

				if (FStartDate != null)
					cases = cases.Where(c => c.firstContactDate >= FStartDate);

				if (FEndDate != null)
					cases = cases.Where(c => c.firstContactDate <= FEndDate);

				if (clientTypeId != null)
					cases = cases.Where(c => c.clientTypeId == clientTypeId);

				if (!string.IsNullOrEmpty(clientCode) && !string.IsNullOrWhiteSpace(clientCode))
					cases = cases.Where(c => c.clientCode.Contains(clientCode));

				cases = cases.OrderBy(c => c.clientCode).ThenByDescending(m => m.caseId);

				total = cases.Distinct().Count();

				var results = cases.Select(c => new { c.clientId, c.clientCode, c.caseId, total }).Distinct();

				return Json(results.OrderBy(c => c.clientCode).ThenByDescending(m => m.caseId).Skip(skipAmount).Take(50), JsonRequestBehavior.AllowGet);
			}

			var clients = from sdc in db.Tl_ServiceDetailOfClient
						  join plu in db.TLU_Codes_ProgramsAndServices on sdc.ServiceID equals plu.CodeID
						  join cc in db.T_ClientCases on sdc.ClientID equals cc.ClientId
						  join c in db.T_Client on cc.ClientId equals c.ClientId
						  where c.CenterId == centerId && plu.IsGroupService == true
						  select new {
							  clientId = sdc.ClientID, clientCode = c.ClientCode, caseId = sdc.CaseID,
							  firstContactDate = cc.FirstContactDate, clientTypeId = c.ClientTypeId, serviceDate = sdc.ServiceDate
						  };

			if (existingClientIds != null)
				clients = clients.Where(c => !existingClientIds.Contains(c.clientId));

			//First contact date range filter
			if (FStartDate != null)
				clients = clients.Where(c => c.firstContactDate >= FStartDate);

			if (FEndDate != null)
				clients = clients.Where(c => c.firstContactDate <= FEndDate);
			//Client type (Adult,Child) filter
			if (clientTypeId != null)
				clients = clients.Where(c => c.clientTypeId == clientTypeId);
			//ClientCode filter
			if (!string.IsNullOrEmpty(clientCode) && !string.IsNullOrWhiteSpace(clientCode))
				clients = clients.Where(c => c.clientCode.Contains(clientCode));

			//Service date range filter
			if (SStartDate != null)
				clients = clients.Where(c => c.serviceDate >= SStartDate);

			if (SEndDate != null)
				clients = clients.Where(c => c.serviceDate <= SEndDate);

			var clientsResult = clients.Select(c => new { c.clientId, c.clientCode, c.caseId }).OrderBy(c => c.clientCode).ThenByDescending(m => m.caseId);

			total = clientsResult.Distinct().Count();

			var result = clientsResult.Select(c => new { c.clientId, c.clientCode, c.caseId, total }).Distinct();

			return Json(result.OrderBy(c => c.clientCode).ThenByDescending(m => m.caseId).Skip(skipAmount).Take(50), JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetClients(string clientCode) {
			int centerId = Session.Center().Top.Id;
			var clients = db.T_Client.Where(m => m.ClientCode.Contains(clientCode) && m.CenterId == centerId).Select(m => new { m.ClientCode }).Take(8).ToList();

			return Json(clients, JsonRequestBehavior.AllowGet);
		}

		public JsonResult GetCases(string clientCode) {
			int centerId = Session.Center().Top.Id;
			var cases = db.T_ClientCases.Where(cc => cc.Client.CenterId == centerId && cc.Client.ClientCode == clientCode).ToList().Select(cc => new { caseId = cc.CaseId, clientId = cc.ClientId, firstContactDate = cc.FirstContactDate.Value.ToShortDateString(), caseClosed = cc.CaseClosed != null && cc.CaseClosed > 0 ? "Closed" : "Open" });

			cases = cases.OrderByDescending(c => c.caseId);

			return Json(cases, JsonRequestBehavior.AllowGet);
		}

		public ActionResult AddNewAttendee() {
			return PartialView("_NewAttendeePartial");
		}

		public ActionResult AddNewStaff(string date) {
			return PartialView("_NewStaffPartial", new GroupServiceViewModel {
				PDate = DateTime.Parse(date),
				ProgramDetailStaff = new List<ProgramDetailStaff> { new ProgramDetailStaff() }
			});
		}

		#endregion

		#region Search
		public ActionResult Search(GroupServiceViewModel model, int? page) {
			int centerId = Session.Center().Id;
			var results = from grpServices in db.Tl_ProgramDetail
						  join programsAndServices in db.TLU_Codes_ProgramsAndServices on grpServices.ProgramID equals programsAndServices.CodeID
						  where grpServices.CenterID == centerId && programsAndServices.IsGroupService == true
						  orderby grpServices.PDate descending
						  select new GroupServiceViewModel.GroupServiceSearchResult {
							  PDate = grpServices.PDate, Hours = grpServices.Hours,
							  NumOfSession = grpServices.NumOfSession,
							  ParticipantsNum = grpServices.ParticipantsNum,
							  ProgramName = programsAndServices.Description, ICS_ID = grpServices.ICS_ID
						  };

			if (model.StartDate != null && model.EndDate != null)
				results = results.Where(c => c.PDate >= model.StartDate
											&& c.PDate <= model.EndDate);

			else if (model.StartDate != null)
				results = results.Where(c => c.PDate >= model.StartDate);

			else if (model.EndDate != null)
				results = results.Where(c => c.PDate <= model.EndDate);

			int pageNumber = page ?? 1;
			model.PageNumber = pageNumber;
			model.RecordCount = results.Count();

			model.GroupServiceList = results.ToPagedList(pageNumber, model.PageSize == -1 ? (int)model.RecordCount : model.PageSize);

			foreach (var grp in model.GroupServiceList) {
				grp.employeeNames = GetProgramDetailStaffNamesAsString(db, grp.ICS_ID);
			}

			return View(model);
		}

		[HttpPost]
		public ActionResult Search() {
			ModelState.Clear();
			return RedirectToAction("Search");
		}
		#endregion

		#region Delete
		public ActionResult DeleteGroupService(int? id) {
			if (id == null)
				return RedirectToAction("Form");

			int locationId = Session.Center().Id;
			try {
				db.Tl_ProgramDetail.Remove(db.Tl_ProgramDetail.Single(p => p.ICS_ID == id && p.CenterID == locationId));
				db.SaveChanges();
				AddSuccessMessage("You have successfully deleted a group service record!");
			} catch (Exception) {
				ModelState.AddModelError("", "Unable to delete record. Try again, and if the problem persists, see your system administrator.");
			}
            if (string.IsNullOrEmpty((string)TempData.Peek("GroupServiceReturnUrl")))
                return RedirectToAction("Search");
            else
                return Redirect(TempData.Peek("GroupServiceReturnUrl").ToString());
        }
		#endregion

		#region Add/Edit
		public ActionResult Form(int? id) {
			var grpService = id == null ? new GroupServiceViewModel() : LoadOrCreate(id);

			if (grpService == null)
				return RedirectToAction("Search");

			TempData["GroupServiceReturnUrl"] = !string.IsNullOrEmpty((string)TempData.Peek("GroupServiceReturnUrl")) ? TempData["GroupServiceReturnUrl"] : "/GroupService/Search";

			ViewBag.EmptyCaseList = new List<int?>();
			BagGroupServices();
			return View(grpService);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[PreventDuplicateRequest]
		public ActionResult Form(GroupServiceViewModel outputModel) {

			//delete the rows with no data
			((List<ProgramDetailStaff>)outputModel.ProgramDetailStaff)?.RemoveAll(m => m.SVID == 0);
			((List<GroupServiceViewModel.AttendeeViewModel>)outputModel.Attendees)?.RemoveAll(m => m.ClientCode == null);

			TempData["GroupServiceReturnUrl"] = outputModel.ReturnURL;

			ValidateGroup(outputModel);
			if (ModelState.IsValid)
				return RedirectToAction("Form", "GroupService", new { id = outputModel.ICS_ID == null ? AddNewEntity(outputModel) : EditExistingEntity(outputModel) });

			AddErrorMessage("An error occured while saving! Please try again!");
			BagGroupServices();
			if (outputModel.Attendees != null)
				foreach (var attendee in outputModel.Attendees) {
					attendee.Cases = GetCurrentClientCases(attendee.ServiceDetailOfClient);
				}

			return View(outputModel);
		}

		public ActionResult FormRedirect(int? id) {
			if (Request != null && Request.UrlReferrer != null) {
				TempData["GroupServiceReturnUrl"] = Request.UrlReferrer.AbsoluteUri;
			}
			return RedirectToAction("Form", new { id });
		}

		#endregion

		#region Private
		private void ValidateGroup(GroupServiceViewModel model) {

			var nonActiveStaff = new List<Staff>();
			var validateDic = new Dictionary<int, string>();

			for (int i = 0; i < model.ProgramDetailStaff.Count; i++) {
				int svid = model.ProgramDetailStaff[i].SVID;
				var employee = db.T_StaffVolunteer.FirstOrDefault(s => s.SvId == svid);

				if (employee.StartDate != null && employee.StartDate > model.PDate || employee.TerminationDate != null && employee.TerminationDate <= model.PDate) {
					ModelState.AddModelError("ProgramDetailStaff[" + i + "].SVID", "Staff/Volunteer was not active during the time of the service.");
					nonActiveStaff.Add(new Staff {
						EmployeeName = employee.LastName + ", " + employee.FirstName,
						SVID = employee.SvId
					});
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


			if (model.Attendees != null) {
				Dictionary<string, string> validateDic2 = new Dictionary<string, string>();
				for (int i = 0; i < model.Attendees.Count; i++) {
					string added;
					bool isPresent = validateDic2.TryGetValue(model.Attendees[i].ClientCode.ToLower(), out added);
					if (isPresent) {
						ModelState.AddModelError(validateDic2[model.Attendees[i].ClientCode.ToLower()], "Attendee added more than once");
						ModelState.AddModelError("Attendees[" + i + "].ClientCode", "Attendee added more than once");
					} else {
						validateDic2.Add(model.Attendees[i].ClientCode.ToLower(), "Attendees[" + i + "].ClientCode");
					}
					string clientCode = model.Attendees[i].ClientCode;
					int centerId = Session.Center().Top.Id;
					if (!db.T_Client.Any(m => clientCode == m.ClientCode && m.CenterId == centerId))
						ModelState.AddModelError("Attendees[" + i + "].ClientCode", "The Client ID entered is not valid.");
				}
			}
		}

		private GroupServiceViewModel LoadOrCreate(int? icsId) {
			if (icsId == null)
				return null;

			var grpServ = new GroupServiceViewModel();
			int centerId = Session.Center().Id;
			var groupServiceDetail = db.Tl_ProgramDetail.FirstOrDefault(c => c.ICS_ID == icsId && c.CenterID == centerId);
			if (groupServiceDetail == null) {
				return null;
			}
			var progDetail = db.Tl_ProgramDetail.FirstOrDefault(c => c.ICS_ID == icsId);
			grpServ.Attendees = new List<GroupServiceViewModel.AttendeeViewModel>();
			foreach (var clientServiceDetail in progDetail.ServiceDetailsOfClient) {
				var attendee = new GroupServiceViewModel.AttendeeViewModel();
				attendee.ClientCode = db.T_Client.SingleOrDefault(c => c.ClientId == clientServiceDetail.ClientID).ClientCode;
				attendee.Cases = GetCurrentClientCases(clientServiceDetail);
				attendee.ServiceDetailOfClient = clientServiceDetail;

				grpServ.Attendees.Add(attendee);
			}
			grpServ.Attendees = grpServ.Attendees.OrderBy(gs => gs.ClientCode).ToList();
			grpServ.ProgramDetailStaff = progDetail.ProgramDetailStaff;
			grpServ.NumOfSession = progDetail.NumOfSession;
			grpServ.ParticipantsNum = progDetail.ParticipantsNum;
			grpServ.PDate = progDetail.PDate;
			grpServ.Hours = progDetail.Hours;
			grpServ.ProgramID = progDetail.ProgramID;
			grpServ.ICS_ID = icsId;
			return grpServ;
		}

		private List<SelectListItem> GetCurrentClientCases(ServiceDetailOfClient clientServiceDetail) {
			var currentClientCases = db.T_ClientCases.Where(cc => cc.ClientId == clientServiceDetail.ClientID).ToList().Select(cc => new SelectListItem { Value = cc.CaseId.ToString(), Text = cc.CaseId.ToString() + " - " + cc.FirstContactDate.Value.ToShortDateString() + " - " + (cc.CaseClosed != null && cc.CaseClosed > 0 ? "Closed" : "Open"), Selected = cc.CaseId == clientServiceDetail.CaseID });
			List<SelectListItem> cases = new List<SelectListItem>();
			foreach (var @case in currentClientCases)
				cases.Add(@case);
			return cases;
		}

		private int? EditExistingEntity(GroupServiceViewModel groupService) {
			int parentCenterId = Session.Center().Top.Id;
			int locationId = Session.Center().Id;

			ChangeNullToZero(groupService.ProgramDetailStaff);
			var originalGroupService = db.Tl_ProgramDetail.Single(p => p.ICS_ID == groupService.ICS_ID && p.CenterID == locationId);

			var thisGroupService = db.Entry(originalGroupService);
			thisGroupService.CurrentValues.SetValues(groupService);

			if ((DateTime)thisGroupService.OriginalValues["PDate"] != groupService.PDate)
				thisGroupService.CurrentValues["FundDateID"] = groupService.PDate.NotNull(d => Data.FundingForStaff.GetFundDateId((DateTime)d, locationId));

			foreach (var each in groupService.ProgramDetailStaff) {
				var originalProgramDetailStaff = originalGroupService.ProgramDetailStaff.SingleOrDefault(pds => pds.ICS_Staff_ID == each.ICS_Staff_ID && pds.ICS_Staff_ID != null);
				if (originalProgramDetailStaff != null) {
					db.Entry(originalProgramDetailStaff).State = EntityState.Detached;
					db.Ts_ProgramDetail_Staffs.Attach(each);
					db.Entry(each).State = originalProgramDetailStaff.IsUnchanged(each) ? EntityState.Unchanged : EntityState.Modified;
				} else {
					each.ICS_ID = groupService.ICS_ID;
					db.Ts_ProgramDetail_Staffs.Add(each);
					db.Entry(each).State = EntityState.Added;
				}
			}
			foreach (var originalProgramDetailStaff in originalGroupService.ProgramDetailStaff.ToList()) {
				if (groupService.ProgramDetailStaff.All(p => p.ICS_Staff_ID != originalProgramDetailStaff.ICS_Staff_ID)) {
					db.Ts_ProgramDetail_Staffs.Remove(originalProgramDetailStaff);
				}
			}

			if (groupService.Attendees != null) {
				foreach (var each in groupService.Attendees) {
					each.ServiceDetailOfClient.ICS_ID = groupService.ICS_ID;
					each.ServiceDetailOfClient.ServiceDate = groupService.PDate;
					each.ServiceDetailOfClient.ServiceID = groupService.ProgramID;
					each.ServiceDetailOfClient.LocationID = locationId;
					each.ServiceDetailOfClient.ClientID = db.T_Client.Where(c => c.ClientCode == each.ClientCode && c.CenterId == parentCenterId).Select(c => c.ClientId).Single();
					each.ServiceDetailOfClient.CityTownTownshpID = GetTownTownshipCountyId(each.ServiceDetailOfClient.ClientID, groupService.PDate.Value);
					each.ServiceDetailOfClient.FundDateID = originalGroupService.FundDateID;

					var originalServiceDetailOfClient = originalGroupService.ServiceDetailsOfClient.FirstOrDefault(sdc => sdc.ServiceDetailID == each.ServiceDetailOfClient.ServiceDetailID && sdc.ServiceDetailID != null);
					if (originalServiceDetailOfClient != null) {
						db.Entry(originalServiceDetailOfClient).State = EntityState.Detached;
						db.Tl_ServiceDetailOfClient.Attach(each.ServiceDetailOfClient);
						db.Entry(each.ServiceDetailOfClient).State = originalServiceDetailOfClient.IsUnchanged(each.ServiceDetailOfClient) ? EntityState.Unchanged : EntityState.Modified;
					} else {
						db.Tl_ServiceDetailOfClient.Add(each.ServiceDetailOfClient);
						db.Entry(each.ServiceDetailOfClient).State = EntityState.Added;
					}
				}
				foreach (var exisitingServiceDetailOfClient in originalGroupService.ServiceDetailsOfClient.ToList()) {
					if (groupService.Attendees.All(p => p.ServiceDetailOfClient.ServiceDetailID != exisitingServiceDetailOfClient.ServiceDetailID)) {
						db.Tl_ServiceDetailOfClient.Remove(exisitingServiceDetailOfClient);
					}
				}
			} else {
				foreach (var exisitingServiceDetailOfClient in originalGroupService.ServiceDetailsOfClient.ToList())
					db.Tl_ServiceDetailOfClient.Remove(exisitingServiceDetailOfClient);
			}
			db.SaveChanges();

			AddSuccessMessage("You have successfully modified a group service record and your changes have been saved!");
			return groupService.saveAddNew == 0 ? groupService.ICS_ID : null;
		}

		private int? AddNewEntity(GroupServiceViewModel groupService) {
			int centerId = Session.Center().Id;
			int topCenterId = Session.Center().Top.Id;
			bool isNewGroupService = groupService.ICS_ID == null;

			try {
				var grpServ = new ProgramDetail();
				if (isNewGroupService) {
					grpServ.ProgramID = groupService.ProgramID;
					grpServ.NumOfSession = groupService.NumOfSession;
					grpServ.Hours = groupService.Hours;
					grpServ.ParticipantsNum = groupService.ParticipantsNum;
					grpServ.PDate = groupService.PDate;
					ChangeNullToZero(groupService.ProgramDetailStaff);
					grpServ.ProgramDetailStaff = groupService.ProgramDetailStaff;
					var fundDateId = groupService.PDate.NotNull(d => Data.FundingForStaff.GetFundDateId((DateTime)d, centerId));

					IList<ServiceDetailOfClient> serviceDetailOfClient = new List<ServiceDetailOfClient>();
					if (groupService.Attendees != null)
						foreach (var attendee in groupService.Attendees) {
							attendee.ServiceDetailOfClient.FundDateID = fundDateId;
							attendee.ServiceDetailOfClient.ServiceBegDate = groupService.PDate;
							attendee.ServiceDetailOfClient.ServiceDate = groupService.PDate;
							attendee.ServiceDetailOfClient.ServiceID = groupService.ProgramID;
							attendee.ServiceDetailOfClient.LocationID = centerId;
							attendee.ServiceDetailOfClient.ClientID = db.T_Client.SingleOrDefault(c => c.ClientCode == attendee.ClientCode && c.CenterId == topCenterId).ClientId;
							attendee.ServiceDetailOfClient.CityTownTownshpID = GetTownTownshipCountyId(attendee.ServiceDetailOfClient.ClientID, (DateTime)groupService.PDate);
							serviceDetailOfClient.Add(attendee.ServiceDetailOfClient);
						}
					grpServ.ServiceDetailsOfClient = serviceDetailOfClient;
					grpServ.CenterID = centerId;
					grpServ.FundDateID = fundDateId;
					db.Tl_ProgramDetail.Add(grpServ);
				}
				db.SaveChanges();
				AddSuccessMessage("You have successfully added a new group service record!");

				return groupService.saveAddNew == 0 ? grpServ.ICS_ID : null;
			} catch (RetryLimitExceededException) {
				if (isNewGroupService)
					groupService.ICS_ID = null;
				AddErrorMessage("Unable to save changes. Try again, and if the problem persists, see your system administrator.");
				return 0;
			}
		}

		private void ChangeNullToZero(IList<ProgramDetailStaff> programDetailStaff) {
			foreach (var each in programDetailStaff) {
				each.HoursPrep = each.HoursPrep ?? 0;
				each.HoursTravel = each.HoursTravel ?? 0;
			}
		}

		private void BagGroupServices() {
			ViewBag.GrpServices = Lookups.GroupServices[Session.Center().Provider];
		}
		#endregion

		public static string GetProgramDetailStaffNamesAsString(InfonetServerContext db, int? icsId) {
			return db.Database.SqlQuery<string>("SELECT STUFF((SELECT '; ' + T_StaffVolunteer.LastName + ', ' + T_StaffVolunteer.FirstName FROM Tl_ProgramDetail join Ts_ProgramDetail_Staffs on Tl_ProgramDetail.ICS_ID = Ts_ProgramDetail_Staffs.ICS_ID join T_StaffVolunteer on Ts_ProgramDetail_Staffs.SVID = T_StaffVolunteer.SVID where Tl_ProgramDetail.ICS_ID = @p0 FOR XML PATH('')), 1, 1, '')", icsId).SingleOrDefault();
		}

		public int? GetTownTownshipCountyId(int? clientId, DateTime moveDate) {
			return db.Ts_TwnTshipCounty
				.Where(t => t.ClientID == clientId &&
							t.MoveDate ==
							db.Ts_TwnTshipCounty
								.Where(ts => ts.ClientID == clientId && ts.MoveDate <= moveDate)
								.OrderByDescending(i => i.MoveDate)
								.Select(s => s.MoveDate).FirstOrDefault())
				.OrderByDescending(o => o.LocID)
				.Select(s => s.LocID).FirstOrDefault();
		}
	}
}