$(function () {
	rescanUnobtrusiveValidation("#main");
});

$(document.body).on('click', "#cancellationsSearch,#cancellationsReset", function () {
	cancellationsDeletesCheckedConfirm(this.id == 'cancellationsReset' ? true : false);
});

function cancellationsSearch($searchButton) {
	var elem = $searchButton;
	elem.addClass('icjia-spinner-active');
	$.ajax({
		url: '/Case/CancellationsSearch',
		type: 'GET',
		datatype: 'html',
		data: { ClientId: $("#clientId").val(), CaseId: $("#caseId").val(), fromDate: $("#cancellationsSearchDates_0__CancellationsDateRangeStart").val(), toDate: $("#cancellationsSearchDates_0__CancellationsDateRangeEnd").val() },
		success: function (data) {
			$('#cancellationsTableEdit').closest("div[id='searchResult']").html(data);
			rescanUnobtrusiveValidation("#main");
		},
		error: function (jqXHR, textStatus, errorThrown) {
		}
	}).always(function () {
		elem.removeClass('icjia-spinner-active');
	});
}

function cancellationsCheckIfEmpty(getThis, group) {

	if ($("#cancellationsDetailTableAdd").find('tbody tr:visible').last().find("[name$='.ServiceID']").prop('name') == $(getThis).closest('tr').find("[name$='.ServiceID']").prop('name')) {
		cntNoShowRow = cntEmptyFields("[name$='.ReasonID'],[name$='.SVID'],[name$='.Date']", getThis);

		if (cntNoShowRow > 0) {//If errors in row don't add a new row.
			//$(getThis).closest('tr').find("[name$='.ReasonID'],[name$='.SVID'],[name$='.Date'] ").each(function () {
			//	if ($(this).prop('disabled') == false) {
			//		$(this).valid();
			//	}
			//});
		} else {
			var isAddedHiddenFld = $(getThis).closest('tr').find("[id$='IsAdded']");
			isAddedHiddenFld.val(true);
			cancellationsDetailTableAddRow(isAddedHiddenFld);
			setTimeout(function () {
				//$('#cancellationsDetailTableAdd tr:last-child').find('select:first').focus();
			}, 60);
			rescanUnobtrusiveValidation("#main");
		}
	}
}

function cancellationsCheckboxDisableRow(checkBox) {
	var $trElement = $(checkBox).closest("tr");
	$trElement.find(":input:visible").not("[name$='.Index'], [name$='.LocationID'], [name$='.ID'], [name$='.IsEdited'], [name$='.IsDeleted']").attr("disabled", true);

	$.when($trElement.find(":input:visible").not("[name$='.Index'], [name$='.LocationID'], [name$='.ID'], [name$='.IsEdited'], [name$='.IsDeleted']").attr("disabled", true)).then(function (x) {
		$trElement.find(":input:visible[name$='.ServiceID']").val($trElement.find("input:hidden[name$='.ServiceID']").val());
		$trElement.find(":input:visible[name$='.ReasonID']").val($trElement.find("input:hidden[name$='.ReasonID']").val());
		$trElement.find(":input:visible[name$='.SVID']").val($trElement.find("input:hidden[name$='.SVID']").val());
		$trElement.find(":input:visible[name$='.Date']").val($trElement.find("input:hidden[name$='.Date']").val().blur());
	});
}

function cancellationsDetailTableAddRow(isAddedHiddenFld) {
	var nextIndex = ($("table#cancellationsDetailTableAdd > tbody > tr").length);
	$.ajax({
		url: '/Case/CancellationsAdd',
		type: 'GET',
		data: { index: nextIndex },
		success: function (results) {
			$("table#cancellationsDetailTableAdd tbody").append(results);
			isAddedHiddenFld.val(true);
			rescanUnobtrusiveValidation("#main");
		},
		error: function (jqXHR, textStatus, errorThrown) {
	}
	});

}

function cancellationsDeletesCheckedConfirm(resetCancellationsDateRange) {
	var cancellationsMessageDelete = cancellationsMessageEdit = "";

	var cancellationsCheckedEdited = $("#cancellationsTableEdit input[name$='.IsEdited']:checked").length;
	var cancellationsCheckedDeleted = $("#cancellationsTableEdit input[name$='.IsDeleted']:checked").length;

	if (cancellationsCheckedEdited > 0) {
		cancellationsMessageEdit = confrimCreateListItemSearch(cancellationsCheckedEdited, "Cancellation/No Show Record", true);
	}

	if (cancellationsCheckedDeleted > 0) {
		cancellationsMessageDelete = confrimCreateListItemSearch(cancellationsCheckedDeleted, "Cancellation/No Show Record", false);
	}

	if (cancellationsCheckedEdited > 0 || cancellationsCheckedDeleted > 0) {
		$.confirm({
			title: confirmEditServicesTitle,
			text: confirmEditServicesTextStart + cancellationsMessageEdit + cancellationsMessageDelete + confirmEditServicesTextEndSearch,
			confirmButton: confirmEditServicesConfirmButton,
			cancelButton: confirmEditServicesCancelButton,
			confirmButtonClass: confirmEditServicesConfirmButtonClass,
			dialogClass: confirmEditServicesDiaglogClass,
			confirm: function () {
				if (resetCancellationsDateRange) {
					cancellationsDateRangeReset();
				}
				cancellationsSearch($('#cancellationsSearch'));
			},
			cancel: function () {
				// nothing to do
			}
		});
	} else {
		if (resetCancellationsDateRange) {
			cancellationsDateRangeReset();
		}
		cancellationsSearch($('#cancellationsSearch'));
	}
}

function cancellationsDateRangeReset() {
	$("input.cancellationsDateRange").val("");
	$("select.cancellationsDateRange").val(0);
}

$(document.body).on('focusout', "[name^='Cancellations'][name$='.Date']", function () {
	refreshSelectGetStaffRetainCurrentSvid($(this).closest('tr').find("[name$='.SVID']"), $(this));
});