var confirmEditLookupListTitle = "Are you sure you want to do that?";
var confirmEditLookupListTextStart = "You've marked following for deletion:<ul>";
var confirmEditLookupListTextEnd = "</ul>If you continue, the record/records will be <strong>permanently deleted</strong>.";
var confirmEditLookupListConfirmButtonClass = "btn-danger";
var confirmEditLookupListDiaglogClass = "modal-dialog icjia-modal-danger";

AddNewAgencyRecord();

$('#saveButton').click(function () {
    submit();
});

$('#clearCriteria').click(function () {
    $('#AgencyName').val('');
    $('#IsActive').val('');
    $('#DisplayOrder').val('');
});

$('.addagency').on("click", function (e) {
    e.preventDefault();
    $('.addagency').val(parseInt($('.addagency').val()) + 1);

    AddNewAgencyRecord();
});

$("#newAgencies").on("click", ".delete", function () {

    if ($(this).find('button').hasClass('deleteButton')) {
        $(this).find('button').removeClass('deleteButton');
    }
    var $killrow = $(this).parent('tr');
    $killrow.fadeOut(50, function () {
        $(this).remove();
        reorderRowIndexes("AgencyRecord", "agencyData", parseInt($('#recordCount').val()));
        hideTable("#newAgencies");
        deleteButtonClassFix($('.AgencyRecord'), $("#newAgencies"));
        hideShowDeleteButton('#newAgencies');
        $('.addagency').val($('.addagency').val() - 1);
        $('.addagency').change();
    });
});

$('#newAgencies').on('input', 'input[name$="AgencyName"]', function () {
    var $this = $(this);

    if ($this.val().trim() != "" && $this.valid()) {
        $this.closest('tr').find('.agencyData').removeAttr('disabled');
    } else {
        $this.closest('tr').find('.agencyData').not('input[name$="AgencyName"]').attr('disabled', 'disabled');
    }
});

function beforeFormSubmission() {
	if ($('tr.AgencyRecord').last().find('input[name$="DisplayOrder"]').prop('disabled') == true) { $('tr.AgencyRecord').last().remove(); }

	$('#searchResults').find('[disabled]').prop('disabled', false);
    $('#searchResults').find('#newAgencies').remove();
    $('#newAgencies').clone().appendTo('#searchResults > tbody').hide();
    $('#secondForm').find('tr.AgencyRecord').find('span.help-block').remove();
}

function submit() {
	removeEmptyRow($('#newAgencies'));
    if ($('#firstForm').valid() && $('#secondForm').valid()) {
        var cntRecordsMarkedForDeletion = $('.deleteRecord:checked').length;

        if (cntRecordsMarkedForDeletion > 0) {
            var recordDeleteMessage = confrimCreateListItem(cntRecordsMarkedForDeletion, 'Agenc', 'ies', 'y');
            $.confirm({
                title: confirmEditLookupListTitle,
                text: confirmEditLookupListTextStart + recordDeleteMessage + confirmEditLookupListTextEnd,
                confirmButtonClass: confirmEditLookupListConfirmButtonClass,
                dialogClass: confirmEditLookupListDiaglogClass,
                confirm: function () {
                	submitMyForm($('#secondForm'));
                },
                cancel: function () {
                    if ($('tr.AgencyRecord').length == 0) { AddNewAgencyRecord(); }
                }
            });
        } else {
        	submitMyForm($('#secondForm'));
        }
    } else {
        $('#searchResults').find('#newAgencies').remove();
        $('.input-validation-error').first().focus();
    }
}

function AddNewAgencyRecord() {
    $.ajax({
    	url: "/AgencyLookup/AddNewRecord",
        success: function (partialViewResult) {
            $('#newAgencies > tbody').append(partialViewResult);
            reorderRowIndexes("AgencyRecord", "agencyData", parseInt($('#recordCount').val()));
            $("#newAgencies").find('tr').last().find('input[name$="AgencyName"]').focus();
            deleteButtonClassFix($('.AgencyRecord'), $("#newAgencies"));
            hideShowDeleteButton('#newAgencies');
        },
        error: function (xhr, ajaxOptions, thrownError) {
        	//console.log(xhr);
        	alert('There was an error while adding a new record.');
        }
    });
}