var confirmEditServicesTitle = "Are you sure you want to do that?";
var confirmEditServicesTextStart = "You have marked the following:<ul>";
var confirmEditServicesTextEnd = "</ul>If you continue, the records will be <strong>permanently deleted</strong>.";
var confirmEditServicesTextEndSearch = "</ul>Clicking Confirm will cancel these changes and continue the search.";
var confirmEditServicesConfirmButtonClass = "btn-danger";
var confirmEditServicesDiaglogClass = "modal-dialog icjia-modal-danger";
var confirmEditServicesConfirmButton = "Confirm";
var confirmEditServicesCancelButton = "Cancel";

(function () {
	var deletesConfirmed = false;

	$(document).on('submit-valid.icjia', '#main', function (e) {
		if (deletesConfirmed) {
			deletesConfirmed = false;
			return;
		}
		
		var $main = $(this);

		var cntCancellationsDeleted = $main.find('[data-icjia-role="cancellation.isdeleted"]:checked').length;
		var cntDeparturesDeleted = $('#departureModify').find("[name$='.IsDeleted'][value=true]").not('.departures-add').length;  //Don't Count records in add section of table
		var cntDirectServicesDeleted = $main.find('[data-icjia-role="direct.service.isdeleted"]:checked').length;
		var cntHousingServicesDeleted = $main.find('[data-icjia-role="housing.service.isdeleted"]:checked').length;
		var cntReferralsDeleted = $main.find('[data-icjia-role="referral.service.isdeleted"]:checked').length;

		var cntTotalDeleted = cntCancellationsDeleted + cntDirectServicesDeleted + cntHousingServicesDeleted + cntDeparturesDeleted + cntReferralsDeleted;

		if (cntTotalDeleted == 0)
			return;

		e.preventDefault();

		var cancellationsMessageDelete = departureMessageDelete = directServicesMessageDelete = housingServicesMessageDelete = referralMessageDelete = "";

		if (cntCancellationsDeleted > 0)
			cancellationsMessageDelete = confrimCreateListItem(cntCancellationsDeleted, 'Cancellation/No Show Record');

		if (cntDirectServicesDeleted > 0)
			directServicesMessageDelete = confrimCreateListItem(cntDirectServicesDeleted, 'Direct Service Record');

		if (cntDeparturesDeleted > 0)
			departureMessageDelete = confrimCreateListItem(cntDeparturesDeleted, 'Departure Record');

		if (cntHousingServicesDeleted > 0)
			housingServicesMessageDelete = confrimCreateListItem(cntHousingServicesDeleted, 'Housing Service Record');

		if (cntReferralsDeleted > 0)
			referralMessageDelete = confrimCreateListItem(cntReferralsDeleted, 'Referral Record');

		$.confirm({
			title: confirmEditServicesTitle,
			text: confirmEditServicesTextStart + directServicesMessageDelete + housingServicesMessageDelete + departureMessageDelete + cancellationsMessageDelete + referralMessageDelete + confirmEditServicesTextEnd,
			confirmButtonClass: confirmEditServicesConfirmButtonClass,
			dialogClass: confirmEditServicesDiaglogClass,
			confirm: function () {
				deletesConfirmed = true;
				$main.submit();
			}
		});
	});

	$(document.body).on('focusout', '.group1', function () {
		$(this).valid();
	});
})();

function actionCheckboxUncheck(getThis) {
	clearRowValidationErrors(getThis);
	$(getThis).parents('td').find("input:checkbox").not(getThis).each(function () {
		$(this).prop('checked', false);
	});
}

function addHideRow(getThis) {
	$closestRow = $(getThis).closest('tr');
	if ($("table#" + $(getThis).closest('table').attr('id') + " > tbody > tr:visible").length == 1) {
		$closestRow.find("input:visible").val("");
		$closestRow.find("select").each(function () {
			this.selectedIndex = 0;
		});
		$closestRow.find("[name$='.IsAdded'],[name$='.IsDeleted']").val(false);
		$closestRow.find("[name$='.IsEmpty']").val(true);
		clearRowValidationErrors(getThis);
		$closestRow.find($(":input:visible:first-child")).change();
	} else {
		$closestRow.find("[name$='.IsAdded']").val(false);
		$closestRow.find("[name$='.IsDeleted']").val(true);
		$closestRow.hide();
	}
}


function checkboxEnableRow(checkBox) {
	var $trElement = $(checkBox).closest("tr");
	if ($(checkBox).prop('checked')) {
		$trElement.find(":input").not(":checkbox").attr("disabled", false);

		primaryColumn = $trElement.find("input[type=text],select").filter(':visible:first').val();
		if (primaryColumn == '') {
			$trElement.find('.group1').prop('disabled', true).val('');
		} else {
			$trElement.find('.group1').prop('disabled', false);
		}

		$trElement.find('[data-icjia-change-dropdown]').each(function () {
			if ($(this).find('option').length <= 2) {
				var selectedValue = $(this).val();
				$(this).find('option').remove().end().append($("select#" + $(this).data('icjia-change-dropdown') + " > option").clone());
				$(this).find('option[value="' + selectedValue + '"]').attr("selected", true);
			}
		});

		$trElement.find('[data-icjia-service-staff]').each(function () {
			refreshSelectGetStaffRetainCurrentSvid($(this), $trElement.find("[data-icjia-service-date]"));
		});
	} else {
		$trElement.find(":input").not(":checkbox").attr("disabled", true);
	}
}

function clearRowValidationErrors(getThis) {
	$(getThis).closest('tr').find(':input.input-validation-error').each(function () {
		$(this).removeClass('input-validation-error').closest('.form-group').removeClass('has-error').find('span.field-validation-error').removeClass('field-validation-error').html('');
	});
	$(getThis).closest('tr').find('span').each(function () {
		$(this).removeClass('input-validation-error');
		$(this).closest('.form-group').removeClass('has-error');
		$(this).removeClass('field-validation-error').html('');
	});

	var $form = $("#main");
	var $validator = $form.validate();
	$validator.resetForm();
}

function clientSidePagination(rowCnt, pagingContainerName, table) {
	new Pagination('#tablePaging' + pagingContainerName, {
		itemsCount: rowCnt,
		onPageSizeChange: function (ps) {
			//console.log('changed to ' + ps);
		},
		onPageChange: function (paging) {
			var start = paging.pageSize * (paging.currentPage - 1),
				end = start + paging.pageSize,
				$rows = $(table).find('[data-icjia-role="clientsidepagination"]');
			$rows.hide();
			for (var i = start; i < end; i++) {
				$rows.eq(i).show();
			}
		}
	});
}

function cntEmptyFields(selector, getThis) {
	cntNoShowRow = 0;
	$(getThis).closest('tr').find(selector).each(function () {
		if ($(this).is('select') && $('option:selected', this).index() == 0) { cntNoShowRow++; }
		if (($(this).attr('type') == 'text' || $(this).attr('type') == 'number') && $(this).val().length == 0) { cntNoShowRow++; }
	});
	return cntNoShowRow;
} 

function confrimCreateListItem(cntDeleted, title) {
	return "<li class='text-danger' style='font-weight:bold; list-style: square;'>" + cntDeleted + " " + $.trim(title) + ((cntDeleted > 1) ? 's' : '') + "</li>";
}

function confrimCreateListItemSearch(cntDeleted, title, isEdit) {
	var isEditOrDelete = "Delete";
	if (isEdit)
		isEditOrDelete = "Edit";

	return "<li class='text-danger' style='font-weight:bold; list-style: square;'>" + cntDeleted + " " + $.trim(title) + ((cntDeleted > 1) ? 's' : '') + " marked as " + isEditOrDelete + "</li>";
}


$(document.body).on("click", ".pagedListMultiples a[href]", function (e) {
	e.preventDefault();
	var isHref = $(this).attr('href');

	if (isHref && $(this).closest('li').hasClass('directServicesPagedList')) {
		paginationConfirm("Direct Service", "serviceDetailTableEdit", $("#directServiceSearchDates_0__DirectServicesDateRangeStart").val(), $("#directServiceSearchDates_0__DirectServicesDateRangeEnd").val(), $('[data-pagination-for="direct.services"]').val(), $(this).attr('href'));
	} else if (isHref && $(this).closest('li').hasClass('cancellationsPagedList')) {
		paginationConfirm("Cancellation", "cancellationsTableEdit", $("#cancellationsSearchDates_0__CancellationsDateRangeStart").val(), $("#cancellationsSearchDates_0__CancellationsDateRangeEnd").val(), $('[data-pagination-for="cancellations"]').val(), $(this).attr('href'));
	} else if (isHref && $(this).closest('li').hasClass('referralPagedList')) {
		paginationConfirm("Referral", "referralTableEdit", $("#referralSearchDates_0__ReferralDateRangeStart").val(), $("#referralSearchDates_0__ReferralDateRangeEnd").val(), $('[data-pagination-for="referrals"]').val(), $(this).attr('href'));
	} else if (isHref && $(this).closest('li').hasClass('housingServicesPagedList')) {
		paginationConfirm("Housing Service", "housingServiceTableEdit", $("#housingServiceSearchDates_0__HousingServiceDateRangeStart").val(), $("#housingServiceSearchDates_0__HousingServiceDateRangeEnd").val(), $('[data-pagination-for="housing.services"]').val(), $(this).attr('href'));
	} else {
		console.log("There is no data-pagination set for section: #" + $(this).closest("[data-pagination-link]").attr('id'));
	}
});

$(document.body).on("change", ".icjia-pagedlist-drop-menu", function () {
	var pageSize = this.value;
	var fromDate = "";
	var toDate = "";
	$this = $(this);

	if ($this.attr('data-pagination-multiples')) {
		if ($this.attr('data-pagination-for')) {
			switch ($this.data('pagination-for')) {
				case "direct.services":
					fromDate = $("#directServiceSearchDates_0__DirectServicesDateRangeStart").val();
					toDate = $("#directServiceSearchDates_0__DirectServicesDateRangeEnd").val();
					break;
				case "cancellations":
					fromDate = $("#cancellationsSearchDates_0__CancellationsDateRangeStart").val();
					toDate = $("#cancellationsSearchDates_0__CancellationsDateRangeEnd").val();
					break;
				case "referrals":
					fromDate = $("#referralSearchDates_0__ReferralDateRangeStart").val();
					toDate = $("#referralSearchDates_0__ReferralDateRangeEnd").val();
					break;
				case "housing.services":
					fromDate = $("#housingServiceSearchDates_0__HousingServiceDateRangeStart").val();
					toDate = $("#housingServiceSearchDates_0__HousingServiceDateRangeEnd").val();
					break;
				default:
					console.log("[data-pagination-for] attribute for dropdown #" + $this.att('id') + "doesn't have a match");
			}
		} else {
			console.log("There is no [data-pagination-for] attribute set for dropdown #" + $this.att('id'));
		}
	} else {
		console.log("There is no [data-pagination-multiples] attribute set for dropdown #" + $this.att('id'));
	}

	$.ajax({
		type: "GET",
		url: $this.closest("[data-pagination-link]").data("pagination-link"),
		data: { ClientId: $("#clientId").val(), CaseId: $("#caseId").val(), fromDate: fromDate, toDate: toDate, pageSize: pageSize },
		cache: false,
		success: function (result) {
			$this.closest("div[id='searchResult']").html(result);
		},
		error: function () {
			//alert('Error occured');
		}
	});
});

function paginationConfirm(serviceType, serviceTableId, fromDate, toDate, pageSize, paginationUrl) {
	var messageDelete = messageEdit = "";

	var cntCheckedEdit = $("#" + serviceTableId + " input[name$='.IsEdited']:checked").length;
	var cntCheckedDeleted = $("#" + serviceTableId + " input[name$='.IsDeleted']:checked").length;

	if (cntCheckedEdit > 0)
		messageEdit = confrimCreateListItemSearch(cntCheckedEdit, serviceType + " Record", true);

	if (cntCheckedDeleted > 0)
		messageDelete = confrimCreateListItemSearch(cntCheckedDeleted, serviceType + " Record", false);

	if (cntCheckedEdit > 0 || cntCheckedDeleted > 0) {
		$.confirm({
			title: confirmEditServicesTitle,
			text: confirmEditServicesTextStart + messageEdit + messageDelete + confirmEditServicesTextEndSearch,
			confirmButton: confirmEditServicesConfirmButton,
			cancelButton: confirmEditServicesCancelButton,
			confirmButtonClass: confirmEditServicesConfirmButtonClass,
			dialogClass: confirmEditServicesDiaglogClass,
			confirm: function () {
				paginationAjax(fromDate, toDate, pageSize, paginationUrl, serviceTableId);
			},
			cancel: function () {
				// nothing to do
			}
		});
	} else {
		paginationAjax(fromDate, toDate, pageSize, paginationUrl, serviceTableId);
	}
}

function paginationAjax(fromDate, toDate, pageSize, paginationUrl, serviceTableId) {
	$.ajax({
		url: paginationUrl,
		data: { ClientId: $("#clientId").val(), CaseId: $("#caseId").val(), fromDate: fromDate, toDate: toDate, pageSize: pageSize },
		type: 'GET',
		cache: false,
		success: function (result) {
			$('#'+serviceTableId).closest("div[id='searchResult']").html(result);
			rescanUnobtrusiveValidation("#main");
		}
	});
}

function refreshSelectGetStaffRetainCurrentSvid($svidSelect, $serviceDate) {
	if ($serviceDate.valid()) {
		var currentSelectedVal = $svidSelect.val();
		$.ajax({
			url: "/Service/GetStaffRetainCurrentSvid?serviceDate=" + $serviceDate.val() + "&currentSvid=" + currentSelectedVal,
			dataType: 'json',
			success: function (data) {
				var listitems = '';
				$.each(data, function (key, value) {
					listitems += '<option value=' + value.SVID + '>' + value.EmployeeName + '</option>';
				});
				$svidSelect.find('option').remove().end().append($('<option>').text("<Pick One>").attr('value', "")).append(listitems).val(currentSelectedVal);
			},
			error: function (xhr, ajaxOptions, thrownError) {
				//console.log(xhr.responseText);
			}
		});
	}
}

$("input[name$='.DepartureDate']").on('focusout',function(){
	if (new Date($(this).val()).getTime() === new Date($('input[name="' + $(this).attr("name") + 'Hidden"][type="hidden"]').val()).getTime() && $(this).hasClass('input-validation-error')) return false;
});
