using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Infonet.Web.Mvc;
using Infonet.Web.ViewModels.Account;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace Infonet.Web.Controllers {
	[Authorize]
	public class ManageController : InfonetControllerBase {
		private ApplicationRoleManager RoleManager {
			get { return HttpContext.GetOwinContext().Get<ApplicationRoleManager>(); }
		}

		private ApplicationSignInManager SignInManager {
			get { return HttpContext.GetOwinContext().Get<ApplicationSignInManager>(); }
		}

		private ApplicationUserManager UserManager {
			get { return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
		}

		public ActionResult Index() {
			int userId = User.Identity.GetUserId<int>();
			var rolesList = UserManager.GetRoles(userId);
			return View(new ManageAccountViewModel {
				Roles = RoleManager.Roles.Where(r => rolesList.Contains(r.Name)).ToList(),
				Email = UserManager.GetEmail(userId),
				Username = User.Identity.Name,
				CenterName = Session.Center().Name
			});
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[PreventDuplicateRequest]
		public ActionResult Index(ManageAccountViewModel model) {
			if (ModelState.IsValid) {
				var user = UserManager.FindById(User.Identity.GetUserId<int>());
				user.Email = model.Email;
				user.UserName = model.Username;
				UserManager.Update(user);
				AddSuccessMessage("Your changes have been saved successfully.");
				return RedirectToAction("Index");
			}

			model.CenterName = Session.Center().Name;
			var rolesList = UserManager.GetRoles(User.Identity.GetUserId<int>());
			model.Roles = RoleManager.Roles.Where(r => rolesList.Contains(r.Name)).ToList();
			return View(model);
		}

		public ActionResult ChangePassword() {
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[PreventDuplicateRequest]
		public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model) {
			if (!ModelState.IsValid)
				return View(model);

			var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId<int>(), model.OldPassword, model.NewPassword);
			if (result.Succeeded) {
				var user = await UserManager.FindByIdAsync(User.Identity.GetUserId<int>());
				if (user != null)
					await SignInManager.SignInAsync(user, false, false);
				AddSuccessMessage("Password has been successfully changed.");
				return RedirectToAction("Index");
			}

			foreach (string error in result.Errors)
				AddErrorMessage(error);
			return View(model);
		}
	}
}