$(function () {
	jQuery.validator.addMethod('notlessthannineteenseventy', function (value, element, params) {
		return !(Date.parse(value) < Date.parse("01/01/1970"));
	}, '');

	jQuery.validator.unobtrusive.adapters.add('notlessthannineteenseventy', function (options) {
		options.rules['notlessthannineteenseventy'] = {};
		options.messages['notlessthannineteenseventy'] = options.message;
	});

}(jQuery));