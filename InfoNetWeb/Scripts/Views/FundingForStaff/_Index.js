var confirmConfirmButton = "Yes";
var confirmCancelButton = "Cancel";
var confirmConfirmButtonClass = "btn-danger";
var confirmDiaglogClassDanger = "modal-dialog icjia-modal-danger";

$(function () {
	$('#AssignedStaffList').change();

	updateLastFundingDateDisplay();
});

//#region AssignServices
function multiselectModalAssignServices() {
	multiselectModal("assignServices", $('#FFSAssignServices_AvailableAndAssignedServiceIDs > option').length, 'Select Assigned Services', bootstrapFindSize() == 'lg' ? $(window).height() - (320) : $(window).height() - (340));
}

$('#AssignServices').submit(function () {
	$.ajax({
		url: this.action,
		type: 'POST',
		data: $("#FFSAssignServices_SelectedEmployeeToDuplicateID,#FFSAssignServices_AvailableAndAssignedServiceIDs,#SelectedFundIssueDateId,#SelectedStaffSVID").serialize() + "&SelectedFundIssueDate=" + $('#SelectedFundIssueDateId :selected').text(),
		success: function (data) {
			if (data.hasOwnProperty('Error')) {
				errorShow("Unable to update Services/Programs.");
			} else if (data.hasOwnProperty('Success')) {
				servicesProgramsUpdate();
				$('#assignServicesModal').modal('hide');
			} else {
				$("#assignServicesModalBody").html(data);
			}
			ffsSetClean("#AssignServices");
		},
		error: function (jqXHR, textStatus, errorThrown) {
			errorShow("Unable to update Services/Programs.");
		}
	});
	return false;
});

$("[data-icjia-role='ffs.assignServices']").click(function () {
	matchServicesForUpdate();
});

$("[data-icjia-role='ffs.assignServices']").on('click', function () {
	assignServicesUpdate();
	$("#currentStaffMemberName").text($("#AssignedStaffList option:selected").text());
});

$('#FFSAssignServices_SelectedEmployeeToDuplicateID').on('change', function () {
	this.value == '' ? $('#assignServicesMultiSelect').show() : $('#assignServicesMultiSelect').hide();
});

$('[data-icjia-save="ffs.AssignServices.save"]').on('click', function (e) {
	var isRemovingAssignedServices = false;

	$("#FFSAssignServices_AvailableAndAssignedServiceIDs > option:not(:selected)").each(function () {
		if (this.text.lastIndexOf("*", 0) === 0) {
			isRemovingAssignedServices = true;
			return false;
		}
	});

	if (isRemovingAssignedServices) {
		e.preventDefault();
		$.confirm({
			text: 'Removing the services marked with an asterisk (*) will remove the service and funding information assigned for the current funding period.',
			confirmButton: confirmConfirmButton,
			confirm: function () {
				$('#AssignServices').submit();
			}
		});
	}
});

function assignServicesUpdate() {
	$.ajax({
		url: '/FundingForStaff/AssignServicesUpdate',
		type: 'GET',
		datatype: 'html',
		data: $('#SelectedFundIssueDateId,#SelectedStaffSVID').serialize() + "&SelectedFundIssueDate=" + $('#SelectedFundIssueDateId :selected').text(),
		success: function (data) {
			var result = '';

			var assignedServices = JSON.parse(data['AssignedServices']);
			var availableServices = JSON.parse(data['AvailableServices']);

			for (var i = 0, iL = availableServices.length; i < iL; i++) {
				result += '<option value="' + availableServices[i].ServiceProgramID + '"' + (jQuery.inArray(availableServices[i].ServiceProgramID, assignedServices) !== -1 ? ' selected ' : '') + '>' + availableServices[i].Name + '</option>';
			}

			$("#FFSAssignServices_AvailableAndAssignedServiceIDs").html(result);
			ffsSetClean("#AssignServices");
			multiselectModalAssignServices();
		},
		error: function (jqXHR, textStatus, errorThrown) {
			errorShow("Unable to assign Services/Programs for selected staff.");
		}
	});
}
//#endregion AssignServices

//#region EditStaffList
function multiselectModalEditStaffList() {
	multiselectModal("editStaffList", $('#FFSEditStaffList_AvailableAndAssignedStaffSVIDs > option').length, 'Select Assigned Staff', $(window).height() - (240));
}

$('#editStaffList').on('click', function () {
	editStaffListUpdate();
});

$(document).on('onload, change', '#AssignedStaffList', function () {
	$('#SelectedStaffSVID').val(this.value);
	staffListDropdownColors();
});

$(document).on('change', '#AssignedStaffList', function () {
	servicesProgramsUpdate();
	$('#cntStaff').text('(Results: ' + $('#AssignedStaffList > option').length + ')');
});

$('#EditStaffList').submit(function () {
	$.ajax({
		url: this.action,
		type: this.method,
		data: $('#FFSEditStaffList_AvailableAndAssignedStaffSVIDs,#SelectedFundIssueDateId').serialize(),
		success: function (data) {
			if (data.hasOwnProperty('Error')) {
				errorShow("Unable to update the Staff List.");
			} else if (data.hasOwnProperty('Success')) {
				staffListUpdate();
				$('#editStaffListModal').modal('hide');
			} else {
				$("#editStaffListModalBody").html(data);
			}
			ffsSetClean("#EditStaffList");
		},
		error: function (jqXHR, textStatus, errorThrown) {
			errorShow("Unable to update the Staff List.");
		}
	});
	return false;
});

$('[data-icjia-save="ffs.EditStaffList.save"]').on('click', function (e) {
	var isRemovingAssignedStaffVolunteer = false;

	$("#FFSEditStaffList_AvailableAndAssignedStaffSVIDs > option:not(:selected)").each(function () {
		if (this.text.lastIndexOf("*", 0) === 0) {
			isRemovingAssignedStaffVolunteer = true;
			return false;
		}
	});

	if (isRemovingAssignedStaffVolunteer) {
		e.preventDefault();
		$.confirm({
			text: 'Removing the staff members marked with an asterisk (*) will remove any service and funding information assigned to them for the current funding period.',
			confirm: function () {
				$('#EditStaffList').submit();
			}
		});
	}
});

function editStaffListUpdate() {
	$.ajax({
		url: '/FundingForStaff/EditStaffListUpdate',
		type: 'GET',
		datatype: 'html',
		data: $('#SelectedFundIssueDate,#SelectedFundIssueDateId').serialize(),
		success: function (data) {

			var result = '';

			var assignedStaff = JSON.parse(data['AssignedStaff']);
			var availableStaff = JSON.parse(data['AvailableStaff']);

			for (var i = 0, iL = availableStaff.length; i < iL; i++) {
				result += '<option value="' + availableStaff[i].SVID + '"' + (jQuery.inArray(availableStaff[i].SVID, assignedStaff) !== -1 ? ' selected ' : '') + '>' + availableStaff[i].Name + '</option>';
			}

			$("#FFSEditStaffList_AvailableAndAssignedStaffSVIDs").html(result);
			ffsSetClean("#EditStaffList");
			multiselectModalEditStaffList();
		},
		error: function (jqXHR, textStatus, errorThrown) {
			errorShow("Unable to update the Staff List.");
		}
	});
}

function matchServicesForUpdate() {
	$.ajax({
		url: '/FundingForStaff/MatchServicesForUpdate',
		type: 'GET',
		datatype: 'html',
		data: $('#SelectedFundIssueDateId,#SelectedStaffSVID').serialize(),
		success: function (data) {
			var resultMatchServices = '';

			var matchServicesFor = JSON.parse(data['MatchServicesFor']);

			for (var i = 0, iL = matchServicesFor.length; i < iL; i++) {
				resultMatchServices += '<option value="' + matchServicesFor[i].SVID + '">' + matchServicesFor[i].Name + '</option>';
			}

			$("#FFSAssignServices_SelectedEmployeeToDuplicateID").html(resultMatchServices).prepend("<option value=''></option>").val('').change();

			ffsSetClean("#AssignServices");
		},
		error: function (jqXHR, textStatus, errorThrown) {
			errorShow("Unable to match Services/Programs for selected staff.");
		}
	});
}

function staffListDropdownColors() {
	$('#AssignedStaffList > option').each(function () {
		var $this = $(this);
		var last3 = $this.text().substr($this.text().length - 3);
		switch (last3) {
			case "(S)":
				$this.css('background-color', '#f7f4d3');
				break;
			case "(V)":
				$this.css('background-color', '#afd9ee');
				break;
		}
	});

}
//#endregion EditStaffList

//#region FundIssueDate
$('#SelectedFundIssueDateId').on('change', function () {
	$('#SelectedFundIssueDate').val($('#SelectedFundIssueDateId :selected').text());

	$(this).prop('selectedIndex') == 0 ? $('[data-icjia-role="ffs.fundDate.delete"]').prop('disabled', false) : $('[data-icjia-role="ffs.fundDate.delete"]').prop('disabled', true);

	staffListUpdate();
});

function updateLastFundingDateDisplay() {
	$('#showFFSAdd_SelectedFundIssueDate').val($('#SelectedFundIssueDateId option:first').text());
	$('#FFSAdd_SelectedFundIssueDate').val($('#SelectedFundIssueDateId option:first').text());
}

$('[data-icjia-role="ffs.fundDate.add"]').click(function () {
	$.ajax({
		url: '/FundingForStaff/AddFundingIssueDateClean',
		type: 'GET',
		success: function (data) {
			$("#addFundingIssueDateModalBody").html(data);
			updateLastFundingDateDisplay();
			ffsSetClean("#AddFundingIssueDate");
			$("#main").dirtyForms('setClean');
		},
		error: function (jqXHR, textStatus, errorThrown) {
			errorConfirmDateIssued("Unable to add Date Issued Record.");
		}
	});

	$('#addFundingIssueDateModal').modal({ backdrop: 'static', keyboard: false });
});

$('#AddFundingIssueDate').submit(function () {
	$.ajax({
		url: this.action,
		type: this.method,
		data: $('#FFSAdd_NewFundIssueDate,#SelectedFundIssueDateId,#FFSAdd_SelectedFundIssueDate').serialize(),
		success: function (data) {
			if (data.hasOwnProperty('Error')) {
				errorConfirmDateIssued(data['Error'][0]);
			} else if (data.hasOwnProperty('Success')) {
				ffsSetClean("#AddFundingIssueDate");
				window.location.href = data['Success'][0];
			} else {
				$("#addFundingIssueDateModalBody").html(data);
			}

			updateLastFundingDateDisplay();
		},
		error: function (jqXHR, textStatus, errorThrown) {
			errorConfirmDateIssued("Unable to add Date Issued Record.");
		}
	});

	return false;
});

$('[data-icjia-role="ffs.fundDate.delete"]').on('click', function () {
	var confirmTextStart = "You have marked the following:<ul>";
	var confirmTextEnd = "</ul>If you continue, the record will be <strong>permanently deleted</strong>.";

	$.confirm({
		text: confirmTextStart + "<li class='text-danger' style='font-weight:bold; list-style: square;'> Date Issued: " + $('#SelectedFundIssueDateId :selected').text() + "</li>" + confirmTextEnd,
		confirmButtonClass: confirmConfirmButtonClass,
		dialogClass: confirmDiaglogClassDanger,
		confirm: function () {
			$('.icjia-loading').show();
			$.ajax({
				url: '/FundingForStaff/DeleteFundingIssueDate',
				type: 'POST',
				data: { fundingDateID: $('#SelectedFundIssueDateId :selected').val() },
				success: function (data) {
					$('.icjia-loading').hide();

					if (data.hasOwnProperty('Error')) {
						errorConfirmDateIssued(data['Error'][0]);
					} else {
						$('form[data-icjia-role="dirtyform"]').dirtyForms('setClean');
						window.location.href = data;
					}
				},
				error: function (jqXHR, textStatus, errorThrown) {
					errorConfirmDateIssued("Unable to delete Date Issued Record.");
				}
			});
		},
		cancel: function () {
			// nothing to do
		}
	});
});
//#endregion FundIssueDate

//#region Multiple Uses
function servicesProgramsUpdate() {
	$.ajax({
		url: '/FundingForStaff/ServicesProgramsUpdate',
		type: 'GET',
		datatype: 'html',
		data: $('#SelectedFundIssueDateId, #SelectedStaffSVID').serialize() + "&SelectedFundIssueDate=" + $('#SelectedFundIssueDateId :selected').text(),
		success: function (data) {
			$('#servicesPrograms tbody').html(data);

			$('#cntServicesPrograms').text('(Results: ' + $('#servicesPrograms .cntServicesPrograms').length + ')');

			$('[data-icjia-role="ffs.multiFundAssignment"]').prop('disabled', $('#servicesPrograms .cntServicesPrograms').length > 0 && $('#AssignedStaffList').has('option').length > 0 ? false : true);
			$('[data-icjia-role="ffs.assignServices"]').prop('disabled', $('#AssignedStaffList').has('option').length > 0 ? false : true);
			$('[data-icjia-role="ffs.fundDate.delete"], [data-icjia-role="ffs.reports"], #AssignedStaffList, #editStaffList, #SelectedFundIssueDateId').prop('disabled', !$('#SelectedFundIssueDateId').val() ? true : false);
			 
			ffsSetClean("#EditStaffList");
		},
		error: function (jqXHR, textStatus, errorThrown) {
			alert(errorThrown);
		}
	});
}

function staffListUpdate() {
	$.ajax({
		url: '/FundingForStaff/StaffListUpdate',
		type: 'GET',
		datatype: 'json',
		data: $('#SelectedFundIssueDateId, #SelectedFundIssueDate').serialize(),
		success: function (data) {
			var result = '';
			var assignedStaff = JSON.parse(data['AssignedStaff']);

			for (var i = 0, iL = assignedStaff.length; i < iL; i++) {
				result += '<option value="' + assignedStaff[i].SVID + '">' + assignedStaff[i].Name + '</option>';
			}

			$("#AssignedStaffList").html(result).change();
			ffsSetClean("#EditStaffList");
		},
		error: function (jqXHR, textStatus, errorThrown) {
			alert(errorThrown);
		}
	});
}

function calculateHeightDifferenceTwoElements(topElement, bottomElement) {
	var topElementBottomPixel = topElement.offset().top + topElement.outerHeight(true);
	return (bottomElement.offset().top - topElementBottomPixel);
}

function multiselectSetFFSColors() {
	$('.ms-options > ul > li').each(function () {
		var $this = $(this);
		var last3 = $this.text().substr($this.text().length - 3);
		switch (last3) {
			case "(S)":
				$this.css('border-left', 'solid 25px #f7f4d3');
				break;
			case "(P)":
				$this.css('border-left', 'solid 25px #afd9ee');
				break;
			case "(V)":
				$this.css('border-left', 'solid 25px #afd9ee');
				break;
			case "(H)":
				$this.css('border-left', 'solid 25px #e4b9b9');
				break;
			default:
				$this.css('border-left', 'solid 25px #ececec');
				break;
		}
	});
}

function multiselectSetColor() {
	$('.ms-options > ul > li').each(function () {
		$(this).css('border-left', 'solid 25px #ececec');
	});
}

function multiselectNumberOfColumns(selectOptions) {
	var columns = 1;

	if (selectOptions == 0 || selectOptions == 1) {
		columns = 1;
	} else if (selectOptions == 2) {
		columns = 2;
	} else {
		var screenSize = bootstrapFindSize();

		if (screenSize == 'sm' || screenSize == 'md') { columns = 2 };
		if (screenSize == 'lg') { columns = 3 };
	}

	return columns;
}
//#endregion Multiple Uses

//#region Multi-Fund Assignment
$(document).on("click", "[data-icjia-role='ffs.multiFundAssignment']", function () {
	$.ajax({
		url: '/FundingForStaff/MultiFundAssignmentUpdate',
		type: 'POST',
		data: $('#SelectedFundIssueDate,#SelectedFundIssueDateId,#SelectedStaffSVID').serialize(),
		success: function (data) {
			$("#multiFundAssignmentModalBody").html(data);
			$("#currentStaffMemberNameMultiFund").text($("#AssignedStaffList option:selected").text());

			ffsSetClean("#MultiFundAssignment");
			multiFundAssignmentModal();
		},
		error: function (jqXHR, textStatus, errorThrown) {
			errorShow("Unable to complete Multi-Fund Assignment.");
		}
	});
});

$('#MultiFundAssignment').submit(function () {
	$.ajax({
		url: this.action,
		type: 'POST',
		data: $('#MultiFundAssignment,#SelectedFundIssueDateId,#SelectedStaffSVID').serialize(),
		success: function (data) {
			if (data.hasOwnProperty('Error')) {
				errorShow("Unable to complete Multi-Fund Assignment.");
			} else if (data.hasOwnProperty('Success')) {
				servicesProgramsUpdate();
				$('#multiFundAssignmentModal').modal('hide');
				ffsSetClean("#MultiFundAssignment");
			} else {
				$("#multiFundAssignmentDiv").html(data);

				multiFundFooterSummary();
				multiFundAssignmentSetMaxHeightScrollableDivs();
			}
		},
		error: function (jqXHR, textStatus, errorThrown) {
			errorShow("Unable to complete Multi-Fund Assignment.");
		}
	});

	return false;
});

function multiFundAssignmentModal() {
	$('#multiFundAssignmentModal .modal-content').hide();
	$('#multiFundAssignmentModal .modal-content').css('height', $(window).height() - 20);

	multiFundFooterSummary();

	$('#multiFundAssignmentModal').modal({ backdrop: 'static', keyboard: false });
	$('#multiFundAssignmentModal .modal-content').show();
	multiFundAssignmentSetMaxHeightScrollableDivs();
}

function multiFundFooterSummary() {
	var sum = 0;
	var cntServicesSelected = $('#divMultiFundServices').find('input:checked').length;

	$(".multiFundFundCheckbox:checked").each(function () {
	    sum += +parseInt($(this).closest('tr').find('.multiFundFundPercent').val());
	});

	$("#multiFundFundsTotalPercent").text(sum);
	$("#multiFundServicesTotalSelected").text(cntServicesSelected);

	if (sum == 100) {
	    $("#multiFundFundsTotalPercent, #multiFundFundsTotal, #multiFundFundsTotalError").removeClass('field-validation-error');
	    $("#multiFundFundsTotalError").html('');
	} else {
		$("#multiFundFundsTotalPercent, #multiFundFundsTotal, #multiFundFundsTotalError").addClass('field-validation-error');
	}

	if (cntServicesSelected > 0) {
	    $("#multiFundServicesTotal, #multiFundServicesTotalSelected").removeClass('field-validation-error');
	    $("#MFAServicesSelected").html("");
	} else {
		$("#multiFundServicesTotal, #multiFundServicesTotalSelected").addClass('field-validation-error');
	}
}

$(document).on('change', '#multiFundServicesSelectAll', function () {
	var checkedStatus = this.checked;
	$('.multiFundServicesCheckbox:checkbox').each(function () {
		$(this).prop('checked', checkedStatus);
	});
});

$(document).on('change', '.multiFundServicesCheckbox', function (e) {
	multiFundFooterSummary();
});

$(document).on('change', '#multiFundServicesSelectAll', function (e) {
	multiFundFooterSummary();
});

$(document).on('change', '.multiFundFundCheckbox', function () {
	var $this = $(this);
	var replacementVal = 0;

	if ($this.is(':checked')) {
		var sum = 0;
		$(".multiFundFundCheckbox:checked").not($this).each(function () {
			sum += +parseInt($(this).closest('tr').find('.multiFundFundPercent').val());
		});
		if (sum >= 0 || sum <= 100) { replacementVal = 100 - sum; }
	}

	$(this).closest('tr').find('.multiFundFundPercent').val(replacementVal);
	multiFundFooterSummary();
});

$(document).on('change', '.multiFundFundPercent', function () {
	multiFundToggleCheckboxInSameRow(this);
	multiFundFooterSummary();
});

$(document).on('keyup', '.multiFundFundPercent', function () {
	multiFundToggleCheckboxInSameRow(this);
	multiFundFooterSummary();
});

$(document).on('click', '#multiFundFundsDeselectAll', function () {
	multiFundClearAllCheckboxes();
	multiFundClearAllPercents();

	multiFundFooterSummary();
});

function multiFundClearAllPercents() {
	$('.multiFundFundPercent').each(function () {
		$(this).val(0);
	});
}

function multiFundClearAllCheckboxes() {
	$('.multiFundFundCheckbox:checkbox').each(function () {
		$(this).prop('checked', false);
	});
}

function multiFundToggleCheckboxInSameRow(selector) {
	$(selector).val() > 0 ? $(selector).closest('tr').find('.multiFundFundCheckbox').prop("checked", true) : $(selector).closest('tr').find('.multiFundFundCheckbox').prop("checked", false);
}

function multiFundAssignmentSetMaxHeightScrollableDivs() {
	var heightDiff = calculateHeightDifferenceTwoElements($('#multiFundInstructionsPanel'), $('#multiFundAssignmentModal .modal-footer'));
	var mffHeaderFooterHeight = $('#multiFundFundsHeaderMFA').outerHeight(true) + $('#multiFundFundsFooter').outerHeight(true);
	var mfsHeaderFooterHeight = $('#multiFundServicesHeader').outerHeight(true) + $('#multiFundServicesFooter').outerHeight(true);

	$('#divMultiFundFunds').css('max-height', heightDiff - mffHeaderFooterHeight);
	$('#divMultiFundServices').css('max-height', heightDiff - mfsHeaderFooterHeight);
}

$(document).on('hidden.bs.collapse', '#multiFundInstructionsPanel', function (event) {
	multiFundAssignmentSetMaxHeightScrollableDivs();
});

$(document).on('show.bs.collapse', '#multiFundInstructionsPanel', function (event) {
	$('#multiFundAssignmentDiv').hide();
});

$(document).on('shown.bs.collapse', '#multiFundInstructionsPanel', function (event) {
	$('#multiFundAssignmentDiv').show();
	multiFundAssignmentSetMaxHeightScrollableDivs();
});

$('[data-icjia-save="ffs.MultiFundAssignment.save"]').on('click', function (e) {
	if (!$("#MultiFundAssignment").valid()) { e.preventDefault(); $("#MultiFundAssignment").find('.input-validation-error').focus(); }
});
//#endregion Multi-Fund Assignment

//#region Assign Funding Source
$(document).on('click', "[data-icjia-role='ffs.assignFundingSource']", function () {
	var currentServiceProgramID = $(this).attr('data-ffs-currentServiceProgramID');
	var currentServiceDescription = $("~ span:first", $("input[name$='.ServiceProgramID'][value=" + currentServiceProgramID + "]")).text();

	$.ajax({
		url: '/FundingForStaff/AssignFundingSourceUpdate',
		type: 'GET',
		data: $('#SelectedFundIssueDate,#SelectedFundIssueDateId,#SelectedStaffSVID').serialize() + "&CurrentServiceProgramID=" + currentServiceProgramID,
		success: function (data) {
			$("#assignFundingSourceModalBody").html(data);
			$("#currentStaffMemberNameAssignFunding").text($("#AssignedStaffList option:selected").text());
			$(".currentService").text(currentServiceDescription);
			ffsSetClean("#AssignFundingSource");
			assignFundingSourceModal();
		},
		error: function (jqXHR, textStatus, errorThrown) {
			errorShow("Unable to complete Assign Funding Source.");
		}
	});
});

$('#AssignFundingSource').submit(function () {
	$.ajax({
		url: this.action,
		type: 'POST',
		data: $('#AssignFundingSource,#SelectedFundIssueDateId,#SelectedStaffSVID').serialize(),
		success: function (data) {
			if (data.hasOwnProperty('Error')) {
				errorShow("Unable to complete Assign Funding Source.");
			} else if (data.hasOwnProperty('Success')) {
				servicesProgramsUpdate();
				$('#assignFundingSourceModal').modal('hide');
				ffsSetClean("#AssignFundingSource");
			} else {
				$("#servicesSelectionTableAFS").html(data);
				assignFundingSourceFooterSummary();
				assignFundingSourceSetMaxHeightScrollableDivs();
			}
		},
		error: function (jqXHR, textStatus, errorThrown) {
			errorShow("Unable to complete Assign Funding Source.");
		}
	});

	return false;
});

function assignFundingSourceModal() {
	$('#assignFundingSourceModal .modal-content').hide();
	$('#assignFundingSourceModal .modal-content').css('height', $(window).height() - 20);

	assignFundingSourceFooterSummary();

	$('#assignFundingSourceModal').modal({ backdrop: 'static', keyboard: false });
	$('#assignFundingSourceModal .modal-content').show();

	assignFundingSourceSetMaxHeightScrollableDivs();
}

function assignFundingSourceFooterSummary() {
	var sum = 0;

    $(".assignFundingFundCheckbox:checked").each(function () {
        sum += +parseInt($(this).closest('tr').find('.assignFundingFundPercent').val());
    });
	    

	$("#assignFundingFundsTotalPercent").text(sum);

	if (sum == 100) {
	    $("#assignFundingFundsTotalPercent, #assignFundingFundsTotal, #assignFundingFundsTotalError").removeClass('field-validation-error');
	    $("#AFSPercentError").html('');
	} else {
		$("#assignFundingFundsTotalPercent, #assignFundingFundsTotal, #assignFundingFundsTotalError").addClass('field-validation-error');
	}
}

$(document).on('change', '.assignFundingFundCheckbox', function () {
	var $this = $(this);
	var replacementVal = 0;

	if ($this.is(':checked')) {
		var sum = 0;
		$(".assignFundingFundCheckbox:checked").not($this).each(function () {
			sum += +parseInt($(this).closest('tr').find('.assignFundingFundPercent').val());
		});
		if (sum >= 0 || sum <= 100) {
			replacementVal = 100 - sum;
		}
	}

	$(this).closest('tr').find('.assignFundingFundPercent').val(replacementVal);
	assignFundingSourceFooterSummary();
});

$(document).on('change', '.assignFundingFundPercent', function () {
	assignFundingSourceToggleCheckboxInSameRow(this);
	assignFundingSourceFooterSummary();
});

$(document).on('keyup', '.assignFundingFundPercent', function () {
	assignFundingSourceToggleCheckboxInSameRow(this);
	assignFundingSourceFooterSummary();
});

$(document).on('click', '#assignFundingFundsDeselectAll', function () {
	assignFundingSourceClearAllCheckboxes();
	assignFundingSourceClearAllPercents();

	assignFundingSourceFooterSummary();
});

function assignFundingSourceClearAllPercents() {
	$('.assignFundingFundPercent').each(function () {
		$(this).val(0);
	});
}

function assignFundingSourceClearAllCheckboxes() {
	$('.assignFundingFundCheckbox:checkbox').each(function () {
		$(this).prop('checked', false);
	});
}

function assignFundingSourceToggleCheckboxInSameRow(selector) {
	$(selector).val() > 0 ? $(selector).closest('tr').find('.assignFundingFundCheckbox').prop("checked", true) : $(selector).closest('tr').find('.assignFundingFundCheckbox').prop("checked", false);
}

function assignFundingSourceSetMaxHeightScrollableDivs() {
	var heightDiff = calculateHeightDifferenceTwoElements($('#assignFundingSourceInstructionsPanel'), $('#assignFundingSourceModal .modal-footer'));
	var afsHeaderFooterHeight = $('#assignFundingFundsHeader').outerHeight(true) + $('#assignFundingSourceModal .modal-footer').outerHeight(true);

	$('#divAssignFundingFunds').css('max-height', heightDiff - afsHeaderFooterHeight);
}

$(document).on('hidden.bs.collapse', '#assignFundingSourceInstructionsPanel', function (event) {
	assignFundingSourceSetMaxHeightScrollableDivs();
});

$(document).on('show.bs.collapse', '#assignFundingSourceInstructionsPanel', function (event) {
	$('#assignFundingSourceDiv').hide();
});

$(document).on('shown.bs.collapse', '#assignFundingSourceInstructionsPanel', function (event) {
	$('#assignFundingSourceDiv').show();
	assignFundingSourceSetMaxHeightScrollableDivs();
});

$('[data-icjia-save="ffs.AssignFundingSource.save"]').on('click', function (e) {
	if (!$("#AssignFundingSource").valid()) { e.preventDefault(); $("#AssignFundingSource").find('.input-validation-error').focus(); }
});
//#endregion Assign Funding Source

//#region ReportSelectedFunding
$(document).on('click', "[data-icjia-role='ffs.reports.selectedfunding']", function () {
	return reportAjax("SelectedFunding", "SelectedFundingAll", $('#SelectedFundIssueDateId,#SelectedStaffSVID'), "Unable to show Selected Funding.");
});

$(document).on('click', "[data-icjia-pdf-view='ffs.ReportSelectedFunding.viewPDF']", function () {
	return handlePDF($(this), $('#SelectedFundIssueDateId,#SelectedStaffSVID,#FFSReports_FundingDateIDs'), 'SELECTEDFUNDING', '', 'FundingForStaff');
});

$(document).on('click', "[data-icjia-pdf-view='ffs.ReportSelectedFunding.savePDF']", function () {
	return handlePDF($(this), $('#SelectedFundIssueDateId,#SelectedStaffSVID,#FFSReports_FundingDateIDs'), 'SELECTEDFUNDING', 'SAVE', 'FundingForStaff');
});
//#endregion

//#region ReportStaffSelectedFunding
$(document).on('click', "[data-icjia-role='ffs.reports.staffselectedfunding']", function () {
	return reportAjax("StaffSelectedFunding", "SelectedFunding", $('#SelectedFundIssueDateId,#SelectedStaffSVID'), "Unable to show Staff Selected Funding.");
});

$(document).on('click', "[data-icjia-pdf-view='ffs.ReportStaffSelectedFunding.viewPDF']", function () {
	return handlePDF($(this), $('#SelectedFundIssueDateId,#SelectedStaffSVID'), 'STAFFSELECTEDFUNDING', '', 'FundingForStaff');
});

$(document).on('click', "[data-icjia-pdf-view='ffs.ReportStaffSelectedFunding.savePDF']", function () {
	return handlePDF($(this), $('#SelectedFundIssueDateId,#SelectedStaffSVID'), 'STAFFSELECTEDFUNDING', 'SAVE', 'FundingForStaff');
});
//#endregion ReportStaffSelectedFunding

//#region ReportStaffFundingHistory
$(document).on('click', "[data-icjia-role='ffs.ReportStaffFundingHistory.select']", function () {
	return reportAjax("StaffFundingHistory", "ReportStaffFundingHistorySelect", $('#FFSReports_FundingDateIDs,#SelectedStaffSVID'), "Unable to show Staff Funding History.");
});

$(document).on('click', "[data-icjia-pdf-view='ffs.ReportStaffFundingHistory.viewPDF']", function () {
	return handlePDF($(this), $('#SelectedFundIssueDateId,#SelectedStaffSVID,#FFSReports_FundingDateIDs'), 'STAFFFUNDINGHISTORY', '', 'FundingForStaff');
});

$(document).on('click', "[data-icjia-pdf-view='ffs.ReportStaffFundingHistory.savePDF']", function () {
	return handlePDF($(this), $('#SelectedFundIssueDateId,#SelectedStaffSVID,#FFSReports_FundingDateIDs'), 'STAFFFUNDINGHISTORY', 'SAVE', 'FundingForStaff');
});

$(document).on('click', "[data-icjia-role='ffs.reports.stafffundinghistory']", function () {
	reportFundingHistoryUpdate('StaffFundingHistory', 'ReportStaffFundingHistoryUpdate', "Unable to display Staff Funding History.");
});
//#endregion ReportStaffFundingHistory


//#region ReportFundingHistory
$(document).on('click', "[data-icjia-role='ffs.ReportFundingHistory.select']", function () {
	return reportAjax("FundingHistory", "ReportFundingHistorySelect", $('#FFSReports_FundingDateIDs,#SelectedStaffSVID'), "Unable to show Funding History.");
});

$(document).on('click', "[data-icjia-pdf-view='ffs.ReportFundingHistory.viewPDF']", function () {
	return handlePDF($(this), $('#SelectedFundIssueDateId,#SelectedStaffSVID,#FFSReports_FundingDateIDs'), 'FUNDINGHISTORY', '', 'FundingForStaff');
});

$(document).on('click', "[data-icjia-pdf-view='ffs.ReportFundingHistory.savePDF']", function () {
	return handlePDF($(this), $('#SelectedFundIssueDateId,#SelectedStaffSVID,#FFSReports_FundingDateIDs'), 'FUNDINGHISTORY', 'SAVE', 'FundingForStaff');
});

$(document).on('click', "[data-icjia-role='ffs.reports.fundinghistory']", function () {
	reportFundingHistoryUpdate('FundingHistory', 'ReportFundingHistoryUpdate', "Unable to display Staff Funding History.");
});
//#endregion ReportFundingHistory

//#region ReportFundingHistorySelction
function multiselectModalReportFundingHistorySelection() {
	multiselectModal("reportFundingHistorySelection", $('#FFSReports_FundingDateIDs > option').length, 'Select Dates', bootstrapFindSize() == 'lg' ? $(window).height() - (240) : $(window).height() - (260));
}
//#endregion ReportFundingHistorySelection

//#region ReportsShared
function reportSetSelectedStaffMemberName(reportName) {
	$(".reportCurrentStaffMemberName").each(function () {
		if (reportName == "FUNDINGHISTORY") {
			$(this).text("ALL");
		} else {
			$(this).text($("#AssignedStaffList option:selected").text());
		}
	});
}

function handlePDF(e, selectorToSerialze, reportName, pdfAction, reportTitle) {
	pdfAction = pdfAction.trim().toUpperCase();
	reportTitle = reportTitle.trim().toUpperCase();
	var url = e.data('icjia-url') + '?' + selectorToSerialze.serialize() + "&FFSReports.ReportName=" + reportName + "&FFSReports.PDFAction=" + pdfAction;
	window.open(url, reportTitle);
	return false;
}

function reportModal(modalIdName) {
	$modal = $('#' + modalIdName);
	$modalContent = $('#' + modalIdName + ' .modal-content');

	$modalContent.hide();
	$modalContent.css('height', $(window).height() - 20);

	$modal.modal({ backdrop: 'static', keyboard: false });
	$modalContent.show();
}

function reportAjax(reportName, url, selectorToSerialze, errMsg) {
	$.ajax({
		url: "/FundingForStaff/" + url,
		type: "GET",
		data: selectorToSerialze.serialize(),
		success: function (data) {
			$("#report"+ reportName +"ModalBody").html(data);
			if (reportName == "FundingHistory" || reportName == "StaffFundingHistory") {
				$('#reportFundingHistorySelectionModal').modal('hide');
				reportModal("report" + reportName + "Modal");
			} else {
				reportModal("report" + reportName + "Modal");
			}
			$("#Report" + reportName).dirtyForms('setClean');
		},
		error: function (jqXHR, textStatus, errorThrown) {
			errorShow(errMsg);
		}
	});
	return false;
}

function multiselectModal(name, columnsCntSelector, placeholder, maxHeight) {
	$('#' + name + 'Modal .modal-content').hide();
	$('#' + name + 'Modal .modal-content').css('height', $(window).height() - 20);

	$('#' + name + 'MultiSelect select[multiple]').multiselect('unload');

	$('#' + name + 'MultiSelect select[multiple]').promise().done(function () {
		$('#' + name + 'MultiSelect select[multiple]').multiselect({
			columns: multiselectNumberOfColumns(columnsCntSelector),
			search: true,
			maxHeight: maxHeight,
			texts: {
				placeholder: 'Select Assigned Services'
			}
		});
		$('#' + name + 'Modal').modal({ backdrop: 'static', keyboard: false });
	});

	setTimeout(function () {
		multiselectSetFFSColors();
	}, 500);

	$('#' + name + 'Modal .modal-content').show();
}

function reportFundingHistoryUpdate(reportName, url, errMsg) {
	var uppercaseReportName = reportName.toUpperCase();
	$.ajax({
		url: '/FundingForStaff/'+url,
		type: 'GET',
		datatype: 'html',
		data: $('#SelectedStaffSVID').serialize() + "&FFSReports.ReportName=" + uppercaseReportName,
		success: function (data) {
			var result = '';
			var fundingDates = JSON.parse(data['FundingDates']);

			for (var i = 0, iL = fundingDates.length; i < iL; i++) {
				result += '<option value="' + fundingDates[i].FundDateID + '">' + fundingDates[i].FundDate + '</option>';
			}

			$("#FFSReports_FundingDateIDs").html(result);
			$("#ReportFundingHistorySelection").dirtyForms('setClean');

			reportSetSelectedStaffMemberName(uppercaseReportName);
			multiselectModalReportFundingHistorySelection();

			setTimeout(function () {
				$('#selectReportFundingHistorySelection').attr('data-icjia-role', 'ffs.Report' + reportName + '.select');
			}, 500);
		},
		error: function (jqXHR, textStatus, errorThrown) {
			errorShow(errMsg);
		}
	});
	return false;
}

function resizeReportWindow(name) {
	$('#report' + name + 'Modal .modal-content').hide();
	$('#report' + name + 'Modal .modal-content').css('height', $(window).height() - 20);
	$('#report' + name + 'Modal .modal-content').show();
}
//#endregion ReportsShared

//#region Modals
$('.modal').on('shown.bs.modal', function () {
	switch (this.id) {
		case 'assignServicesModal':
			$(this).find("select:visible").focus();
			break;
		case 'editStaffListModal':
			$(this).find("input:checkbox:first:visible").focus();
			break;
		case 'multiFundAssignmentModal':
			multiFundAssignmentSetMaxHeightScrollableDivs();
			$(this).find("input:checkbox:first:visible").focus();
			break;
		case 'assignFundingSourceModal':
			assignFundingSourceSetMaxHeightScrollableDivs();
			$('#assignFundingFundsDeselectAll').focus();
			break;
		case 'reportFundingHistorySelectionModal':
			$(this).find("input:checkbox:first:visible").focus();
			break;
		case 'reportSelectedFundingModal':
			$("#reportSelectedFundingModalBody").scrollTop(0);
			break;
		case 'reportStaffSelectedFundingModal':
			$("#reportStaffSelectedFundingModalBody").scrollTop(0);
			break;
		case 'reportFundingHistoryModal':
			$("#reportFundingHistoryModalBody").scrollTop(0);
			break;
		case 'reportStaffFundingHistoryModal':
			$("#reportStaffFundingHistoryModalBody").scrollTop(0);
			break;
		default:
			$(this).find("input:visible").focus();
	}
});

$('.modal').on('hide.bs.modal', function () {
	$(this).closest('form').dirtyForms('setClean');
});

$(window).on('resize', debounce(function () {
	if ($('#editStaffListModal').hasClass('in')) { multiselectModalEditStaffList(); }

	if ($('#assignServicesModal').hasClass('in')) { multiselectModalAssignServices(); }

	if ($('#multiFundAssignmentModal').hasClass('in')) { multiFundAssignmentModal(); }

	if ($('#assignFundingSourceModal').hasClass('in')) { assignFundingSourceModal(); }

	if ($('#reportStaffFundingHistoryModal').hasClass('in')) { resizeReportWindow("StaffFundingHistory"); }

	if ($('#reportSelectedFundingModal').hasClass('in')) { resizeReportWindow("SelectedFunding"); }

	if ($('#reportStaffSelectedFundingModal').hasClass('in')) { resizeReportWindow("StaffSelectedFunding"); }

	if ($('#reportFundingHistoryModal').hasClass('in')) { resizeReportWindow("FundingHistory"); }

	if ($('#reportFundingHistorySelectionModal').hasClass('in')) { multiselectModalReportFundingHistorySelection(); }

}, 400));
//#endregion

//#region DisplayErrors
function errorConfirmDateIssued(userErrMsg) {
	$.confirm({
		title: "Error",
		text: userErrMsg + "<li class='text-danger' style='font-weight:bold; list-style: square;'> Date Issued: " + $('#SelectedFundIssueDateId :selected').text() + "</li>",
		confirmButton: confirmConfirmButton,
		cancelButton: false,
		confirmButtonClass: confirmConfirmButtonClass,
		dialogClass: confirmDiaglogClassDanger,
		confirm: function () {
			// nothing to do			
		}
	});
}

function errorShow(userErrMsg) {
	var msg = '';
	if (jqXHR.status === 0) {
		msg = 'Not connect.\n Verify Network.';
	} else if (jqXHR.status == 404) {
		msg = 'Requested page not found. [404]';
	} else if (jqXHR.status == 500) {
		msg = 'Internal Server Error [500].';
	} else if (exception === 'parsererror') {
		msg = 'Requested JSON parse failed.';
	} else if (exception === 'timeout') {
		msg = 'Time out error.';
	} else if (exception === 'abort') {
		msg = 'Ajax request aborted.';
	} else {
		msg = 'Uncaught Error.\n' + jqXHR.responseText;
	}

	$.confirm({
		title: "Error",
		text: userErrMsg,
		confirmButton: confirmConfirmButton,
		cancelButton: false,
		confirmButtonClass: confirmConfirmButtonClass,
		dialogClass: confirmDiaglogClassDanger,
		confirm: function () {
			// nothing to do			
		}
	});
}

function ffsSetClean(formToClean) {
	rescanUnobtrusiveValidation(formToClean);
	$(formToClean).dirtyForms('setClean');

	rescanUnobtrusiveValidation("#main");
	$("#main").dirtyForms('setClean');
}
//#endregion 

