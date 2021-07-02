using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using Infonet.Web.Mvc;
using Infonet.Web.ViewModels.Admin;

namespace Infonet.Web.Controllers {
	[Authorize(Roles = "DVCOALITIONADMIN, SACOALITIONADMIN, CACCOALITIONADMIN, DHSCOALITIONADMIN,CDFSSCOALITIONADMIN")]
	public class DirectorEmailController : InfonetControllerBase {
		public ActionResult Index() {
			int centerId = Session.Center().Id;

			var directorEmails = db.Ts_CenterAdministrators
				.Where(s => s.CenterAdminId == centerId)
				.Join(db.T_Center, src => src.CenterId, des => des.CenterID, (src, des) => new DirectorEmailViewModel {
					CenterId = des.CenterID,
					CenterName = des.CenterName,
					DirectorEmail = des.DirectorEmail
				})
				.OrderBy(m => m.CenterName).ToList();

			return View(directorEmails);
		}

		[HttpPost, ActionName("Index")]
		[ValidateAntiForgeryToken]
		[PreventDuplicateRequest]
		public ActionResult Index_Post(IList<DirectorEmailViewModel> model) {
			if (model.Count(m => m.ShouldEdit) == 0) {
				AddInfoMessage("No changes were made to the form. Nothing was saved to the database.");
				return RedirectToAction("Index", "DirectorEmail");
			}

			var markedForEdit = model.Where(l => l.ShouldEdit).ToList();

			using (var transaction = db.Database.BeginTransaction()) {
				try {
					var cntEmailNotAdded = 0;
					foreach (var each in model) {
						if (model[model.IndexOf(each)].ShouldEdit) {
							var centerId = db.T_Center.Single(c => c.CenterID == each.CenterId).CenterID;
							try {
								var sqlRet = db.Database.ExecuteSqlCommand(
											   "UPDATE T_Center SET DirectorEmail = @DirectorEmail WHERE CenterID = @CenterID",
													 new SqlParameter("DirectorEmail", each.DirectorEmail),
													 new SqlParameter("CenterID", centerId)
											   );
								if (sqlRet == 0) {
									ModelState.AddModelError("[" + model.IndexOf(each) + "].DirectorEmail", "Email address could not be added.");
									cntEmailNotAdded++;
								}
							} catch (Exception) {
								AddErrorMessage("There was an error that prevented emails from being saved.");
								throw;
							}
						}
					}
					if (cntEmailNotAdded > 0) {
						transaction.Rollback();
						return View(model);
					}
					transaction.Commit();
					AddSuccessMessage("Your changes have been successfully saved.");
					return RedirectToAction("Index", "DirectorEmail");
				} catch {
					transaction.Rollback();
					AddErrorMessage("There was an error that prevented emails from being saved.");
					throw;
				}
			}
		}
	}
}