using System.Web.Mvc;

namespace Infonet.Web.Mvc.Html {
	public static class MvcHtmlStringExtensions {
		public static bool IsNullOrEmpty(this MvcHtmlString self) {
			return MvcHtmlString.IsNullOrEmpty(self);
		}
	}
}