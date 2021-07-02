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
	[Authorize(Roles = "SYSADMIN, CACADMIN")]
	public class ContactLookupController : InfonetControllerBase {
		public ActionResult AddNewRecord() {
			return PartialView("_ContactLookupPartial");
		}

		#region Search/Edit
		public ActionResult Index(ContactLookupViewModel model, int? page) {
			int centerId = Session.Center().Id;
			int tableId = GetTableID("T_Contact");
			model.CenterID = centerId;
			var results = from contact in db.T_Contact
						  join itemAssignment in db.LookupList_ItemAssignment
						  on contact.ContactId equals itemAssignment.CodeId
						  where (contact.CenterId == centerId || contact.CenterId == 0) && itemAssignment.TableId == tableId
						  orderby itemAssignment.IsActive descending, itemAssignment.DisplayOrder, contact.ContactName
						  select new ContactLookupViewModel.ContactResultListItem() {
							  ContactID = contact.ContactId, ContactName = contact.ContactName, ItemAssignmentID = itemAssignment.Id,
							  CenterID = contact.CenterId, DisplayOrder = itemAssignment.DisplayOrder, IsActive = itemAssignment.IsActive
						  };
			if (model.ContactName != null) {
				results = results.Where(a => a.ContactName.Contains(model.ContactName));
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

			model.ContactList = results.ToPagedList(pageNumber, model.PageSize == -1 ? results.Count() : model.PageSize);
			model.displayForPaging = model.ContactList.ToList();

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
		public ActionResult Index_Post(ContactLookupViewModel model) {
			if (model.displayForPaging == null) {
				AddInfoMessage("No changes were made to the form. Nothing was saved to the database.");
			} else {
				model.displayForPaging?.RemoveAll(m => string.IsNullOrWhiteSpace(m.ContactName) || m.shouldDelete == false && m.shouldEdit == false && m.ItemAssignmentID != null);

				if (model.displayForPaging.Any(m => m.shouldEdit) || model.displayForPaging.Any(m => m.ContactID == null))
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
					return RedirectToAction("Index", "ContactLookup", model);
				}
				if (!HasMessage("Error"))
					AddErrorMessage("Unable to save the changes.");
			}
			return RedirectToAction("Index", "ContactLookup", model);
		}
		#endregion

		#region Private
		private bool UpdateEntities(List<ContactLookupViewModel.ContactResultListItem> ContactList) {
			using (var transaction = db.Database.BeginTransaction()) {
				try { //Check if records marked for deletion are used if yes add them to the error message
					string message = "Unable to delete the following because of a reference by other records in the system." + "<ul>";
					bool someReferenced = false;
					foreach (var record in ContactList.Where(a => a.shouldDelete).ToList()) {
						if (record.hasReference) {
							message = message + "<li><b>" + record.ContactName.Replace("'", "&apos;") + "</b></li>";
						}
						someReferenced = someReferenced || record.hasReference;
					}
					if (someReferenced) {
						message = message + "</ul>" + "Remove it from the other records first to delete it from the list. Else mark it inactive to remove it from drop-down lists.";
						AddErrorMessage(message);
					}

					//Do the CUD operations 
					foreach (var record in ContactList.ToList()) {
						var originalContactRecord = db.T_Contact.FirstOrDefault(t => t.ContactId == record.ContactID);
						var currentContact = CreateNewContactEntity(record);
						record.thisContact = currentContact;

						if (originalContactRecord != null) {
							if (record.shouldEdit && !originalContactRecord.ContactName.Equals(currentContact.ContactName)) {
								db.Entry(originalContactRecord).State = EntityState.Detached;
								db.T_Contact.Attach(currentContact);
								db.Entry(currentContact).State = EntityState.Modified;
							}

						} else if (!record.alreadyExists && !record.isDuplicate) {
							db.T_Contact.Add(currentContact);
							db.Entry(currentContact).State = EntityState.Added;
						}

						if (record.shouldDelete && !record.hasReference)
							db.T_Contact.Remove(originalContactRecord);
					}
					db.SaveChanges();

					foreach (var record in ContactList.ToList()) {
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

		private void ValidateItemsForExistance(List<ContactLookupViewModel.ContactResultListItem> list) {
			var duplicates = new HashSet<string>();
			string messageForDuplicates = "It looks like you tried to add the following more than once." + "<ul>";

			int centerId = Session.Center().Id;
			bool hasDataBaseReference = false, duplicateAddsOccured = false;
			string message = "Unable to add the following because they already exist in the system." + "<ul>";

			foreach (var each in list) {
				string currentContactName = each.ContactName.ToLower();

				bool addedMoreThanOnce = list.Count(l => currentContactName == l.ContactName.ToLower()) > 1;
				if (addedMoreThanOnce && !duplicates.Contains(currentContactName) && !each.shouldDelete) {
					duplicateAddsOccured = true;
					messageForDuplicates = messageForDuplicates + "<li><b>" + each.ContactName.Replace("'", "&apos;") + "</b></li>";
				} else if (addedMoreThanOnce && duplicates.Contains(currentContactName)) {
					each.isDuplicate = true;
				}
				duplicates.Add(currentContactName);
			}

			list.RemoveAll(l => l.isDuplicate);

			foreach (var each in list) {
				string currentContactName = each.ContactName.ToLower();
				bool isPresent = db.T_Contact.Any(p => currentContactName == p.ContactName.ToLower() && p.CenterId == centerId);
				each.alreadyExists = isPresent;
				if (isPresent && each.ContactID == null) {
					message = message + "<li><b>" + each.ContactName.Replace("'", "&apos;") + "</b></li>";
					hasDataBaseReference = hasDataBaseReference || isPresent;
				}
			}
			if (hasDataBaseReference)
				AddErrorMessage(message);

			if (duplicateAddsOccured)
				AddErrorMessage(messageForDuplicates);
		}

		private void MarkReferencedEntities(List<ContactLookupViewModel.ContactResultListItem> displayForPaging) {
			foreach (var record in displayForPaging) {
				var originalContactRecord = db.T_Contact.FirstOrDefault(t => record.ContactID == t.ContactId);
				record.hasReference = originalContactRecord != null && (originalContactRecord.ClientMDTs.Any() || originalContactRecord.VSIObservers.Any()/* || originalContactRecord.VictimSensitiveInterview.Any()*/);
			}
		}

		private Contact CreateNewContactEntity(ContactLookupViewModel.ContactResultListItem record) {
			return new Contact {
				CenterId = record.CenterID ?? Session.Center().Id,
				ContactId = record.ContactID,
				ContactName = record.ContactName
			};
		}

		private LookupListItemAssignment CreateNewLookpListItem(ContactLookupViewModel.ContactResultListItem record) {
			return new LookupListItemAssignment {
				Id = record.ItemAssignmentID,
				CodeId = record.thisContact.ContactId,
				DisplayOrder = record.DisplayOrder ?? 0,
				IsActive = record.IsActive,
				ProviderId = Session.Center().Provider.Id(),
				TableId = GetTableID("T_Contact")
			};
		}

		private bool IsSameItem(LookupListItemAssignment originalItem, LookupListItemAssignment currentItem) {
			return originalItem.IsActive == currentItem.IsActive &&
					originalItem.ProviderId == currentItem.ProviderId &&
					originalItem.TableId == currentItem.TableId &&
					originalItem.DisplayOrder == currentItem.DisplayOrder;
		}

		private int GetTableID(string tableName) {
			return db.LookupList_Tables.Where(l => l.TableName == tableName).Select(l => l.TableId).FirstOrDefault();
		}
		#endregion
	}
}