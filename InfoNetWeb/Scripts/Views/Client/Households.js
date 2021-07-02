var confirmEditLookupListTitle = "Are you sure you want to do that?";
var confirmEditLookupListTextStart = "You've marked following for deletion:<ul>";
var confirmEditLookupListTextEnd = "</ul>If you continue, the record/records will be <strong>permanently deleted</strong>. <br/> To undo these changes, click on the Undo Changes button.";
var confirmEditLookupListConfirmButtonClass = "btn-danger";
var confirmEditLookupListDiaglogClass = "modal-dialog icjia-modal-danger";
var fcdStart, fcdEnd, clientCode, clientType;

$(document).ready(function () {

	$('#rangePeriodChosen').change(function () {
		calculateSelectedDateRange('#rangePeriodChosen', '#Start', '#End');
	});

	$("#originalClientCount").val($('.clientRow').length);

    $('#clients').on('click', '.deleteButton', function () {
        var $killrow = $(this).closest('tr');
        $(this).addClass('hide');
        strikeOut($killrow, "clientRow", '.restoreClientRow');
		reorderIndexes();
		var count = parseInt($('#clientCount').val());
		$('#clientCount').val(count - 1);
		$('#clientCount').change();
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
		getCases($(this), null);
	});

	// Add clients via search panel
	$(".clientCases").on("click", ".addClient", function () {
		var $this = $(this);
		var clientID = parseInt($this.closest('tr.client').find('.clientId').html());
		var caseID = parseInt($this.closest('tr.client').find('.caseId').html());
		AddNewClient(clientID, caseID, $('#clientAddNew').val());
		$this.closest('tr').remove();
	});

	$('#clientAddSearch').click(function () {
		$("#searchClients").click();
	});
});

function strikeOut($killrow, killClass, restoreClass) {
    $killrow.find(restoreClass).removeClass('hide');
    $killrow.find('span.help-block').removeClass('help-block').addClass('help-block-deleted');
    $killrow.addClass("deleted");
    $killrow.removeClass(killClass);
    $killrow.children().find("input[type=text],input[type=number],select").attr("disabled", "disabled");
}

// Staff restore button clicks
$(document).on('click', '[data-icjia-role="client.restore"]', function () {
    var $row = $(this).closest('tr');
    $row.removeClass("deleted ");
    $row.addClass("clientRow");
    $row.find(".deleteButton").removeClass("hide");
    $row.find(".restoreClientRow").addClass("hide");
    $row.find('span.help-block-deleted').removeClass('help-block-deleted').addClass('help-block');
    $row.children().find("input[type=text],input[type=number],select").removeAttr("disabled");
    reorderIndexes();
    var count = parseInt($('#clientCount').val());
    $('#clientCount').val(count + 1);
    $('#clientCount').change();
});

function AddNewClient(clientId, caseId, count) {
	$('#clientAddNew').val(parseInt($('#clientAddNew').val()) + 1);
	$.ajax({
		url: "/Households/AddNewClient?clientID=" + clientId + "&caseID=" + caseId + "&count=" + count,
		success: function (partialViewResult) {
			$('#clients tbody').append(partialViewResult);
			var target = $(".newClientCode").removeClass('newClientCode');
			setTypeahead(target);
			//$("#clients tr:last-child").find('input[name$="ClientCode"]').focus();
			$('#clientCount').val(parseInt($('#clientCount').val()) + 1);
			$('#clientCount').change();
			reorderIndexes();
		},
		error: function (xhr, ajaxOptions, thrownError) {
			//console.log(xhr);
		},
		complete: function () {
			if (clientId == null) {
				$("#clients tr:last-child").find('input[name$="ClientCode"]').focus();
			}
		}
	});
}

function callAjaxClientSearch() {
	var skip = parseInt($('#clientResultCount').html());
	$.ajax({
		type: 'GET',
		url: "/Households/SearchCasesHousehold",
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
			$.ajax('/Households/GetClientsHousehold?ClientCode=' + query,
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
	rescanUnobtrusiveValidation('#main');
}

function getCases($this, selected) {
	if ($this.val() != "") {
		var isSA = $this.closest('tr').find('input[type="hidden"][name$="CaseID"]').length;
		var $caseList = $this.closest('tr').find('select[name$="CaseID"]');

		$.ajax({
			type: 'GET',
			url: "/Households/GetCasesForHousehold",
			data: { clientCode: $this.val(), householdID: $('#ID').val() },
			dataType: 'json',
			success: function (data) {
				if (isSA) {
					$this.closest('tr').find('input[type="hidden"][name$="CaseID"]').val('1');
				}
				else {
					$caseList.find('option').remove().end();
					$.each(data, function (i, caseId) {
						if (i == (data.length - 1))
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
	var response = true;
    if (response) {
        $("body").find('.deleted').remove();
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

function getAllExistingClientIds() {
	var alreadyExists = {};
	var existingClientIds = $('input[name$="ClientID"]');
	existingClientIds.each(function (i) {
		alreadyExists[i] = $(this).val();
	});
	return alreadyExists;
}

$("#saveButton").click(function () {
    checkForDeletes("save");
});


$("#saveAddButton").click(function () {
    checkForDeletes("saveAdd");
});

function checkForDeletes(validate) {

    if ($("#originalClientCount").val() > $('.clientRow').length) {
        var count = $("#originalClientCount").val() - $('.clientRow').length;
        var recordDeleteMessage = "<li><strong>" + count + " Client(s) from Household Members</li></strong>";

        $.confirm({
            title: confirmEditLookupListTitle,
            text: confirmEditLookupListTextStart + recordDeleteMessage + confirmEditLookupListTextEnd,
            confirmButtonClass: confirmEditLookupListConfirmButtonClass,
            dialogClass: confirmEditLookupListDiaglogClass,
            confirm: function () {
                if (validate === "save") {
                    if (validateMyForm()) {
                        $("body").find('.deleted').remove();
                        $('#main').submit();
                    }
                }
                else {
                    if (validateMyForm2()) {
                        $("body").find('.deleted').remove();
                        $('#main').submit();
                    }
                }
            }
        });

    } else {
        if (validate === "save") {
            if (validateMyForm()) {
                $("body").find('.deleted').remove();
                $('#main').submit();
            }
        }
        else {
              if (validateMyForm2()) {
                        $("body").find('.deleted').remove();
                        $('#main').submit();
                    }
        }
    }
}