var confirmEditLookupListTitle = "Are you sure you want to do that?";
var confirmEditLookupListTextStart = "You've marked following for deletion:<ul>";
var confirmEditLookupListTextEnd = "</ul>If you continue, the record/records will be <strong>permanently deleted</strong>. <br/> To undo these changes, click on the Undo Changes button.";
var confirmEditLookupListConfirmButtonClass = "btn-danger";
var confirmEditLookupListDiaglogClass = "modal-dialog icjia-modal-danger";
var prevDate;
var staffDeleted = 0;

$(document).ready(function () {
    staffListAssignData();
    updateCountdown();
    $('#Comment_Pub').change(updateCountdown);
    $('#Comment_Pub').keyup(updateCountdown);

    $("#originalStaffCount").val($('.staffInfo').length);


    //click to add new staff
    $(".staff_AddNew").click(function (e) {
        populateNewRow("staff", "staffInfo");
        reorderRowIndexes("staffInfo", "staffData", "add");
        $('#staffCount').val($('.staffInfo').length);
        $('#staffCount').change();
        e.preventDefault();
    });

    //delete a staff row
    $("#staff").delegate(".deleteStaffRow", "click", function () {
        var rowLen = $(".staffInfo").length;
        if (rowLen <= 1) {
            $.confirm({
                title: "Unable to delete staff member.",
                text: "At least one staff member is needed for a Media/Publication record.",
                cancelButton: null,
                confirmButton: "OK",
                confirmButtonClass: "btn btn-warning",
                confirm: function () {
                    return;
                }
            });
        }
        else {
            var $killrow = $(this).parent('tr');
            $(this).addClass('hide');
            strikeOut($killrow, "staffInfo", '.restoreStaffRow');
            staffDeleted += 1;
            reorderRowIndexes("staffInfo", "staffData", "delete");
            var previousIndex = parseInt($('#insert-staff').val());
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
        $('#staffCount').val($('.staffInfo').length).change();
    });


    // Check if Date has changed
    $('#main').on('focusin blur', '[data-provide="datepicker"] input', function (event) {
        //console.log('prevDate = ' + prevDate);
        //console.log('currentDate = ' + $(this).val());
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
    $("#" + tableId).each(function () {
        var $tableBody = $("#" + tableId).find("tbody"),
            $trFirst = $tableBody.find("tr:first"),
            $trNew = $trFirst.clone();

        //Remove Error Styling
        $trNew.find('div.has-error').removeClass('has-error').find('input').removeClass('input-validation-error').removeAttr('aria-invalid').removeAttr('aria-describedby');
        $trNew.find('.help-block').removeClass('field-validation-error');
        $trNew.find('span.help-block').find('span').remove();
        $trNew.find('select').removeClass('input-validation-error').removeAttr('aria-invalid');
        $trNew.removeClass("deleted ");
        $trNew.addClass("staffInfo");
        $trNew.find(".deleteStaffRow").removeClass("hide");
        $trNew.find(".restoreStaffRow").addClass("hide");
        $trNew.find('span.help-block-deleted').removeClass('help-block-deleted').addClass('help-block');
        $trNew.children().find("input[type=text],input[type=number],select").removeAttr("disabled");
        $trNew.find('select').val("");
        $trNew.find('input[name$=".HoursPrep"]').val("");

        if ($('tbody', this).length > 0) {
            $('tbody', this).append($trNew);
        } else {
            $(this).append($trNew);
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
            //	alert("current name and id= "+currentname +" ** "+currentid+"\n name= " + newname + "\n" + " id= " + newid  + "\n change to  "+ (index));// +"\n last index " + currentIndex);
            $(this).attr('name', newname);
            $(this).attr('id', newid);
        });
    });

    if (action == "add") {
        var selector = newname.substr(0, newname.indexOf('.'));
        $("input[name*='" + selector + "']").val("");
        $("select[name*='" + selector + "']").find(":selected").removeAttr("selected");
    }

    rescanUnobtrusiveValidation('#main');
}

function updateCountdown() {
    // 200 is the max message length
    var remaining = 200 - jQuery('#Comment_Pub').val().length;
    if (0) {

    }
    jQuery('#charRemaining').text(remaining);
}


function removeEmptyStaff() {
    var doesRemove = false;
    var isFirst = true;
    var first;
    $('tr.staffInfo').each(function () {
        var staffID = $(this).find('select[name$=".SVID"]').find('option:selected').val();
        var prep = $(this).find('input[name$=".HoursPrep"]').val();

        if ((staffID == "" || staffID === "undefined") && (prep == "" || prep === "undefined")) {
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


$("#saveButton").click(function () {
    checkForDeletes("save");
});


$("#saveAddButton").click(function () {
    checkForDeletes("saveAdd");
});

function checkForDeletes(validate) {
    var pluralize = function (noun, count) { return noun + (count > 1 ? 's' : ''); };

    if (staffDeleted > 0) {
        var recordDeleteMessage = "<li><strong>" + staffDeleted + pluralize(' Staff Member', staffDeleted) + "</li></strong>";

        $.confirm({
            title: confirmEditLookupListTitle,
            text: confirmEditLookupListTextStart + recordDeleteMessage + confirmEditLookupListTextEnd,
            confirmButtonClass: confirmEditLookupListConfirmButtonClass,
            dialogClass: confirmEditLookupListDiaglogClass,
            confirm: function () {
                $("body").find('.deleted').remove();
                if (validate === "save") {
                    if (validateMyForm()) $('#main').submit();
                }
                else {
                    if (validateMyForm2()) $('#main').submit();
                }
            }
        });

    } else {
        if (validate === "save") {
            if (validateMyForm()) $('#main').submit();
        }
        else {
            if (validateMyForm2()) $('#main').submit();
        }
    }
}