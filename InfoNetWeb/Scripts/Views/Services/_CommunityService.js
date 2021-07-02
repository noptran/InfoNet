var prevDate;
var staffDeleted = 0;

$(document).ready(function () {

    staffListAssignData();

    $.validator.unobtrusive.adapters.addBool("mandatory", "required");

    updateCountdown($('#Comment_Act').val().length);

    $('#Comment_Act').on('change keyup paste', function (e) {
        updateCountdown($(this).val().length);
    });

    $('#Comment_Act').on('paste', function (e) {
        updateCountdown(e.clipboardData.length);
    });

    // Saev button click
    $('#saveButton').click(function () {
        if (validateMyForm())
            SubmitForm();
        //$('#main').submit();
    });

    // Save and add new button click
    $('#saveAddButton').click(function () {
        if (validateMyForm2())
            SubmitForm();
        //$('#main').submit();
    });

    //click to add new staff
    $(".staff_AddNew").click(function (e) {
        populateNewRow("staff", "staffInfo");
        e.preventDefault();
    });

    //delete a staff row
    $("#staff").on("click", ".deleteStaffRow", function () {
        var rowLen = $(".staffInfo").length;
        if (rowLen <= 1) {
            $.confirm({
                title: "Can not delete staff row.",
                text: "At least one staff member is needed for a Community/Institutional service.",
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
            $('#staffCount').val($('.staffInfo').length).change();
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
        reorderRowIndexes("staffInfo", "staffData");
        var previousIndex = parseInt($('#insert-staff').val());
        var currentIndex = previousIndex + 1;
        $('#insert-staff').val(currentIndex);
        $('#staffCount').val($('.staffInfo').length).change();
    });


    // Check if Date has changed
    $('#main').on('focusin blur', '[data-provide="datepicker"] input', function (event) {
        //console.log('prevDate = ' +prevDate);
        //console.log('currentDate = ' +$(this).val());
        if (event.type === 'focusin') {
            prevDate = $(this).val();
        }
        else {
            if (prevDate !== $(this).val()) {
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
                                if (selectedValue === data[i].SVID) {
                                    $(selectList).append($('<option>').text(data[i].EmployeeName).attr('value', data[i].SVID).attr('selected', 'selected'));
                                    isPresent = true;
                                }
                                else {
                                    $(selectList).append($('<option>').text(data[i].EmployeeName).attr('value', data[i].SVID));
                                }
                            });
                            if (!isPresent && selectedValue !== "") {
                                $(selectList).find('option:selected').removeAttr('selected');
                                $(selectedOption).attr('selected', 'selected');
                                $(selectList).append(selectedOption);
                            }
                        });
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        //console.log(xhr.responseText);
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

//Populate new row
function populateNewRow(tableId, rowClass) {
    var icsId = $('#ICS_ID').val();
    var date = $('#PDate').val();
    if ($('#PDate').val() !== null && $('#PDate').val() !== "" && !$('#PDate').valid()) {
        $('#PDate').focus();
        return;
    }
    $.ajax({
        url: "/CommunityServices/AddStaff",
        data: { icsid: icsId, date: date },
        success: function (partialViewResult) {
            var $tableBody = $("#" + tableId).find("tbody");
            $tableBody.append(partialViewResult);
            $tableBody.find('tr:last select').focus();
            reorderRowIndexes("staffInfo", "staffData", "add");
            $('#staffCount').val($('.staffInfo').length);
            $('#staffCount').change();
            rescanUnobtrusiveValidation("#main");
        },
        error: function (xhr) {
            //console.log(xhr.responseText);
        }
    });
}

//Reorder row indexes
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

function updateCountdown(length) {
    // 200 is the max message length
    var remaining = 200 - length;
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
        var hours = $(this).find('input[name$=".ConductHours"]').val();
        var prep = $(this).find('input[name$=".HoursPrep"]').val();
        var travel = $(this).find('input[name$=".HoursTravel"]').val();

        if ((staffID === "" || staffID === "undefined") && (hours === "" || hours === "undefined") && (prep === "" || prep === "undefined") && (travel === "" || travel === "undefined")) {
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

//Submit
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


//Validate Form
function validateMyForm() {
    var response = true;

    removeEmptyStaff();

    return response;
}

function validateMyForm2() {
    $('#saveAddNew').val('1');
    return validateMyForm();
}