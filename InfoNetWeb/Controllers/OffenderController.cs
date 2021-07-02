using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Mvc;
using Infonet.Data.Looking;
using Infonet.Data.Models.Offenders;
using Infonet.Web.Mvc;

namespace Infonet.Web.Controllers {
	public class OffenderController : InfonetControllerBase {
		#region Offender Summary Edit
		public ActionResult Edit(int? id) {
			int parentCenterId = Session.Center().Top.Id;

			var offender = id == null ? null : db.T_OffenderList.SingleOrDefault(c => c.OffenderListingId == id);
			if (offender == null)
				return HttpNotFound();

			LoadTiedCases(offender, parentCenterId); //KMS DO put this in a view bag
			return View(offender);
		}

		[HttpPost, ActionName("Edit")]
		[ValidateAntiForgeryToken]
		[PreventDuplicateRequest]
		public ActionResult EditPost(int? id) {
			int parentCenterId = Session.Center().Top.Id;

			var offender = LoadOffender(id) ?? new OffenderListing { ParentCenterId = parentCenterId };
			TryUpdateModel(offender);
			ValidateOffender(offender, parentCenterId);
			if (ModelState.IsValid && Save())
				return RedirectToAction("Edit", new { id = offender.OffenderListingId });

			LoadTiedCases(offender, parentCenterId);
			AddErrorMessage("Errors have prevented the changes from being saved.");
			return View(offender);
		}

		private void ValidateOffender(OffenderListing offender, int parentCenterId) {
			if (ModelState["OffenderCode"].Errors.Count == 0) {
				int matches = db.Database.SqlQuery<int>("SELECT COUNT(*) FROM T_OffenderList WHERE ParentCenterID = @p0 AND OffenderCode = @p1 AND OffenderID <> @p2", parentCenterId, offender.OffenderCode, offender.OffenderListingId).Single();
				if (matches != 0)
					ModelState.AddModelError("OffenderCode", "This Offender ID is already in use.");
			}
		}

		private void LoadTiedCases(OffenderListing offender, int parentCenterId) {
			offender.CasesTiedToThisOffender = (from offenders in db.T_Offender
				join clientcases in db.T_ClientCases on new { offenders.ClientId, offenders.CaseId } equals new { clientcases.ClientId, clientcases.CaseId }
				join clients in db.T_Client on offenders.ClientId equals clients.ClientId
				where offenders.OffenderListingId == offender.OffenderListingId && clients.CenterId == parentCenterId
				select new OffenderListing.ClientCaseTied {
					ClientId = clientcases.ClientId,
					CaseId = clientcases.CaseId,
					ClientCode = clients.ClientCode,
					FirstContactDate = clientcases.FirstContactDate,
					ClientTypeId = clients.ClientTypeId
				}).ToList();
		}

		private OffenderListing LoadOffender(int? offenderId) {
			if (offenderId == null)
				return null;

			return db.T_OffenderList.SingleOrDefault(c => c.OffenderListingId == offenderId);
		}

		private bool Save() {
			try {
				db.SaveChanges();
				AddSuccessMessage("Your changes have been successfully saved.");
				return true;
			} catch (RetryLimitExceededException) {
				ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
				return false;
			}
		}
		#endregion

		#region Offender Search
		public JsonResult GetOffenders(string offenderCode, int? ageFrom, int? ageTo, int? raceId, int? genderIdentityId, int? skip, IList<int?> existingOffenders) {
			int skipAmount = skip ?? 0;
			int parentCenterId = Session.Center().Top.Id;
			var offenders = from offenderListing in db.T_OffenderList
				where offenderListing.ParentCenterId == parentCenterId
				select new {
					offenderListing.OffenderListingId,
					offenderListing.OffenderCode,
					Age = DateTime.Today.Year - offenderListing.BirthYear,
					offenderListing.RaceId,
					GenderIdentityId = offenderListing.SexId
				};

			if (ageFrom != null)
				offenders = offenders.Where(o => ageFrom <= o.Age);
			if (ageTo != null)
				offenders = offenders.Where(o => ageTo >= o.Age);

			if (!string.IsNullOrWhiteSpace(offenderCode))
				offenders = offenders.Where(o => o.OffenderCode.Contains(offenderCode));

			if (raceId != null)
				offenders = offenders.Where(o => o.RaceId == raceId);

			if (genderIdentityId != null)
				offenders = offenders.Where(o => o.GenderIdentityId == genderIdentityId);

			if (existingOffenders != null)
				offenders = offenders.Where(o => !existingOffenders.Contains(o.OffenderListingId));

			int total = offenders.Count(); //KMS DO runs query twice
			var results = offenders.Distinct().OrderBy(o => o.OffenderCode).Skip(skipAmount).Take(50).ToList().Select(o => new { o.OffenderListingId, o.OffenderCode, o.Age, o.RaceId, o.GenderIdentityId, Race = Lookups.Race[o.RaceId].Description, GenderIdentity = Lookups.GenderIdentity[o.GenderIdentityId].Description, total });

			return Json(results, JsonRequestBehavior.AllowGet);
		}
		#endregion
	}
}