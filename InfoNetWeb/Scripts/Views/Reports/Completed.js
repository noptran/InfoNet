var CompletedReports = {};

var confirmDeleteTitle = "Are you sure you want to do that?";
var confirmDeleteTextStart = "You have marked the following:<ul>";
var confirmDeleteTextEnd = "</ul>If you continue, <strong>the changes will be discarded.</strong>";
var confirmDeleteConfirmButtonClass = "btn-danger";
var confirmDeleteDiaglogClass = "modal-dialog icjia-modal-danger";

$(function () {
    CompletedReports.RejectCommentRequired = "Comment is required when rejecting the report.";
    if ($("#RptJobId").val() != null) {
		var id = $("#RptJobId").val();
		var $row = $('#reports').find('input[name$=".Id"][value="' + id + '"]').closest('tr');
		
		completedReportApproval($row, id);

		$('#completedReportApproval').modal('show');
	}
});

function completedReportApproval($row, id) {
	var $modal = $("#completedReportApproval");

	$("#approvalReport").attr('src',"/Report/ViewCompleted/" + id + "?format=html");

	if ($("#ViewRole").val() != 'funder') {
		$("#approvalCSV").attr('href', "/Report/ViewCompleted/" + id + "?format=csv");
	}

	$("#approvalHTML").attr('href', "/Report/ViewCompleted/" + id + "?format=html");
	$("#approvalPDF").attr('href', "/Report/ViewCompleted/" + id + "?format=pdf");

	if ($("#ViewRole").val() != 'funder' && $row.find('input[name$=".ApprovalStatusId"]').val() == $("#approvalPending").val()) {
		$modal.find('#approvalApproverComment').val($row.find('input[name$=".CenterComment"]').val());
		$modal.find('#approvalStatusId').val($row.find('input[name$=".ApprovalStatusId"]').val());
		$modal.find("[data-icjia-approve='true'], [data-icjia-approve-show='true']").show();
		$('#approvalReport').css('height', '87%');
	} else {
		$modal.find("[data-icjia-approve='true'], [data-icjia-approve-show='true']").hide();
		$('#approvalReport').css('height', '95%');	
	}

	$("#approvalCurrentStatus").text("Status: " + $row.find('.openCompletedReportModal').text());
	$("#approvalId").val(id);
}

	$(document.body).on('click', '#Save', function () {
		if (ValidateReportSelections()) {
			if ($("#reports input[name$='.FlagForDelete']:checked").length > 0) {
				$('#main').submit();
			}
		}
	});

	$(document.body).on('click', '[data-icjia-approve]', function () {

	    if ($(this).attr('id') == 'approvalReject' && !$.trim($("#approvalApproverComment").val())) {
	        ErrorFormat($("#approvalApproverComment"), "approvalApproverComment", CompletedReports.RejectCommentRequired);
	        $('#approvalApproverComment').focus();
	        return false;
	    }
	    
		if (ValidateReportSelections()) {
			$(this).attr("data-icjia-approved") == "true" ? $("#approvalStatusId").val($("#approvalApproved").val()) : $("#approvalStatusId").val($("#approvalRejected").val());
			$('#approveReject').submit();
		}
	});


	function ValidateReportSelections() {
		return true;
	}

	$("#approvalApproverComment").on('keypress', function () {
	    ErrorClear($(this), "approvalApproverComment", CompletedReports.RejectCommentRequired);
	});

	$('#completedReportApproval').on('hide.bs.modal', function (e) {
		var id = $('#approvalId').val();
		var approvalRow = $('#reports').find('input[name$=".Id"][value="' + id + '"]').closest('tr');

		var status = approvalRow.find('input[name$=".Status"]').val();
		var comment = approvalRow.find('input[name$=".CenterComment"]').val();
		ErrorClear($("#approvalApproverComment"), "approvalApproverComment", CompletedReports.RejectCommentRequired);
		//$('body, html').removeClass('overflow-hidden');
		
		$("#approvalReport").attr('src', "");
	});

	$('#completedReportApproval').on('show.bs.modal', function (e) {
		$('.modal .modal-body').css('overflow-y', 'auto');
		$('.modal .modal-body').css('height', $(window).height() * 0.75);
		//$('body, html').addClass('overflow-hidden');
	});

	$(document.body).on("click", ".openCompletedReportModal", function () {
		var $row = $(this).closest('tr');
		var id = $row.find('input[name$=".Id"]').val();

		completedReportApproval($row, id);
	});

	function ScheduledReportFilter() {
		$.ajax({
			url: '/Report/CompletedReportFilter',
			type: 'GET',
			datatype: 'html',
			data: {
				ReportTitle: $("#ReportTitle").val(),
				SelectedRunDate: $("#SelectedRunDate").val(),
				SelectedTitle: $("#SelectedTitle").val(),
				SelectedBeginDate: $("#SelectedBeginDate").val(),
				SelectedEndDate: $("#SelectedEndDate").val(),
				SelectedSubmittedDate: $("#SelectedSubmittedDate").val(),
				SelectedType: $("#SelectedType").val(),
				SelectedCenterId: $("#SelectedCenterId").val(),
				SelectedFundingSource: $("#SelectedFundingSource").val(),
				SelectedCenterApprovalRejectionDate: $("#SelectedCenterApprovalRejectionDate").val(),
				SelectedCenterApproval: $("#SelectedCenterApproval").val(),
				SelectedSubmitterCenterId: $("#SelectedSubmitterCenterId").val(),
				ViewRole: $("#ViewRole").val(),
				PageSize: $("#icjia-pagedlist-drop-menu").val(),
				PageNumber: $("#PageNumber").val()
			},
			success: function (data) {
				$('#reportsDiv').html(data);
				rescanUnobtrusiveValidation("#main");
			},
			error: function (jqXHR, textStatus, errorThrown) {
				//alert(errorThrown);
			}
		});
	}

	$(document.body).on('click', '#filterClear', function () {
	    if ($("#main thead th select").length != $("#main thead th option:selected[value='']").length) {
	        $('#filters').children().find('select').each(function () {
		    	$(this).val('');
		    });
            DeletesCheckedConfirm();
	    }
	});

	$(document.body).on('change', "#SelectedRunDate,#SelectedTitle,#SelectedBeginDate,#SelectedEndDate,#SelectedType,#SelectedCenterId,#SelectedFundingSource,#SelectedSubmittedDate,#SelectedCenterApprovalRejectionDate,#SelectedCenterApproval,#SelectedSubmitterCenterId", function () {
	
		DeletesCheckedConfirm();
	});

	function ValidateReportSelections() {
		return true;
	}

	function DeletesCheckedConfirm() {
		var modifiedMessage = deleteMessage = "";

		if ($("#ViewRole").val() == 'funder') {
			var cntModifiedActions = $('[name$="CenterActionModified"][value="true"]').siblings('[name$=".FlagForDelete"]:checkbox:not(:checked)').length;

			if (cntModifiedActions > 0) {
				modifiedMessage = confrimCreateListItem(cntModifiedActions, "Center Action", "Center Action");
			}
		} else {
			cntModifiedActions = 0;
		}

		var checkedDeleted = $("#reports input[name$='.FlagForDelete']:checked").length;

		if (checkedDeleted > 0) {
			deleteMessage = confrimCreateListItem(checkedDeleted, "Scheduled Report Record", "Delete");
		}

		if (checkedDeleted > 0 || cntModifiedActions > 0) {
			$.confirm({
				title: confirmDeleteTitle,
				text: confirmDeleteTextStart + modifiedMessage + deleteMessage + confirmDeleteTextEnd,
				confirmButtonClass: confirmDeleteConfirmButtonClass,
				dialogClass: confirmDeleteDiaglogClass,
				confirm: function () {
					ScheduledReportFilter();
				},
				cancel: function () {
				    // nothing to do
				}
			});
		} else {
			ScheduledReportFilter();
		}
	}

	function confrimCreateListItem(cntModified, title, section) {
		if (section == 'Delete') {
			return "<li class='text-danger' style='font-weight:bold; list-style: square;'>" + cntModified + " " + $.trim(title) + ((cntModified > 1) ? 's' : '') + " marked as Delete </li>";
		} else if (section == 'Center Action') {
			return "<li class='text-danger' style='font-weight:bold; list-style: square;'>" + cntModified + " " + $.trim(title) + ((cntModified > 1) ? 's have been' : ' has been') + " Modified </li>";
		}
	}

	$(document.body).on("click", ".completedPagedList a[href]", function (e) {
		e.preventDefault();
		$('#main').find('input:hidden[name="PageNumber"]').val(getUrlParameter('page',$(this).attr('href')));
		DeletesCheckedConfirm();
	});

	$(document.body).on("change", ".icjia-pagedlist-drop-menu", function (e) {
		e.preventDefault();
		$('#main').find('input:hidden[name="PageNumber"]').val(1);
		DeletesCheckedConfirm();
	});