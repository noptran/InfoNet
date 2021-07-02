using System.Collections.Generic;
using System.Web.Mvc;
using Infonet.Core.Collections;

namespace Infonet.Web.Mvc.Collections {
	public static class ViewDataDictionaryExtensions {
		public static ViewDataDictionary<TModel> CopyWith<TModel>(this ViewDataDictionary<TModel> viewData, object dictionary) {
			return viewData.CopyWith(HtmlHelper.AnonymousObjectToHtmlAttributes(dictionary));
		}

		public static ViewDataDictionary<TModel> CopyWith<TModel>(this ViewDataDictionary<TModel> viewData, IDictionary<string, object> dictionary) {
			var result = new ViewDataDictionary<TModel>(viewData);
			result.AddRange(dictionary);
			return result;
		}
	}
}