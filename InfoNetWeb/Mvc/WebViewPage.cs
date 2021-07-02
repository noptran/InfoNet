using System;
using Infonet.Web.Controllers;

namespace Infonet.Web.Mvc {
	public abstract class WebViewPage : System.Web.Mvc.WebViewPage {
		public InfonetControllerBase.DataHelpers Data {
			get {
				var controller = ViewContext.Controller as InfonetControllerBase;
				if (controller == null)
					throw new NotImplementedException("@Data can only be used in views with controllers extending InfonetControllerBase");
				return controller.Data;
			}
		}
	}

	public abstract class WebViewPage<TModel> : System.Web.Mvc.WebViewPage<TModel> {
		public InfonetControllerBase.DataHelpers Data {
			get {
				var controller = ViewContext.Controller as InfonetControllerBase;
				if (controller == null)
					throw new NotImplementedException("@Data can only be used in views with controllers extending InfonetControllerBase");
				return controller.Data;
			}
		}
	}
}