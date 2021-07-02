using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web.Mvc;
using Infonet.Data;
using Infonet.Data.Helpers;
using Infonet.Usps.Data.Helpers;
using Microsoft.AspNet.Identity;

namespace Infonet.Web.Controllers {
	[Authorize]
	public abstract class InfonetControllerBase : Controller {
		private readonly DataHelpers _data;
		private readonly List<IDisposable> _disposables = new List<IDisposable>();

		[SuppressMessage("ReSharper", "InconsistentNaming")]
		protected readonly InfonetServerContext db = new InfonetServerContext();

		protected InfonetControllerBase() {
			_data = new DataHelpers(db.Helpers.Center, db.Helpers.FundingForStaff, new UspsHelper());
		}

		protected int UserId {
			get { return Convert.ToInt32(User.Identity.GetUserId()); }
		}

		public DataHelpers Data {
			get { return _data; }
		}

		protected override void Dispose(bool disposing) {
			if (disposing) {
				db.Dispose();
				_data.Usps.Dispose();
				foreach (var each in _disposables)
					each.Dispose();
				_disposables.Clear();
			}
			base.Dispose(disposing);
		}

		protected TDisposable EnsureDisposal<TDisposable>(TDisposable disposable) where TDisposable : IDisposable {
			_disposables.Add(disposable);
			return disposable;
		}

		//KMS DO odd that we don't remove displayed messages from the ModelState
		//KMS DO given above, maybe this doesn't need to be here...this could be done directly in derived classes.
		protected override void OnActionExecuted(ActionExecutedContext filterContext) {
			base.OnActionExecuted(filterContext);
			if (ModelState.ContainsKey(""))
				foreach (var each in ModelState[""].Errors)
					AddErrorMessage(each.ErrorMessage);
		}

		#region adding growls
		public void AddSuccessMessage(string message) {
			AddMessage("Success", message);
		}

		public void AddInfoMessage(string message) {
			AddMessage("Info", message);
		}

		public void AddWarningMessage(string message) {
			AddMessage("Warning", message);
		}

		public void AddErrorMessage(string message) {
			AddMessage("Error", message);
		}

		private void AddMessage(string severity, string message) {
			var list = (List<string>)TempData[severity] ?? new List<string>();
			list.Insert(0, message);
			TempData[severity] = list; //reset temp value since we've "popped" it?
		}

		protected bool HasMessage(string messageName) {
			return TempData.Peek(messageName) != null && ((List<string>)TempData.Peek(messageName)).Any();
		}
		#endregion

		protected void ClearModelStateErrorsForKeysWithoutIncomingValues(params string[] unlessKeyPassedHere) {
			var keysWithoutIncomingValues = ModelState.Keys.Where(k => !ValueProvider.ContainsPrefix(k) && !unlessKeyPassedHere.Contains(k));
			foreach (string each in keysWithoutIncomingValues)
				ModelState[each].Errors.Clear();
		}

		public ActionResult IcjiaNotFound() {
			return RedirectToAction("PageNotFound", "Error");
		}

		#region inner
		public struct DataHelpers {
			internal DataHelpers(CenterHelper centers, FundingForStaffHelper fundingForStaff, UspsHelper usps) {
				Centers = centers;
				FundingForStaff = fundingForStaff;
				Usps = usps;
			}

			public CenterHelper Centers { get; }

			public FundingForStaffHelper FundingForStaff { get; }

			public UspsHelper Usps { get; }
		}
		#endregion
	}
}