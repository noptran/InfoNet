using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Infonet.Web.Models.Identity;
using Infonet.Web.ViewModels.Account;
using Microsoft.AspNet.Identity.Owin;
using PagedList;

namespace Infonet.Web.Controllers {
	[Authorize(Roles = "SYSADMIN")]
	public class RolesAdminController : InfonetControllerBase {
		private ApplicationUserManager UserManager {
			get { return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
		}

		private ApplicationRoleManager RoleManager {
			get { return HttpContext.GetOwinContext().Get<ApplicationRoleManager>(); }
		}

		public ActionResult Index(RoleSearchViewModel model, int? page) {
			int pageNumber = page ?? 1;
			model.PageNumber = pageNumber;
			model.RecordCount = RoleManager.Roles.Count();

			model.RoleList = RoleManager.Roles.OrderBy(r => r.Name).ToPagedList(pageNumber, model.PageSize);
			return View(model);
		}

		public ActionResult Create() {
			return View();
		}

		[HttpPost]
		public async Task<ActionResult> Create(RoleViewModel roleViewModel) {
			if (!ModelState.IsValid)
				return View();

			var role = new ApplicationRole(roleViewModel.Name) { Description = roleViewModel.Description };
			var roleresult = await RoleManager.CreateAsync(role);
			if (!roleresult.Succeeded) {
				foreach (string each in roleresult.Errors)
					AddErrorMessage(each);
				return View();
			}
			return RedirectToAction("Edit", new { id = role.Id });
		}

		public async Task<ActionResult> Edit(int id) {
			var role = await RoleManager.FindByIdAsync(id);
			if (role == null)
				return HttpNotFound();

			return View(new RoleViewModel {
				Id = role.Id,
				Name = role.Name,
				Description = role.Description,
				Users = UserManager.Users.Where(u => u.Roles.Any(r => r.RoleId == id)).OrderBy(u => u.UserName)
			});
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[PreventDuplicateRequest]
		public async Task<ActionResult> Edit([Bind(Include = "Name,Id,Description")] RoleViewModel roleModel) {
			if (!ModelState.IsValid) {
				roleModel.Users = UserManager.Users.Where(u => u.Roles.Any(r => r.RoleId == roleModel.Id)).OrderBy(u => u.UserName);
				return View(roleModel);
			}

			var role = await RoleManager.FindByIdAsync(roleModel.Id);
			role.Name = roleModel.Name;
			role.Description = roleModel.Description;
			await RoleManager.UpdateAsync(role);
			return RedirectToAction("Edit", new { id = roleModel.Id });
		}
	}
}