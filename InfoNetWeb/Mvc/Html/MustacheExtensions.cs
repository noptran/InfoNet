using System.Collections.Generic;
using System.Web.Mvc;

namespace Infonet.Web.Mvc.Html {
	public static class MustacheExtensions {
		// ReSharper disable once UnusedParameter.Global
		// ReSharper disable once UnusedMember.Global
		public static MvcMustache BeginMustache(this HtmlHelper htmlHelper, object htmlAttributes = null) {
			return BeginMustache(htmlHelper, null, null, htmlAttributes);
		}

		public static MvcMustache BeginMustache(this HtmlHelper htmlHelper, string openTag, string closeTag, object htmlAttributes = null) {
			return BeginMustache(htmlHelper, null, openTag, closeTag, htmlAttributes);
		}

		public static MvcMustache BeginMustache(this HtmlHelper htmlHelper, string htmlElement, string openTag, string closeTag, object htmlAttributes = null) {
			var mustache = new MvcMustache(htmlHelper.ViewContext, htmlElement, openTag, closeTag);
			BeginMustache(htmlHelper, mustache, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
			return mustache;
		}

		private static void BeginMustache(HtmlHelper htmlHelper, MvcMustache mustache, IDictionary<string, object> htmlAttributes) {
			TagBuilder tag = new TagBuilder(mustache.HtmlElement);
			tag.MergeAttributes(htmlAttributes);
			tag.MergeAttribute("type", "x-tmpl-mustache"); /* htmlAttributes take precedence over this */
			tag.MergeAttribute("data-icjia-role", "mustache");
			if (mustache.HasTags)
				tag.MergeAttribute("data-icjia-mustache-tags", mustache.TagsJson, true); /* this take precedence over htmlAttributes */
			htmlHelper.ViewContext.Writer.Write(tag.ToString(TagRenderMode.StartTag));
		}

		// ReSharper disable once UnusedParameter.Global
		public static void EndMustache(this HtmlHelper htmlHelper, MvcMustache mustache) {
			mustache.ViewContext.Writer.Write("</" + mustache.HtmlElement + ">");
		}
	}
}