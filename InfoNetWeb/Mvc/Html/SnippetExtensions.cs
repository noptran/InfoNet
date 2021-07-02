using System.Web.Mvc;
using Infonet.Web.ViewModels.Shared;
using PagedList.Mvc;
using HtmlHelper = System.Web.Mvc.HtmlHelper;

namespace Infonet.Web.Mvc.Html {
	public static class SnippetExtensions {
		public static SnippetFactory Snippets(this HtmlHelper html) {
			return new SnippetFactory(html);
		}
	}

	public struct SnippetFactory {
		private readonly HtmlHelper _html;

		public SnippetFactory(HtmlHelper html) {
			_html = html;
		}

		public MvcHtmlString SaveButton(object htmlAttributes = null) {
			return SaveButton(DirtyMode.Page, htmlAttributes);
		}

		public MvcHtmlString SaveButton(DirtyMode mode, object htmlAttributes = null) {
			return SaveButton(mode, "Save", htmlAttributes);
		}

		public MvcHtmlString SaveButton(DirtyMode mode, string innerText, object htmlAttributes = null) {
			var tag = new TagBuilder("button");
			tag.Attributes["type"] = "button";
			tag.Attributes["class"] = "btn btn-success";
			tag.MergeAttributes(_html.Attributes(htmlAttributes), true);
			tag.InnerHtml = "<span class=\"text-nowrap\">" + innerText + " <span class=\"icjia-if-" + mode.ToString().ToLower() + "-dirty glyphicon glyphicon-ok\" aria-hidden=\"true\"></span></span>";
			return new MvcHtmlString(tag.ToString(TagRenderMode.Normal));
		}

		public MvcHtmlString SaveAddButton(object htmlAttributes = null) {
			return SaveAddButton("Save & Add New", htmlAttributes);
		}

		public MvcHtmlString SaveAddButton(string innerText, object htmlAttributes = null) {
			var tag = new TagBuilder("a");
			tag.Attributes["class"] = "btn-success";
			tag.Attributes["data-icjia-role"] = "preventDuplicateRequest";
			tag.Attributes["href"] = "#";
			tag.MergeAttributes(_html.Attributes(htmlAttributes), true);
			tag.InnerHtml = innerText;
			return new MvcHtmlString(tag.ToString(TagRenderMode.Normal));
		}

		public MvcHtmlString UndoButton(DirtyMode mode, object htmlAttributes = null) {
			var tag = new TagBuilder("button");
			if (mode == DirtyMode.Modal)
				tag.Attributes["data-icjia-role"] = "dirty.modal.confirm";
			tag.Attributes["data-text"] = "You've made changes on this page which aren't saved.<br/><br/>If you continue, those changes will be undone.";
			tag.Attributes["type"] = "button";
			tag.Attributes["class"] = "btn btn-warning";
			tag.MergeAttributes(_html.Attributes(htmlAttributes), true);
			tag.InnerHtml = "<span class=\"icjia-if-" + mode.ToString().ToLower() + "-not-dirty\">Nothing to Undo</span><span class=\"icjia-if-" + mode.ToString().ToLower() + "-dirty\">Undo Changes <span class=\"glyphicon glyphicon-undo\" aria-hidden=\"true\"></span></span>";
			return new MvcHtmlString(tag.ToString(TagRenderMode.Normal));
		}

		public MvcHtmlString UndoLink(string href, object htmlAttributes = null) {
			var tag = new TagBuilder("a");
			tag.Attributes["data-confirm-button-class"] = "btn-warning";
			tag.Attributes["data-dialog-class"] = "modal-dialog icjia-modal-warning";
			tag.Attributes["data-icjia-role"] = "dirty.page.confirm";
			tag.Attributes["data-text"] = "You've made changes on this page which aren't saved.<br/><br/>If you continue, those changes will be undone.";
			tag.Attributes["class"] = "btn btn-warning";
			tag.MergeAttributes(_html.Attributes(htmlAttributes), true);
			tag.Attributes["href"] = href;
			tag.InnerHtml = "<span class=\"icjia-if-page-not-dirty\">Nothing to Undo</span><span class=\"icjia-if-page-dirty\">Undo Changes <span class=\"glyphicon glyphicon-undo\" aria-hidden=\"true\"></span></span>";
			return new MvcHtmlString(tag.ToString(TagRenderMode.Normal));
		}

		public MvcHtmlString DeleteLink(string href, object htmlAttributes = null) {
			var tag = new TagBuilder("a");
			tag.Attributes["data-confirm-button-class"] = "btn-danger";
			tag.Attributes["data-dialog-class"] = "modal-dialog icjia-modal-danger";
			tag.Attributes["data-icjia-role"] = "dirty.page.confirm.always";
			tag.Attributes["data-text"] = "This action cannot be undone. If you continue, your data will be <span style='font-weight: bold'>permanently deleted</span>.";
			tag.Attributes["class"] = "btn btn-danger";
			tag.MergeAttributes(_html.Attributes(htmlAttributes), true);
			tag.Attributes["href"] = href;
			tag.InnerHtml = "Delete";
			return new MvcHtmlString(tag.ToString(TagRenderMode.Normal));
		}

		public MvcHtmlString CancelLink(string href, object htmlAttributes = null) {
			var tag = new TagBuilder("a");
			tag.Attributes["data-icjia-role"] = "dirty.page.confirm";
			tag.Attributes["data-text"] = "You've made changes on this page which aren't saved.<br/><br/>If you continue, those changes will be undone.";
			tag.Attributes["class"] = "btn btn-warning";
			tag.MergeAttributes(_html.Attributes(htmlAttributes), true);
			tag.Attributes["href"] = href;
			tag.InnerHtml = "<span class=\"icjia-if-page-not-dirty\">Cancel</span><span class=\"icjia-if-page-dirty\">Cancel <span class=\"glyphicon glyphicon-remove\" aria-hidden=\"true\"></span></span>";
			return new MvcHtmlString(tag.ToString(TagRenderMode.Normal));
		}

		public MvcHtmlString ModalCloseButton(object htmlAttributes = null) {
			var tag = new TagBuilder("button");
			tag.Attributes["class"] = "close";
			tag.Attributes["data-icjia-role"] = "dirty.modal.confirm";
			tag.Attributes["data-text"] = "You've made changes on this page which aren't saved.<br/><br/>If you continue, those changes will be undone.";
			tag.Attributes["type"] = "button";
			tag.MergeAttributes(_html.Attributes(htmlAttributes), true);
			tag.InnerHtml = "&times;";
			return new MvcHtmlString(tag.ToString(TagRenderMode.Normal));
		}

		public string PaginationFirstPage() {
			return "<span class='glyphicon glyphicon-chevron-left' aria-hidden='true'></span><span class='glyphicon glyphicon-chevron-left' style='margin-left:-6px' aria-hidden='true'></span><span class='sr-only'>First Page</span>";
		}

		public string PaginationLastPage() {
			return "<span class='glyphicon glyphicon-chevron-right' aria-hidden='true'></span><span class='glyphicon glyphicon-chevron-right' style='margin-left:-6px' aria-hidden='true'></span><span class='sr-only'>Last Page</span>";
		}

		public string PaginationNextPage() {
			return "<span class='glyphicon glyphicon-chevron-right' aria-hidden='true'></span><span class='sr-only'>Next Page</span>";
		}

		public string PaginationPreviousPage() {
			return "<span class='glyphicon glyphicon-chevron-left' aria-hidden='true'></span><span class='sr-only'>Previous Page</span>";
		}

		public PagedListRenderOptions PagedListRenderOptions() {
			return new PagedListRenderOptions { LinkToFirstPageFormat = PaginationFirstPage(), LinkToNextPageFormat = PaginationNextPage(), LinkToPreviousPageFormat = PaginationPreviousPage(), LinkToLastPageFormat = PaginationLastPage() };
		}

		public PagedListRenderOptions PagedListRenderOptions(string[] liClassNames, string[] containterClassNames) {
			return new PagedListRenderOptions { LinkToFirstPageFormat = PaginationFirstPage(), LinkToNextPageFormat = PaginationNextPage(), LinkToPreviousPageFormat = PaginationPreviousPage(), LinkToLastPageFormat = PaginationLastPage(), LiElementClasses = liClassNames, ContainerDivClasses = containterClassNames };
		}

		public object PagedListPagerRouteValues(int page, PagedListPagination Model) {
			return new { page, Model.Range, StartDate = Model.StartDate.HasValue ? string.Format("{0:MM/dd/yyyy}", Model.StartDate) : " ", EndDate = Model.EndDate.HasValue ? string.Format("{0:MM/dd/yyyy}", Model.EndDate) : " ", Model.PageSize };
		}
	}

	public enum DirtyMode {
		// ReSharper disable once UnusedMember.Global
		Form,
		Modal,
		Page
	}
}