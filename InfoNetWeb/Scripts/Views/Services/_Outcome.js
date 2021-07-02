var startFocus = false;

if ($('#Start').val() == ' ') { $('#Start').val(''); }
if ($('#End').val() == ' ')	  { $('#End').val(''); }

hideShowDeleteButton('#newOutcomes');

$(function () {
	reformatDateRanges();

	$('#outcomeAddNew').click(function () {
		AddNewOutcomeRecord();
	});

	//Delete new record row
	$("#newOutcomes").on("click", ".delete", function () {
		$('#newOutcomes').find('tr[data-addcount="' + $(this).closest('tr').attr('data-addcount') + '"]').dirtyForms('setClean');
		$('#newOutcomes').find('tr[data-addcount="' + $(this).closest('tr').attr('data-addcount') + '"]').remove();
		reorderRowIndexes('newOutcomeRow', 'newOutcomeRowData', $('#recordCount').val());
		hideShowDeleteButton('#newOutcomes');
	});

	//Add rows for all questions after a valid date and selection for serviceID
	$('.newOutcomes').on('change', 'select[name$="ServiceID"].newOutcomeRowData', function () {

		var serviceID = $(this).val();
		var $row = $(this).closest('tr');
		var addCount = $('#addCount').val();
		var rowAddCount = parseInt($row.attr('data-addCount'));
		var date = $row.find('input[name$="OutcomeDate"]').val();
		var regExp = /^(\d{1,2})[\/\-](\d{1,2})[\/\-](\d{1,4})$/;

		if (date.match(regExp) && !$row.find('input[name$="OutcomeDate"]').hasClass('input-validation-error')) {
			$.ajax({
				url: "/ServiceOutcome/GetQuestionsPartial?serviceID=" + serviceID + "&date=" + date + "&addCount=" + addCount,
				type: "GET",
				data: "json",
				success: function (partialViewResult) {
					$row.nextAll().each(function () {
						console.log("data: " + $(this).attr('data-addCount') + ", rowAddCount: " + rowAddCount);
						if (parseInt($(this).attr('data-addCount')) == rowAddCount && rowAddCount < 999) {
							$(this).remove();
						}
					});
					$(partialViewResult).insertAfter($row);
					$row.remove();
					$('.newOutcomes tbody').find('.removeDisabled').show().prop('disabled', false).removeClass('removeDisabled');
					var $select = $('.newOutcomes tbody').find('.newSelectList');
					$select.find('option').eq(0).remove();
					$select.removeClass('newSelectList');
					$('.newOutcomes tbody').find('.setFocus').removeClass('setFocus').focus();
					$('#addCount').val(parseInt(addCount) + 1);
				},
				complete: function () {
					reorderRowIndexes('newOutcomeRow', 'newOutcomeRowData', $('#recordCount').val());
				},
				error: function (xhr, ajaxSettings, thrownError) {
					//console.log(xhr.responseText);
				}
			});
		}
	});

	$('#newOutcomes').on('blur input', 'input[name$="OutcomeDate"]', function () {
		var value = $(this).val();
		var isValidatable = (value != "" && value != null);
		$(this).attr('data-val', isValidatable);
		validateDateInput($(this));
	});

	// Clean AddForm before submission to prevent unwanted dialog popups
	$(document).on('submit-valid.icjia', 'form.main', function (e) {
		$('form[data-icjia-role="dirty.form"]').dirtyForms('setClean');
		$('#searchResults').find(':disabled').prop('disabled', false);
		reorderRowIndexes('rowClass', 'rowData');
	});

	// Refresh survey question list when service group is changed in search results
	$('#searchResults select[name$="ServiceID"]').change(function () {

		var $this = $(this);
		var serviceID = $this.find('option:selected').val();
		var questionList = $this.closest('tr').find('select[name$="OutcomeID"]');

		$.ajax({
			url: "/ServiceOutcome/GetQuestionListForServiceID?serviceID=" + serviceID,
			type: "GET",
			data: "json",
			success: function (data) {
				//console.log(data);
				$(questionList).find('option').remove().end();
				$(questionList).append($('<option>').text("<Pick One>").attr('value', ""));
				$.each(data, function (i, question) {
					$(questionList).append($('<option>').text(question.Description).attr('value', question.CodeID));
				});
			},
			error: function (xhr) {
				//console.log(xhr.responseText);
			}
		});
	});

	$(document.body).on('focusout', "[name$='.ResponseYes'],[name$='.ResponseNo']", function () {
		if ($(this).closest('tr').hasClass('newOutcomeRow'))
			$(".newOutcomeRow[data-addcount='" + $(this).closest('tr').data('addcount') + "']").find($("[name$='.ServiceID']:visible:enabled")).valid();
	});
});

function submitMyForms() {
	rescanUnobtrusiveValidation('.main');
	$(".newOutcomeRow[data-addcount]").find($("[name$='.ServiceID']:visible:enabled")).each(function () {
		$(this).valid();
		return true;
	});
	//removeEmptyOutcomes($('#newOutcomes'));
	var $newOutcomes = $('#newOutcomes').clone();
	if (validSearchEditForms($('#addForm'), $('#searchResults'))) {
		removeBlankResponses($newOutcomes);
		$newOutcomes.find('[disabled]').prop('disabled', false);
		$newOutcomes.id = "newOutcomesPosted";
		$('#addFormPlaceholder').find('table').remove();
		$newOutcomes.appendTo('#addFormPlaceholder');
		var $main = $('#searchResults');
		rescanUnobtrusiveValidation('.main');
		var outcomesDeleted = $main.find("[name$='.shouldDelete']:checked").length;
		if (outcomesDeleted > 0) {
			var message = "You've marked the following for deletion from the database: <ul>";

			// append Residence message
			message = message + "<li class='text-danger' style='font-weight:bold; list-style: square;'><b>" + outcomesDeleted + " Service Outcome" + ((outcomesDeleted > 1) ? "s" : "") + "</b></li>";
			message = message + "</ul>If you continue, " + ((outcomesDeleted > 1) ? "these records" : "this record") + " will be <strong>permanently deleted</strong>.";

			$.confirm({
				title: "Are you sure you want to do that?",
				text: message,
				confirmButtonClass: "btn-danger",
				dialogClass: "modal-dialog icjia-modal-danger",
				confirm: function () {
					$main.submit();
				},
				cancel: function () {
					return false;
				}
			});
		}
		else {
			$main.submit();
		}
	}
	$('.input-validation-error').first().focus();

	return false;
}

function removeEmptyOutcomes($newOutcomes) {
	$newOutcomes.find('tbody tr').each(function () {
		var date = $(this).find('input[name$="OutcomeDate"]').val();
		var survey = $(this).find('select[name$="ServiceID"]').val();

		if ((date == null || date == "") && (survey == null || survey == "")) {
			$(this).remove();
		}
	});
}

function removeBlankResponses($newOutcomes) {
	$newOutcomes.find('tbody tr').each(function () {
		var yesVal = $(this).find('input[name$="ResponseYes"]').val();
		var noVal = $(this).find('input[name$="ResponseNo"]').val();

		if ((yesVal == null || yesVal == "") && (noVal == null || noVal == "")) {
			$(this).remove();
		} else {
			if (yesVal == null || yesVal == "") {
				$(this).find('input[name$="ResponseYes"]').val(0);
			}
			if (noVal == null || noVal == "") {
				$(this).find('input[name$="ResponseNo"]').val(0);
			}
			$(this).addClass('rowClass');
		}
	});
}

function AddNewOutcomeRecord() {
	var addCount = $('#addCount').val();
	$.ajax({
		url: "/ServiceOutcome/AddNewRecord",
		success: function (partialViewResult) {
			$('#newOutcomes tbody').append(partialViewResult);
			$('#newOutcomes tbody tr:last-child input[name$="OutcomeDate"]').focus();
		},
		complete: function () {
			reorderRowIndexes('newOutcomeRow', 'newOutcomeRowData', $('#recordCount').val());
			hideShowDeleteButton('#newOutcomes');
			rescanUnobtrusiveValidation('.main');
		},
		error: function (xhr, ajaxOptions, thrownError) {
			//KMS DO use growl
			alert('There was an error while adding a new client.');
		}
	});
}

function validateDateInput($element)
{
	var value = $element.val();
	var regExp = /^(\d{1,2})[\/\-](\d{1,2})[\/\-](\d{1,4})$/;
	if (!(value == '' || value == null) && value.match(regExp)) {
		// Valid Date
		var addCount = $element.closest('tr').attr('data-addcount');
		$element.closest('tr').find('select[name$="ServiceID"]').prop('disabled', false);
		if(addCount != 999){
			$('tbody tr[data-addcount="' + addCount + '"]').find('input[name$="OutcomeDate"]').val($element.val());
		}
	}
	else {
		$element.closest('tr').find('select[name$="ServiceID"], select[name$="OutcomeID"], input[name$="ResponseYes"], input[name$="ResponseNo"]').prop('disabled', true);
	}
}