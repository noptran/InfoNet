var confirmDeleteTitle = "Are you sure you want to do that?";
var confirmDeleteTextStart = "You have marked the following:<ul>";
var confirmDeleteTextEnd = "</ul>If you continue, <strong>the changes will be discarded.</strong>";
var confirmDeleteConfirmButtonClass = "btn-danger";
var confirmDeleteDiaglogClass = "modal-dialog icjia-modal-danger";

$(document.body).on('click', '#Save', function () {
	if (ValidateReportSelections()) {
		IsModifiedCenterAction();
		$('#main').submit();
	}
});

$(document.body).on('click', '#filterClear', function () {
    if ($("#main thead th select").length != $("#main thead th option:selected[value='']").length) {
        $('#filters').children().find('select').each(function () {
            $(this).val('');
        });
        DeletesCheckedConfirm();
    }
});

$(document.body).on('change', "#SelectedRunDate,#SelectedTitle,#SelectedBeginDate,#SelectedEndDate,#SelectedType,#SelectedCenterId,#SelectedFundingSource,#SelectedCenterApproval,#SelectedSubmitterCenterId", function () {
	IsModifiedCenterAction();
	DeletesCheckedConfirm();
});

function IsModifiedCenterAction() {
	if ($("#ViewRole").val() == 'funder') {
		$("select[name$='.SelectedCenterActionId']").each(function (i, obj) {
			if ($(this).find('option[selected]').val() != $(this).val()) {
				$(this).siblings('input[type="hidden"][name$="CenterActionModified"]').val(true);
			} else {
				$(this).siblings('input[type="hidden"][name$="CenterActionModified"]').val(false);
			}
		});
	}
}

function ValidateReportSelections() {
	return true;
}

function ScheduledReportFilter() {
    $.ajax({
        url: '/Report/ScheduledReportFilter',
        type: 'GET',
        datatype: 'html',
        data: {
            SelectedRunDate: $("#SelectedRunDate").val(),
            SelectedTitle: $("#SelectedTitle").val(),
            SelectedBeginDate: $("#SelectedBeginDate").val(),
            SelectedEndDate: $("#SelectedEndDate").val(),
            SelectedType: $("#SelectedType").val(),
            SelectedCenterId: $("#SelectedCenterId").val(),
            SelectedFundingSource: $("#SelectedFundingSource").val(),
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

$(document.body).on("click", ".scheduledPagedList a[href]", function (e) {
	e.preventDefault();
	$('#main').find('input:hidden[name="PageNumber"]').val(getUrlParameter('page', $(this).attr('href')));
	DeletesCheckedConfirm();
});

$(document.body).on("change", ".icjia-pagedlist-drop-menu", function (e) {
	e.preventDefault();
	$('#main').find('input:hidden[name="PageNumber"]').val(1);
	DeletesCheckedConfirm();
});