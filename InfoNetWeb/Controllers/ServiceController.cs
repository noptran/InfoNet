using System;
using System.Web.Mvc;
using Infonet.Web.Mvc;

namespace Infonet.Web.Controllers {
	[Authorize]
	public class ServiceController : InfonetControllerBase {
		#region AJAX
		// Used _GroupService.js, _CommunityService.js, _Publication.js, _Hotline.js, Event.js, CrisisIntervention.js 
		// Gets the list of active staff during time of service
		public ActionResult GetStaff(string serviceDate) {
			return Json(Data.Centers.GetStaffForCenterAndDate(serviceDate, Session.Center().Id), JsonRequestBehavior.AllowGet);
		}
        public ActionResult GetStaffDateRange(DateTime? startDate, DateTime? endDate)
        {
            return Json(Data.Centers.GetStaffForCentersAndDateRange(startDate, endDate, Session.Center().Id), JsonRequestBehavior.AllowGet);
        }
        #endregion
        public ActionResult GetStaffRetainCurrentSvid(DateTime? serviceDate, int? currentSvid) {
			return Json(Data.Centers.GetStaffForCenterAndDateRetainCurrentSvid(serviceDate, Session.Center().Id, currentSvid), JsonRequestBehavior.AllowGet);
		}
	}
}