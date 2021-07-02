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
using System;

namespace Infonet.Web.Controllers {
	[DenyCoalition]
	[Authorize(Roles = "SYSADMIN, DVADMIN, SAADMIN, CACADMIN")]
	public class FundingSourceLookupController : InfonetControllerBase {
		public ActionResult AddNewRecord() {
			return PartialView("_FundingSourceLookupPartial");
		}

		#region Search/Edit
		public ActionResult Index(FundingSourceLookupViewModel model, int? page) {
			var providerId = Session.Center().Provider.Id();

			int centerId = Session.Center().Id;
			int tableId = GetTableID("TLU_Codes_FundingSource");
			model.CenterID = centerId;
			var results = from fundingSource in db.TLU_Codes_FundingSource
						  join itemAssignment in db.LookupList_ItemAssignment
						  on fundingSource.CodeID equals itemAssignment.CodeId
						  where (fundingSource.CenterID == centerId || fundingSource.CenterID == 0) && itemAssignment.TableId == tableId && itemAssignment.ProviderId == providerId
						  orderby itemAssignment.IsActive descending, itemAssignment.DisplayOrder, fundingSource.Description
						  select new FundingSourceLookupViewModel.FundingSourceResult() {
							  FundingSourceID = fundingSource.CodeID, FundingSourceName = fundingSource.Description, ItemAssignmentID = itemAssignment.Id,
							  CenterID = fundingSource.CenterID, DisplayOrder = itemAssignment.DisplayOrder, IsActive = itemAssignment.IsActive,
							  ICADVAdmin = fundingSource.ICADVAdmin, ICASAAdmin = fundingSource.ICASAAdmin, BeginDate = fundingSource.BeginDate, EndDate = fundingSource.EndDate
						  };
			if (model.FundingSource != null) {
				results = results.Where(a => a.FundingSourceName.Contains(model.FundingSource));
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

			model.FundingSourceList = results.ToPagedList(pageNumber, model.PageSize == -1 ? results.Count() : model.PageSize);
			model.displayForPaging = model.FundingSourceList.ToList();

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
		public ActionResult Index_Post(FundingSourceLookupViewModel model) {
			if (model.displayForPaging == null) {
				AddInfoMessage("No changes were made to the form. Nothing was saved to the database.");
			} else {
				model.displayForPaging?.RemoveAll(m => string.IsNullOrWhiteSpace(m.FundingSourceName) || m.shouldDelete == false && m.shouldEdit == false && m.ItemAssignmentID != null);

				if (model.displayForPaging.Any(m => m.shouldEdit) || model.displayForPaging.Any(m => m.FundingSourceID == null))
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
					return RedirectToAction("Index", "FundingSourceLookup", model);
				}
				if (!HasMessage("Error"))
					AddErrorMessage("Unable to save the changes.");
			}
			return RedirectToAction("Index", "FundingSourceLookup", model);
		}
		#endregion

		#region Private
		private bool UpdateEntities(List<FundingSourceLookupViewModel.FundingSourceResult> FundingSourceList) {
			using (var transaction = db.Database.BeginTransaction()) {
				try { //Check if records marked for deletion are used if yes add them to the error message
					string message = "Unable to delete the following because of a reference by other records in the system." + "<ul>";
					bool someReferenced = false;
					foreach (var record in FundingSourceList.Where(a => a.shouldDelete).ToList()) {
						if (record.hasReference) {
							message = message + "<li><b>" + record.FundingSourceName.Replace("'", "&apos;") + "</b></li>";
						}
						someReferenced = someReferenced || record.hasReference;
					}
					if (someReferenced) {
						message = message + "</ul>" + "Remove it from the other records first to delete it from the list. Else mark it inactive to remove it from drop-down lists.";
						AddErrorMessage(message);
					}

					//Do the CUD operations 
					foreach (var record in FundingSourceList.ToList()) {
						var originalFundingSourceRecord = db.TLU_Codes_FundingSource.FirstOrDefault(t => t.CodeID == record.FundingSourceID);
						var currentFundingSource = CreateNewFundingSourceEntity(record);
						record.thisFundingSource = currentFundingSource;

						if (originalFundingSourceRecord != null) {
							if (record.shouldEdit && (!originalFundingSourceRecord.Description.Equals(currentFundingSource.Description) || !originalFundingSourceRecord.EndDate.Equals(currentFundingSource.EndDate))) {
								db.Entry(originalFundingSourceRecord).State = EntityState.Detached;
								db.TLU_Codes_FundingSource.Attach(currentFundingSource);
								db.Entry(currentFundingSource).State = EntityState.Modified;
							}

						} else if (!record.alreadyExists && !record.isDuplicate) {
							db.TLU_Codes_FundingSource.Add(currentFundingSource);
							db.Entry(currentFundingSource).State = EntityState.Added;
						}

						if (record.shouldDelete && !record.hasReference)
							db.TLU_Codes_FundingSource.Remove(originalFundingSourceRecord);
					}
					db.SaveChanges();

					foreach (var record in FundingSourceList.ToList()) {
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

		private void ValidateItemsForExistance(List<FundingSourceLookupViewModel.FundingSourceResult> list) {
			var noDuplicates = new HashSet<string>();
			string messageForDuplicates = "It looks like you tried to add the following more than once." + "<ul>";

			int centerId = Session.Center().Id;
			bool hasDataBaseReference = false, duplicateAddsOccured = false;
			string message = "Unable to add the following because they already exist in the system." + "<ul>";

			foreach (var each in list) {
				string currentFundingSourceName = each.FundingSourceName.ToLower();

				bool addedMoreThanOnce = list.Count(l => currentFundingSourceName == l.FundingSourceName.ToLower()) > 1;
				if (addedMoreThanOnce && !noDuplicates.Contains(currentFundingSourceName) && !each.shouldDelete) {
					duplicateAddsOccured = true;
					messageForDuplicates = messageForDuplicates + "<li><b>" + each.FundingSourceName.Replace("'", "&apos;") + "</b></li>";
				} else if (addedMoreThanOnce && noDuplicates.Contains(currentFundingSourceName)) {
					each.isDuplicate = true;
				}
				noDuplicates.Add(currentFundingSourceName);
			}

			list.RemoveAll(l => l.isDuplicate);

			foreach (FundingSourceLookupViewModel.FundingSourceResult each in list) {
				string currentFundingSourceName = each.FundingSourceName.ToLower();
				bool isPresent = db.TLU_Codes_FundingSource.Any(p => currentFundingSourceName == p.Description.ToLower() && (p.CenterID == centerId || p.CenterID == 0));
				string thisFundSourceName = "";
				int? thisFundSourceId = each.FundingSourceID;
				if (each.FundingSourceID != null)
					thisFundSourceName = db.TLU_Codes_FundingSource.Where(a => thisFundSourceId == a.CodeID).Select(a => a.Description).Single();
				each.alreadyExists = isPresent;
				if (isPresent && (each.FundingSourceID == null || each.shouldEdit) && !string.Equals(each.FundingSourceName, thisFundSourceName)) {
					message = message + "<li><b>" + each.FundingSourceName.Replace("'", "&apos;") + "</b></li>";
					hasDataBaseReference = hasDataBaseReference || isPresent;
				}
			}
			if (hasDataBaseReference)
				AddErrorMessage(message);

			if (duplicateAddsOccured)
				AddErrorMessage(messageForDuplicates);
		}

		private void MarkReferencedEntities(List<FundingSourceLookupViewModel.FundingSourceResult> displayForPaging) {
			foreach (var record in displayForPaging) {
				bool recordHasReference = db.Tl_FundServiceProgramOfStaffs.Any(t => record.FundingSourceID == t.FundingSourceID) || db.Ts_CenterAdminFundingSources.Any(t => record.FundingSourceID == t.CodeID);
				record.hasReference = recordHasReference;
			}
		}

		private bool IsSameItem(LookupListItemAssignment originalItem, LookupListItemAssignment currentItem) {
			return originalItem.IsActive == currentItem.IsActive &&
					originalItem.ProviderId == currentItem.ProviderId &&
					originalItem.TableId == currentItem.TableId &&
					originalItem.DisplayOrder == currentItem.DisplayOrder;
		}

		private LookupListItemAssignment CreateNewLookpListItem(FundingSourceLookupViewModel.FundingSourceResult record) {
			return new LookupListItemAssignment {
				Id = record.ItemAssignmentID,
				CodeId = record.thisFundingSource.CodeID,
				DisplayOrder = record.DisplayOrder ?? 0,
				IsActive = record.IsActive,
				ProviderId = Session.Center().Provider.Id(),
				TableId = GetTableID("TLU_Codes_FundingSource")
			};
		}

		private TLU_Codes_FundingSource CreateNewFundingSourceEntity(FundingSourceLookupViewModel.FundingSourceResult record) {
			return new TLU_Codes_FundingSource {
				CenterID = record.CenterID ?? Session.Center().Id,
				CodeID = record.FundingSourceID,
				Description = record.FundingSourceName,
				BeginDate = record.BeginDate,
				EndDate = record.EndDate,
				ICASAAdmin = record.ICASAAdmin,
				ICADVAdmin = record.ICADVAdmin
			};
		}

		private int GetTableID(string tableName) {
			return db.LookupList_Tables.Where(l => l.TableName == tableName).Select(l => l.TableId).FirstOrDefault();
		}

		#endregion
	}
}