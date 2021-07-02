$(function () {
	jQuery.validator.addMethod('betweennineteenseventytoday', function (value, element, params) {
		var entry = Date.parse(value);
		return !entry || Date.parse("01/01/1970") <= entry && entry <= new Date();
	}, '');

	jQuery.validator.unobtrusive.adapters.add('betweennineteenseventytoday', function (options) {
		options.rules['betweennineteenseventytoday'] = {};
		options.messages['betweennineteenseventytoday'] = options.message;
	});

}(jQuery));