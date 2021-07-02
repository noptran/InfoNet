var confirmEditLookupListTitle = "Are you sure you want to do that?";
var confirmEditLookupListTextStart = "You've marked following for deletion:<ul>";
var confirmEditLookupListTextEnd = "</ul>If you continue, the record/records will be <strong>permanently deleted</strong>.";
var confirmEditLookupListConfirmButtonClass = "btn-danger";
var confirmEditLookupListDiaglogClass = "modal-dialog icjia-modal-danger";

AddNewFundingSourceRecord();



$('#saveButton').click(function () {
    submit();
});

$('#clearCriteria').click(function () {
    $('#FundingSource').val('');
    $('#IsActive').val('');
    $('#DisplayOrder').val('');
});

$('.addfundingsource').on("click", function (e) {
    e.preventDefault();
    $('.addfundingsource').val(parseInt($('.addfundingsource').val()) + 1);

    AddNewFundingSourceRecord();
});

$("#newFundingSources").on("click", ".delete", function () {

    if ($(this).find('button').hasClass('deleteButton')) {
        $(this).find('button').removeClass('deleteButton');
    }
    var $killrow = $(this).parent('tr');
    $killrow.fadeOut(50, function () {
        $(this).remove();
        reorderRowIndexes("FundingSourceRecord", "fundingData", parseInt($('#recordCount').val()));
        hideTable("#newFundingSources");
        deleteButtonClassFix($('.FundingSourceRecord'), $("#newFundingSources"));
        hideShowDeleteButton('#newFundingSources');
        $('.addfundingsource').val($('.addfundingsource').val() - 1);
        $('.addfundingsource').change();
    });
});

$('#newFundingSources').on('input', 'input[name$="FundingSourceName"]', function () {
    var $this = $(this);
    var activityName = $this.val().trim();

    if (activityName != "" && $this.valid()) {
        $this.closest('tr').find('.fundingData').removeAttr('disabled');
    } else {
        $this.closest('tr').find('.fundingData').not('input[name$="FundingSourceName"]').attr('disabled', 'disabled');
    }
});

function beforeFormSubmission() {
    if ($('tr.FundingSourceRecord').last().find('input[name$="DisplayOrder"]').prop('disabled') == true) {
        $('tr.FundingSourceRecord').last().remove();
    }

    $('#searchResults').find('[disabled]').prop('disabled', false);
    $('#searchResults').find('#newFundingSources').remove();

    var $newRecords = $('#newFundingSources').clone();
    $newRecords.find('tbody > tr').each(function () {
        $(this).addClass('rowClass');
        if ($(this).find('input[name$="DisplayOrder"]').prop('disabled') == true)
            $(this).remove();
    });
    $newRecords.appendTo('#searchResults > tbody').hide();

    $('#secondForm').find('tr.FundingSourceRecord').find('span.help-block').remove();
}

function submit() {
	removeEmptyRow($('#newFundingSources'));
    if ($('#firstForm').valid() && $('#secondForm').valid()) {
        var cntRecordsMarkedForDeletion = $('.deleteRecord:checked').length;

        if (cntRecordsMarkedForDeletion > 0) {

            var recordDeleteMessage = confrimCreateListItem(cntRecordsMarkedForDeletion, 'Funding Source');
            $.confirm({
                title: confirmEditLookupListTitle,
                text: confirmEditLookupListTextStart + recordDeleteMessage + confirmEditLookupListTextEnd,
                confirmButtonClass: confirmEditLookupListConfirmButtonClass,
                dialogClass: confirmEditLookupListDiaglogClass,
                confirm: function () {
                	submitMyForm($('#secondForm'));
                },
                cancel: function () {
                    if ($('tr.FundingSourceRecord').length == 0)
                        AddNewFundingSourceRecord();
                }
            });
        } else {
        	submitMyForm($('#secondForm'));
        }
    } else {
        $('#searchResults').find('#newFundingSources').remove();
        $('.input-validation-error').first().focus();
    }
}

function AddNewFundingSourceRecord() {
    $.ajax({
    	url: "/FundingSourceLookup/AddNewRecord",
        success: function (partialViewResult) {
            $('#newFundingSources > tbody').append(partialViewResult);
            reorderRowIndexes("FundingSourceRecord", "fundingData", parseInt($('#recordCount').val()));
            deleteButtonClassFix($('.FundingSourceRecord'), $("#newFundingSources"));
            $("#newFundingSources").find('tr').last().find('input[name$="FundingSourceName"]').focus();
            hideShowDeleteButton('#newFundingSources');
        },
        error: function (xhr, ajaxOptions, thrownError) {
			//console.log(xhr);
        	alert('There was an error while adding a new record.');
        }
    });
}