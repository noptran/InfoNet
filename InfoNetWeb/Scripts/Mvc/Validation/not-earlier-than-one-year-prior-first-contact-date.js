$(function () {
	jQuery.validator.addMethod('firstcontactdateyearbefore', function (value, element, params) {
		if ($('#FirstContactDate').val() == "") { return true; }

		var firstContactDate = new Date($('#FirstContactDate').val());
		firstContactDate.setFullYear(firstContactDate.getFullYear() - 1);

		return !(Date.parse(value) < (Date.parse(firstContactDate)));
	}, '');

	jQuery.validator.unobtrusive.adapters.add('firstcontactdateyearbefore', function (options) {
		options.rules['firstcontactdateyearbefore'] = {};
		options.messages['firstcontactdateyearbefore'] = options.message + " (" + $('#FirstContactDate').val() + ") ";
	});

}(jQuery));