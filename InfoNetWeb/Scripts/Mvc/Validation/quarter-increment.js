$(function () {
	jQuery.validator.addMethod('quarterincrement', function (value, element, params) {
		return value % 0.25 == 0;
	}, '');

	jQuery.validator.unobtrusive.adapters.add('quarterincrement', function (options) {
		options.rules['quarterincrement'] = {};
		options.messages['quarterincrement'] = options.message;
	});

}(jQuery));