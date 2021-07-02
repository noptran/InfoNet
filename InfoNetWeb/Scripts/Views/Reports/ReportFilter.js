var centerColors = ["#f7ecb5", "#afd9ee", "#e4b9b9", "#dff0d8", "#e8d6ff", "#cbece7", "#e3e5ef", "#007295", "#552b80", "#005252"];
var prevStartVal = null;
var prevEndVal = null;

$(document).ready(function () {

	$('[data-icjia-role="chosen"]').chosen();

	$('input[type="checkbox"][name="CenterIds"]').change(function () {
		$('span[data-valmsg-for="CenterIds"]').html('');
		RefreshStaffList();
		$('[data-icjia-ajax-url]').each(function () {
			RefreshFilterList(this);
		});
	});

	RefreshStaffList();

	$('[data-icjia-ajax-url]').each(function () {
		RefreshFilterList(this);
	});
});

$(document.body).on('focusout', "#Start", function () {
    if ($('#Start').valid())
        RefreshStaffList();
});

$(document.body).on('focusout', "#End", function () {
    if ($('#End').valid())
        RefreshStaffList();
});

function RefreshStaffList() {
    var centerIds = [];
    var startDate = $('#Start').val();
    var endDate = $('#End').val();
	$('input[type="checkbox"][name="CenterIds"]').each(function (index, element) {
		if (element.checked) {
			centerIds.push(element.value);
		}
	});
	$.ajax({
		url: '/Report/StaffNames',
		method: 'GET',
		data: {
            centerIds: centerIds,
            startDate: startDate,
            endDate: endDate
		},
		traditional: true,
		success: function (data) {
			if (data) {
				var selectedVals = $('.SVIDSelections').val();
				var options = '';
				for (var i = 0; i < data.length; i++) {
					var selected = '';
					if ($.inArray(String(data[i].CodeId), selectedVals) > -1) {
						selected = 'selected="selected"';
					}
					options += '<option class="reportStaff" value="' + data[i].CodeId + '" ' + selected + ' style="border-left-color:' + centerColors[data[i].CenterID] + '">' + data[i].Description + '</option>';
				}
				$('.SVIDSelections').html(options);
			}
			$('.SVIDSelections').trigger("chosen:updated");
		}
	});
}

function RefreshFilterList(element) {
	var $this = $(element);
	var centerIds = [];
	$('input[type="checkbox"][name="CenterIds"]').each(function (index, element) {
		if (element.checked) {
			centerIds.push(element.value);
		}
	});
	$.ajax({
		url: $this.data('icjia-ajax-url'),
		method: 'GET',
		data: {
			centerIds: centerIds
		},
		traditional: true,
		success: function (data) {
			if (data && data.length) {
				var selectedVals = $this.val();
				var options = '';
				for (var i = 0; i < data.length; i++) {
					var selected = '';
					if (data[i].CodeId != null && $.inArray(String(data[i].CodeId), selectedVals) > -1) {
						selected = 'selected="selected"';
					}
					options += '<option value="' + data[i].CodeId + '" ' + selected + '>' + data[i].Description + '</option>';
				}
				$this.html(options);
			}
			$this.trigger("chosen:updated");
		}
	});
}