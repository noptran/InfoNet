$(function () {
	jQuery.validator.addMethod('fileextension', function(value, element, params) {
		if (value == undefined || value == '')
			return true;

		var fileExtensions = params.allowedextensions.split(',');
		for (let i = 0; i < fileExtensions.length; i++) {
			fileExtensions[i] = fileExtensions[i].toLowerCase().replace(/^\./, '');
		}
		return $.inArray(value.split('.').pop().toLowerCase(), fileExtensions) != -1;
	}, '');

	jQuery.validator.unobtrusive.adapters.add('fileextension', ['allowedextensions'], function (options) {
		options.rules['fileextension'] = options.params;
		options.messages['fileextension'] = options.message;
	});
}(jQuery));