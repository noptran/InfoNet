// Configure defaults
// NOTE: This must include all options found in the source file.  The options
//       below were tailored for v2.3.1.  When upgrading, ensure that any new
//       options are included here as well.
$.confirm.options = {
	text: "",
	title: "Are you sure you want to do that?",
	confirmButton: "Yes",
	cancelButton: "No",
	post: false,
	submitForm: false,
	confirmButtonClass: "btn-warning",
	cancelButtonClass: "btn-default",
	dialogClass: "modal-dialog icjia-modal-warning",
	modalOptionsBackdrop: true,
	modalOptionsKeyboard: true
};


(function ($) {

	/**
     * Replaces the $.fn.confirm created by jquery.confirm.js.  This version
	 * triggers an event prior to opening the modal.  Otherwise identical to
	 * original version.
     */
	$.fn.confirm = function (options) {
		if (typeof options === 'undefined') {
			options = {};
		}

		this.click(function (e) {
			/* BEGIN ICJIA CHANGES */
			var event = jQuery.Event("prompt-confirm.icjia");
			$(this).trigger(event);
			if (event.isDefaultPrevented())
				return;
			/* END ICJIA CHANGES */

			e.preventDefault();

			var newOptions = $.extend({
				button: $(this)
			}, options);

			$.confirm(newOptions, e);
		});

		return this;
	};

})(jQuery);