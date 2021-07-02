$(function () {
	jQuery.validator.addMethod('betweenjulytwothousandeighttoday', function (value, element, params) {
		var entry = Date.parse(value);
		return !entry || Date.parse("07/01/2008") <= entry && entry <= new Date();
	}, '');

	jQuery.validator.unobtrusive.adapters.add('betweenjulytwothousandeighttoday', function (options) {
		options.rules['betweenjulytwothousandeighttoday'] = {};
		options.messages['betweenjulytwothousandeighttoday'] = options.message;
	});

}(jQuery));