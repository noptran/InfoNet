using System.Web.Optimization;

namespace Infonet.Web {
	public static class BundleConfig {
		// For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
		public static void RegisterBundles(BundleCollection bundles) {
			bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
				"~/Scripts/jquery-{version}.js",
				"~/Scripts/ICJIA/jquery.ajax.setup.js",
				"~/Scripts/ICJIA/jquery.regex.js"
			));

			//requires jquery.js and bootstrap.css
			bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
				"~/Scripts/jquery.validate.js",
				"~/Scripts/jquery.validate.unobtrusive.js",
				"~/Scripts/ICJIA/errors-clear-format.js",
				"~/Scripts/ICJIA/jquery.validate.unobtrusive.addons.js", //needs bootstrap.css
				"~/Scripts/Mvc/Validation/between-july-twothousandeight-today.js", //KMS DO does datepicker support this?
                "~/Scripts/Mvc/Validation/between-nineteenfifty-today.js",
                "~/Scripts/Mvc/Validation/between-nineteenninety-today.js",
				"~/Scripts/Mvc/Validation/between-nineteenseventy-today.js",
				"~/Scripts/Mvc/Validation/between-nineteenthirty-today.js",
				"~/Scripts/Mvc/Validation/compare-with-date.js",
				"~/Scripts/Mvc/Validation/file-extension.js",
				"~/Scripts/Mvc/Validation/not-earlier-than-first-contact-date.js",
				"~/Scripts/Mvc/Validation/not-earlier-than-one-year-prior-first-contact-date.js",
				"~/Scripts/Mvc/Validation/not-greater-than-today.js",
				"~/Scripts/Mvc/Validation/not-greater-than-first-contact-date.js",
				"~/Scripts/Mvc/Validation/not-less-than-nineteenseventy.js",
				"~/Scripts/Mvc/Validation/number-of-children.js",
				"~/Scripts/Mvc/Validation/quarter-increment.js",
				"~/Scripts/Mvc/Validation/sentence-before-charge.js",
				"~/Scripts/Mvc/Validation/service-group-required.js",
				"~/Scripts/Mvc/Validation/service-outcome-client-service-group.js",
				"~/Scripts/Mvc/Validation/whole-number.js"
			));

			//requires jquery.js and bootstrap.css
			bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
				"~/Scripts/bootstrap.js",
				"~/Scripts/ICJIA/bootstrap.addons.js",
				"~/Scripts/jquery.confirm.js", //needs bootstrap.css+js
				"~/Scripts/ICJIA/jquery.confirm.addons.js",
				"~/Scripts/ICJIA/bootstrap.datepicker.js", //needs jquery.js and bootstrap.css
				"~/Scripts/ICJIA/bootstrap.datepicker.addons.js",
				"~/Scripts/jquery.bootstrap-growl.js"
			));

			//requires jquery.js, bootstrap.css+js
			bundles.Add(new ScriptBundle("~/bundles/dirtyforms").Include(
				"~/Scripts/jquery.dirtyforms.js", //dirtyforms must be last jquery plugin loaded
				"~/Scripts/jquery.dirtyforms.dialogs.bootstrap.min.js", //needs bootstrap.css+js
				"~/Scripts/ICJIA/jquery.dirtyforms.addons.js" //needs boostrap.css, jquery.confirm.js, and [jquery.validate.unobtrusive].cshtml
			));

			//requires jquery.js
			bundles.Add(new ScriptBundle("~/bundles/mustache").Include(
				"~/Scripts/mustache.js",
				"~/Scripts/ICJIA/jquery.mustache.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/pagination").Include(
				"~/Scripts/pagination.js"
			));

			bundles.Add(new ScriptBundle("~/bundles/typeahead").Include(
				"~/Scripts/typeahead.jquery.js"
			));

			bundles.Add(new StyleBundle("~/Content/css/ICJIA/DV").Include(
				"~/Content/ICJIA/DV/bootstrap-dv.css",
				"~/Content/ICJIA/bootstrap-buttons.css",
				"~/Content/ICJIA/DV/bootstrap-btn-primary-dv.css",
				"~/Content/ICJIA/DV/icjia-stacked-nav-dv.css",
				"~/Content/ICJIA/DV/icjia-panel-header-btn-dv.css",
				"~/Content/ICJIA/DV/icjia-logo-dv.css",
				"~/Content/ICJIA/DV/icjia-misc-dv.css",
				"~/Content/ICJIA/bootstrap-addons.css",
				"~/Content/ICJIA/DV/bootstrap-addons-dv.css",
				"~/Content/ICJIA/bootstrap-container-widths.css",
				"~/Content/ICJIA/bootstrap-panel-overlays.css",
				"~/Content/ICJIA/bootstrap-dirtyforms.css",
				"~/Content/bootstrap-datepicker3.css",
				"~/Content/ICJIA/session-tracker.css",
				"~/Content/Typeahead.css",
				"~/Content/site.css"
			));

			bundles.Add(new StyleBundle("~/Content/css/ICJIA/SA").Include(
				"~/Content/ICJIA/SA/bootstrap-sa.css",
				"~/Content/ICJIA/bootstrap-buttons.css",
				"~/Content/ICJIA/SA/bootstrap-btn-primary-sa.css",
				"~/Content/ICJIA/SA/icjia-stacked-nav-sa.css",
				"~/Content/ICJIA/SA/icjia-panel-header-btn-sa.css",
				"~/Content/ICJIA/SA/icjia-logo-sa.css",
				"~/Content/ICJIA/SA/icjia-misc-sa.css",
				"~/Content/ICJIA/bootstrap-addons.css",
				"~/Content/ICJIA/SA/bootstrap-addons-sa.css",
				"~/Content/ICJIA/bootstrap-container-widths.css",
				"~/Content/ICJIA/bootstrap-panel-overlays.css",
				"~/Content/ICJIA/bootstrap-dirtyforms.css",
				"~/Content/bootstrap-datepicker3.css",
				"~/Content/ICJIA/session-tracker.css",
				"~/Content/Typeahead.css",
				"~/Content/site.css"
			));

			bundles.Add(new StyleBundle("~/Content/css/ICJIA/CAC").Include(
				"~/Content/ICJIA/CAC/bootstrap-cac.css",
				"~/Content/ICJIA/bootstrap-buttons.css",
				"~/Content/ICJIA/CAC/bootstrap-btn-primary-cac.css",
				"~/Content/ICJIA/CAC/icjia-stacked-nav-cac.css",
				"~/Content/ICJIA/CAC/icjia-panel-header-btn-cac.css",
				"~/Content/ICJIA/CAC/icjia-logo-cac.css",
				"~/Content/ICJIA/CAC/icjia-misc-cac.css",
				"~/Content/ICJIA/bootstrap-addons.css",
				"~/Content/ICJIA/CAC/bootstrap-addons-cac.css",
				"~/Content/ICJIA/bootstrap-container-widths.css",
				"~/Content/ICJIA/bootstrap-panel-overlays.css",
				"~/Content/ICJIA/bootstrap-dirtyforms.css",
				"~/Content/bootstrap-datepicker3.css",
				"~/Content/ICJIA/session-tracker.css",
				"~/Content/Typeahead.css",
				"~/Content/site.css"
			));

			bundles.Add(new StyleBundle("~/Content/css/ICJIA/ADMIN").Include(
				"~/Content/ICJIA/ADMIN/bootstrap-admin.css",
				"~/Content/ICJIA/bootstrap-buttons.css",
				"~/Content/ICJIA/ADMIN/bootstrap-btn-primary-admin.css",
				"~/Content/ICJIA/ADMIN/icjia-stacked-nav-admin.css",
				"~/Content/ICJIA/ADMIN/icjia-panel-header-btn-admin.css",
				"~/Content/ICJIA/ADMIN/icjia-logo-admin.css",
				"~/Content/ICJIA/ADMIN/icjia-misc-admin.css",
				"~/Content/ICJIA/bootstrap-addons.css",
				"~/Content/ICJIA/ADMIN/bootstrap-addons-admin.css",
				"~/Content/ICJIA/bootstrap-container-widths.css",
				"~/Content/ICJIA/bootstrap-panel-overlays.css",
				"~/Content/ICJIA/bootstrap-dirtyforms.css",
				"~/Content/bootstrap-datepicker3.css",
				"~/Content/ICJIA/session-tracker.css",
				"~/Content/Typeahead.css",
				"~/Content/site.css"
			));
		}
	}
}