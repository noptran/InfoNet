$(function () {
    rescanUnobtrusiveValidation("#main");
    if ($('#serviceDetailTableEdit >tbody >tr').length > 0) { $('#tablePagingDirectServices').show(); }
});

$(document.body).on('click', "#directServiceSearch,#directServiceReset", function () {
    directServiceDeletesCheckedConfirm(this.id == 'directServiceReset' ? true : false);
});

function directServiceCheckIfEmpty(getThis, group) {
    if ($("#serviceDetailTableAdd").find('tbody tr:visible').last().find("[name$='.ServiceID']").prop('name') == $(getThis).closest('tr').find("[name$='.ServiceID']").prop('name')) {
        cntNoShowRow = cntEmptyFields("[name$='.SVID'],[name$='.ServiceDate'],[name$='.ReceivedHours']", getThis);

        if (cntNoShowRow > 0) {//If errors in row don't add a new row.
            //$(getThis).closest('tr').find("[name$='.SVID'],[name$='.ServiceDate'],[name$='.ReceivedHours'] ").each(function () {
            //	if ($(this).prop('disabled') == false) {
            //		$(this).valid();
            //	}
            //});
        } else {
            var isAddedHiddenFld = $(getThis).closest('tr').find("[id$='IsAdded']");
            isAddedHiddenFld.val(true);

            directServiceDetailTableAddRow(isAddedHiddenFld);
            setTimeout(function () {
                //$('#serviceDetailTableAdd tr:last-child').find('select:first').focus();
            }, 60);
            rescanUnobtrusiveValidation("#main");
        }
    }
}

function directServiceCheckboxDisableRow(checkBox) {
    var $trElement = $(checkBox).closest("tr");
    $trElement.find(":input:visible").not("[name$='.Index'], [name$='.ServiceDetailID'], [name$='.ICS_ID'], [name$='.IsEdited'], [name$='.IsDeleted']").attr("disabled", true);

    $.when($trElement.find(":input:visible").not("[name$='.Index'], [name$='.ServiceDetailID'], [name$='.ICS_ID'], [name$='.IsEdited'], [name$='.IsDeleted']").attr("disabled", true)).then(function (x) {
        $trElement.find(":input:visible[name$='.ServiceID']").val($trElement.find("input:hidden[name$='.ServiceID']").val());
        $trElement.find(":input:visible[name$='.SVID']").val($trElement.find("input:hidden[name$='.SVID']").val());
        $trElement.find(":input:visible[name$='.ServiceDate']").val($trElement.find("input:hidden[name$='.ServiceDate']").val()).blur();
        $trElement.find(":input:visible[name$='.ReceivedHours']").val($trElement.find("input:hidden[name$='.ReceivedHours']").val());
    });
}

function directServiceSearch($searchButton) {
    var elem = $searchButton;
    elem.addClass('icjia-spinner-active');
    $.ajax({
        url: '/Case/DirectServicesSearch',
        type: 'GET',
        datatype: 'html',
        data: { ClientId: $("#clientId").val(), CaseId: $("#caseId").val(), fromDate: $("#directServiceSearchDates_0__DirectServicesDateRangeStart").val(), toDate: $("#directServiceSearchDates_0__DirectServicesDateRangeEnd").val() },
        success: function (data) {
            $('#searchResult').html(data);
            rescanUnobtrusiveValidation("#main");
        },
        error: function (jqXHR, textStatus, errorThrown) {
            //alert(errorThrown);
        }
    }).always(function () {
        elem.removeClass('icjia-spinner-active');
    });
}

function directServiceDetailTableAddRow(isAddedHiddenFld) {
    var nextIndex = ($("table#serviceDetailTableAdd > tbody > tr").length);
    $.ajax({
        url: '/Case/DirectServicesAdd',
        type: 'GET',
        data: { index: nextIndex },
        success: function (results) {
            $("table#serviceDetailTableAdd tbody").append(results);
            isAddedHiddenFld.val(true);
            rescanUnobtrusiveValidation("#main");
        }
    });

}

function directServiceDeletesCheckedConfirm(resetDirectServiceDateRange) {
    var directServicesMessageDelete = directServicesMessageEdit = "";

    var directServicesCheckedEdited = $("#serviceDetailTableEdit input[name$='.IsEdited']:checked").length;
    var directServicesCheckedDeleted = $("#serviceDetailTableEdit input[name$='.IsDeleted']:checked").length;

    if (directServicesCheckedEdited > 0)
        directServicesMessageEdit = confrimCreateListItemSearch(directServicesCheckedEdited, "Direct Service Record", true);

    if (directServicesCheckedDeleted > 0)
        directServicesMessageDelete = confrimCreateListItemSearch(directServicesCheckedDeleted, "Direct Service Record", false);

    if (directServicesCheckedEdited > 0 || directServicesCheckedDeleted > 0) {
        $.confirm({
            title: confirmEditServicesTitle,
            text: confirmEditServicesTextStart + directServicesMessageEdit + directServicesMessageDelete + confirmEditServicesTextEndSearch,
            confirmButton: confirmEditServicesConfirmButton,
            cancelButton: confirmEditServicesCancelButton,
            confirmButtonClass: confirmEditServicesConfirmButtonClass,
            dialogClass: confirmEditServicesDiaglogClass,
            confirm: function () {
                if (resetDirectServiceDateRange)
                    directServiceDateRangeReset();
                directServiceSearch($('#directServiceSearch'));
            },
            cancel: function () {
                // nothing to do
            }
        });
    } else {
        if (resetDirectServiceDateRange) {
            directServiceDateRangeReset();
        }
        directServiceSearch($('#directServiceSearch'));
    }
}

function directServiceDateRangeReset() {
    var today = new Date();
    var dd = today.getDate(),
		mm = today.getMonth(),
		yyyy = today.getFullYear();

    $("#directServiceSearchDates_0__DirectServicesDateRangeStart").val(formatDate(new Date(today.getFullYear(), mm - 3, dd)));
    $("#directServiceSearchDates_0__DirectServicesDateRangeEnd").val(formatDate(new Date(yyyy, mm, dd)));
    $("#directServicesDateRangesSelect").val(0);
}

$(document.body).on('focusout', "[name^='DirectServices'][name$='.ServiceDate']", function () {
	refreshSelectGetStaffRetainCurrentSvid($(this).closest('tr').find("[name$='.SVID']"), $(this));
});
