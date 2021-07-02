var prevDate;
var staffDeleted = 0;

$(document).ready(function () {
    staffListAssignData();
    updateCountdown();
    $('#Comment').change(updateCountdown);
    $('#Comment').keyup(updateCountdown);

    $(".staff_AddNew").click(function (e) {
        e.preventDefault();
        var ICS_ID = $('#ICS_ID').val();
        var date = $('#EventDate').val();
        var ajaxURL = "/Event/AddNewStaff?ICS_ID=" + ICS_ID + "&date=" + date;
        $.ajax({
            url: ajaxURL,
            success: function (partialViewResult) {
                $('#staff > tbody').append(partialViewResult);
                reorderRowIndexes("staffInfo", "staffData", "add");
                $('#staffCount').val($('.staffInfo').length);
                $('#staffCount').change();
            },
            error: function (xhr, ajaxOptions, thrownError) {
            }
        });
    });

    $("#staff").on("click", ".deleteStaffRow", function () {
        var rowLen = $(".staffInfo").length;
        if (rowLen <= 1) {
            $.confirm({
                title: "Can not delete staff row.",
                text: "At least one staff member is needed for an Event.",
                confirmButtonClass: "btn-warning",
                confirmButton: "OK",
                cancelButton: null,
                confirm: function () {
                    return;
                }
            }
            );
        }
        else {
            var $killrow = $(this).parent('tr');
            $(this).addClass('hide');
            strikeOut($killrow, "staffInfo", '.restoreStaffRow');
            staffDeleted += 1;
            reorderRowIndexes("staffInfo", "staffData", "delete");
            var previousIndex = parseInt($('#insert-staff').val());
            var currentIndex = previousIndex - 1;
            $('#insert-staff').val(currentIndex);
            $('#staffCount').val($('.staffInfo').length);
            $('#staffCount').change();
        }
    });

    function strikeOut($killrow, killClass, restoreClass) {
        $killrow.find(restoreClass).removeClass('hide');
        $killrow.find('span.help-block').removeClass('help-block').addClass('help-block-deleted');
        $killrow.addClass("deleted");
        $killrow.removeClass(killClass);
        $killrow.children().find("input[type=text],input[type=number],select").attr("disabled", "disabled");
    }

    // Staff restore button clicks
    $(document).on('click', '[data-icjia-role="staff.restore"]', function () {
        var $row = $(this).closest('tr');
        $row.removeClass("deleted ");
        $row.addClass("staffInfo");
        $row.find(".deleteStaffRow").removeClass("hide");
        $row.find(".restoreStaffRow").addClass("hide");
        $row.find('span.help-block-deleted').removeClass('help-block-deleted').addClass('help-block');
        $row.children().find("input[type=text],input[type=number],select").removeAttr("disabled");
        staffDeleted -= 1;
        reorderRowIndexes("staffInfo", "staffData", "delete");
        var previousIndex = parseInt($('#insert-staff').val());
        var currentIndex = previousIndex + 1;
        $('#insert-staff').val(currentIndex);
        $('#staffCount').val($('.staffInfo').length);
        $('#staffCount').change();
    });


    //Validation on save or save and add new
    $("#main").on('click', '#saveButton', function () {
        SubmitForm();
    });

    $('#main').on('focusin blur', '[data-provide="datepicker"] input', function (event) {
        if (event.type === 'focusin') {
            prevDate = $(this).val();
        }
        else {
            if (prevDate != $(this).val()) {
                var ajaxURL = "/Service/GetStaff?serviceDate=" + $(this).val();

                $.ajax({
                    url: ajaxURL,
                    success: function (data) {
                        $('select[name$=".SVID"]').each(function () {
                            var selectList = this;
                            var selectedOption = $(this).find('option:selected');
                            var selectedValue = $(selectedOption).val();
                            var isPresent = false;
                            $(this).find('option').remove().end();
                            $(this).append($('<option>').text("<Pick One>").attr('value', ""));
                            $.each(data, function (i, staff) {
                                if (selectedValue == data[i].SVID) {
                                    $(selectList).append($('<option>').text(data[i].EmployeeName).attr('value', data[i].SVID).attr('selected', 'selected'));
                                    isPresent = true;
                                }
                                else {
                                    $(selectList).append($('<option>').text(data[i].EmployeeName).attr('value', data[i].SVID));
                                }
                            });
                            if (!isPresent && selectedValue != "") {
                                $(selectList).find('option:selected').removeAttr('selected');
                                $(selectedOption).attr('selected', 'selected');
                                $(selectList).append(selectedOption);
                            }
                        });
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                    }
                });
            }
        }
    });

});

function staffListAssignData() {
    $('#staff > tbody').find('tr').each(function (idx, elm) {
        $(this).data("fromServer", "true");
    });
}

function SubmitForm() {

    if (staffDeleted === 0) {
        $('#main').submit();
        return;
    }

    var deletesConfirmed = false;

    if (deletesConfirmed) {
        deletesConfirmed = false;
        return;
    }

    var pluralize = function (noun, count) { return noun + (count > 1 ? 's' : ''); };

    var buffer = '';
    if (staffDeleted > 0)
        buffer += '<li>' + staffDeleted + pluralize(' Staff Member', staffDeleted) + '</li>';

    $.confirm({
        text: "You've marked the following for deletion: <ul class='text-danger' style='font-weight: bold; list-style: square; margin-top: 10px;'>" + buffer + "</ul> If you continue, these records will be <span style='font-weight: bold'>permanently deleted</span>.",
        confirmButtonClass: "btn-danger",
        dialogClass: "modal-dialog icjia-modal-danger",
        confirm: function () {
            deletesConfirmed = true;
            $('#main').submit();
        }
    });
}

function populateNewRow(tableId, rowClass) {
    $("#" + tableId).each(function () {
        var $tableBody = $("#" + tableId).find("tbody"),
            $trFirst = $tableBody.find("tr:first"),
            $trNew = $trFirst.clone();

        $trNew.find('div.has-error').removeClass('has-error').find('input').removeClass('input-validation-error').removeAttr('aria-invalid').removeAttr('aria-describedby');
        $trNew.find('.help-block').removeClass('field-validation-error');
        $trNew.find('span.help-block').find('span').remove();
        $trNew.find('select').removeClass('input-validation-error').removeAttr('aria-invalid');

        if ($('tbody', this).length > 0) {
            $('tbody', this).append($trNew);
        } else {
            $(this).append($trNew);
        }
        $tableBody.find('tr:last select').focus();
    });
}

function reorderRowIndexes(rowClass, rowInfo, action) {
    var currentname, newname, currentid, newid;

    $("." + rowClass).each(function (index) {
        $(this).find('span.help-block').each(function () {
            currentname = $(this).attr('data-valmsg-for');
            newname = currentname.substr(0, currentname.indexOf('[') + 1) + index + currentname.substr(currentname.indexOf(']'), currentname.length - 1);
            $(this).attr('data-valmsg-for', newname);
        });
        $(this).find('.' + rowInfo).each(function () {
            currentname = $(this).attr('name');
            currentid = $(this).attr('id');

            newname = currentname.substr(0, currentname.indexOf('[') + 1) + index + currentname.substr(currentname.indexOf(']'), currentname.length - 1);
            newid = currentid.substr(0, currentid.indexOf('_') + 1) + index + currentid.substr(currentid.indexOf('__'), currentid.length - 1);
            $(this).attr('name', newname);
            $(this).attr('id', newid);
        });
    });
    rescanUnobtrusiveValidation('#main');
}

function updateCountdown() {
    var remaining = 200 - jQuery('#Comment').val().length;
    if (remaining < 0) {
        $('#charRemaining').removeClass('text-info').addClass('text-danger');
    }
    else {
        $('#charRemaining').removeClass('text-danger').addClass('text-info');
    }
    $('#charRemainingCount').text(remaining);
}


function removeEmptyStaff() {
    var doesRemove = false;
    var isFirst = true;
    var first;
    $('tr.staffInfo').each(function () {
        var staffID = $(this).find('select[name$=".SVID"]').find('option:selected').val();
        var hours = $(this).find('input[name$=".HoursConduct"]').val();
        var prep = $(this).find('input[name$=".HoursPrep"]').val();
        var travel = $(this).find('input[name$=".HoursTravel"]').val();

        if ((staffID == "" || staffID === "undefined") && (hours == "" || hours === "undefined") && (prep == "" || prep === "undefined") && (travel == "" || travel === "undefined")) {
            if (isFirst) {
                first = $(this).detach();
            }
            else {
                $(this).remove();
            }

            doesRemove = true;
        }
    });
    if (doesRemove) {
        if (!$('tr.staffInfo').length) {
            $('#staff tbody').append(first);
        }
        reorderRowIndexes("staffInfo", "staffData", "delete");
    }
}