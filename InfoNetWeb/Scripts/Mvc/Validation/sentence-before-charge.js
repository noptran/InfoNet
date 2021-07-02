$(function () {
	jQuery.validator.addMethod('sentencebeforecharge', function (value, element, params) {
		$chargeDate = $(element).closest("[id$='Collapse']").find("[name$='.ChargeDate']");
		var entry = Date.parse(value);
		return !entry || !Date.parse($chargeDate.val()) || Date.parse(value) > Date.parse($chargeDate.val());
	}, '');

	jQuery.validator.unobtrusive.adapters.add('sentencebeforecharge', function (options) {
		options.rules['sentencebeforecharge'] = {};
		options.messages['sentencebeforecharge'] = options.message;
	});

}(jQuery));