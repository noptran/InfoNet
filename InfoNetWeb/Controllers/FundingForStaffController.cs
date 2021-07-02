using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Infonet.Core.Collections;
using Infonet.Data.Helpers;
using Infonet.Web.Mvc;
using Infonet.Web.Mvc.Authorization;
using Infonet.Web.ViewModels.Admin;
using Newtonsoft.Json;
using Rotativa;

namespace Infonet.Web.Controllers {
	[DenyCoalition]
	[Authorize(Roles = "DVADMIN, SAADMIN, CACADMIN")]
	[OutputCache(Duration = 0)]
	public class FundingForStaffController : InfonetControllerBase {
		#region MainDisplay
		public ActionResult Index() {
			int centerId = Session.Center().Id;

			var model = new FundingForStaff();
			model.FFSEditStaffList = new FFSEditStaffList();
			model.FFSAssignServices = new FFSAssignServices();
			model.FFSMFA = new FFSMFA();
			model.FFSAFS = new FFSAFS();
			model.FFSReports = new FFSReports();

			var issueDates = Data.FundingForStaff.GetFundIssueDates(Session.Center().Id).ToArray();
			int? firstFundingDateId = issueDates.Select(x => x.FundDateID).FirstOrDefault();

			model.SelectedFundIssueDateId = 1;
			model.SelectedFundIssueDate = Convert.ToDateTime(issueDates.Select(x => x.FundDate).FirstOrDefault());
			model.FundIssueDatesList = issueDates.Select(x => new SelectListItem {
				Value = x.FundDateID.ToString(),
				Text = x.FundDate
			});

			AutoRemoveTerminatedEmployee(firstFundingDateId, model);

			var assignedStaff = Data.FundingForStaff.GetAssignedStaff(firstFundingDateId).ToList().Select(l => l.SVID).ToArray();
			int? selectedStaffMember = assignedStaff.FirstOrDefault();

			ViewBag.FFSAvailableStaff = Data.FundingForStaff.GetAvailableAndAssignedStaff(firstFundingDateId, centerId, model.SelectedFundIssueDate);
			model.FFSEditStaffList.AvailableAndAssignedStaffSVIDs = assignedStaff;
			model.FFSEditStaffList.AvailableAndAssignedStaff = new MultiSelectList(ViewBag.FFSAvailableStaff, "SVID", "Name", assignedStaff);

			ViewBag.FFSAssignedStaff = Data.FundingForStaff.GetAssignedStaff(firstFundingDateId);

			model.FFSAssignServices.AvailableAndAssignedServices = new MultiSelectList(Data.FundingForStaff.GetAvailableAndAssignedServices(firstFundingDateId, selectedStaffMember, Convert.ToInt32(Session.Center().Provider), null), "ServiceProgramID", "Name");

			model.FFSAssignServices.EmployeeToDuplicateID = Data.FundingForStaff.GetAssignedStaff(firstFundingDateId).Skip(1).Select(x => new SelectListItem {
				Value = x.SVID.ToString(),
				Text = x.Name
			});

			model.FFSAssignServices.AssignedServices = issueDates.Any() ? Data.FundingForStaff.GetAssignedServices(firstFundingDateId, selectedStaffMember, model.SelectedFundIssueDate).ToList() : null;

			model.StaffFundedSources = Data.FundingForStaff.GetStaffFundedSources(model.SelectedFundIssueDateId, model.SelectedStaffSVID, model.SelectedFundIssueDate).ToList();
			model.FFSMFA.FundingSelectionList = Data.FundingForStaff.GetStaffFundingSources(Session.Center().Id, model.SelectedFundIssueDateId, model.SelectedStaffSVID, null, Convert.ToInt32(Session.Center().Provider), model.SelectedFundIssueDate).Select(ffs => new FundingSelection { CodeID = ffs.CodeID, EndDate = ffs.EndDate, Name = ffs.Name, PercentFund = ffs.PercentFund }).ToList();
			model.FFSMFA.ServicesSelectionList = model.FFSAssignServices.AssignedServices?.Select(asgsrv => new ServicesSelection { Name = asgsrv.Name, ServiceProgramID = asgsrv.ServiceProgramID, PercentFunded = asgsrv.PercentFunded }).ToList();

			model.FFSAFS.FundingSelectionList = model.FFSMFA.FundingSelectionList;

			model.FFSReports.FundingDates = new MultiSelectList(Data.FundingForStaff.GetFundIssueDatesBySvIdCenter(selectedStaffMember, centerId), "FundDateID", "FundDateID");
			model.FFSReports.UseDefaultMasterPage = true;

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[PreventDuplicateRequest]
		public ActionResult Index(FundingForStaff model) {
			if (ModelState.IsValid)
				return RedirectToAction("Index");

			var issueDates = Data.FundingForStaff.GetFundIssueDates(Session.Center().Id);

			model.FundIssueDatesList = issueDates.Select(x => new SelectListItem {
				Value = x.FundDateID.ToString(),
				Text = x.FundDate
			});

			return View(model);
		}
		#endregion

		#region Add/Delete Funding Date
		public ActionResult AddFundingIssueDate(FundingForStaff model) {
			if (model.FFSAdd.NewFundIssueDate <= model.FFSAdd.SelectedFundIssueDate)
				ModelState.AddModelError("FFSAdd.NewFundIssueDate", "Must be greater than the last saved Funding Statement date.");

			if (!TryUpdateModel(model.FFSAdd))
				return PartialView("_AddFundingIssueDate");

			int newId = 0;
			string userErrMsg = "Unable to add Date Issued Record.";
			string errMsg = "FFS_AddFundingDate Failed: Parameters" + " FundingDate=" + model.FFSAdd.NewFundIssueDate + " CenterID=" + Session.Center().Id + " NewID=" + newId;
			bool isErr = false;

			try {
				int sqlRet = db.Database.ExecuteSqlCommand(
					"FFS_AddFundingDate @FundingDate, @CenterID, @NewID",
					new SqlParameter("FundingDate", model.FFSAdd.NewFundIssueDate),
					new SqlParameter("CenterID", Session.Center().Id),
					new SqlParameter("NewID", newId)
				);
				if (sqlRet == 0)
					isErr = ProcessErrors(errMsg, userErrMsg, "DateIssuedError");
			} catch (Exception ex) {
				isErr = ProcessErrors(errMsg + " " + ex, userErrMsg, "DateIssuedError");
			}

			if (isErr)
				return Json(new { Error = new[] { userErrMsg } }, JsonRequestBehavior.AllowGet);
			return Json(new { Success = new[] { Url.Action("Index") } }, JsonRequestBehavior.AllowGet);
		}

		public ActionResult AddFundingIssueDateClean() {
			return PartialView("_AddFundingIssueDate");
		}

		public ActionResult DeleteFundingIssueDate(int fundingDateId) {
			int newId = 0;
			string userErrMsg = "Unable to delete Date Issued Record.";
			string errMsg = "FFS_DeleteFundingDate Failed: Parameters" + " FundingDateID=" + fundingDateId + " CenterID=" + Session.Center().Id + " NewID=" + newId;
			bool isErr = false;

			try {
				int sqlRet = db.Database.ExecuteSqlCommand(
					"FFS_DeleteFundingDate @FundingDateID, @CenterID, @NewID",
					new SqlParameter("FundingDateID", fundingDateId),
					new SqlParameter("CenterID", Session.Center().Id),
					new SqlParameter("NewID", newId)
				);
				if (sqlRet == 0)
					isErr = ProcessErrors(errMsg, userErrMsg, "DateIssuedError");
			} catch (Exception ex) {
				isErr = ProcessErrors(errMsg + " " + ex, userErrMsg, "DateIssuedError");
			}

			if (isErr)
				return Json(new { Error = new[] { userErrMsg } }, JsonRequestBehavior.AllowGet);
			return Json(Url.Action("Index"));
		}
		#endregion

		[HttpPost]

		#region AssignServices
		public ActionResult AssignServices(FundingForStaff model) {
			if (!ModelState.IsValidField("FFSAssignServices_AvailableAndAssignedServiceIDs"))
				return PartialView("_AssignServices");

			if (model.FFSAssignServices == null)
				model.FFSAssignServices = new FFSAssignServices();

			string userErrMsg = "Unable to Update Program/Service Selection. - Error Match Services For";

			string errMsg = "FFS_AssignServices Failed: Parameters" + " FundingDateID=" + model.SelectedFundIssueDateId + " StaffID=" + model.SelectedStaffSVID + " ServiceID = 0 EmployeeToDuplicateID = " + (string.IsNullOrEmpty(model.FFSAssignServices.SelectedEmployeeToDuplicateID.ToString()) ? model.FFSAssignServices.SelectedEmployeeToDuplicateID.ToString() : "");
			bool isErr = false;

			using (var transaction = db.Database.BeginTransaction())
				try {
					if (model.FFSAssignServices.SelectedEmployeeToDuplicateID > 0) {
						try {
							int sqlRet = db.Database.ExecuteSqlCommand(
								"FFS_AssignServices @FundingDateID, @StaffID, @ServiceID, @EmployeeToDuplicateID",
								new SqlParameter("FundingDateID", model.SelectedFundIssueDateId),
								new SqlParameter("StaffID", model.SelectedStaffSVID),
								new SqlParameter("ServiceID", DBNull.Value),
								new SqlParameter("EmployeeToDuplicateID", model.FFSAssignServices.SelectedEmployeeToDuplicateID)
							);
							if (sqlRet == 0)
								isErr = ProcessErrors(errMsg, userErrMsg, "AssignServicesError");
						} catch (Exception ex) {
							isErr = ProcessErrors(errMsg + " " + ex, userErrMsg, "AssignServicesError");
						}
					} else {
						userErrMsg = "Unable to Update Program/Service Selection.";

						errMsg = "FFS_AssignServices Failed: Parameters" + " FundingDateID=" + model.SelectedFundIssueDateId + " StaffID=" + model.SelectedStaffSVID + " ServiceID = " + (model.FFSAssignServices.AvailableAndAssignedServiceIDs != null ? string.Join(",", model.FFSAssignServices.AvailableAndAssignedServiceIDs) : "") + " EmployeeToDuplicateID = null";

						var currentAssignedServices = Data.FundingForStaff.GetAssignedServices(model.SelectedFundIssueDateId, model.SelectedStaffSVID, model.SelectedFundIssueDate).ToList().Select(l => l.ServiceProgramID).ToArray();

						if (model.FFSAssignServices.AvailableAndAssignedServiceIDs != null) {
							var addServiceProgramIDs = model.FFSAssignServices.AvailableAndAssignedServiceIDs.Except(currentAssignedServices);

							foreach (var currentProgramServiceId in addServiceProgramIDs)
								try {
									int sqlRet = db.Database.ExecuteSqlCommand(
										"FFS_AssignServices @FundingDateID, @StaffID, @ServiceID, @EmployeeToDuplicateID",
										new SqlParameter("FundingDateID", model.SelectedFundIssueDateId),
										new SqlParameter("StaffID", model.SelectedStaffSVID),
										new SqlParameter("ServiceID", currentProgramServiceId.Value),
										new SqlParameter("EmployeeToDuplicateID", DBNull.Value)
									);
									if (sqlRet == 0)
										isErr = ProcessErrors(errMsg, userErrMsg, "AssignServicesError");
								} catch (Exception ex) {
									isErr = ProcessErrors(errMsg + " " + ex, userErrMsg, "AssignServicesError");
								}
						}
						var deleteServiceProgramIDs = model.FFSAssignServices.AvailableAndAssignedServiceIDs != null ? currentAssignedServices.Except(model.FFSAssignServices.AvailableAndAssignedServiceIDs).ToArray() : currentAssignedServices;

						foreach (var currentServiceProgramId in deleteServiceProgramIDs)
							try {
								int sqlRet = db.Database.ExecuteSqlCommand(
									"FFS_RemoveServices @FundingDateID, @StaffID, @ServiceID",
									new SqlParameter("FundingDateID", model.SelectedFundIssueDateId),
									new SqlParameter("StaffID", model.SelectedStaffSVID),
									new SqlParameter("ServiceID", currentServiceProgramId)
								);
								if (sqlRet == 0)
									isErr = ProcessErrors(errMsg, userErrMsg, "AssignServicesError");
							} catch (Exception ex) {
								isErr = ProcessErrors(errMsg + " " + ex, userErrMsg, "AssignServicesError");
							}
					}
					transaction.Commit();
				} catch {
					transaction.Rollback();
					throw;
				}
			return IsError(isErr, userErrMsg);
		}

		public ActionResult AssignServicesUpdate(FundingForStaff model) {
			int providerId = Session.Center().Provider.ToInt32();
			var assignedServices = Data.FundingForStaff.GetAssignedServices(model.SelectedFundIssueDateId, model.SelectedStaffSVID, model.SelectedFundIssueDate).ToList().Select(l => l.ServiceProgramID).ToArray();
			var availableServices = Data.FundingForStaff.GetAvailableAndAssignedServices(model.SelectedFundIssueDateId, model.SelectedStaffSVID, providerId, model.SelectedFundIssueDate);

			return Json(new { AssignedServices = new[] { JsonConvert.SerializeObject(assignedServices) }, AvailableServices = new[] { JsonConvert.SerializeObject(availableServices) } }, JsonRequestBehavior.AllowGet);
		}
		#endregion

		#region EditStaffList
		public ActionResult EditStaffList(FundingForStaff model) {
			if (!ModelState.IsValidField("FFSEditStaffList_AvailableAndAssignedStaffSVIDs"))
				return PartialView("_EditStaffList");

			if (model.FFSEditStaffList == null)
				model.FFSEditStaffList = new FFSEditStaffList();

			string userErrMsg = "Unable to Update Edit Staff List.";
			string errMsg = "FFS_AssignEmployee Failed: Parameters" + " FundingDateID=" + model.SelectedFundIssueDateId + " EmployeeIDs=" + model.FFSEditStaffList.AvailableAndAssignedStaffSVIDs;
			bool isErr = false;

			var currentAssignedStaff = Data.FundingForStaff.GetAssignedStaff(model.SelectedFundIssueDateId).ToList().Select(l => l.SVID).ToArray();
			var addStaffSvIds = Enumerable.Empty<int?>();
			IEnumerable<int?> deleteStaffSvIds;
			if (model.FFSEditStaffList.AvailableAndAssignedStaffSVIDs != null) {
				addStaffSvIds = model.FFSEditStaffList.AvailableAndAssignedStaffSVIDs.Except(currentAssignedStaff);
				deleteStaffSvIds = currentAssignedStaff.Except(model.FFSEditStaffList.AvailableAndAssignedStaffSVIDs);
			} else {
				deleteStaffSvIds = currentAssignedStaff;
			}

			using (var transaction = db.Database.BeginTransaction())
				try {
					foreach (var currentSvId in addStaffSvIds)
						try {
							int sqlRet = db.Database.ExecuteSqlCommand(
								"FFS_AssignEmployee @FundingDateID, @EmployeeID",
								new SqlParameter("FundingDateID", model.SelectedFundIssueDateId),
								new SqlParameter("EmployeeID", currentSvId)
							);
							if (sqlRet == 0)
								isErr = ProcessErrors(errMsg, userErrMsg, "EditStaffListError");
						} catch (Exception ex) {
							isErr = ProcessErrors(errMsg + " " + ex, userErrMsg, "EditStaffListError");
						}

					foreach (var currentSvId in deleteStaffSvIds)
						try {
							int sqlRet = db.Database.ExecuteSqlCommand(
								"FFS_RemoveEmployee @FundingDateID, @EmployeeID",
								new SqlParameter("FundingDateID", model.SelectedFundIssueDateId),
								new SqlParameter("EmployeeID", currentSvId)
							);
							if (sqlRet == 0)
								isErr = ProcessErrors(errMsg, userErrMsg, "EditStaffListError");
						} catch (Exception ex) {
							isErr = ProcessErrors(errMsg + " " + ex, userErrMsg, "EditStaffListError");
						}
					transaction.Commit();
				} catch {
					transaction.Rollback();
					throw;
				}

			return IsError(isErr, userErrMsg);
		}

		public ActionResult EditStaffListUpdate(FundingForStaff model) {
			var assignedStaff = Data.FundingForStaff.GetAssignedStaff(model.SelectedFundIssueDateId).ToList().Select(l => l.SVID).ToArray();
			var availableStaff = Data.FundingForStaff.GetAvailableAndAssignedStaff(model.SelectedFundIssueDateId, Session.Center().Id, model.SelectedFundIssueDate);

			return Json(new { AssignedStaff = new[] { JsonConvert.SerializeObject(assignedStaff) }, AvailableStaff = new[] { JsonConvert.SerializeObject(availableStaff) } }, JsonRequestBehavior.AllowGet);
		}

		public ActionResult MatchServicesForUpdate(FundingForStaff model) {
			var percentFunded = Data.FundingForStaff.GetPercentFunded(model.SelectedFundIssueDateId).Where(x => x.PercentFunded > 0).ToList();
			var assignedStaff = Data.FundingForStaff.GetAssignedStaff(model.SelectedFundIssueDateId).ToList();

			var matchServicesFor = (from first in assignedStaff
				join second in percentFunded
					on first.SVID equals second.SVID
				where first.SVID != model.SelectedStaffSVID
				select new { first.SVID, first.Name }).ToList();

			return Json(new { MatchServicesFor = new[] { JsonConvert.SerializeObject(matchServicesFor) } }, JsonRequestBehavior.AllowGet);
		}
		#endregion

		[HttpPost]

		#region MultiFundAssignment
		public ActionResult MultiFundAssignment(FundingForStaff model) {
			bool redisplayPartial = false;

			if (model.FFSMFA.FundingSelectionList.Where(fsl => fsl.IsChecked).Select(x => x.PercentFund ?? 0).Sum() != 100) {
				ModelState.AddModelError("MFAPercentError", "Percentages for all funds must sum to 100%.");
				redisplayPartial = true;
			}

			int numServicesSelected = model.FFSMFA.ServicesSelectionList.Count(cc => cc.IsChecked);

			if (numServicesSelected == 0) {
				ModelState.AddModelError("MFAServicesSelected", "At least 1 service must be selected.");
				redisplayPartial = true;
			}

			if (redisplayPartial)
				return PartialView("_MultiFundAssignmentFundsServices", model);

			if (model.FFSMFA == null)
				model.FFSMFA = new FFSMFA();

			string userErrMsg = "Unable to Update Multi-Fund Assignment.";
			string errMsg = "FFS_AssignFunding Failed: Parameters" + " FundingDateID=" + model.SelectedFundIssueDateId + " StaffID=" + model.SelectedStaffSVID;
			bool isErr = false;

			var servicesToUpdate = model.FFSMFA.ServicesSelectionList.Where(x => x.IsChecked).ToList();
			var originalFundingSource = Data.FundingForStaff.GetStaffFundingSources(Session.Center().Id, model.SelectedFundIssueDateId, model.SelectedStaffSVID, null, Convert.ToInt32(Session.Center().Provider), model.SelectedFundIssueDate).Select(ffs => new FundingSelection { CodeID = ffs.CodeID, EndDate = ffs.EndDate, Name = ffs.Name, PercentFund = ffs.PercentFund }).ToList();

			using (var transaction = db.Database.BeginTransaction())
				try {
					foreach (var serviceToUpdate in servicesToUpdate) {
						try {
							int sqlRet = db.Database.ExecuteSqlCommand("Delete FROM Tl_FundServiceProgramOfStaffs WHERE FundDateID = @FundDateId AND SVID = @SVID AND ServiceProgramID = @ServiceProgramId",
								new SqlParameter("FundDateID", model.SelectedFundIssueDateId),
								new SqlParameter("SVID", model.SelectedStaffSVID),
								new SqlParameter("ServiceProgramId", serviceToUpdate.ServiceProgramID)
							);
							if (sqlRet == 0)
								isErr = ProcessErrors(errMsg, userErrMsg, "MultiFundAssignmentError");
						} catch (Exception ex) {
							isErr = ProcessErrors(errMsg + " " + ex, userErrMsg, "MultiFundAssignmentError");
						}

						if (isErr)
							break;

						foreach (var fundingSource in model.FFSMFA.FundingSelectionList) {
							//KMS DO was this supposed to be used?
							var originalFundPercent = originalFundingSource.Where(x => x.CodeID == fundingSource.CodeID).Select(x => x.PercentFund).FirstOrDefault();

							if (fundingSource.IsChecked && fundingSource.PercentFund != null && fundingSource.PercentFund != 0)
								try {
									int sqlRet = db.Database.ExecuteSqlCommand(
										"FFS_AssignFunding @FundingDateID, @StaffID, @ServiceID, @FundingSourceID, @FundingPercent",
										new SqlParameter("FundingDateID", model.SelectedFundIssueDateId),
										new SqlParameter("StaffID", model.SelectedStaffSVID),
										new SqlParameter("ServiceID", serviceToUpdate.ServiceProgramID),
										new SqlParameter("FundingSourceID", fundingSource.CodeID),
										new SqlParameter("FundingPercent", fundingSource.PercentFund)
									);
									if (sqlRet == 0)
										isErr = ProcessErrors(errMsg, userErrMsg, "MultiFundAssignmentError");
								} catch (Exception ex) {
									isErr = ProcessErrors(errMsg + " " + ex, userErrMsg, "MultiFundAssignmentError");
								}
							if (isErr)
								break;
						}
					}
					transaction.Commit();
				} catch {
					transaction.Rollback();
					throw;
				}

			return IsError(isErr, userErrMsg);
		}

		public ActionResult MultiFundAssignmentUpdate(FundingForStaff model) {
			model.FFSMFA = new FFSMFA {
				FundingSelectionList = Data.FundingForStaff.GetStaffFundingSources(Session.Center().Id, model.SelectedFundIssueDateId, model.SelectedStaffSVID, null, Convert.ToInt32(Session.Center().Provider), model.SelectedFundIssueDate).Select(ffs => new FundingSelection { CodeID = ffs.CodeID, EndDate = ffs.EndDate, Name = ffs.Name, PercentFund = ffs.PercentFund }).ToList(),
				ServicesSelectionList = Data.FundingForStaff.GetAssignedServices(model.SelectedFundIssueDateId, model.SelectedStaffSVID, model.SelectedFundIssueDate).Select(asgsrv => new ServicesSelection { Name = asgsrv.Name, ServiceProgramID = asgsrv.ServiceProgramID, PercentFunded = asgsrv.PercentFunded }).ToList()
			};

			return PartialView("_MultiFundAssignment", model);
		}
		#endregion

		#region AssignFundingSource
		[HttpPost]
		public ActionResult AssignFundingSource(FundingForStaff model) {
			if (model.FFSAFS.FundingSelectionList.Where(fsl => fsl.IsChecked).Select(x => x.PercentFund ?? 0).Sum() != 100) {
				ModelState.AddModelError("AFSPercentError", "Percentages for all funds must sum to 100%.");
				return PartialView("_AssignFundingSourceFunds", model);
			}

			if (model.FFSAFS == null)
				model.FFSAFS = new FFSAFS();

			string userErrMsg = "Unable to Assign Funding Source.";
			string errMsg = "Error Deleting from Tl_FundServiceProgramOfStaffs" + " SVID=" + model.SelectedStaffSVID + " FundingDateID=" + model.SelectedFundIssueDateId + " ServiceProgramID=" + model.CurrentServiceProgramID;
			bool isErr = false;

			using (var transaction = db.Database.BeginTransaction())
				try {
					if (model.CurrentServiceProgramID == null)
						isErr = true;
					else
						try {
							int sqlRet = db.Database.ExecuteSqlCommand(
								"delete from Tl_FundServiceProgramOfStaffs where SVID = @StaffID and FundDateID = @FundingDateID and ServiceProgramID = @ServiceID",
								new SqlParameter("StaffID", model.SelectedStaffSVID),
								new SqlParameter("FundingDateID", model.SelectedFundIssueDateId),
								new SqlParameter("ServiceID", model.CurrentServiceProgramID)
							);
						} catch (Exception ex) {
							isErr = ProcessErrors(errMsg + " " + ex, userErrMsg, "AssignFundingSourceError");
						}
					if (!isErr) {
						var originalFundingSource = Data.FundingForStaff.GetStaffFundingSources(Session.Center().Id, model.SelectedFundIssueDateId, model.SelectedStaffSVID, null, Convert.ToInt32(Session.Center().Provider), model.SelectedFundIssueDate).Select(ffs => new FundingSelection { CodeID = ffs.CodeID, EndDate = ffs.EndDate, Name = ffs.Name, PercentFund = ffs.PercentFund }).ToList();

						foreach (var fundingSource in model.FFSAFS.FundingSelectionList) {
							errMsg = "FFS_AssignFunding Failed: Parameters" + " FundingDateID=" + model.SelectedFundIssueDateId + " StaffID=" + model.SelectedStaffSVID + " ServiceID=" + model.CurrentServiceProgramID + " FundingSourceID=" + fundingSource.CodeID + " PercentFund=" + fundingSource.PercentFund;

							//KMS DO was this supposed to be used?
							var originalFundPercent = originalFundingSource.Where(x => x.CodeID == fundingSource.CodeID).Select(x => x.PercentFund).FirstOrDefault();

							if (fundingSource.IsChecked && fundingSource.PercentFund != null && fundingSource.PercentFund != 0)
								try {
									int sqlRet = db.Database.ExecuteSqlCommand(
										"FFS_AssignFunding @FundingDateID, @StaffID, @ServiceID, @FundingSourceID, @FundingPercent",
										new SqlParameter("FundingDateID", model.SelectedFundIssueDateId),
										new SqlParameter("StaffID", model.SelectedStaffSVID),
										new SqlParameter("ServiceID", model.CurrentServiceProgramID),
										new SqlParameter("FundingSourceID", fundingSource.CodeID),
										new SqlParameter("FundingPercent", fundingSource.PercentFund)
									);
									if (sqlRet == 0)
										isErr = ProcessErrors(errMsg, userErrMsg, "AssignFundingSourceError");
								} catch (Exception ex) {
									isErr = ProcessErrors(errMsg + " " + ex, userErrMsg, "AssignFundingSourceError");
								}
							if (isErr)
								break;
						}
					}
					transaction.Commit();
				} catch {
					transaction.Rollback();
					throw;
				}
			return IsError(isErr, userErrMsg);
		}

		public ActionResult AssignFundingSourceUpdate(FundingForStaff model) {
			model.FFSAFS = new FFSAFS {
				FundingSelectionList = Data.FundingForStaff.GetStaffFundingSources(Session.Center().Id, model.SelectedFundIssueDateId, model.SelectedStaffSVID, model.CurrentServiceProgramID, Convert.ToInt32(Session.Center().Provider), model.SelectedFundIssueDate).Select(ffs => new FundingSelection { CodeID = ffs.CodeID, EndDate = ffs.EndDate, Name = ffs.Name, PercentFund = ffs.PercentFund, IsChecked = ffs.PercentFund > 0 }).OrderByDescending(x => x.IsChecked).ThenBy(x => x.Name).ToList()
			};

			return PartialView("_AssignFundingSource", model);
		}
		#endregion

		#region Reports
		public ActionResult ReportStaffFundingHistoryUpdate(FundingForStaff model) {
			model.FFSReports = new FFSReports();
			var fundingDates = Data.FundingForStaff.GetFundIssueDatesBySvIdCenter(model.SelectedStaffSVID, Session.Center().Id);

			return Json(new { FundingDates = new[] { JsonConvert.SerializeObject(fundingDates) } }, JsonRequestBehavior.AllowGet);
		}

		public ActionResult ReportFundingHistoryUpdate(FundingForStaff model) {
			model.FFSReports = new FFSReports();
			var fundingDates = Data.FundingForStaff.GetFundIssueDates(Session.Center().Id);

			return Json(new { FundingDates = new[] { JsonConvert.SerializeObject(fundingDates) } }, JsonRequestBehavior.AllowGet);
		}

		public ActionResult ReportStaffFundingHistorySelect(FundingForStaff model) {
			bool redisplayPartial = false;

			int selectedFundingDates = model.FFSReports.FundingDateIDs.Length;

			if (selectedFundingDates == 0) {
				ModelState.AddModelError("ErrorFundingHistory", "At least 1 date must be selected.");
				redisplayPartial = true;
			}

			if (redisplayPartial)
				return PartialView("_ReportGetStaffSelected", model);

			if (model.FFSReports == null)
				model.FFSReports = new FFSReports();

			StaffFundingHistoryModel(model, true);

			return PartialView("_ReportGetStaffSelected", model);
		}

		public ActionResult ReportFundingHistorySelect(FundingForStaff model) {
			bool redisplayPartial = false;

			int selectedFundingDates = model.FFSReports.FundingDateIDs.Length;

			if (selectedFundingDates == 0) {
				ModelState.AddModelError("ErrorFundingHistory", "At least 1 date must be selected.");
				redisplayPartial = true;
			}

			if (redisplayPartial)
				return PartialView("_ReportGetStaffAll", model);

			if (model.FFSReports == null)
				model.FFSReports = new FFSReports();

			FundingHistoryModel(model, true);

			return PartialView("_ReportGetStaffAll", model);
		}

		public ActionResult SelectedFunding(FundingForStaff model) {
			return PartialView("_ReportGetStaffSelected", StaffSelectedFundingModel(model, true));
		}

		public ActionResult SelectedFundingAll(FundingForStaff model) {
			return PartialView("_ReportGetStaffAll", SelectedFundingModel(model, true));
		}

		public ActionResult FundingHistory(FundingForStaff model) {
			return PartialView("_ReportGetStaffAll", FundingHistoryModel(model, true));
		}

		public FundingForStaff FundingHistoryModel(FundingForStaff model, bool useDefaultMasterPage = false) {
			ReportsUseDefaultMasterPage(model, useDefaultMasterPage, false);
			model.FFSReports.ReportDetails = new List<FFSReportFundingHistory>();

			foreach (var fundingDate in model.FFSReports.FundingDateIDs)
				model.FFSReports.ReportDetails.AddRange(Data.FundingForStaff.ReportCurrentStaffFundingAll(Session.Center().Id, fundingDate).Where(x => x.ProgramOrService != null).ToList());

			model.FFSReports.ReportDetails.RemoveAll(x => (x.FundingSource == "Unassigned" || x.ProgramOrService.StartsWith("unassigned", StringComparison.OrdinalIgnoreCase)) && x.PercentFund == 0);
			model.FFSReports.ReportName = "FUNDINGHISTORY";
			return model;
		}

		public FundingForStaff StaffFundingHistoryModel(FundingForStaff model, bool useDefaultMasterPage = false) {
			ReportsUseDefaultMasterPage(model, useDefaultMasterPage, false);
			model.FFSReports.ReportDetails = new List<FFSReportFundingHistory>();

			foreach (var fundingDate in model.FFSReports.FundingDateIDs)
				model.FFSReports.ReportDetails.AddRange(Data.FundingForStaff.ReportCurrentStaffFunding(Session.Center().Id, model.SelectedStaffSVID, fundingDate).Where(x => x.ProgramOrService != null).ToList());

			model.FFSReports.ReportDetails.RemoveAll(x => (x.FundingSource == "Unassigned" || x.ProgramOrService.StartsWith("unassigned", StringComparison.OrdinalIgnoreCase)) && x.PercentFund == 0);
			model.FFSReports.ReportName = "STAFFFUNDINGHISTORY";
			return model;
		}

		public FundingForStaff SelectedFundingModel(FundingForStaff model, bool useDefaultMasterPage = false) {
			ReportsUseDefaultMasterPage(model, useDefaultMasterPage);
			model.FFSReports.ReportDetails = Data.FundingForStaff.ReportCurrentStaffFundingAll(Session.Center().Id, model.SelectedFundIssueDateId).Where(x => x.ProgramOrService != null).ToList();
			model.FFSReports.ReportDetails.RemoveAll(x => (x.FundingSource == "Unassigned" || x.ProgramOrService.StartsWith("unassigned", StringComparison.OrdinalIgnoreCase)) && x.PercentFund == 0);
			model.FFSReports.ReportName = "SELECTEDFUNDING";
			return model;
		}

		public FundingForStaff StaffSelectedFundingModel(FundingForStaff model, bool useDefaultMasterPage = false) {
			ReportsUseDefaultMasterPage(model, useDefaultMasterPage);
			model.FFSReports.ReportDetails = Data.FundingForStaff.ReportCurrentStaffFunding(Session.Center().Id, model.SelectedStaffSVID, model.SelectedFundIssueDateId).Where(x => x.ProgramOrService != null).ToList();
			model.FFSReports.ReportDetails.RemoveAll(x => (x.FundingSource == "Unassigned" || x.ProgramOrService.StartsWith("unassigned", StringComparison.OrdinalIgnoreCase)) && x.PercentFund == 0);
			model.FFSReports.ReportName = "STAFFSELECTEDFUNDING";
			return model;
		}

		public ActionResult PDF(FundingForStaff model) {
			if (model.FFSReports == null) {
				string userErrMsg = "Unable to process Funding For Staff Report";
				return Json(new { Error = new[] { userErrMsg } }, JsonRequestBehavior.AllowGet);
			}

			string partialName = "";
			string fileName = "";
			string pdfAction = model.FFSReports.PDFAction;

			switch (model.FFSReports.ReportName) {
				case "FUNDINGHISTORY":
					partialName = "_ReportGetStaffAll";
					fileName = "FundingHistory" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
					model = FundingHistoryModel(model);
					break;
				case "SELECTEDFUNDING":
					partialName = "_ReportGetStaffAll";
					fileName = "SelectedFunding" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
					model = SelectedFundingModel(model);
					break;
				case "STAFFFUNDINGHISTORY":
					partialName = "_ReportGetStaffSelected";
					fileName = "StaffFundingHistory" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
					model = StaffFundingHistoryModel(model);
					break;
				case "STAFFSELECTEDFUNDING":
					partialName = "_ReportGetStaffSelected";
					fileName = "StaffSelectedFunding" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
					model = StaffSelectedFundingModel(model);
					break;
			}

			if (pdfAction == null)
				pdfAction = "";

			switch (pdfAction.ToUpper()) {
				case "SAVE":
					return new PartialViewAsPdf(partialName, model) { FileName = fileName + ".pdf" };
				default:
					return new PartialViewAsPdf(partialName, model);
			}
		}

		public FundingForStaff ReportsUseDefaultMasterPage(FundingForStaff model, bool useDefaultMasterPage = false, bool createFFSReports = true) {
			if (createFFSReports)
				model.FFSReports = new FFSReports();

			model.FFSReports.UseDefaultMasterPage = useDefaultMasterPage;
			return model;
		}
		#endregion

		#region VariousActionResults
		public JsonResult IsError(bool isErr, string userErrMsg) {
			if (isErr)
				return Json(new { Error = new[] { userErrMsg } }, JsonRequestBehavior.AllowGet);
			return Json(new { Success = new[] { Url.Action("Index") } }, JsonRequestBehavior.AllowGet);
		}

		public ActionResult StaffListUpdate(FundingForStaff model) {
			AutoRemoveTerminatedEmployee(model.SelectedFundIssueDateId, model);

			var assignedStaff = Data.FundingForStaff.GetAssignedStaff(model.SelectedFundIssueDateId).ToArray();

			return Json(new { AssignedStaff = new[] { JsonConvert.SerializeObject(assignedStaff) } }, JsonRequestBehavior.AllowGet);
		}

		public ActionResult ServicesProgramsUpdate(FundingForStaff model) {
			model.FFSAssignServices = new FFSAssignServices();
			model.FFSAssignServices.AssignedServices = Data.FundingForStaff.GetAssignedServices(model.SelectedFundIssueDateId, model.SelectedStaffSVID, model.SelectedFundIssueDate).ToList();
			model.StaffFundedSources = Data.FundingForStaff.GetStaffFundedSources(model.SelectedFundIssueDateId, model.SelectedStaffSVID, model.SelectedFundIssueDate).ToList();

			return PartialView("_Services", model);
		}

		public JsonResult AutoRemoveTerminatedEmployee(int? selectedFundingDateId, FundingForStaff model) {
			string userErrMsg = "Unable to Automatically Remove Terminated Employee.";
			bool isErr = false;

			var terminatedStaff = Data.FundingForStaff.GetAssignedStaff(selectedFundingDateId).Where(l => l.TerminationDate <= model.SelectedFundIssueDate).ToList().Select(l => l.SVID).ToArray();

			using (var transaction = db.Database.BeginTransaction())
				try {
					foreach (var currentSvId in terminatedStaff) {
						string errMsg = "FFS_RemoveEmployee Failed: Parameters" + " FundingDateID=" + selectedFundingDateId + " EmployeeID=" + currentSvId;
						try {
							int sqlRet = db.Database.ExecuteSqlCommand(
								"FFS_RemoveEmployee @FundingDateID, @EmployeeID",
								new SqlParameter("FundingDateID", selectedFundingDateId),
								new SqlParameter("EmployeeID", currentSvId)
							);
							if (sqlRet == 0)
								isErr = ProcessErrors(errMsg, userErrMsg, "EditStaffListError");
						} catch (Exception ex) {
							isErr = ProcessErrors(errMsg + " " + ex, userErrMsg, "EditStaffListError");
						}
					}
					transaction.Commit();
				} catch {
					transaction.Rollback();
					throw;
				}
			return IsError(isErr, userErrMsg);
		}
		#endregion

		public bool ProcessErrors(string errMsg, string errMsg2Display2User, string fld2DisplayErr2User) {
			Debug.WriteLine(errMsg);
			ModelState.AddModelError(fld2DisplayErr2User, errMsg2Display2User);
			//throw new System.InvalidOperationException(errMsg2Display2User);
			return true;
		}
	}
}