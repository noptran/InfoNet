using System;
using System.Web.Mvc;

namespace Infonet.Web.Mvc.Html {
	public class MvcMustache : IDisposable {
		private readonly ViewContext _viewContext;
		private readonly string _htmlElement;
		private readonly string _openTag;
		private readonly string _closeTag;
		private bool _disposed;

		public MvcMustache(ViewContext viewContext, string htmlElement, string openTag, string closeTag) {
			if (viewContext == null)
				throw new ArgumentNullException(nameof(viewContext));
			if (openTag != null || closeTag != null) {
				if (openTag == null)
					throw new ArgumentNullException(nameof(openTag));
				if (closeTag == null)
					throw new ArgumentNullException(nameof(closeTag));
			}

			_viewContext = viewContext;
			_htmlElement = htmlElement ?? "script";
			_openTag = openTag;
			_closeTag = closeTag;
		}

		public ViewContext ViewContext {
			get { return _viewContext; }
		}

		public string HtmlElement {
			get { return _htmlElement; }
		}

		// ReSharper disable once MemberCanBePrivate.Global
		public string OpenTag {
			get { return _openTag; }
		}

		// ReSharper disable once MemberCanBePrivate.Global
		public string CloseTag {
			get { return _closeTag; }
		}

		public bool HasTags {
			get { return _openTag != null; }
		}

		public string TagsJson {
			get { return OpenTag == null ? null : string.Format("[ \"{0}\", \"{1}\" ]", OpenTag, CloseTag); }
		}

		public string Tag(string value) {
			if (!HasTags)
				throw new InvalidOperationException("Tag(value) requires non-null Open and CloseTags");

			return string.Format("{0}{1}{2}", OpenTag, value, CloseTag);
		}

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		// ReSharper disable once UnusedParameter.Global
		protected virtual void Dispose(bool disposing) {
			if (!_disposed) {
				_disposed = true;
				MustacheExtensions.EndMustache(null, this);
			}
		}
	}
}