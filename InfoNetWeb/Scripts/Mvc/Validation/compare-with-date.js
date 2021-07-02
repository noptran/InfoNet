$(function () {
	jQuery.validator.addMethod('comparewithdate', function (value, element, params) {

		var otherFieldName = $(element).attr('name').replace(params.propertyname, params.otherpropertyname);

		var date = new Date(value);
		var otherDate = new Date($('input[name="' + otherFieldName + '"').val());

        // If either date field does not have a value, pass validation
        if (isNaN(date.getTime()) || isNaN(otherDate.getTime())) return true;

		if (params.comparisontype === 'GreaterThan')
			return date > otherDate;
		else if (params.comparisontype === 'GreaterThanEqualTo')
			return date >= otherDate;
		else if (params.comparisontype === 'LessThan')
			return date < otherDate;
		else if (params.comparisontype === 'LessThanEqualTo')
			return date <= otherDate;

		return true;
	}, '');

	jQuery.validator.unobtrusive.adapters.add('comparewithdate', ['propertyname', 'otherpropertyname', 'comparisontype'], function (options) {
		options.rules['comparewithdate'] = options.params;
		options.messages['comparewithdate'] = options.message;
	});

}(jQuery));