$(function () {
	jQuery.validator.addMethod('firstcontactdate', function (value, element, params) {
		if ($('#FirstContactDate').val() == "") { return true; }
		return !(Date.parse(value) < Date.parse($('#FirstContactDate').val()));
	}, '');

	jQuery.validator.unobtrusive.adapters.add('firstcontactdate', function (options) {
		options.rules['firstcontactdate'] = {};
		options.messages['firstcontactdate'] = options.message + ' (' + $('#FirstContactDate').val() + ')';
	});

}(jQuery));