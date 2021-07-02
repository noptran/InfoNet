using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Infonet.Data.Looking;

namespace Infonet.Web.Mvc.Html {
	public static class LookupExtensions {
		[SuppressMessage("ReSharper", "UnusedMember.Global")]
		public static MvcHtmlString LookupDisplayFor<TModel>(this HtmlHelper<TModel> html, Expression<Func<TModel, int?>> expression) {
			var metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
			var property = metadata.ContainerType.GetProperty(metadata.PropertyName);
			if (property == null)
				throw new ArgumentException(nameof(expression) + " must end with a property");

			var attribute = (LookupAttribute)property.GetCustomAttributes(typeof(LookupAttribute), true).Single();
			return LookupDisplayFor(html, expression, attribute.Lookup);
		}

		public static MvcHtmlString LookupDisplayFor<TModel>(this HtmlHelper<TModel> html, Expression<Func<TModel, int?>> expression, ILookupIndex lookup) {
			var codeId = expression.Compile().Invoke((TModel)html.ViewContext.ViewData.Model);
			//KMS DO retrieve attempted value from ModelState?
			if (codeId == null)
				return html.DisplayFor(m => "");

			return html.DisplayFor(m => lookup[codeId].Description);
		}

		public static MvcHtmlString LookupFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, string optionLabel, object htmlAttributes = null) {
			return LookupFor(html, expression, optionLabel, false, htmlAttributes);
		}

		public static MvcHtmlString LookupFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, string optionLabel, bool forceCurrentValue, object htmlAttributes = null) {
			if (!(html.ViewData.Model is IProvided))
				throw new InvalidOperationException("Html.LookupFor(expression, optionLabel, [forceCurrentValue], [htmlAttributes]) may only be called when Model is IProvided");

			var model = (IProvided)html.ViewData.Model;
			return LookupFor(html, expression, model.Provider, optionLabel, forceCurrentValue, htmlAttributes);
		}

		public static MvcHtmlString LookupFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, Provider provider, string optionLabel, object htmlAttributes = null) {
			return LookupFor(html, expression, provider, optionLabel, false, htmlAttributes);
		}

		public static MvcHtmlString LookupFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, Provider provider, string optionLabel, bool forceCurrentValue, object htmlAttributes = null) {
			var metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
			var property = metadata.ContainerType.GetProperty(metadata.PropertyName);
			if (property == null)
				throw new ArgumentException(nameof(expression) + " must end with a property");

			var attribute = (LookupAttribute)property.GetCustomAttributes(typeof(LookupAttribute), true).Single();
			return LookupFor(html, expression, attribute.Lookup[provider], optionLabel, forceCurrentValue, htmlAttributes);
		}

		public static MvcHtmlString LookupFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, IEnumerable<LookupCode> lookups, string optionLabel, object htmlAttributes = null) {
			return LookupFor(html, expression, lookups, optionLabel, false, htmlAttributes);
		}

		public static MvcHtmlString LookupFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, IEnumerable<LookupCode> lookups, string optionLabel, bool forceCurrentValue, object htmlAttributes = null) {
			SelectList options;
			if (forceCurrentValue)
				options = new SelectList(lookups, "CodeId", "Description", expression.Compile().Invoke((TModel)html.ViewContext.ViewData.Model)?.ToString());
			else
				options = new SelectList(lookups, "CodeId", "Description");

			var htmlAttributeDictionary = htmlAttributes as IDictionary<string, object>;
			if (htmlAttributeDictionary != null)
				return html.DropDownListFor(expression, options, optionLabel, htmlAttributeDictionary);

			return html.DropDownListFor(expression, options, optionLabel, htmlAttributes);
		}
	}
}