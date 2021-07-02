using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Infonet.Data.Looking;
using Infonet.Data.Models.Looking;
using Infonet.Data.Models._TLU;
using Infonet.Web.Mvc;
using Infonet.Web.Mvc.Authorization;
using Infonet.Web.ViewModels.Admin;
using PagedList;

namespace Infonet.Web.Controllers {
	[DenyCoalition]
	[Authorize(Roles = "SYSADMIN, DVADMIN, SAADMIN, CACADMIN")]
	public class OtherStaffActivityLookupController : InfonetControllerBase {
		public ActionResult AddNewRecord() {
			return PartialView("_OSALookupPartial");
		}

		#region Search/Edit
		public ActionResult Index(OtherStaffActivityLookUpViewModel model, int? page) {
			int providerId = Session.Center().ProviderId;
			int centerId = Session.Center().Id;
			int tableId = GetTableID("TLU_Codes_OtherStaffActivity");
			model.CenterID = centerId;
			var results = from activity in db.TLU_Codes_OtherStaffActivity
				join itemAssignment in db.LookupList_ItemAssignment
				on activity.CodeID equals itemAssignment.CodeId
				where (activity.CenterID == centerId || activity.CenterID == 0) && itemAssignment.TableId == tableId && itemAssignment.ProviderId == providerId
				orderby itemAssignment.IsActive descending, itemAssignment.DisplayOrder, activity.Description
				select new OtherStaffActivityLookUpViewModel.OtherStaffActivityResultList() {
					CodeID = activity.CodeID, ActivityName = activity.Description, ItemAssignmentID = itemAssignment.Id,
					CenterID = activity.CenterID, DisplayOrder = itemAssignment.DisplayOrder, IsActive = itemAssignment.IsActive
				};

			if (!string.IsNullOrEmpty(model.ActivityName) && !string.IsNullOrWhiteSpace(model.ActivityName)) {
				results = results.Where(a => a.ActivityName.Contains(model.ActivityName));
			}

			if (model.IsActive != null) {
				results = results.Where(a => a.IsActive == model.IsActive);
			}

			if (model.DisplayOrder != null) {
				results = results.Where(a => a.DisplayOrder == model.DisplayOrder);
			}

			int pageNumber = page ?? (model.PageNumber ?? 1);
			model.PageNumber = pageNumber;
			model.RecordCount = results.Count();

			model.OtherStaffActivityList = results.ToPagedList(pageNumber, model.PageSize == -1 ? results.Count() : model.PageSize);
			model.displayForPaging = model.OtherStaffActivityList.ToList();

			MarkReferencedEntities(model.displayForPaging);
			return View(model);
		}

		[HttpPost]
		public ActionResult Index() {
			ModelState.Clear();
			return RedirectToAction("Index");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[PreventDuplicateRequest]
		public ActionResult Index_Post(OtherStaffActivityLookUpViewModel model) {
			if (model.displayForPaging == null) {
				AddInfoMessage("No changes were made to the form. Nothing was saved to the database.");
			} else {
				model.displayForPaging?.RemoveAll(m => string.IsNullOrWhiteSpace(m.ActivityName) || m.shouldDelete == false && m.shouldEdit == false && m.ItemAssignmentID != null);

				if (model.displayForPaging.Any(m => m.shouldEdit) || model.displayForPaging.Any(m => m.CodeID == null))
					ValidateItemsForExistance(model.displayForPaging.ToList());

				if (ModelState.IsValid) {
					if (model.displayForPaging.Count != 0) {
						bool saved = UpdateEntities(model.displayForPaging);
						if (saved) {
							if (!HasMessage("Error"))
								AddSuccessMessage("Changes have been successfully saved.");
						} else {
							if (!HasMessage("Error"))
								AddErrorMessage("Unable to save the changes. If you are editing make sure the edited activity does not already exist.");
						}
					} else {
						AddInfoMessage("No changes were made to the form. Nothing was saved to the database.");
					}
					return RedirectToAction("Index", "OtherStaffActivityLookup", model);
				}
				if (!HasMessage("Error"))
					AddErrorMessage("Unable to save the changes.");
			}
			return RedirectToAction("Index", "OtherStaffActivityLookup", model);
		}
		#endregion

		#region Private
		private bool UpdateEntities(List<OtherStaffActivityLookUpViewModel.OtherStaffActivityResultList> ActivityList) {
			using (var transaction = db.Database.BeginTransaction()) {
				try {//Check if records marked for deletion are used if yes add them to the error message
					string message = "Unable to delete the following because of a reference by other records in the system." + "<ul>";
					bool someReferenced = false;
					foreach (var record in ActivityList.Where(a => a.shouldDelete).ToList()) {
						if (record.hasReference) {
							message = message + "<li><b>" + record.ActivityName.Replace("'", "&apos;") + "</b></li>";
						}
						someReferenced = someReferenced || record.hasReference;
					}
					if (someReferenced) {
						message = message + "</ul>" + "Remove it from the other records first to delete it from the list. Else mark it inactive to remove it from drop-down lists.";
						AddErrorMessage(message);
					}

					//Do the CUD operations 
					foreach (var record in ActivityList.ToList()) {
						var originalActivityRecord = db.TLU_Codes_OtherStaffActivity.FirstOrDefault(t => t.CodeID == record.CodeID);
						var currentActivity = CreateNewStaffActivityEntity(record);
						record.Activity = currentActivity;

						if (originalActivityRecord != null) {
							if (record.shouldEdit && !originalActivityRecord.Description.Equals(currentActivity.Description)) {
								db.Entry(originalActivityRecord).State = EntityState.Detached;
								db.TLU_Codes_OtherStaffActivity.Attach(currentActivity);
								db.Entry(currentActivity).State = EntityState.Modified;
							}

						} else if (!record.alreadyExists && !record.isDuplicate) {
							db.TLU_Codes_OtherStaffActivity.Add(currentActivity);
							db.Entry(currentActivity).State = EntityState.Added;
						}

						if (record.shouldDelete && !record.hasReference)
							db.TLU_Codes_OtherStaffActivity.Remove(originalActivityRecord);
					}
					db.SaveChanges();

					foreach (var record in ActivityList.ToList()) {
						var originalItem = db.LookupList_ItemAssignment.FirstOrDefault(t => t.Id == record.ItemAssignmentID);
						var currentItem = CreateNewLookpListItem(record);

						if (originalItem != null) {
							if (record.shouldEdit && !IsSameItem(originalItem, currentItem)) {
								db.Entry(originalItem).State = EntityState.Detached;
								db.LookupList_ItemAssignment.Attach(currentItem);
								db.Entry(currentItem).State = EntityState.Modified;
							}
						} else if (!record.alreadyExists && !record.isDuplicate) {
							db.LookupList_ItemAssignment.Add(currentItem);
							db.Entry(currentItem).State = EntityState.Added;
						}

						if (record.shouldDelete && !record.hasReference)
							db.LookupList_ItemAssignment.Remove(originalItem);
					}
					db.SaveChanges();

					transaction.Commit();
					return !someReferenced;
				} catch (Exception e) {
					transaction.Rollback();
					return false;
				}
			}
		}

		private void ValidateItemsForExistance(List<OtherStaffActivityLookUpViewModel.OtherStaffActivityResultList> list) {
			var noDuplicates = new HashSet<string>();
			string messageForDuplicates = "It looks like you tried to add the following more than once." + "<ul>";

			int centerId = Session.Center().Id;
			bool hasDataBaseReference = false, duplicateAddsOccured = false;
			string message = "Unable to add the following because they already exist in the system." + "<ul>";

			foreach (var each in list) {
				string currentFundingSourceName = each.ActivityName.ToLower();

				bool addedMoreThanOnce = list.Count(l => currentFundingSourceName == l.ActivityName.ToLower()) > 1;
				if (addedMoreThanOnce && !noDuplicates.Contains(currentFundingSourceName) && !each.shouldDelete) {
					duplicateAddsOccured = true;
					messageForDuplicates = messageForDuplicates + "<li><b>" + each.ActivityName.Replace("'", "&apos;") + "</b></li>";
				} else if (addedMoreThanOnce && noDuplicates.Contains(currentFundingSourceName)) {
					each.isDuplicate = true;
				}
				noDuplicates.Add(currentFundingSourceName);
			}

			list.RemoveAll(l => l.isDuplicate);

			foreach (var each in list) {
				string currentActivityName = each.ActivityName.ToLower();
				bool isPresent = db.TLU_Codes_OtherStaffActivity.Any(p => currentActivityName == p.Description.ToLower() && (p.CenterID == centerId || p.CenterID == 0));
				each.alreadyExists = isPresent;
				string thisActivityName = "";
				int? thisActivityId = each.CodeID;


				if (each.CodeID != null)
					thisActivityName = db.TLU_Codes_OtherStaffActivity.Where(a => thisActivityId == a.CodeID).Select(a => a.Description).Single();
				if (isPresent && (each.CodeID == null || each.shouldEdit) && !string.Equals(each.ActivityName, thisActivityName)) {
					message = message + "<li><b>" + each.ActivityName.Replace("'", "&apos;") + "</b></li>";
					hasDataBaseReference = hasDataBaseReference || isPresent;
				}
			}
			if (hasDataBaseReference)
				AddErrorMessage(message);

			if (duplicateAddsOccured)
				AddErrorMessage(messageForDuplicates);
		}

		private void MarkReferencedEntities(List<OtherStaffActivityLookUpViewModel.OtherStaffActivityResultList> displayForPaging) {
			foreach (var record in displayForPaging) {
				var originalActivityRecord = db.TLU_Codes_OtherStaffActivity.FirstOrDefault(t => record.CodeID == t.CodeID);
				bool recordHasReference = originalActivityRecord != null && originalActivityRecord.OtherStaffActivities.Any();
				if (recordHasReference)
					record.hasReference = true;
			}
		}

		private bool IsSameItem(LookupListItemAssignment originalItem, LookupListItemAssignment currentItem) {
			return originalItem.IsActive == currentItem.IsActive &&
					originalItem.ProviderId == currentItem.ProviderId &&
					originalItem.TableId == currentItem.TableId &&
					originalItem.DisplayOrder == currentItem.DisplayOrder;
		}

		private LookupListItemAssignment CreateNewLookpListItem(OtherStaffActivityLookUpViewModel.OtherStaffActivityResultList record) {
			return new LookupListItemAssignment {
				Id = record.ItemAssignmentID,
				CodeId = record.Activity.CodeID,
				DisplayOrder = record.DisplayOrder ?? 0,
				IsActive = record.IsActive,
				ProviderId = Session.Center().Provider.Id(),
				TableId = GetTableID("TLU_Codes_OtherStaffActivity")
			};
		}

		private TLU_Codes_OtherStaffActivity CreateNewStaffActivityEntity(OtherStaffActivityLookUpViewModel.OtherStaffActivityResultList record) {
			return new TLU_Codes_OtherStaffActivity {
				CenterID = record.CenterID ?? Session.Center().Id,
				CodeID = record.CodeID,
				Description = record.ActivityName
			};
		}

		private int GetTableID(string tableName) {
			return db.LookupList_Tables.Where(l => l.TableName == tableName).Select(l => l.TableId).FirstOrDefault();
		}
		#endregion
	}
}