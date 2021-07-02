$(function () {
	jQuery.validator.addMethod('wholenumber', function (value, element, params) {
		return value %1 == 0;
	}, '');

	jQuery.validator.unobtrusive.adapters.add('wholenumber', function (options) {
		options.rules['wholenumber'] = {};
		options.messages['wholenumber'] = options.message;
	});

}(jQuery));