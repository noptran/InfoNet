using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Infonet.Web.Models.Identity;
using Infonet.Web.ViewModels.Account;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PagedList;

namespace Infonet.Web.Controllers {
	[Authorize(Roles = "SYSADMIN")]
	public class UsersAdminController : InfonetControllerBase {
		private ApplicationUserManager UserManager {
			get { return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
		}

		public ActionResult Index(UserSearchViewModel model, int? page) {
			int pageNumber = page ?? 1;
			model.PageNumber = pageNumber;
			model.RecordCount = UserManager.Users.Count();
			model.UserList = UserManager.Users.OrderBy(u => u.UserName).ToPagedList(pageNumber, model.PageSize);
			return View(model);
		}

		public ActionResult Create() {
			return View(new RegisterViewModel());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[PreventDuplicateRequest]
		public async Task<ActionResult> Create(RegisterViewModel model) {
			if (!model.Roles.Any())
				ModelState.AddModelError("Roles", "At least one User Role must be selected.");

			if (!ModelState.IsValid)
				return View(model);

			var user = new ApplicationUser {
				UserName = model.Username,
				Email = model.Email,
				CenterId = (int)model.CenterId
			};

			var result = await UserManager.CreateAsync(user, model.Password);
			if (!result.Succeeded) {
				foreach (string each in result.Errors)
					AddErrorMessage(each);
				return View(model);
			}

			result = await UserManager.AddToRolesAsync(user.Id, model.Roles);
			if (!result.Succeeded) {
				foreach (string each in result.Errors)
					AddErrorMessage(each);
				return View(model);
			}
			return RedirectToAction("Edit", new { id = user.Id});
		}

		public async Task<ActionResult> Edit(int id) {
			var user = await UserManager.FindByIdAsync(id);
			if (user == null)
				return HttpNotFound();

			return View(new EditUserViewModel {
				Id = user.Id,
				Username = user.UserName,
				Email = user.Email,
				CenterId = user.CenterId,
				Roles = UserManager.GetRoles(user.Id).ToArray()
			});
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[PreventDuplicateRequest]
		public async Task<ActionResult> Edit(EditUserViewModel model) {
			if (!model.Roles.Any())
				ModelState.AddModelError("Roles", "At least one User Role must be selected.");

			if (!ModelState.IsValid)
				return View(model);

			var user = await UserManager.FindByIdAsync(model.Id);
			if (user == null)
				return HttpNotFound();

			user.UserName = model.Username;
			user.Email = model.Email;
			user.CenterId = model.CenterId;

			var userRoles = await UserManager.GetRolesAsync(user.Id);
			var result = await UserManager.AddToRolesAsync(user.Id, model.Roles.Except(userRoles).ToArray());
			if (!result.Succeeded) {
				foreach (string each in result.Errors)
					AddErrorMessage(each);
				return View(model);
			}
			result = await UserManager.RemoveFromRolesAsync(user.Id, userRoles.Except(model.Roles).ToArray());
			if (!result.Succeeded) {
				foreach (string each in result.Errors)
					AddErrorMessage(each);
				return View(model);
			}

			return RedirectToAction("Edit", new { id = user.Id});
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[PreventDuplicateRequest]
		public async Task<ActionResult> DeleteUser(int id) {
			var user = await UserManager.FindByIdAsync(id);
			if (user == null)
				return Json(new { Error = "The user cannot be found! Please try again." });

			var result = await UserManager.DeleteAsync(user);
			if (!result.Succeeded)
				return Json(new { Error = result.Errors.First() });

			return Json(new { Success = new[] { Url.Action("Index") } });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> PasswordReset(PasswordResetViewModel model) {
			var user = await UserManager.FindByIdAsync(model.Id);
			string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
			var result = await UserManager.ResetPasswordAsync(user.Id, code, model.NewPassword);

			if (!result.Succeeded)
				foreach (string errorr in result.Errors) {
					string[] err = errorr.Split(new[] { ". " }, StringSplitOptions.None);
					foreach (string error in err)
						ModelState.AddModelError("", error);
				}

			if (!ModelState.IsValid)
				return PartialView("_ResetPassword");

			if (!result.Succeeded)
				return Json(new { Error = "Password change was unsuccessful! Please try again." });

			return Json(new { Success = "Password has been successfully changed." });
		}
	}
}