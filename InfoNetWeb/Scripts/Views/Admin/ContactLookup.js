var confirmEditLookupListTitle = "Are you sure you want to do that?";
var confirmEditLookupListTextStart = "You've marked following for deletion:<ul>";
var confirmEditLookupListTextEnd = "</ul>If you continue, the record/records will be <strong>permanently deleted</strong>.";
var confirmEditLookupListConfirmButtonClass = "btn-danger";
var confirmEditLookupListDiaglogClass = "modal-dialog icjia-modal-danger";

AddNewContactRecord();

$('#saveButton').click(function () { submit(); });

$('#clearCriteria').click(function () {
	$('#ContactName').val('');
	$('#IsActive').val('');
	$('#DisplayOrder').val('');
});

$('.addcontact').on("click", function (e) {
    e.preventDefault();
    $('.addcontact').val(parseInt($('.addcontact').val()) + 1);

    AddNewContactRecord();
});

$("#newContacts").on("click", ".delete", function () {

    if ($(this).find('button').hasClass('deleteButton')) {
        $(this).find('button').removeClass('deleteButton');
    }
    var $killrow = $(this).parent('tr');
    $killrow.fadeOut(50, function () {
        $(this).remove();
        reorderRowIndexes("ContactRecord", "contactData", parseInt($('#recordCount').val()));
        hideTable("#newContacts");
        deleteButtonClassFix($('.ContactRecord'), $("#newContacts"));
        hideShowDeleteButton('#newContacts');
        $('.addcontact').val($('.addcontact').val() - 1);
        $('.addcontact').change();
    });
});

$('#newContacts').on('input', 'input[name$="ContactName"]', function () {
    var $this = $(this);

    if ($this.val().trim() != "" && $this.valid()) {
        $this.closest('tr').find('.contactData').removeAttr('disabled');
    } else {
        $this.closest('tr').find('.contactData').not('input[name$="ContactName"]').attr('disabled', 'disabled');
    }
});

function beforeFormSubmission() {
    if ($('tr.ContactRecord').last().find('input[name$="DisplayOrder"]').prop('disabled') == true) {
        $('tr.ContactRecord').last().remove();
    }

    $('#searchResults').find('[disabled]').prop('disabled', false);
    $('#searchResults').find('#newContacts').remove();
    $('#newContacts').clone().appendTo('#searchResults > tbody').hide();
    $('#secondForm').find('tr.ContactRecord').find('span.help-block').remove();
}

function submit() {
	removeEmptyRow($('#newContacts'));
    if ($('#firstForm').valid() && $('#secondForm').valid()) {
        var cntRecordsMarkedForDeletion = $('.deleteRecord:checked').length;

        if (cntRecordsMarkedForDeletion > 0) {
            var recordDeleteMessage = confrimCreateListItem(cntRecordsMarkedForDeletion, 'Contact');
            $.confirm({
                title: confirmEditLookupListTitle,
                text: confirmEditLookupListTextStart + recordDeleteMessage + confirmEditLookupListTextEnd,
                confirmButtonClass: confirmEditLookupListConfirmButtonClass,
                dialogClass: confirmEditLookupListDiaglogClass,
                confirm: function () {
                	submitMyForm($('#secondForm'));
                },
                cancel: function () {
                    if ($('tr.ContactRecord').length == 0)
                        AddNewContactRecord();
                }
            });
        } else {
        	submitMyForm($('#secondForm'));
        }
    } else {
        $('#searchResults').find('#newContacts').remove();
        $('.input-validation-error').first().focus();
    }
}

function AddNewContactRecord() {
    $.ajax({
        url: "/ContactLookup/AddNewRecord",
        success: function (partialViewResult) {
            $('#newContacts > tbody').append(partialViewResult);
            reorderRowIndexes("ContactRecord", "contactData", parseInt($('#recordCount').val()));
			deleteButtonClassFix($('.ContactRecord'), $("#newContacts"));
            $("#newContacts").find('tr').last().find('input[name$="ContactName"]').focus();
            hideShowDeleteButton('#newContacts');
        },
        error: function (xhr, ajaxOptions, thrownError) {
        	//console.log(xhr);
        	alert('There was an error while adding a new record.');
        }
    });
}