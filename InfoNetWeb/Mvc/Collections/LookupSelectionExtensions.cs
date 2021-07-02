using System.Web.Mvc;
using Infonet.Data.Looking;

namespace Infonet.Web.Mvc.Collections {
	public static class LookupSelectionExtensions {
		public static SelectList ToSelectList(this ILookupSelection lookup) {
			return new SelectList(lookup, "CodeId", "Description");
		}

		// ReSharper disable once UnusedMember.Global
		public static SelectList ToSelectList(this ILookupSelection lookup, int? selected) {
			return new SelectList(lookup, "CodeId", "Description", selected);
		}
	}
}