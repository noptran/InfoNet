using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Infonet.Web.Mvc.Html {
	public static class InputExtensions {
		public static MvcHtmlString TextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, bool useDisplayFormatString, object htmlAttributes = null) {
			if (!useDisplayFormatString)
				return html.TextBoxFor(expression, htmlAttributes);

			try {
				var metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
				var property = metadata.ContainerType.GetProperty(metadata.PropertyName);
				var attribute = (DisplayFormatAttribute)property.GetCustomAttributes(typeof(DisplayFormatAttribute), true).Single();
				return html.TextBoxFor(expression, attribute.DataFormatString, htmlAttributes);
			} catch (Exception e) {
				throw new InvalidOperationException("Failed to retrieve [DisplayFormat(DataFormatString)] from expression's final property", e);
			}
		}
	}
}