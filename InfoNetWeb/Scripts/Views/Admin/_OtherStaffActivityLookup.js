var confirmEditLookupListTitle = "Are you sure you want to do that?";
var confirmEditLookupListTextStart = "You've marked following for deletion:<ul>";
var confirmEditLookupListTextEnd = "</ul>If you continue, the record/records will be <strong>permanently deleted</strong>.";
var confirmEditLookupListConfirmButtonClass = "btn-danger";
var confirmEditLookupListDiaglogClass = "modal-dialog icjia-modal-danger";

AddNewActivityRecord();

$('#saveButton').click(function () { submit(); });

$('#clearCriteria').click(function () {
    $('#ActivityName').val('');
    $('#IsActive').val('');
    $('#DisplayOrder').val('');
});

$('.addactivity').on("click", function (e) {
    e.preventDefault();
    $('.addactivity').val(parseInt($('.addactivity').val()) + 1);

    AddNewActivityRecord();
});

$("#newActivities").on("click", ".delete", function () {

    if ($(this).find('button').hasClass('deleteButton')) {
        $(this).find('button').removeClass('deleteButton');
    }
    var $killrow = $(this).parent('tr');
    $killrow.fadeOut(50, function () {
        $(this).remove();
        reorderRowIndexes("staffActivityRecord", "activityData", parseInt($('#recordCount').val()));
        hideTable("#newActivities");
        deleteButtonClassFix($('.staffActivityRecord'), $("#newActivities"));
        hideShowDeleteButton('#newActivities');

        $('.addactivity').val($('.addactivity').val() - 1);
        $('.addactivity').change();
    });
});

$('#newActivities').on('input', 'input[name$="ActivityName"]', function () {
    var $this = $(this);
    var activityName = $this.val().trim();

    if (activityName != "" && $this.valid()) {
        $this.closest('tr').find('.activityData').removeAttr('disabled');
    } else {
        $this.closest('tr').find('.activityData').not('input[name$="ActivityName"]').attr('disabled', 'disabled');
    }
});

function beforeFormSubmission() {
    if ($('tr.staffActivityRecord').last().find('input[name$="DisplayOrder"]').prop('disabled') == true) {
        $('tr.staffActivityRecord').last().remove();
    }

    $('#searchResults').find('[disabled]').prop('disabled', false);
    $('#searchResults').find('#newActivities').remove();

    var $newRecords = $('#newActivities').clone();
    $newRecords.find('input[name$="DisplayOrder"]').filter(function () { return $(this).prop('disabled') == true }).remove();
    $newRecords.appendTo('#searchResults > tbody').hide();

    $('.main').find('tr.staffActivityRecord').find('span.help-block').remove();
}

function submit() {
	removeEmptyRow($('#newActivities'));
    if ($('#firstForm').valid() && $('#secondForm').valid()) {
        var cntRecordsMarkedForDeletion = $('.deleteRecord:checked').length;

        if (cntRecordsMarkedForDeletion > 0) {

            var recordDeleteMessage = confrimCreateListItem(cntRecordsMarkedForDeletion, 'Other Staff Activit', 'ies', 'y');
            $.confirm({
                title: confirmEditLookupListTitle,
                text: confirmEditLookupListTextStart + recordDeleteMessage + confirmEditLookupListTextEnd,
                confirmButtonClass: confirmEditLookupListConfirmButtonClass,
                dialogClass: confirmEditLookupListDiaglogClass,
                confirm: function () {
                	submitMyForm($('#secondForm'));
                },
                cancel: function () {
                    if ($('tr.staffActivityRecord').length == 0)
                        AddNewActivityRecord();
                }
            });
        } else {
        	submitMyForm($('#secondForm'));
        }
    } else {
        $('#searchResults').find('#newActivities').remove();
        $('.input-validation-error').first().focus();
    }
}

function AddNewActivityRecord() {
    $.ajax({
    	url: "/OtherStaffActivityLookup/AddNewRecord",
        success: function (partialViewResult) {
            $('#newActivities > tbody').append(partialViewResult);
            reorderRowIndexes("staffActivityRecord", "activityData", parseInt($('#recordCount').val()));
            deleteButtonClassFix($('.staffActivityRecord'), $("#newActivities"));
            $("#newActivities").find('tr').last().find('input[name$="ActivityName"]').focus();
            hideShowDeleteButton('#newActivities');
            $('.addactivity').change();
        },
        error: function (xhr, ajaxOptions, thrownError) {
			//console.log(xhr);
        	alert('There was an error while adding a new record.');
        }
    });
}