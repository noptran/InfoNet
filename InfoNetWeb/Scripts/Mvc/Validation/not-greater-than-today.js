$(function () {
	jQuery.validator.addMethod('notgreaterthantoday', function (value, element, params) {
		return !(Date.parse(value) > new Date());
	}, '');

	jQuery.validator.unobtrusive.adapters.add('notgreaterthantoday', function (options) {
		options.rules['notgreaterthantoday'] = {};
		options.messages['notgreaterthantoday'] = options.message;
	});

}(jQuery));