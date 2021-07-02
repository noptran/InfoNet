$(function () {
	$(".editRecord").on("change", function () {
		if ($(this).prop('checked') == false) { clearRowValidationErrors(this, $("#searchResults")); }
	});

	$(".deleteRecord").on("change", function () {
		if ($(this).prop('checked')) { clearRowValidationErrors(this, $("#searchResults")); }
	});

	$("#searchResults").on("change", ".editRecord", function () {
		var $parent = $(this).closest('tr');
		$parent.find(':input:visible').not('.editRecord, .deleteRecord').attr('disabled', !($(this).is(':checked')));
		$parent.find('input[name$=".shouldDelete"]').val(false);
		enforceRadio(".deleteRecord", $parent);
	});

	$("#searchResults").on("change", ".deleteRecord", function () {
		var $parent = $(this).closest('tr');
		$parent.find(':input:visible').not('.editRecord, .deleteRecord').attr('disabled', true);
		$parent.find('input[name$=".shouldDelete"]').val($(this).is(':checked'));
		enforceRadio(".editRecord", $parent);
	});
});

function reformatDateRanges() {
	$('input[name$="StartDate"]').val(formatDate($('input[name$="StartDate"]').val()));
	$('input[name$="EndDate"]').val(formatDate($('input[name$="EndDate"]').val()));
}

function hideShowDeleteButton(selector) {
	($(selector).find('tr').length <= 2) ? $(selector).find('.delete').hide() : $(selector).find('.delete').show();
}

function enforceRadio(selector, $parent) {
	$parent.find(selector).prop('checked', false);
}

function clearRowValidationErrors(getThis, $form) {
	var $parentRow = $(getThis).parents('tr');
	$parentRow.find('span.field-validation-error').each(function () {
		$parentRow.find(':input').removeClass('input-validation-error');
		$parentRow.find('.input-group').removeClass('has-error');
		$(this).parentsUntil('has-error').removeClass('has-error');
		$(this).removeClass('field-validation-error').html('');
	});
	$parentRow.find(':input').dirtyForms('setClean');
	var $validator = $form.validate();
	$validator.resetForm();
}

function countEmptyFields($tableRow) {
	var cntNoShowRow = 0;
	$tableRow.find(":input").each(function () {
		if ($(this).is('select') && $('option:selected', this).index() == 0) { cntNoShowRow++; }
		if (($(this).attr('type') == 'text' || $(this).attr('type') == 'number') && $(this).val().length == 0) { cntNoShowRow++; }
	});
	return cntNoShowRow;
}

function removeEmptyRow($table) {
	$table.find('tbody tr').each(function () {
		if (countEmptyFields($(this)) == $(this).find($("input")).length) { $(this).remove(); }
	});
}

function reorderRowIndexes(rowClass, rowData, recordCount) {
	var currentname, newname, currentid, newid, aria;

	if (recordCount !== undefined)
		recordCount = parseInt(recordCount);

	$("." + rowClass).each(function (index) {
		if (recordCount !== undefined) { index = index + recordCount; }

		$(this).find('span.help-block').each(function () {
			currentname = $(this).attr('data-valmsg-for');
			newname = currentname.substr(0, currentname.indexOf('[') + 1) + index + currentname.substr(currentname.indexOf(']'), currentname.length - 1);
			$(this).attr('data-valmsg-for', newname);
		});

		$(this).find('.' + rowData + '[name]').each(function () {
			currentname = $(this).attr('name');
			currentid = $(this).attr('id');
			aria = $(this).attr('aria-describedby');

			newname = currentname.substr(0, currentname.indexOf('[') + 1) + index + currentname.substr(currentname.indexOf(']'), currentname.length - 1);
			newid = currentid.substr(0, currentid.indexOf('_') + 1) + index + currentid.substr(currentid.indexOf('__'), currentid.length - 1);
			if (aria != undefined)
				$(this).attr('aria-describedby', aria.substr(0, currentid.indexOf('_') + 1) + index + currentid.substr(currentid.indexOf('__'), currentid.length - 1));

			$(this).attr('name', newname);
			$(this).attr('id', newid);
		});
	});
	rescanUnobtrusiveValidation('.main');
}

function validSearchEditForms($firstForm, $secondForm) {
	var firstFormValid = $firstForm.valid();
	var secondFormValid = $secondForm.valid();
	return (firstFormValid == true && secondFormValid == true ? true : false);
}

function submitMyForm($form) {
	beforeFormSubmission();
	$('form[data-icjia-role="dirty.form"]').addClass('dirtyignore');
	$('form[data-icjia-role="dirty.form"]').dirtyForms('setClean');
	$form.submit();
}

function hideTable(selector) {
	if ($(selector).find('tr').length <= 1) {
		$(selector).hide();
	}
}

function deleteButtonClassFix($records, $newRecord) {
	$records.each(function (index) {
		$(this).find('td.delete > button').removeClass('deleteButton');
	});
	$newRecord.find('tr').last().find('td.delete > button').addClass('deleteButton');
}

function confrimCreateListItem(cntMarkedForDeletion, title, suffixPlural, suffixSingular) {
	if (suffixPlural === undefined) { suffixPlural = 's'; }
	if (suffixSingular === undefined) { suffixSingular = ''; }
	return "<li class='text-danger' style='font-weight:bold; list-style: square;'>" + cntMarkedForDeletion + " " + $.trim(title) + ((cntMarkedForDeletion > 1) ? suffixPlural : suffixSingular) + "</li>";
}

$(function () {
	jQuery.validator.addMethod('serviceoutcomeyesno', function (value, element, params) {
		if (($(element).val()) == '') {
			var name = $(element).attr("name");
			if (name.match(".ResponseYes$") && $.isNumeric($(element).closest('tr').find("[name$='.ResponseNo']").val())) 
				return false;
			if (name.match(".ResponseNo$") && $.isNumeric($(element).closest('tr').find("[name$='.ResponseYes']").val())) 
				return false;
		} 
		return true;
	}, '');

	jQuery.validator.unobtrusive.adapters.add('serviceoutcomeyesno', function (options) {
		options.rules['serviceoutcomeyesno']= { };
		options.messages['serviceoutcomeyesno']= options.message;
	});

}(jQuery));