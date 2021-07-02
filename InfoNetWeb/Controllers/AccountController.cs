using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Routing;
using Infonet.Web.ViewModels.Account;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace Infonet.Web.Controllers {
	[Authorize]
	public class AccountController : Controller {
		private IAuthenticationManager AuthenticationManager {
			get { return HttpContext.GetOwinContext().Authentication; }
		}

		private ApplicationSignInManager SignInManager {
			get { return HttpContext.GetOwinContext().Get<ApplicationSignInManager>(); }
		}

		private ApplicationUserManager UserManager {
			get { return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
		}

		#region login/logoff
		[AllowAnonymous]
		public ActionResult Login(string returnUrl) {
			ViewBag.ReturnUrl = returnUrl;
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		public async Task<ActionResult> Login(LoginViewModel model, string returnUrl) {
			try {
				AntiForgery.Validate();
			} catch {
				return RedirectToAction("Login");
			}
			if (!ModelState.IsValid)
				return View(model);

			Session.Clear();

			// To enable password failures to trigger account lockout, change to shouldLockout below to true
			var result = await SignInManager.PasswordSignInAsync(model.Username, model.Password, false, false);
			switch (result) {
				case SignInStatus.Success:
					if (!string.IsNullOrEmpty(returnUrl)) {
						int queryStringIndex = returnUrl.IndexOf('?');
						string path = returnUrl.Substring(0, queryStringIndex == -1 ? returnUrl.Length : queryStringIndex);
						string queryString = queryStringIndex == -1 ? "" : returnUrl.Substring(queryStringIndex, returnUrl.Length - queryStringIndex);
						var routeFromUrl = RouteTable.Routes.GetRouteData(new HttpContextWrapper(new HttpContext(new HttpRequest(null, new UriBuilder(Request.Url.Scheme, Request.Url.Host, Request.Url.Port, path).ToString(), queryString), new HttpResponse(new StringWriter()))));
						//KMS DO does this still work?
						if (IsActionAuthorized(routeFromUrl.Values["action"].ToString(), routeFromUrl.Values["controller"].ToString()))
							return RedirectToLocal(returnUrl);
					}
					return RedirectToAction("Index", "Home");
				case SignInStatus.LockedOut:
					return View("Lockout");
				case SignInStatus.RequiresVerification:
					throw new NotSupportedException();
				default:
					ModelState.AddModelError("", "Invalid login attempt.");
					return View(model);
			}
		}

		public ActionResult LogOff() {
			AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
			Session.Clear();
			return RedirectToAction("Login", "Account");
		}
		#endregion

		#region forgot password
		[AllowAnonymous]
		public ActionResult ForgotPassword() {
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		[PreventDuplicateRequest]
		public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model) {
			if (!ModelState.IsValid)
				return View(model);

			var user = await UserManager.FindByNameAsync(model.Username);
			if (user == null)
				return View("ForgotPasswordConfirmation");

			string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
			string callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code }, Request.Url.Scheme);
			using (var smtp = new SmtpClient())
			using (var message = new MailMessage { IsBodyHtml = true }) {
				message.To.Add(new MailAddress(user.Email, user.UserName));
				message.Subject = "Reset Password";
				message.Body = "Please reset your password by clicking <a href='" + callbackUrl + "'>here</a>";
				smtp.Send(message);
			}
			return RedirectToAction("ForgotPasswordConfirmation", "Account");
		}

		[AllowAnonymous]
		public ActionResult ForgotPasswordConfirmation() {
			return View();
		}

		[AllowAnonymous]
		public ActionResult ResetPassword(string code) {
			return code == null ? View("Error") : View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		[PreventDuplicateRequest]
		public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model) {
			if (!ModelState.IsValid)
				return View(model);
			var user = await UserManager.FindByNameAsync(model.Username);
			if (user == null)
				return RedirectToAction("ResetPasswordConfirmation", "Account");
			var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
			if (result.Succeeded)
				return RedirectToAction("ResetPasswordConfirmation", "Account");
			foreach (string error in result.Errors)
				ModelState.AddModelError("", error);
			return View();
		}

		[AllowAnonymous]
		public ActionResult ResetPasswordConfirmation() {
			return View();
		}
		#endregion

		#region debugging
		[AllowAnonymous]
		public ActionResult SessionDebug() {
			return View();
		}
		#endregion

		#region session-tracker ajax
		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
		private class KeepAliveReponse {
			public bool IsSuccess { get; set; }
			public string Message { get; set; }
			public long MillisecondsRemaining { get; set; }
			public DateTime When { get; set; }
			public string SessionId { get; set; }
		}

		[Authorize]
		public JsonResult TouchSession() {
			//KMS DO does this line do anything?
			Session.Timeout = ((SessionStateSection)WebConfigurationManager.GetSection("system.web/sessionState")).Timeout.Minutes;
			//KMS DO why is lastTouched in the future?
			Session["LastTouched"] = DateTime.Now.AddMinutes(Session.Timeout);
			return CheckRemainder();
		}

		[Authorize]
		public JsonResult CheckRemainder() {
			var ret = new KeepAliveReponse {
				IsSuccess = true,
				Message = "",
				SessionId = Session.SessionID,
				MillisecondsRemaining = (long)((DateTime)Session["LastTouched"] - DateTime.Now).TotalMilliseconds,
				When = DateTime.Now
			};

			if (Session.IsNewSession) {
				ret.IsSuccess = false;
				ret.Message = "The session expired before the request completed.";
				ret.MillisecondsRemaining = -1;
			}

			return Json(ret, JsonRequestBehavior.AllowGet);
		}

		public void Expire() {
			Session.Abandon();
		}
		#endregion

		#region helpers
		private bool IsActionAuthorized(string actionName, string controllerName) {
			if (string.IsNullOrWhiteSpace(controllerName))
				return false;

			IController controller;
			try {
				controller = ControllerBuilder.Current.GetControllerFactory().CreateController(HttpContext.Request.RequestContext, controllerName);
			} catch (HttpException) {
				throw new ArgumentException(string.Concat("Specified controller ", controllerName, " does not exist."));
			}
			if (controller == null)
				throw new ArgumentException(string.Concat("Specified controller ", controllerName, " does not exist."));

			var controllerContext = new ControllerContext(HttpContext.Request.RequestContext, (ControllerBase)controller);
			return IsActionAuthorized(new ReflectedControllerDescriptor(controller.GetType()).FindAction(controllerContext, actionName), controllerContext);
		}

		private static bool IsActionAuthorized(ActionDescriptor actionDescriptor, ControllerContext controllerContext) {
			if (actionDescriptor == null)
				return false;

			var authorizationContext = new AuthorizationContext(controllerContext, actionDescriptor);
			using (var enumerator = new FilterInfo(FilterProviders.Providers.GetFilters(controllerContext, actionDescriptor)).AuthorizationFilters.GetEnumerator()) {
				while (enumerator.MoveNext()) {
					enumerator.Current.OnAuthorization(authorizationContext);
					if (authorizationContext.Result != null)
						return false;
				}
				return true;
			}
		}

		private ActionResult RedirectToLocal(string returnUrl) {
			if (Url.IsLocalUrl(returnUrl))
				return Redirect(returnUrl);
			return RedirectToAction("Index", "Home");
		}
		#endregion
	}
}