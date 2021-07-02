$(document).ready(function () {
    removePreventDuplicateRequestDisable();

    $('select[data-icjia-role="dateRanges"]').val(7).change();

	$('input[name="ReportSelectionType"]').change(function () {
		$('.reportProperty').hide();
		var childSelector = $(this).data('icjia-child-id');
		if (childSelector != undefined)
			$(childSelector).show();
		pruneGroupErrors(true);
		styleExistingErrors('#main', true);
	});

	$('#Generate').click(function (e) {
	    e.preventDefault();
        if (ValidateExceptionForm()) {
            $(":input:hidden[name != '__RequestVerificationToken']").attr("disabled", true);
            $('#main').submit();
            $(":input:disabled").attr("disabled", false);
        }
	});

	$('#selectAllButton').click(function () {
		$('input[name="UNRUDataFieldsSelections"]').prop('checked', true);
	});

	$('#deselectAllButton').click(function () {
		$('input[name="UNRUDataFieldsSelections"]').prop('checked', false);
	});
});

$(document).on('change', 'input[name="CenterIds"]', function () {
	$('#CenterIdsHelp').removeClass('field-validation-error').addClass('hide');
	pruneGroupErrors(true);
});

$(document).on('change', 'input[name="UNRUDataFieldsSelections"]', function () {
	$('#UNRUDataFieldsSelectionsHelp').removeClass('field-validation-error').addClass('hide');
	pruneGroupErrors(true);
});

function ValidateExceptionForm() {
	var result = $('#main').valid();
	if (!$('input[name="CenterIds"]:checked').length) {
		$('#CenterIdsHelp').addClass('field-validation-error').removeClass('hide');
		styleExistingErrors('#main', true);
		$('input[name="CenterIds"]:first').focus();
		result = false;
	}
	if ($('#selection49').is(':checked') && $('input[name="UNRUDataFieldsSelections"]:checked').length === 0) {
		$('#UNRUDataFieldsSelectionsHelp').addClass('field-validation-error').removeClass('hide');
		styleExistingErrors('#main', true);
		if (result)
			 $('input[name="UNRUDataFieldsSelections"]:first').focus();
		result = false;
	}

	if ($('.input-daterange:visible input:eq(1)') && $('.input-daterange:visible input:eq(0)') && new Date($('.input-daterange:visible input:eq(1)').val()) < new Date($('.input-daterange:visible input:eq(0)').val())) {
	    ErrorFormat($('.input-daterange:visible input:eq(0)').attr('name'), $('.input-daterange:visible input:eq(0)').attr('name'), "Start Date cannot be greater than End Date.");
	    if (result)
	        $('.input-daterange:visible input:eq(0)').focus();
        result = false;
	}

	return result;
}