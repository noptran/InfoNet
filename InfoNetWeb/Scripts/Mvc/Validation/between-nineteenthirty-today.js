$(function () {
	jQuery.validator.addMethod('betweennineteenthirtytoday', function (value, element, params) {
		var entry = Date.parse(value);
		return !entry || Date.parse("01/01/1930") <= entry && entry <= new Date();
	}, '');

	jQuery.validator.unobtrusive.adapters.add('betweennineteenthirtytoday', function (options) {
		options.rules['betweennineteenthirtytoday'] = {};
		options.messages['betweennineteenthirtytoday'] = options.message;
	});

}(jQuery));