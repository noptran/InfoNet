using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Infonet.Usps.Data;
using Infonet.Web.Mvc;
using Infonet.Web.ViewModels.Admin;

namespace Infonet.Web.Controllers {
	[Authorize(Roles = "DVADMIN, SAADMIN, CACADMIN")]
	public class CenterInformationController : InfonetControllerBase {
		public ActionResult Index() {
			int centerId = Session.Center().Id;

			var model = new CenterInfoViewModel { Center = db.T_Center.Single(m => m.CenterID == centerId) };
			using (var usps = new UspsContext()) {
				model.States = usps.States.Where(s => s.ZipCodes.Any(z => z.Zipcode == model.Center.Zipcode)).ToList();
				model.Counties = usps.Counties.Where(c => c.ZipCodes.Any(z => z.Zipcode == model.Center.Zipcode) && c.States.Any(s => s.ID == model.Center.StateID)).ToList();
				model.Cities = usps.Cities.Where(c => c.ZipCodes.Any(z => z.Zipcode == model.Center.Zipcode) && c.States.Any(s => s.ID == model.Center.StateID) && c.Counties.Any(c2 => c2.ID == model.Center.CountyID)).ToList();
			}

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[PreventDuplicateRequest]
		public ActionResult Index(CenterInfoViewModel model) {
			if (ModelState.IsValid) {
				UpdateCenterInformation(model);
				return RedirectToAction("Index");
			}

			using (var usps = new UspsContext()) {
				model.States = usps.States.Where(s => s.ZipCodes.Any(z => z.Zipcode == model.Center.Zipcode)).ToList();
				model.Counties = usps.Counties.Where(c => c.ZipCodes.Any(z => z.Zipcode == model.Center.Zipcode) && c.States.Any(s => s.ID == model.Center.StateID)).ToList();
				model.Cities = usps.Cities.Where(c => c.ZipCodes.Any(z => z.Zipcode == model.Center.Zipcode) && c.States.Any(s => s.ID == model.Center.StateID) && c.Counties.Any(c2 => c2.ID == model.Center.CountyID)).ToList();
			}

			return View(model);
		}

		private void UpdateCenterInformation(CenterInfoViewModel model) {
			int centerId = Session.Center().Id;
			var originalCenterRecord = db.T_Center.Single(m => m.CenterID == centerId);

#pragma warning disable 612
			model.Center.CenterID = centerId;
			model.Center.CenterName = originalCenterRecord.CenterName;
			model.Center.City = originalCenterRecord.City;
			model.Center.CreationDate = originalCenterRecord.CreationDate;
			model.Center.ProviderID = originalCenterRecord.ProviderID;
			model.Center.AgeAnnBudget = originalCenterRecord.AgeAnnBudget;
			model.Center.ProgAnnBudget = originalCenterRecord.ProgAnnBudget;
			model.Center.IsRealCenter = originalCenterRecord.IsRealCenter;
			model.Center.DirectorEmail = originalCenterRecord.DirectorEmail;
			model.Center.TerminationDate = originalCenterRecord.TerminationDate;
			model.Center.ShelterStatus = originalCenterRecord.ShelterStatus;
            model.Center.ParentCenterID = originalCenterRecord.ParentCenterID;
#pragma warning restore 612

			db.Entry(originalCenterRecord).State = EntityState.Detached;
			db.T_Center.Attach(model.Center);
			db.Entry(model.Center).State = originalCenterRecord.IsUnchanged(model.Center) ? EntityState.Unchanged : EntityState.Modified;

			db.SaveChanges();

			AddSuccessMessage("You have successfully altered the Center Information!");
		}
	}
}