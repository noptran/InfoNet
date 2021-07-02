using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Infonet.Data.Looking;
using Infonet.Data.Models.Centers;
using Infonet.Data.Models.Looking;
using Infonet.Web.Mvc;
using Infonet.Web.Mvc.Authorization;
using Infonet.Web.ViewModels.Admin;
using PagedList;

namespace Infonet.Web.Controllers {
	[DenyCoalition]
	[Authorize(Roles = "SYSADMIN, DVADMIN, SAADMIN, CACADMIN")]
	public class AgencyLookupController : InfonetControllerBase {
		public ActionResult AddNewRecord() {
			return PartialView("_AgencyLookupPartial");
		}

		#region Search/Edit
		public ActionResult Index(AgenciesLookupViewModel model, int? page) {
			int centerId = Session.Center().Id;
			int tableId = GetTableID("T_Agency");
			model.CenterID = centerId;
			var results = from agency in db.T_Agency
				join itemAssignment in db.LookupList_ItemAssignment
				on agency.AgencyID equals itemAssignment.CodeId
				where (agency.CenterID == centerId || agency.CenterID == 0) && itemAssignment.TableId == tableId
				orderby itemAssignment.IsActive descending, itemAssignment.DisplayOrder, agency.AgencyName
				select new AgenciesLookupViewModel.AgencyResultList() {
					AgencyID = agency.AgencyID, AgencyName = agency.AgencyName, ItemAssignmentID = itemAssignment.Id,
					CenterID = agency.CenterID, DisplayOrder = itemAssignment.DisplayOrder, IsActive = itemAssignment.IsActive
				};
			if (model.AgencyName != null) {
				results = results.Where(a => a.AgencyName.Contains(model.AgencyName));
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

			model.AgencyList = results.ToPagedList(pageNumber, model.PageSize == -1 ? results.Count() : model.PageSize);
			model.displayForPaging = model.AgencyList.ToList();

			MarkReferencedEntities(model.displayForPaging);
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[PreventDuplicateRequest]
		public ActionResult Index() {
			ModelState.Clear();
			return RedirectToAction("Index");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[PreventDuplicateRequest]
		public ActionResult Index_Post(AgenciesLookupViewModel model) {
			if (model.displayForPaging == null) {
				AddInfoMessage("No changes were made to the form. Nothing was saved to the database.");
			} else {
				model.displayForPaging?.RemoveAll(m => string.IsNullOrWhiteSpace(m.AgencyName) || m.shouldDelete == false && m.shouldEdit == false && m.ItemAssignmentID != null);

				if (model.displayForPaging.Any(m => m.shouldEdit) || model.displayForPaging.Any(m => m.AgencyID == null))
					ValidateItemsForExistance(model.displayForPaging.ToList());

				if (ModelState.IsValid) {
					if (model.displayForPaging.Count != 0) {
						bool saved = UpdateEntities(model.displayForPaging);
						if (saved) {
							if (!HasMessage("Error"))
								AddSuccessMessage("Changes have been successfully saved.");
						} else {
							if (!HasMessage("Error"))
								AddErrorMessage("Unable to save the changes.");
						}
					} else {
						AddInfoMessage("No changes were made to the form. Nothing was saved to the database.");
					}
					return RedirectToAction("Index", "AgencyLookup", model);
				}
				if (!HasMessage("Error"))
					AddErrorMessage("Unable to save the changes.");
			}
				return RedirectToAction("Index", "AgencyLookup", model);
			}
		#endregion

		#region Private
		private bool UpdateEntities(List<AgenciesLookupViewModel.AgencyResultList> AgencyList) {
			using (var transaction = db.Database.BeginTransaction()) {
				try { //Check if records marked for deletion are used if yes add them to the error message
					string message = "Unable to delete the following because of a reference by other records in the system." + "<ul>";
					bool someReferenced = false;
					foreach (var record in AgencyList.Where(a => a.shouldDelete).ToList()) {
						if (record.hasReference) {
							message = message + "<li><b>" + record.AgencyName.Replace("'", "&apos;") + "</b></li>";
						}
						someReferenced = someReferenced || record.hasReference;
					}
					if (someReferenced) {
						message = message + "</ul>" + "Remove it from the other records first to delete it from the list. Else mark it inactive to remove it from drop-down lists.";
						AddErrorMessage(message);
					}

					//Do the CUD operations 
					foreach (var record in AgencyList.ToList()) {
						var originalAgencyRecord = db.T_Agency.FirstOrDefault(t => t.AgencyID == record.AgencyID);
						var currentAgency = CreateNewAgencyEntity(record);
						record.thisAgency = currentAgency;

						if (originalAgencyRecord != null) {
							if (record.shouldEdit && !originalAgencyRecord.AgencyName.Equals(currentAgency.AgencyName)) {
								db.Entry(originalAgencyRecord).State = EntityState.Detached;
								db.T_Agency.Attach(currentAgency);
								db.Entry(currentAgency).State = EntityState.Modified;
							}

						} else if (!record.alreadyExists && !record.isDuplicate) {
							db.T_Agency.Add(currentAgency);
							db.Entry(currentAgency).State = EntityState.Added;
						}

						if (record.shouldDelete && !record.hasReference)
							db.T_Agency.Remove(originalAgencyRecord);
					}
					db.SaveChanges();

					foreach (var record in AgencyList.ToList()) {
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
				} catch {
					transaction.Rollback();
					return false;
				}
			}
		}

		private void ValidateItemsForExistance(List<AgenciesLookupViewModel.AgencyResultList> list) {
			HashSet<string> duplicates = new HashSet<string>();
			string messageForDuplicates = "It looks like you tried to add the following more than once." + "<ul>";

			int centerId = Session.Center().Id;
			bool hasDataBaseReference = false, duplicateAddsOccured = false;
			string message = "Unable to add the following because they already exist in the system." + "<ul>";

			for (int i = 0; i < list.Count; i++) {
				string currentAgencyName = list[i].AgencyName.ToLower();

				bool addedMoreThanOnce = list.Count(l => currentAgencyName == l.AgencyName.ToLower()) > 1;
				if (addedMoreThanOnce && !duplicates.Contains(currentAgencyName) && !list[i].shouldDelete) {
					duplicateAddsOccured = true;
					messageForDuplicates = messageForDuplicates + "<li><b>" + list[i].AgencyName.Replace("'", "&apos;") + "</b></li>";
				} else if (addedMoreThanOnce && duplicates.Contains(currentAgencyName)) {
					list[i].isDuplicate = true;
				}
				duplicates.Add(currentAgencyName);
			}

			list.RemoveAll(l => l.isDuplicate);

			for (int i = 0; i < list.Count; i++) {
				string currentAgencyName = list[i].AgencyName.ToLower();
				bool isPresent = db.T_Agency.Any(p => currentAgencyName == p.AgencyName.ToLower() && (p.CenterID == centerId || p.CenterID == 0));
				string thisAgencyName = "";
				int? thisAgencyId = list[i].AgencyID;

				list[i].alreadyExists = isPresent;
				if (list[i].AgencyID != null)
					thisAgencyName = db.T_Agency.Where(a => thisAgencyId == a.AgencyID).Select(a => a.AgencyName).Single();
				if (isPresent && (list[i].AgencyID == null || list[i].shouldEdit) && !string.Equals(list[i].AgencyName, thisAgencyName)) {
					message = message + "<li><b>" + list[i].AgencyName.Replace("'", "&apos;") + "</b></li>";
					hasDataBaseReference = hasDataBaseReference || isPresent;
					list[i].shouldEdit = false; //a record that was marked for edit and but the equivalant already exist in the db
				}
			}
			if (hasDataBaseReference)
				AddErrorMessage(message);

			if (duplicateAddsOccured)
				AddErrorMessage(messageForDuplicates);
		}

		private void MarkReferencedEntities(List<AgenciesLookupViewModel.AgencyResultList> displayForPaging) {
			foreach (var record in displayForPaging) {
				var originalAgencyRecord = db.T_Agency.FirstOrDefault(t => record.AgencyID == t.AgencyID);
				bool recordHasReference = originalAgencyRecord != null && (originalAgencyRecord.ClientReferralDetails.Any() || originalAgencyRecord.ClientReferralSources.Any() || originalAgencyRecord.ClientCJProcesses.Any() || originalAgencyRecord.ClientMDTs.Any() || originalAgencyRecord.ProgramDetails.Any() || originalAgencyRecord.ClientMDTs.Any());
				record.hasReference = recordHasReference;
			}
		}

		private LookupListItemAssignment CreateNewLookpListItem(AgenciesLookupViewModel.AgencyResultList record) {
			LookupListItemAssignment currentItem = new LookupListItemAssignment();

			currentItem.Id = record.ItemAssignmentID;
			currentItem.CodeId = record.thisAgency.AgencyID;
			currentItem.DisplayOrder = record.DisplayOrder ?? 0;
			currentItem.IsActive = record.IsActive;
			currentItem.ProviderId = Session.Center().Provider.Id();
			currentItem.TableId = GetTableID("T_Agency");

			return currentItem;
		}

		private Agency CreateNewAgencyEntity(AgenciesLookupViewModel.AgencyResultList record) {
			Agency currentAgency = new Agency();

			currentAgency.CenterID = record.CenterID ?? Session.Center().Id;
			currentAgency.AgencyID = record.AgencyID;
			currentAgency.AgencyName = record.AgencyName;

			return currentAgency;
		}

		private bool IsSameItem(LookupListItemAssignment originalItem, LookupListItemAssignment currentItem) {
			return (originalItem.IsActive == currentItem.IsActive &&
					originalItem.ProviderId == currentItem.ProviderId &&
					originalItem.TableId == currentItem.TableId &&
					originalItem.DisplayOrder == currentItem.DisplayOrder);
		}

		private int GetTableID(string tableName) {
			return db.LookupList_Tables.Where(l => l.TableName == tableName).Select(l => l.TableId).FirstOrDefault();
		}
		#endregion
	}
}