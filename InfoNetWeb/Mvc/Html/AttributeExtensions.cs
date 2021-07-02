using System.Web.Mvc;

namespace Infonet.Web.Mvc.Html {
	public static class AttributeExtensions {
		/* reference: http://api.jquery.com/category/selectors */
		//public static string JQUERY_META_CHARS = @" !""#$%&'()*+,./:;<=>?@[\]^`{|}~";

		//public static string EscapeJQueryLiteral(this HtmlHelper html, string s) {
		//	var sb = new StringBuilder(s.Length * 2);
		//	foreach (char each in s)
		//		if (JQUERY_META_CHARS.IndexOf(each) < 0) {
		//			sb.Append(each);
		//		} else {
		//			sb.Append('\\');
		//			sb.Append(each);
		//		}
		//	return sb.ToString();
		//}

		public static AttributeSet Attributes(this HtmlHelper html, object htmlAttributes = null) {
			return AttributeSet.From(htmlAttributes);
		}
	}
}