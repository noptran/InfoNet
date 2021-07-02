using System.Web.Mvc;
using Infonet.Usps.Data.Helpers;

namespace Infonet.Web.Controllers {
	[Authorize]
	public class USPSController : Controller {
		private readonly UspsHelper usps = new UspsHelper();

		protected override void Dispose(bool disposing) {
			if (disposing)
				usps.Dispose();
			base.Dispose(disposing);
		}

		[HttpGet]
		public JsonResult ListCountiesByState(int stateID) {
			return Json(usps.ListCountiesByState(stateID), JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult ListCitiesByStateAndCounty(int stateID, int? countyID) {
			return Json(usps.GetCitiesByStateAndCounty(stateID, countyID), JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult ListTownships(int stateID, int? countyID) {
			return Json(usps.GetTownshipsByStateAndCounty(stateID, countyID), JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult ListStatesCountiesCitiesByZip(string zip) {
			return Json(usps.ListStatesCountiesCitiesByZip(zip), JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult IsValidZip(string zip, int countyID, int stateID) {
			return Json(usps.IsValidZip(zip, countyID, stateID), JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult SearchCityName(string input, int stateID, int countyID) {
			return Json(usps.SearchCityName(input, stateID, countyID), JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult SearchTownshipName(string input, int stateID, int countyID) {
			return Json(usps.SearchTownshipName(input, stateID, countyID), JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult SearchZip(string input, int stateID, int countyID) {
			return Json(usps.SearchZip(input, stateID, countyID), JsonRequestBehavior.AllowGet);
		}
	}

	//KMS DO move or replace these
	public class SimpleListItem {
		public SimpleListItem() { }

		public SimpleListItem(string id, string name) {
			ID = id;
			Name = name;
		}

		public string ID { get; set; }
		public string Name { get; set; }
	}
}