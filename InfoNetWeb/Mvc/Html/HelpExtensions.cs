using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using Infonet.Data.Entity;
using Infonet.Data.Looking;

namespace Infonet.Web.Mvc.Html {
	// ReSharper disable once UnusedMember.Global
	public static class HelpExtensions {
		// ReSharper disable once UnusedMember.Global
		public static MvcHtmlString HelpFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression) {
			if (!(html.ViewData.Model is IProvided))
				throw new InvalidOperationException("Html.HelpFor(expression) may only be called when Model is IProvided");

			var model = (IProvided)html.ViewData.Model;
			return HelpFor(html, expression, model.Provider);
		}

		// ReSharper disable once MemberCanBePrivate.Global
		public static MvcHtmlString HelpFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, Provider provider) {
			var metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
			var property = metadata.ContainerType.GetProperty(metadata.PropertyName);
			if (property == null)
				throw new ArgumentException(nameof(expression) + " must end with a property");

			var attributes = property.GetCustomAttributes(typeof(HelpAttribute), true).Cast<HelpAttribute>().ToArray();
			var result = attributes.SingleOrDefault(a => a.Provider == provider);
			if (result == null)
				result = attributes.SingleOrDefault(a => a.Provider == Provider.None);
			if (result == null)
				throw new ArgumentException(nameof(expression) + " missing applicable HelpAttribute");

			return MvcHtmlString.Create(result.Text);
		}
	}
}