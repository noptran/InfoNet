//REQUIRES jquery.js, jquery.validate.js, jquery.validate.unobtrusive.js, and bootstrap.css
var preventDuplicateRequest;

$(function () {
	$(document).on('click', 'a, button', function (e) {
		if (preventDuplicateRequest && $(this).data("icjia-role") === 'preventDuplicateRequest')
			$(this).prop('disabled', true);
	});

	var $forms = $('form');
	styleRequiredFields($forms);
	enhanceValidators($forms);
	styleExistingErrors($forms);

	var delayForShowInProgress = 0;
	// if page loads with errors in collapsed elements, open them.
	// later, this might need tweaking for accordions.
	$('form .collapse:not(.in):has(.input-validation-error,.field-validation-error)').each(function () {
		$(this).collapse('show');
		delayForShowInProgress = 350;
	});

	// if page loads with errors, change hash to icjia-bookmark of first error
	// and set focus on the input
	var willSetFocus = false;
	$('form .input-validation-error:visible').first().each(function () {
		var $this = $(this);
		var bookmarkId = $this.closest('.icjia-bookmark[id!=""]').attr('id');
		if (bookmarkId != undefined)
			window.location.hash = '#' + bookmarkId;

		willSetFocus = true;
		setTimeout(function() {
			$this.focus();
			ensureActiveVisible();
		}, delayForShowInProgress);
	});
	if (!willSetFocus)
		$('form .field-validation-error:visible').first().each(function () {
			var bookmarkId = $(this).closest('.icjia-bookmark[id!=""]').attr('id');
			if (bookmarkId != undefined)
				window.location.hash = '#' + bookmarkId;
		});

});

function styleRequiredFields(form$) {
	$(form$).find('[data-val="true"][data-val-required][type!="checkbox"]').each(function () {
		var $this = $(this);
		if ($this.has('[id!=""]')) {
			var label = $('label[for="' + $this.attr('id') + '"]');
			if (!label.hasClass('icjia-required'))
				label.addClass('icjia-required');
		}
		if ($this.attr('aria-required') != true)
			$this.attr('aria-required', true);
	});
}

function enhanceValidators(form$) {
	$(form$).each(function () {
		var validator = $.data(this).validator;
		if (undefined != validator) {
			var settings = validator.settings;

			settings.ignore = ':hidden:not(.validate-even-hidden),.tt-hint,.validate-ignore';

			var existingHighlight = settings.highlight;
			settings.highlight = function (element, errorClass, validClass) {
				existingHighlight(element, errorClass, validClass);

				var $element = $(element);
				$element.closest('.form-group:not(.has-error)').addClass('has-error');

				$element.parents('.icjia-error-group:not(.has-group-error)').addClass('has-group-error');

				var bookmarkId = $(element).closest('.icjia-bookmark[id!=""]').attr('id');
				if (undefined != bookmarkId)
					$('.icjia-nav-stacked li li:has(a[href="#' + bookmarkId + '"]):not(.has-error)').addClass('has-error');
			};

			var existingUnhighlight = settings.unhighlight;
			settings.unhighlight = function (element, errorClass, validClass) {
				existingUnhighlight(element, errorClass, validClass);

				var $element = $(element);
				$element.closest('.form-group.has-error:not(:has(.input-validation-error,.field-validation-error))').removeClass('has-error');

				$element.parents('.icjia-error-group.has-group-error:not(:has(.input-validation-error,.field-validation-error))').removeClass('has-group-error');

				var bookmarkId = $element.closest('.icjia-bookmark[id!=""]:not(:has(.input-validation-error,.field-validation-error))').attr('id');
				if (undefined != bookmarkId)
					$('.icjia-nav-stacked li li.has-error:has(a[href="#' + bookmarkId + '"])').removeClass('has-error');
			};

			var existingFocusInvalid = validator.focusInvalid;
			validator.focusInvalid = function() {
				existingFocusInvalid.call(validator);
				ensureActiveVisible();
			};

			settings.submitHandler = function (form, event) {
				var e = jQuery.Event("submit-valid.icjia");
				$(form).trigger(e);
				if (e.isDefaultPrevented()) 
					return false;

				preventDuplicateRequest = true;
				$("form").each(function () {
					if ($(this).valid() == false) { preventDuplicateRequest = false; }
				});

				return true;
			};
		}
	});
}

function styleExistingErrors(form$, detectVisibleErrorsOnly) {
	var errorSelector = '.input-validation-error, .field-validation-error';
	if (detectVisibleErrorsOnly)
		errorSelector = '.input-validation-error:visible, .field-validation-error:visible';

	var $errorElements = $(form$).find(errorSelector);

	$errorElements.closest('.form-group').addClass('has-error');

	$errorElements.parents(".icjia-error-group:not(.has-group-error)").addClass('has-group-error');

	$errorElements.closest('.icjia-bookmark[id!=""]').each(function () {
		var bookmarkId = $(this).attr('id');
		if (undefined != bookmarkId)
			$('.icjia-nav-stacked li li:has(a[href="#' + bookmarkId + '"]):not(.has-error)').addClass('has-error');
	});
}

function reparseUnobtrusiveValidation(form$) {
	$(form$).removeData("validator")
			 .removeData("unobtrusiveValidation")
			 .off("submit.validate click.validate focusin.validate focusout.validate keyup.validate invalid-form.validate");
	$.validator.unobtrusive.parse(form$);
}

function rescanUnobtrusiveValidation(form$, styleVisibleErrorsOnly) {
	styleRequiredFields(form$);
	reparseUnobtrusiveValidation(form$);
	enhanceValidators(form$);
	styleExistingErrors(form$, styleVisibleErrorsOnly);
}

//logic here should match styleExistingErrors
function pruneGroupErrors(detectVisibleErrorsOnly) {
	var errorSelector = '.input-validation-error, .field-validation-error';
	if (detectVisibleErrorsOnly)
		errorSelector = '.input-validation-error:visible, .field-validation-error:visible';

	$('.has-group-error').each(function () {
		var $this = $(this);
		if ($this.find(errorSelector).length == 0)
			$this.removeClass('has-group-error');
	});
}

// adds support for mandatory checkboxes (as 'required' does not support checkboxes).
$.validator.unobtrusive.adapters.addBool('mandatory', 'required');

function removePreventDuplicateRequestDisable() {
    $('[data-icjia-role="preventDuplicateRequest"]').removeAttr('data-icjia-role');
}