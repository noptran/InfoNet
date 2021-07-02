$(function () {
	jQuery.validator.addMethod('servicegrouprequired', function (value, element, params) {
		var $closetTR = $(element).closest('tr');
		if (value == '') {
			if ($closetTR.find(':input[data-val-servicegrouprequired]').length != $closetTR.find(':input[data-val-servicegrouprequired][value=""],option:selected[value=""]').length) {
				return false;
			} else {
				$closetTR.find("[id$='IsAdded']").val(false);
			}
		} else {
			$closetTR.find("[id$='IsAdded']").val(true);
		}
		return true;
	}, '');
	
	jQuery.validator.unobtrusive.adapters.add('servicegrouprequired', function (options) {
		options.rules['servicegrouprequired'] = {};
		options.messages['servicegrouprequired'] = options.message;
	});

}(jQuery));