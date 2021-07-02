var startFocus = false;

hideShowDeleteButton('#newAggregates');

$(function () {

	reformatDateRanges();

	// ------ Adding and Deleting Reccords in the addForm ------ //
	// -- Add new record row
	$('.addAggregateClient').on("click", function (e) {
		e.preventDefault();
		AddNewAggregateRecord();
	});
	// -- Delete new record row
	$("#newAggregates").on("click", ".delete", function () {
		$(this).closest('tr').remove();
		reorderRowIndexes('newAggregateRow', 'newAggregateRowData', $('#recordCount').val());
		hideShowDeleteButton('#newAggregates');
	});

	$('#newAggregates').on('input blur', 'input[name$="HMSDate"]', function () {
		var $this = $(this);
		var value = $this.val();
		var isValidatable = (value != "" && value != null);
		$this.attr('data-val', isValidatable);
		if (validateDateInput($this)) {
			$this.closest('tr').find(':input').prop('disabled', false);
		}
		else {
			// -- When date field is empty, disable row except date input
			$this.closest('tr').find('select, input').not($this).prop('disabled', true);
			$this.prop('disabled', false);
		}
	});

	// ------ Setting the selection to the selected value ------ //
	$('#newAggregates').on('change', 'select[name$="TypeID"]', function () {
		var value = $(this).val();
		$(this).find('option:selected').attr('selected', 'selected');
	});

	$('#saveButton').click(function () {
		submitMyFormsAggregate();
	});

	// -- Clean addForm before submission to prevent unwanted dialog popups
	$(document).on('submit-valid.icjia', 'form.main', function () {
		beforeSubmission();
	});
});

// ------ Vars for Delete Confirmation Modal ------ //
var confirmEditLookupListTitle = "Are you sure you want to do that?";
var confirmEditLookupListTextStart = "You've marked following for deletion:<ul>";
var confirmEditLookupListTextEnd = "</ul>If you continue, the records will be <strong>permanently deleted</strong>.";
var confirmEditLookupListConfirmButtonClass = "btn-danger";
var confirmEditLookupListDiaglogClass = "modal-dialog icjia-modal-danger";
var deletesConfirmed = false;

// -- This is called from the view when save is clicked.
function submitMyFormsAggregate() {
	rescanUnobtrusiveValidation('.main');
	removeEmptyRow($('#addForm'));
	var $newAggregates = $('#newAggregates').clone();

	if (validSearchEditForms($('#addForm'), $('#searchResults'))) {
		$newAggregates.find('[disabled]').prop('disabled', false); // Is this needed? Aren't all fields for new agg always enabled?
		$newAggregates.id = "newAggregatesPosted";
		$('#addFormPlaceholder').find('table').remove();
		$newAggregates.appendTo('#addFormPlaceholder');
		$('#addFormPlaceholder tbody tr').addClass('rowClass');
		// ------ Delete Confirmation Modal ------ //
		var $main = $('#searchResults');
		var cntRecordsMarkedForDeletion = $('.deleteRecord:checked').length;
		if (cntRecordsMarkedForDeletion > 0) {
			var recordDeleteMessage = confrimCreateListItem(cntRecordsMarkedForDeletion, 'Aggregate Client Information');

			$.confirm({
				title: confirmEditLookupListTitle,
				text: confirmEditLookupListTextStart + recordDeleteMessage + confirmEditLookupListTextEnd,
				confirmButtonClass: confirmEditLookupListConfirmButtonClass,
				dialogClass: confirmEditLookupListDiaglogClass,
				confirm: function () {
					$main.submit();
				},
				cancel: function () {
					return false;
				}
			});
		} else {
			$main.submit();
		}
	}
	$('.input-validation-error').first().focus();

	return false;
}

// ------ Cleans up the Search Results ------ //
function beforeSubmission() {
	$('form[data-icjia-role="dirty.form"]').dirtyForms('setClean');
	$('#searchResults').find(':disabled').prop('disabled', false);
	reorderRowIndexes('rowClass', 'rowData');
}

// ------ Ajax to Partial View to add a new row ------ //
function AddNewAggregateRecord() {
	var addCount = $('#addCount').val();
	$.ajax({
		url: "/AggregateInformation/AddNewRecord?addCount=" + addCount,
		success: function (partialViewResult) {
			$('#newAggregates tbody').append(partialViewResult);
			$('#newAggregates tbody tr:last-child input[name$="HMSDate"]').focus();
		},
		complete: function () {
			reorderRowIndexes('newAggregateRow', 'newAggregateRowData', $('#recordCount').val());
			hideShowDeleteButton('#newAggregates');
		},
		error: function (xhr, ajaxOptions, thrownError) {
			//console.log(xhr);
			//PRC TODO add jquery confim message
			alert('There was an error while adding a new client.');
		}
	});
}

function validateDateInput($element) {
	var value = $element.val();
	var regExp = /^(\d{1,2})[\/\-](\d{1,2})[\/\-](\d{1,4})$/;
	if (!(value == '' || value == null) && value.match(regExp)) {
		return true;
	}
	else {
		return false;
	}
}