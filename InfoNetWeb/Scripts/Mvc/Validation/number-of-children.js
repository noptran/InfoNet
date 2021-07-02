$(function () {
	jQuery.validator.addMethod('numberofchildren', function (value, element, params) {
		if (!value) { return true; }
		if ($('#ProviderID').val() == 6) {
			return (value >= 0 && value <= 25);
		} else {
			return (value >= -1 && value <= 20);
		}
	}, '');

	jQuery.validator.unobtrusive.adapters.add('numberofchildren', function (options) {
		options.rules['numberofchildren'] = {};
		options.messages['numberofchildren'] = options.message;
	});

}(jQuery));