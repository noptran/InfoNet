var fcdStart, fcdEnd, clientCode, clientType;

$(document).ready(function () {

    // Show client table if at least one exists
    $('#clients').toggle($('.clientRow').length > 0);

    setMaxHouseHoldVal();

    $('#rangePeriodChosen').change(function () {
        calculateSelectedDateRange('#rangePeriodChosen', '#Start', '#End');
    });

    $('#clients').on('click', '.deleteButton', function () {
        var current = $(this);
        $.confirm({
            title: "Are you sure you want to do that?",
            text: "This action will delete this Client record from this household.",
            confirmButtonClass: "btn-danger",
            dialogClass: "modal-dialog icjia-modal-danger",
            confirm: function () {
                current.closest('tr').remove();
                reorderIndexes();
                var count = parseInt($('#clientCount').val());
                $('#clientCount').val(count - 1);
                $('#clientCount').change();
                $('#clients').toggle($('.clientRow').length > 0);
                setMaxHouseHoldVal();
            },
            cancel: function () {
                return false;
            }
        });

    });

    $('.newClientCode').removeClass('newClientCode');

    $('#clientCount').val(parseInt($('.clientRow').length));

    //Search attendees with search panel
    $("#searchClients").click(function () {
        setSearchParameters();
        $('.clientCases > tbody').empty();
        $('#clientResultCount').html(0);
        callAjaxClientSearch();
    });

    $('#loadMore').click(function () {
        callAjaxClientSearch();
    });

    $('.clientCode').each(function () {
        setTypeahead(this);
    });

    $('#clientAddNew').click(function (e) {
        e.preventDefault();
        AddNewClient(null, null, $('#clientAddNew').val());
    });

    //Call function for populating case after clientCode is chosen
    $('#clients').on('blur', 'input[name$="ClientCode"]', function () {
        getCases($(this), $(this).closest('tr').find('select[name$="CaseID"] option:selected'));
    });

    // Add clients via search panel
    $(".clientCases").on("click", ".addClient", function () {
        var $this = $(this);
        var clientID = parseInt($this.closest('tr.client').find('.clientId').html());
        var caseID = parseInt($this.closest('tr.client').find('.caseId').html());
        AddNewClient(clientID, caseID, $('#clientAddNew').val());
        $this.closest('tr').remove();
        //$.ajax({
        //	url: "/Relationship/AddNewClient?clientID=" + clientID + "&caseID=" + caseID + "&count=" + $('#clientAddNew').val(),
        //	success: function (partialViewResult) {
        //		var lastID = $('#clients tbody tr:last-child input[name$="ClientCode"]').val();
        //		if (lastID == null || lastID == '') {
        //			$('#clients tbody tr:last-child').remove();
        //		}
        //		$('#clients > tbody').append(partialViewResult);
        //		reorderIndexes();
        //		var $killrow = $this.closest("tr");
        //		$killrow.fadeOut(50, function () {
        //			$killrow.remove();
        //			if (!$(".clientCases").is(":visible")) {
        //				$(".clientCases").parent().hide();
        //				$(".clientCases").parent().parent().removeAttr("style");
        //			}
        //		});
        //		var target = $(".newClientCode").removeClass('newClientCode');
        //		setTypeahead(target);
        //		$('#clientCount').val(parseInt($('#clientCount').val()) + 1);
        //		$('#clientCount').change();
        //		//adjustHouseholdIdList(false);
        //		$('#clients').toggle($('.clientRow').length > 0);
        //	},
        //	error: function (xhr, ajaxOptions, thrownError) {
        //		console.log(xhr.responseText);
        //		$(parent).prepend('There was an error while adding a new client.');
        //	},
        //	complete: function () {
        //		setMaxHouseHoldVal();
        //	}
        //});
    });

    $('#clientAddSearch').click(function () {
        $("#searchClients").click();
    });
});

function AddNewClient(clientId, caseId, count) {
    $('#clientAddNew').val(parseInt($('#clientAddNew').val()) + 1);
    $.ajax({
        url: "/Relationship/AddNewClient?clientID=" + clientId + "&caseID=" + caseId + "&count=" + count,
        success: function (partialViewResult) {
            $('#clients tbody').append(partialViewResult);
            var target = $(".newClientCode").removeClass('newClientCode');
            setTypeahead(target);
            $('#clients').toggle($('.clientRow').length > 0);
            $('#clientCount').val(parseInt($('#clientCount').val()) + 1);
            $('#clientCount').change();
            setMaxHouseHoldVal();            
            reorderIndexes();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            //console.log(xhr.responseText);
        },
        complete: function () {
            if (clientId == null) {
                $("#clients tr:last-child").find('input[name$="ClientCode"]').focus();
            }
			rescanUnobtrusiveValidation('#main');
        }
    });
}

function callAjaxClientSearch() {
    var skip = parseInt($('#clientResultCount').html());
    $.ajax({
        type: 'GET',
        url: "/Relationship/SearchClientCases",
        data: {
            FStartDate: fcdStart,
            FEndDate: fcdEnd,
            clientCode: clientCode,
            clientTypeId: clientType,
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
            var txt = "";
            if (len > 0) {
                total = parseInt(data[0].total);
                //console.log(total);
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

function hideShowLoadButton(allShown) {
    $("#loadMore").toggle(!allShown);
}

function setSearchParameters() {
    fcdStart = ($("#FirstContactDateStart").length) ? $("#FirstContactDateStart").val() : null;
    fcdEnd = ($("#FirstContactDateEnd").length) ? $("#FirstContactDateEnd").val() : null;
    sStart = ($("#ServiceDateStart").length) ? $("#ServiceDateStart").val() : null;
    sEnd = ($("#ServiceDateEnd").length) ? $("#ServiceDateEnd").val() : null;
    clientCode = ($("#ClientCode_Search").length) ? $("#ClientCode_Search").val() : null;
    clientType = ($("#TypeId_Search").length) ? $("#TypeId_Search").val() : null;
}

function setMaxHouseHoldVal() {
    var rowCount = $('.clientRow').length;
    var msg = "The field Household ID must be between 1 and " + rowCount + ".";
    $('input[name$="HouseHoldID"]').each(function () {
        $(this).attr('data-val-range-max', rowCount);
        $(this).attr('data-val-range', msg);
    });
   
}
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
	        $.ajax('/Relationship/GetClients?ClientCode=' + query,
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


function reorderRowIndexes(rowClass, rowData) {
    var currentname, newname, currentid, newid, aria;

    $(rowClass).each(function (index) {
        $(this).find('span.help-block').each(function () {
            currentname = $(this).attr('data-valmsg-for');
            newname = currentname.substr(0, currentname.indexOf('[') + 1) + index + currentname.substr(currentname.indexOf(']'), currentname.length - 1);
            $(this).attr('data-valmsg-for', newname);
        });

        $(this).find(rowData + '[name]').each(function () {
            currentname = $(this).attr('name');
            currentid = $(this).attr('id');
            aria = $(this).attr('aria-describedby');


            newname = currentname.substr(0, currentname.indexOf('[') + 1) + index + currentname.substr(currentname.indexOf(']'), currentname.length - 1);
            newid = currentid.substr(0, currentid.indexOf('_') + 1) + index + currentid.substr(currentid.indexOf('__'), currentid.length - 1);
            if (aria != undefined)
                $(this).attr('aria-describedby', aria.substr(0, currentid.indexOf('_') + 1) + index + currentid.substr(currentid.indexOf('__'), currentid.length - 1));

            $(this).attr('name', newname);
            $(this).attr('id', newid);
        });
    });
}

function getCases($this, $selected) {
    if ($this.val() != "") {
        var isSA = $this.closest('tr').find('input[type="hidden"][name$="CaseID"]').length;
        var $caseList = $this.closest('tr').find('select[name$="CaseID"]');

        $.ajax({
            type: 'GET',
            url: "/Relationship/GetCases",
            data: { clientCode: $this.val(), householdID: $('#ID').val() },
            dataType: 'json',
            success: function (data) {
                if (isSA) {
                    $this.closest('tr').find('input[type="hidden"][name$="CaseID"]').val('1');
                }
                else {
                    $caseList.find('option').remove().end();
                    $.each(data, function (i, caseId) {
                        if (data[i].caseId != null && $selected.val() != null && data[i].caseId == $selected.val())
                            $caseList.append($('<option>').text(data[i].caseId).attr('value', data[i].caseId).attr('selected', 'selected'));
                        else
                            $caseList.append($('<option>').text(data[i].caseId).attr('value', data[i].caseId));
                    });
                }
                if (data[0] != null) {
                    $this.closest('tr').find('input[name$="ClientID"]').val(data[0].clientId);
                }
            },
            error: function (xhr) {
                //console.log(xhr.responseText);
            }
        });
    }
}

function validateMyForm() {
    if ($('#main').valid()) {
        $('#main').submit();
    }
}

function validateMyForm2() {
    $('#saveAddNew').val('1');
    validateMyForm();
}

function reorderIndexes() {
    reorderRowIndexes(".clientRow", ".clientData");
}

//function GetCaseOption(count){
//	return "<option value='" + count + "'>" + count + "</option>"
//}

//function adjustHouseholdIdList(remove) {
//	if (remove) {
//		$('select[name$="HouseHoldID"]').each(function () {
//			$(this).find('option').last().remove();
//		});
//	}
//	else {
//		$('select[name$="HouseHoldID"]').each(function () {
//			$(this).append(GetCaseOption($('select[name$="HouseHoldID"]').length));
//		});
//	}
//}

function getAllExistingClientIds() {
    var alreadyExists = {};
    var existingClientIds = $('input[name$="ClientID"]');
    existingClientIds.each(function (i) {
        alreadyExists[i] = $(this).val();
    });
    return alreadyExists;
}