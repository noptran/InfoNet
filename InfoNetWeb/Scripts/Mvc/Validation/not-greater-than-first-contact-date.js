$(function () {
	jQuery.validator.addMethod('notgreaterthanfirstcontactdate', function (value, element, params) {
		if ($('#FirstContactDate').val() == "") { return true; }
		return !(Date.parse(value) > Date.parse($('#FirstContactDate').val()));
	}, '');

	jQuery.validator.unobtrusive.adapters.add('notgreaterthanfirstcontactdate', function (options) {
		options.rules['notgreaterthanfirstcontactdate'] = {};
		options.messages['notgreaterthanfirstcontactdate'] = options.message + ' (' + $('#FirstContactDate').val() + ')';
	});
}(jQuery));