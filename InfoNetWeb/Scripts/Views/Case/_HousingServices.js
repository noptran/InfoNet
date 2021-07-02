$(function () {
	rescanUnobtrusiveValidation("#main");
});

$(document.body).on('click', "#housingServiceReset,#housingServiceSearch", function () {
	housingServiceDeletesCheckedConfirm(this.id == 'housingServiceReset' ? true : false);
});

function housingServiceSearch($searchButton) {
	var elem = $searchButton;
	elem.addClass('icjia-spinner-active');
	$.ajax({
		url: '/Case/HousingServicesSearch',
		type: 'GET',
		datatype: 'html',
		data: { ClientId: $("#clientId").val(), CaseId: $("#caseId").val(), fromDate: $("#housingServiceSearchDates_0__HousingServiceDateRangeStart").val(), toDate: $("#housingServiceSearchDates_0__HousingServiceDateRangeEnd").val() },
		success: function (data) {
			$('#housingServiceTableEdit').closest("div[id='searchResult']").html(data);
			rescanUnobtrusiveValidation("#main");
		},
		error: function (jqXHR, textStatus, errorThrown) {
			//alert(errorThrown);
		}
	}).always(function () {
		elem.removeClass('icjia-spinner-active');
	});
}

function housingServiceCheckIfEmpty(getThis, group) {
	if ($("#housingServiceDetailTableAdd").find('tbody tr:visible').last().find("[name$='.ServiceID']").prop('name') == $(getThis).closest('tr').find("[name$='.ServiceID']").prop('name')) {
		cntNoShowRow = cntEmptyFields("[name$='.ShelterBegDate'],[name$='.ServiceID']", getThis);

		if (cntNoShowRow > 0) {//If errors in row don't add a new row.
			$(getThis).closest('tr').find("[name$='.ServiceID'],[name$='.ShelterBegDate'],[name$='.ShelterEndDate'] ").each(function () {
				if ($(this).prop('disabled') == false) {
					//$(this).valid();
				}
			});
		} else {
			var isAddedHiddenFld = $(getThis).closest('tr').find("[id$='IsAdded']");
			isAddedHiddenFld.val(true);

			housingServiceDetailTableAddRow(isAddedHiddenFld);
			setTimeout(function () {
				//$('#housingServiceDetailTableAdd tr:last-child').find('select:first').focus();
			}, 60);
			rescanUnobtrusiveValidation("#main");
		}
	}
}

function housingServiceCheckboxDisableRow(checkBox) {
	var $trElement = $(checkBox).closest("tr");
	$trElement.find(":input:visible").not("[name$='.Index'], [name$='.ServiceDetailID'], [name$='.ICS_ID'], [name$='.IsEdited'], [name$='.IsDeleted']").attr("disabled", true);

	$.when($trElement.find(":input:visible").not("[name$='.Index'], [name$='.ServiceDetailID'], [name$='.ICS_ID'], [name$='.IsEdited'], [name$='.IsDeleted']").attr("disabled", true)).then(function (x) {
		$trElement.find(":input:visible[name$='.ServiceID']").val($trElement.find("input:hidden[name$='.ServiceID']").val());
		$trElement.find(":input:visible[name$='.ShelterBegDate']").val($trElement.find("input:hidden[name$='.ShelterBegDate']").val().blur());
		$trElement.find(":input:visible[name$='.ShelterEndDate']").val($trElement.find("input:hidden[name$='.ShelterEndDate']").val().blur());
	});
}

function housingServiceDetailTableAddRow(isAddedHiddenFld) {
	var nextIndex = ($("table#housingServiceDetailTableAdd > tbody > tr").length);
	$.ajax({
		url: '/Case/HousingServicesAdd',
		type: 'GET',
		data: { index: nextIndex },
		success: function (results) {
			$("table#housingServiceDetailTableAdd tbody").append(results);
			isAddedHiddenFld.val(true);
			rescanUnobtrusiveValidation("#main");
		},
		error: function (jqXHR, textStatus, errorThrown) {
			//alert(errorThrown);
	}
	});

}

function housingServiceDeletesCheckedConfirm(resetHousingServiceDateRange) {
	var housingServicesMessageDelete = housingServicesMessageEdit = "";

	var housingServicesCheckedEdited = $("#housingServiceTableEdit input[name$='.IsEdited']:checked").length;
	var housingServicesCheckedDeleted = $("#housingServiceTableEdit input[name$='.IsDeleted']:checked").length;

	if (housingServicesCheckedEdited > 0) {
		housingServicesMessageEdit = confrimCreateListItemSearch(housingServicesCheckedEdited, "Housing Service Record", true);
	}

	if (housingServicesCheckedDeleted > 0) {
		housingServicesMessageDelete = confrimCreateListItemSearch(housingServicesCheckedDeleted, "Housing Service Record", false);
	}

	if (housingServicesCheckedEdited > 0 || housingServicesCheckedDeleted > 0) {
		$.confirm({
			title: confirmEditServicesTitle,
			text: confirmEditServicesTextStart + housingServicesMessageEdit + housingServicesMessageDelete + confirmEditServicesTextEndSearch,
			confirmButton: confirmEditServicesConfirmButton,
			cancelButton: confirmEditServicesCancelButton,
			confirmButtonClass: confirmEditServicesConfirmButtonClass,
			dialogClass: confirmEditServicesDiaglogClass,
			confirm: function () {
				if (resetHousingServiceDateRange) {
					housingServiceDateRangeReset();
				}
				housingServiceSearch($('#housingServiceSearch'));
			},
			cancel: function () {
				// nothing to do
			}
		});
	} else {
		if (resetHousingServiceDateRange) {
			housingServiceDateRangeReset();
		}
		housingServiceSearch($('#housingServiceSearch'));
	}
}

function housingServiceDateRangeReset() {
	$("input.housingServicesDateRange").val("");
	$("select.housingServicesDateRange").val(0);
}

