var prevDate;
var fcdStart, fcdEnd, clientCode, clientType;
if (!String.prototype.endsWith) {
    String.prototype.endsWith = function (searchString, position) {
        var subjectString = this.toString();
        if (typeof position !== 'number' || !isFinite(position) || Math.floor(position) !== position || position > subjectString.length) {
            position = subjectString.length;
        }
        position -= searchString.length;
        var lastIndex = subjectString.indexOf(searchString, position);
        return lastIndex !== -1 && lastIndex === position;
    };
}

var staffDeleted = 0;
var attendeesDeleted = 0;


$(document).ready(function () {
    hideTable($('#attendees'));
    setTypeahead($('.typeahead'));
    $("#originalStaffCount").val($('.staffInfo').length);
    $("#originalAttendeeCount").val($('.attendeeInfo').length);
    staffListAssignData();
    attendeesListAssignData();
});

function staffListAssignData() {
    $('#staff > tbody').find('tr').each(function (idx, elm) {
        $(this).data("fromServer", "true");
    });
}

function attendeesListAssignData() {
    $('#attendees > tbody').find('tr').each(function (idx, elm) {
        $(this).data("fromServer", "true");
    });
}

//Reset search criteria for search modal
$('#reset').click(function () {
    $('#firstContactDateRangeChosen').val("0");
    $('#firstContactDateRangeChosen').change();
    $('#serviceRangeChosen').val("13");
    $('#serviceRangeChosen').change();
    $('#ClientCode_Search').val('');
    $('#TypeId_Search').val('');
});

//Validation on save or save and add new
$("#main").on('click', '#saveButton', function () {
    SubmitForm();
});

$("#main").on('click', '#saveAddButton', function () {
    $("#saveAddNew").val("1");
    SubmitForm();
});

//click to add new staff
$(".insert-staff").click(function (e) {
    populateNewRow("staff", "staffInfo");
    e.preventDefault();
});

function SubmitForm() {
    removeEmptyStaff();
    var textMessage = textAttendees = textClientsWithClosedCases = textStaffAttendeesDeleted = "";
    var cntClientsWithClosedCases = $('select[name$="CaseID"] option:selected').filter(function () { return $(this).text().endsWith("Closed"); }).length > 0;
    var staffOrAttendeesDeleted = attendeesDeleted + staffDeleted;
    var cntParticipantsNum = parseInt($("#ParticipantsNum").val());

    if ($('#main').valid() && (cntParticipantsNum !== numberOfAttendees() || cntClientsWithClosedCases || staffOrAttendeesDeleted > 0)) {
        if (cntParticipantsNum !== numberOfAttendees())
            textMessage = textAttendees = "The number of Attendees specified in the field is not equal to the number of attendees added to the group service.";

        if (cntClientsWithClosedCases)
            textMessage = textClientsWithClosedCases = createTextClientsWithClosedCases();

        if (staffOrAttendeesDeleted > 0)
            textStaffAttendeesDeleted = "You've marked the following for deletion: <ul class='text-danger' style='font-weight: bold; list-style: square;'>" + createtextStaffAttendeesDeleted() + "</ul>";

        if (cntParticipantsNum !== numberOfAttendees() && cntClientsWithClosedCases)
            textMessage = '<ol type="I"><li>' + textAttendees + "</li><li>" + textClientsWithClosedCases + "</li></ol>";

        if (cntParticipantsNum !== numberOfAttendees() && cntClientsWithClosedCases && staffOrAttendeesDeleted > 0)
            textMessage = '<ol type="I"><li>' + textAttendees + "</li><li>" + textClientsWithClosedCases + "</li><li>" + textStaffAttendeesDeleted + "</ol>";

        if (cntParticipantsNum !== numberOfAttendees() && staffOrAttendeesDeleted > 0)
            textMessage = '<ol type="I"><li>' + textAttendees + "</li><li>" + textStaffAttendeesDeleted + "</ol>";

        if (cntClientsWithClosedCases && staffOrAttendeesDeleted > 0)
            textMessage = '<ol type="I"><li>' + textClientsWithClosedCases + "</li><li>" + textStaffAttendeesDeleted + "</ol>";

        if (cntParticipantsNum === numberOfAttendees() && !cntClientsWithClosedCases && staffOrAttendeesDeleted > 0) {
            var deletesConfirmed = false;

            if (deletesConfirmed) {
                deletesConfirmed = false;
                return;
            }
            $.confirm({
                text: "You've marked the following for deletion: <ul class='text-danger' style='font-weight: bold; list-style: square; margin-top: 10px;'>" + createtextStaffAttendeesDeleted() + "</ul> If you continue, these records will be <span style='font-weight: bold'>permanently deleted</span>.",
                confirmButtonClass: "btn-danger",
                dialogClass: "modal-dialog icjia-modal-danger",
                confirm: function () {
                    deletesConfirmed = true;
                    $("body").find('.deleted').remove();
                    $('#main').submit();
                }
            });
        }
        else {
            $.confirm({
                title: "Are you sure you want to do that?",
                text: textMessage,
                confirmButton: "Yes",
                cancelButton: "No",
                confirmButtonClass: "btn-warning",
                dialogClass: "modal-dialog icjia-modal-warning",
                confirm: function () {
                    var caseFields = $('.caseList');
                    caseFields.each(function () {
                        if ($(this).find('option').length > 0) {
                            $(this).closest('tr').find('.clientCode').removeAttr('aria-invalid').removeAttr('aria-describedby');
                        }
                    });
                    $("body").find('.deleted').remove();
                    $(".staffInfo.deleted").fadeOut(50, function () {
                        $(this).remove();
                        reorderRowIndexes("staffInfo", "staffData");
                        $('.insert-staff').change();
                    });
                    $(".attendeeInfo.deleted").fadeOut(50, function () {
                        $(this).remove();
                        reorderRowIndexes("attendeeInfo", "attendeeData");
                        hideTable($('#attendees'));
                        $('.insert-attendee').change();
                    });
                    $('.insert-staff').val(parseInt($('.insert-staff').val()) - 1);
                    $('#main').submit();
                },
                cancel: function () {
                    return;
                }
            });
        }



    } else if ($('#main').valid()) {
        $('#main').submit();
    } else {
        $('.input-validation-error').first().focus();
    }
}

function confrimCreateListItem(liText) {
    return "<li class='text-danger' style='font-weight:bold; list-style: square;'>" + liText + "</li>";
}

function createTextClientsWithClosedCases() {
    var clientsWithClosedCaseList = "";
    $('select[name$="CaseID"] option:selected').each(function () {
        if ($(this).text().endsWith("Closed"))
            clientsWithClosedCaseList += confrimCreateListItem($(this).closest('tr').find('input[name$=".ClientCode"]').val());
    });

    return "You are adding services to the following client/s with a closed case. <ul>" + clientsWithClosedCaseList + "</ul>";
}

function createtextStaffAttendeesDeleted() {
    var pluralize = function (noun, count) { return noun + (count > 1 ? 's' : ''); };

    var total = staffDeleted + attendeesDeleted;
    if (total === 0) {
        return;
    }

    var buffer = '';
    if (staffDeleted > 0)
        buffer += '<li>' + staffDeleted + pluralize(' Staff Member', staffDeleted) + '</li>';
    if (attendeesDeleted > 0)
        buffer += '<li>' + attendeesDeleted + pluralize(' Attendee', attendeesDeleted) + '</li>';

    return buffer;

}

//Populate new row
function populateNewRow(tableId, rowClass) {
    var date = $('#PDate').val();
    if ($('#PDate').val() !== null && $('#PDate').val() !== "" && !$('#PDate').valid()) {
        $('#PDate').focus();
        return;
    }
    if (date === null || date === "") {
        var d = new Date();
        d.setHours(0, 0, 0, 0);
        date = d.toLocaleDateString();
    }
    $.ajax({
        url: "/GroupService/AddNewStaff?date=" + date,
        //data: "{date: " + date + "}",
        success: function (partialViewResult) {
            var $tableBody = $("#" + tableId).find("tbody");
            $tableBody.append(partialViewResult);
            $tableBody.find('tr:last select').focus();
            reorderRowIndexes("staffInfo", "staffData");
            $('.insert-staff').val(parseInt($('.insert-staff').val()) + 1);
            $('insert-staff').change();
            rescanUnobtrusiveValidation("#main");
        },
        error: function (xhr) {
            //console.log(xhr.responseText);
        }
    });
    $('#staff').show();
}

//case ID dropdown on click

$(".caseList").on("change", function (e) {
    var caseIDSelect = e.currentTarget;
    $(caseIDSelect.options).each(function () {
        if (this.selected && this.text.indexOf("Closed") >= 0) {
            $.confirm({
                dialogClass: "modal-dialog icjia-modal-warning",
                title: "Client with closed case",
                text: "You are adding services to a client with a closed case.",
                confirmButton: 'OK',
                confirmButtonClass: 'btn btn-warning',
                cancelButton: null
            });
        }
    });


});

//click to add new attendee
$(".insert-attendee").on("click", function (e) {
    e.preventDefault();
    $('.insert-attendee').val(parseInt($('.insert-attendee').val()) + 1);

    $.ajax({
        url: "/GroupService/AddNewAttendee",
        success: function (partialViewResult) {
            $('#attendees > tbody').append(partialViewResult);
            reorderRowIndexes("attendeeInfo", "attendeeData");
            var target = $(".newClientCode").removeClass('newClientCode');
            setTypeahead(target);
            SetRowDefaults("#attendees");
            $("#attendees").find('tr').last().find('input[name$="ClientCode"]').focus();

            $('.insert-attendee').change();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            //console.log(xhr);
            $(parent).prepend('There was an error while adding a new client.');
        }
    });
    $("#attendees").show();
});


//delete a staff row
$("#staff").on("click", ".deleteStaffRow", function (e) {
    var $this = $(this);
    var nextElement;
    e.preventDefault();
    var rowLen = $(".staffInfo").length;
    if (rowLen <= 1) {
        $.confirm({
            dialogClass: "modal-dialog icjia-modal-warning",
            title: "Staff member needed",
            text: "At least one staff member is needed for a Group Service.",
            confirmButton: 'OK',
            confirmButtonClass: 'btn btn-warning',
            cancelButton: null
        });
    }
    else {
        if ($this.closest('tr').next('tr').length > 0) {
            nextElement = $this.closest('tr').next('tr').find('select').first();
        }
        else {
            nextElement = $('button.insert-staff');
        }
        var $killrow = $(this).parent('tr');
        $(this).addClass('hide');
        strikeOut($killrow, "staffInfo", '.restoreStaffRow');
        staffDeleted += 1;
        reorderRowIndexes("staffInfo", "staffData");
        $('.insert-staff').val(parseInt($('.insert-staff').val()) - 1).change();
    }
});

//delete an attendee row
$("#attendees").on("click", ".deleteAttendeeRow", function (e) {
    var $this = $(this);
    var nextElement;
    e.preventDefault();
    if ($this.closest('tr').next('tr').length > 0) {
        nextElement = $this.closest('tr').next('tr').find("input[name$='ClientCode']").first();
    }
    else {
        nextElement = $('button.insert-attendee');
    }
    e.preventDefault();
    var $killrow = $(this).parent('tr');
    $(this).addClass('hide'); 
    strikeOut($killrow, "attendeeInfo", '.restoreAttendeeRow');
    attendeesDeleted += 1;
    reorderRowIndexes("attendeeInfo", "attendeeData");
    $('.insert-attendee').val($('.insert-attendee').val() - 1).change();

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
    $('.insert-staff').val(parseInt($('.insert-staff').val()) + 1).change();
});

// Attendee restore button clicks
$(document).on('click', '[data-icjia-role="attendee.restore"]', function () {
    var $row = $(this).closest('tr');
    $row.removeClass("deleted ");
    $row.addClass("attendeeInfo");
    $row.find(".deleteAttendeeRow").removeClass("hide");
    $row.find(".restoreAttendeeRow").addClass("hide");
    $row.find('span.help-block-deleted').removeClass('help-block-deleted').addClass('help-block');
    $row.children().find("input[type=text],input[type=number],select").removeAttr("disabled");
    attendeesDeleted -= 1;
    reorderRowIndexes("attendeeInfo", "attendeeData");
    $('.insert-attendee').val(parseInt($('.insert-attendee').val()) + 1).change();
});

//Add attendees with search panel
$(".clientCases").delegate(".addClient", "click", function () {
    var $this = $(this);
    $.ajax({
        url: "/GroupService/AddNewAttendee",
        success: function (partialViewResult) {
            $('.insert-attendee').val(parseInt($('.insert-attendee').val()) + 1);
            $('.insert-attendee').change();
            $('#attendees > tbody').append(partialViewResult);
            $('#attendees tr').last().find('input[name$="ClientCode"]').val($this.closest("tr").find(".clientCode").html());
            getCases($('#attendees tr').last().find('input[name$="ClientCode"]'), $this.closest("tr").find(".caseId").html());
            //alert($('#attendees tr').last().find('select[name$="CaseID"]').parent('div').html());
            //$('#attendees tr').last().find('select[name$="CaseID"]').find("option[value=\"" + $this.closest("tr").find(".caseId").html()+ "\"]").attr("selected", "selected");
            $('#attendees tr').last().find('input[name$="ClientID"]').val($this.closest("tr").find(".clientId").html());
            $('#attendees tr').last().find('input[name$="ReceivedHours"]').val($("#Hours").val());
            reorderRowIndexes("attendeeInfo", "attendeeData");

            var $killrow = $this.closest("tr");
            $killrow.fadeOut(50, function () {
                $killrow.remove();
                hideShowLoadButton();
                hideTable($('#attendees'));
                if (!$(".clientCases").is(":visible")) {
                    $(".clientCases").parent().hide();
                    $(".clientCases").parent().parent().show();//.removeAttr("style");
                }
            });
            $("#attendees").show();//.removeAttr("style");
            var target = $(".newClientCode").removeClass('newClientCode');
            setTypeahead(target);
            rescanUnobtrusiveValidation("#main");
        },
        error: function (xhr, ajaxOptions, thrownError) {
            //console.log(xhr);
            $(parent).prepend('There was an error while adding a new client.');
        }
    });
});

//Call function for populating case after clientCode is chosen
$('#attendees').on('blur', 'input[name$="ClientCode"]', function () {
    getCases($(this), null);
});

// Check if Date has changed
$('#main').on('focusin blur', '[data-provide="datepicker"] input', function (event) {
    //console.log('prevDate = ' + prevDate);
    //console.log('currentDate = ' + $(this).val());
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

$('#searchModalButton').click(function () {
    $("#searchClients").click();
});

//Search attendees with search panel
$("#searchClients").click(function () {
    $('#clientResultCount').html(0);
    $('#clientSearchTotal').html(0);
    setSearchParameters();
    $('.clientCases > tbody').empty();
    callAjaxClientSearch();
});

$('#loadMore').click(function () {
    callAjaxClientSearch();
});

function callAjaxClientSearch() {
    var skip = 0;
    if (parseInt($('#clientResultCount').html()) !== parseInt($('#clientSearchTotal').html()))
        skip = parseInt($('#clientResultCount').html());

    $.ajax({
        type: 'GET',
        url: "/GroupService/SearchCases",
        data: {
            FStartDate: fcdStart,
            FEndDate: fcdEnd,
            clientCode: clientCode,
            clientTypeId: clientType,
            SStartDate: sStart,
            SEndDate: sEnd,
            skip: skip,
            existingClientIds: getAllExistingClientIds()
        },
        dataType: 'json',
        beforeSend: function () {
            $("#searchClients").addClass('icjia-spinner-active');
        },
        success: function (data) {
            var total = 0;
            var len = parseInt(data.length);
            var resultCount = parseInt($('#clientResultCount').html()) + len;
            $('#clientResultCount').html(resultCount);
            var txt = "";
            if (len > 0) {
                total = data[0].total;
                $('.noResultsAlert').hide();
                $('#clientResultCount').html(resultCount);
                $('#clientSearchTotal').html(total);
                $('#icjia-results').show();
                for (var i = 0; i < len; i++) {
                    txt += "<tr class=\"client\">"
                        + "<td width='64px'><button type=\"button\" class=\"btn btn-success btn-block addClient\"><span class=\"glyphicon glyphicon-plus\"></span></button></td>"
                        + "<td class=\"clientCode\">" + data[i].clientCode + "</td>"
                        + "<td class=\"caseId\">" + data[i].caseId + "</td>"
                        + "<td class=\"hidden clientId\">" + data[i].clientId + "</td>"
                        + "</tr>";
                }
                $(".clientCases > tbody").append(txt);
                $(".clientCases").show();
                $("#loadMore").removeClass("hidden");
                $(".searchResults").height("300px");
            }
            else {
                $('.noResultsAlert').show();
                $('#clientSearchTotal').html(len);
            }
            hideShowLoadButton((resultCount >= total));
        },
        error: function (xhr) {
            //console.log(xhr.responseText);
        },
        complete: function () {
            $("#searchClients").removeClass('icjia-spinner-active');
        }
    });
}

function setSearchParameters() {
    fcdStart = ($("#FirstContactDateStart").length) ? $("#FirstContactDateStart").val() : null;
    fcdEnd = ($("#FirstContactDateEnd").length) ? $("#FirstContactDateEnd").val() : null;
    sStart = ($("#ServiceDateStart").length) ? $("#ServiceDateStart").val() : null;
    sEnd = ($("#ServiceDateEnd").length) ? $("#ServiceDateEnd").val() : null;
    clientCode = ($("#ClientCode_Search").length) ? $("#ClientCode_Search").val() : null;
    clientType = ($("#TypeId_Search").length) ? $("#TypeId_Search").val() : null;
}

function getAllExistingClientIds() {
    var alreadyExists = {};
    var existingClientIds = $('input[name$="ClientID"]');
    existingClientIds.each(function (i) {
        alreadyExists[i] = $(this).val();
    });
    return alreadyExists;
}

function getCases($this, selected) {
    if ($this.val() !== "") {
        $.ajax({
            type: 'GET',
            url: "/GroupService/GetCases",
            data: { clientCode: $this.val() },
            dataType: 'json',
            success: function (data) {
                var $caseList = $this.closest('tr').find('select[name$="CaseID"]');
                if (data[0] !== null)
                    $this.closest('tr').find('input[name$="ClientID"]').val(data[0].clientId);

                $caseList.empty();
                $.each(data, function (i, obj) {
                    $caseList.append($('<option>').text(obj.caseId + " - " + obj.firstContactDate + " - " + obj.caseClosed).attr('value', obj.caseId));
                    if (selected !== null && obj.caseId === selected)
                        $caseList.val(obj.caseId);
                });
            },
            error: function (exception) { }
        });
    }
}

//Show or hide load more button
function hideShowLoadButton(allShown) {
    $("#loadMore").toggle(!allShown);
}

//Typeahead configuration
function setTypeahead(target) {
    $(target).typeahead({
        hint: true,
        limit: 20,
        minLength: 1
    },
        {
            name: 'ClientCode',
            display: function (item) { return item.ClientCode },
            source: function (query, syncResults, asyncResults) {
                //console.log("clientcode : " + query);
                $.ajax('/GroupService/GetClients?ClientCode=' + query,
                    {
                        dataType: 'json',
                        success: function (data) {
                            //console.log(data);
                            asyncResults(data);
                        }
                    });
            },
            templates: {
                empty: [
                    '<div class="empty-message" style="text-align:center">No Results</div>'
                ].join('\n'),
                suggestion: function (value) { return '<div>' + value.ClientCode + '</div>'; }
            }
        });
}

//Reorder row indexes
function reorderRowIndexes(rowClass, rowData) {
    var currentname, newname, currentid, newid, aria;

    $("." + rowClass).each(function (index) {
        $(this).find('span.help-block').each(function () {

            currentname = $(this).attr('data-valmsg-for');
            newname = currentname.substr(0, currentname.indexOf('[') + 1) + index + currentname.substr(currentname.indexOf(']'), currentname.length - 1);
            $(this).attr('data-valmsg-for', newname);
        });

        $(this).find('.' + rowData + '[name]').each(function () {
            currentname = $(this).attr('name');
            currentid = $(this).attr('id');
            aria = $(this).attr('aria-describedby');

            newname = currentname.substr(0, currentname.indexOf('[') + 1) + index + currentname.substr(currentname.indexOf(']'), currentname.length - 1);
            newid = currentid.substr(0, currentid.indexOf('_') + 1) + index + currentid.substr(currentid.indexOf('__'), currentid.length - 1);
            if (aria !== undefined)
                $(this).attr('aria-describedby', aria.substr(0, currentid.indexOf('_') + 1) + index + currentid.substr(currentid.indexOf('__'), currentid.length - 1));

            //alert("current name and id= " + currentname + " ** " + currentid + "\n name= " + newname + "\n" + " id= " + newid + "\n change to  " + (index));// +"\n last index " + currentIndex);
            $(this).attr('name', newname);
            $(this).attr('id', newid);
        });
    });
    rescanUnobtrusiveValidation('#main');
}


//Clear values after adding a new row
function ClearRowValues(table) {
    $("#" + table + ' > tbody > tr').last().find($(":input")).val("");
}

//Set Row defaults
function SetRowDefaults(table) {
    //Set the received hours to number of hours in session
    var receivedHours = $("#Hours").val();
    //	if ($('#ICS_ID').val() == null) {
    $(table + ' > tbody').find($("[id$='ReceivedHours']")).last().val(receivedHours);
    //	}
}

//Remove empty staff row in table
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
        reorderRowIndexes("staffInfo", "staffData");
    }
}

function numberOfAttendees() {
    var count = 0;
    $('#attendees > tbody').find('tr').each(function () {
        if ($(this).find('input[name$=".ClientCode"]').val() !== "" && !$(this).hasClass("deleted")) {
            count++;
        }
    });
    return count;
}

$('#attendees').on('keyup', "input[name$='ClientCode']", function () {
    var $this = $(this);
    setTimeout(function () {
        var margin = 0;
        var $lastSuggesstion = $this.closest('tr').find('.tt-suggestion.tt-selectable:last');
        var $emptyMsg = $this.closest('tr').find('.empty-message:last');
        var scroll = $('div.tt-menu.tt-open').height();

        if ($lastSuggesstion.length > 0) {
            margin = $(window).innerHeight() - $lastSuggesstion.get(0).getBoundingClientRect().bottom;
            if (margin < 0)
                $("html, body").animate({ scrollTop: $(window).scrollTop() + scroll }, 1000);
        }

        if ($emptyMsg.length > 0) {
            margin = $(window).innerHeight() - $emptyMsg.get(0).getBoundingClientRect().bottom;
            if (margin < 0)
                $("html, body").animate({ scrollTop: $(window).scrollTop() + scroll }, 1000);
        }
    }, 500);
});

function hideTable(selector) {
    if ($(selector).find('tr').length <= 1) {
        $(selector).hide();
    }
}

$(document).ready(function () { if ($('#attendees > tbody > tr').length > 0) { $('#attendees').show(); } });