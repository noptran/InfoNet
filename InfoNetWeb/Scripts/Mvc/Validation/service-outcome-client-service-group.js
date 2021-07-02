$(function () {
	jQuery.validator.addMethod('serviceoutcomeclientservicegroup', function (value, element, params) {
		var notEmpty = yesNo = 0;
		var addCount = $(element).closest('tr').data('addcount');

		if (addCount < 999) {
			$(".newOutcomeRow[data-addcount='" + addCount + "']").find("[name$='.ResponseYes'],[name$='.ResponseNo']").each(function () {
				!$(this).val() && notEmpty++;
				yesNo++;
			});
		}

		if (notEmpty == yesNo) {
			return false;
		} else {
			return true;
		}
	}, '');
	
	jQuery.validator.unobtrusive.adapters.add('serviceoutcomeclientservicegroup', function (options) {
		options.rules['serviceoutcomeclientservicegroup'] = {};
		options.messages['serviceoutcomeclientservicegroup'] = options.message;
	});

}(jQuery));