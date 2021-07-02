using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Infonet.Web.Mvc.Collections {
	public static class ModelStateDictionaryExtensions {
		// ReSharper disable once UnusedMember.Global
		public static void RemoveFor<TModel>(this ModelStateDictionary modelState, Expression<Func<TModel, object>> expression) {
			string expressionText = ExpressionHelper.GetExpressionText(expression);
			foreach (var ms in modelState.ToArray())
				if (ms.Key.StartsWith(expressionText + ".") || ms.Key == expressionText)
					modelState.Remove(ms);
		}

		public static void RemoveWithPrefix(this ModelStateDictionary modelState, string prefix) {
			foreach (var ms in modelState.ToArray())
				if (ms.Key.StartsWith(prefix + "."))
					modelState.Remove(ms);
		}
	}
}