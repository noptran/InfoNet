var confirmEditLookupListTitle = "Are you sure you want to do that?";
var confirmEditLookupListTextStart = "You've marked following for deletion:<ul>";
var confirmEditLookupListTextEnd = "</ul>If you continue, the record/records will be <strong>permanently deleted</strong>.";
var confirmEditLookupListConfirmButtonClass = "btn-danger";
var confirmEditLookupListDiaglogClass = "modal-dialog icjia-modal-danger";

AddNewTurnAwayRecord();
$('#Start').val(formatDate($('#Start').val()));
$('#End').val(formatDate($('#End').val()));

$('#saveButton').click(function () { submit(); });

$('.addturnaway').on("click", function (e) {
    e.preventDefault();
    $('.addturnaway').val(parseInt($('.addturnaway').val()) + 1);
    AddNewTurnAwayRecord();
});

$("#turnAways").on("click", ".delete", function () {
    if ($(this).find('button').hasClass('deleteButton')) {
        $(this).find('button').removeClass('deleteButton');
    }
    var $killrow = $(this).parent('tr');
    $killrow.fadeOut(50, function () {
        $(this).remove();
        reorderRowIndexes("turnAwayRecord", "turnAwayData", parseInt($('#recordCount').val()));
        deleteButtonClassFix($('.turnAwayRecord'), $("#turnAways"));
        hideTable("#turnAways");
        deleteButtonClassFix($('.turnAwayRecord'), $("#turnAways"));
        hideShowDeleteButton('#turnAways');
        $('.addturnaway').val($('.addturnaway').val() - 1);
        $('.addturnaway').change();
    });
});

$('#turnAways').on('blur input', 'input[name$="TurnAwayDate"]', function () {
    var $this = $(this);

    if ($this.val() != "") {
        $this.closest('tr').find('.turnAwayData').removeAttr('disabled');
    } else {
        $this.closest('tr').find('.turnAwayData').not('input[name$="TurnAwayDate"]').attr('disabled', 'disabled');
    }
});

$('#searchResults').on('blur', 'input[name$="TurnAwayDate"]', function () { formatDate($(this).val()); });

function beforeFormSubmission() {
    $('#searchResults').find('[disabled]').prop('disabled', false);
    $('#searchResults').find('#turnAways').remove();

    var $newRecords = $('#turnAways').clone();
    $newRecords.appendTo('#searchResults > tbody').hide();
    var selects = $('#firstForm').find('select');
    $(selects).each(function (i) {
        $('#secondForm').find('select.turnAwayData').eq(i).val($(this).val());
    });
    $('#secondForm').find('tr.turnAwayRecord').find('span.help-block').remove();
}

function submit() {
	removeEmptyRow($('#turnAways'));
	if (validSearchEditForms($('#firstForm'), $('#secondForm'))) {
		var cntRecordsMarkedForDeletion = $('.deleteRecord:checked').length;

        if (cntRecordsMarkedForDeletion > 0) {
            var recordDeleteMessage = confrimCreateListItem(cntRecordsMarkedForDeletion, 'Turn Away');
            $.confirm({
                title: confirmEditLookupListTitle,
                text: confirmEditLookupListTextStart + recordDeleteMessage + confirmEditLookupListTextEnd,
                confirmButtonClass: confirmEditLookupListConfirmButtonClass,
                dialogClass: confirmEditLookupListDiaglogClass,
                confirm: function () {
                	submitMyForm($('#secondForm'));
                },
                cancel: function () {
                	if ($('tr.turnAwayRecord').length == 0) { AddNewTurnAwayRecord(); }
                }
            });
        } else {
        	submitMyForm($('#secondForm'));
        }
    } else {
        $('#searchResults').find('#turnAways').remove();
        $('.input-validation-error').first().focus();
    }
}

function AddNewTurnAwayRecord() {
    $.ajax({
    	url: "/TurnAway/AddNewRecord",
        success: function (partialViewResult) {
            $('#turnAways > tbody').append(partialViewResult);
            reorderRowIndexes("turnAwayRecord", "turnAwayData", parseInt($('#recordCount').val()));
            deleteButtonClassFix($('.turnAwayRecord'), $("#turnAways"));
            $("#turnAways").find('tr').last().find('input[name$="TurnAwayDate"]').focus();

            hideShowDeleteButton('#turnAways');
            $('.addturnaway').change();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            //console.log(xhr);
            alert('There was an error while adding a new client.');
        }
    });
}

$(document.body).on('click', 'form#main', function (e) {
    $("input[name$='TurnAwayDate']:empty").addClass('validate-ignore');
});


$(document.body).on('hidden.bs.modal', function () {
    $("input[name$='TurnAwayDate']:empty").removeClass('validate-ignore');
});