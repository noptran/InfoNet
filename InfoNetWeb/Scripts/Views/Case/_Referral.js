$("#referralAgencyAdd").tooltip({
	template: '<div class="tooltip" role="tooltip"><div class="tooltip-arrow"></div><div class="tooltip-inner" style="max-width: 600px;"></div></div>'
});

$(function () {
	rescanUnobtrusiveValidation("#main");

	$("#referralSearch").on('click', function (event) {
		event.preventDefault();
		referralDeletesCheckedConfirm(false);
	});

	$("#referralReset").on('click', function (event) {
		event.preventDefault();
		referralDeletesCheckedConfirm(true);
	});

});

function referralCheckIfEmpty(getThis, group) {
	if ($("#referralTableAdd").find('tbody tr:visible').last().find("[name$='.ReferralTypeID']").prop('name') == $(getThis).closest('tr').find("[name$='.ReferralTypeID']").prop('name')) {
		cntNoShowRow = cntEmptyFields("[name$='.ReferralDate'],[name$='.AgencyID'],[name$='.ResponseID']", getThis);

		if (cntNoShowRow > 0) {//If errors in row don't add a new row.
			//$(getThis).closest('tr').find("[name$='.ReferralDate'],[name$='.AgencyID'],[name$='.ReceivedHours'] ").each(function () {
			//	if ($(this).prop('disabled') == false) {
			//		$(this).valid();
			//	}
			//});
		} else {
			var isAddedHiddenFld = $(getThis).closest('tr').find("[id$='IsAdded']");
			isAddedHiddenFld.val(true);

			referralDetailTableAddRow(isAddedHiddenFld);
			setTimeout(function () {
				//$('#referralTableAdd tr:last-child').find('select:first').focus();
			}, 60);
			rescanUnobtrusiveValidation("#main");
		}
	}
}

function referralCheckboxDisableRow(checkBox) {
	var $trElement = $(checkBox).closest("tr");
	$trElement.find(":input:visible").not("[name$='.Index'], [name$='.ReferralDetailID'], [name$='.LocationID'], [name$='.IsEdited'], [name$='.IsDeleted']").attr("disabled", true);

	$.when($trElement.find(":input:visible").not("[name$='.Index'], [name$='.ReferralDetailID'], [name$='.LocationID'], [name$='.IsEdited'], [name$='.IsDeleted']").attr("disabled", true)).then(function (x) {
		$trElement.find(":input:visible[name$='.ReferralTypeID']").val($trElement.find("input:hidden[name$='.ReferralTypeID']").val());
		$trElement.find(":input:visible[name$='.ReferralDate']").val($trElement.find("input:hidden[name$='.ReferralDate']").val());
		$trElement.find(":input:visible[name$='.AgencyID']").val($trElement.find("input:hidden[name$='.AgencyID']").val());
		$trElement.find(":input:visible[name$='.ResponseID']").val($trElement.find("input:hidden[name$='.ResponseID']").val());
	});
}

function referralSearch($searchButton) {
	$searchButton.addClass('icjia-spinner-active');
	$.ajax({
		url: '/Case/ReferralSearch',
		type: 'GET',
		datatype: 'html',
		data: { ClientId: $("#clientId").val(), CaseId: $("#caseId").val(), fromDate: $("#referralSearchDates_0__ReferralDateRangeStart").val(), toDate: $("#referralSearchDates_0__ReferralDateRangeEnd").val() },
		success: function (data) {
			$('#referralTableEdit').closest("div[id='searchResult']").html(data);
			rescanUnobtrusiveValidation("#main");
		},
		error: function (jqXHR, textStatus, errorThrown) {
			alert(errorThrown);
		}
	}).always(function () {
		$searchButton.removeClass('icjia-spinner-active');
	});
}

function referralDetailTableAddRow(isAddedHiddenFld) {
	var nextIndex = ($("table#referralTableAdd > tbody > tr").length);
	$.ajax({
		url: '/Case/ReferralAdd',
		type: 'GET',
		data: { index: nextIndex, Provider: $("#providerId").val() },
		success: function (results) {
			$("table#referralTableAdd tbody").append(results);
			isAddedHiddenFld.val(true);
			rescanUnobtrusiveValidation("#main");
		}
	});

}

function referralDeletesCheckedConfirm(resetReferralDateRange) {
	var referralMessageDelete = referralMessageEdit = "";

	var referralCheckedEdited = $("#referralTableEdit input[name$='.IsEdited']:checked").length;
	var referralCheckedDeleted = $("#referralTableEdit input[name$='.IsDeleted']:checked").length;

	if (referralCheckedEdited > 0) {
		referralMessageEdit = confrimCreateListItemSearch(referralCheckedEdited, "Referral Record", true);
	}

	if (referralCheckedDeleted > 0) {
		referralMessageDelete = confrimCreateListItemSearch(referralCheckedDeleted, "Referral Record", false);
	}

	if (referralCheckedEdited > 0 || referralCheckedDeleted > 0) {
		$.confirm({
			title: confirmEditServicesTitle,
			text: confirmEditServicesTextStart + referralMessageEdit + referralMessageDelete + confirmEditServicesTextEndSearch,
			confirmButton: confirmEditServicesConfirmButton,
			cancelButton: confirmEditServicesCancelButton,
			confirmButtonClass: confirmEditServicesConfirmButtonClass,
			dialogClass: confirmEditServicesDiaglogClass,
			confirm: function () {
				if (resetReferralDateRange) {
					referralDateRangeReset();
				}
				referralSearch($('#referralSearch'));
			},
			cancel: function () {
				// nothing to do
			}
		});
	} else {
		if (resetReferralDateRange) {
			referralDateRangeReset();
		}
		referralSearch($('#referralSearch'));
	}
}

function referralDateRangeReset() {
	$("#referralSearchDates_0__ReferralDateRangeStart").val("");
	$("#referralSearchDates_0__ReferralDateRangeEnd").val("");
	$("#referralDateRangesSelect").val(0);
}

