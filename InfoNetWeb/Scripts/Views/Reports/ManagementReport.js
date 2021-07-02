var centerColors = ["#f7ecb5", "#afd9ee", "#e4b9b9", "#dff0d8", "#e8d6ff", "#cbece7", "#e3e5ef", "#007295", "#552b80", "#005252"];
var prevStartVal = null, prevEndVal = null;

$(document).ready(function () {
    removePreventDuplicateRequestDisable();

    $('.chosen').chosen();

    RefreshStaffList();

    $('[data-icjia-ajax-url]').each(function () {
        RefreshFilterList(this);
    });

    $('input[type="checkbox"][name="CenterIds"]').change(function () {
        $('span[data-valmsg-for="CenterIds"]').html('');
        RefreshStaffList();
    });

    $('#Start').blur(function () {
        if (prevStartVal != $(this).val() && $(this).val() != "" && $(this).valid()) {
            RefreshStaffList();
            $('[data-icjia-ajax-url]').each(function () {
                RefreshFilterList(this);
            });
        }
        prevStartVal = $(this).val();
    });

    $('#End').blur(function () {
        if (prevEndVal != $(this).val() && $(this).val() != "" && $(this).valid()) {
            RefreshStaffList();
            $('[data-icjia-ajax-url]').each(function () {
                RefreshFilterList(this);
            });
        }
        prevEndVal = $(this).val();
    });

    $('#rangePeriodChosen').change(function () {
        RefreshStaffList();
        $('[data-icjia-ajax-url]').each(function () {
            RefreshFilterList(this);
        });
    });

    $('#Generate').click(function (e) {
        e.preventDefault();
        if (ValidateReportSelections()) {
            $('#main').submit();
        }
    });

    $('.reportSelectionRadio').change(function () {
        var $this = $(this);
        $('.reportProperty').hide();
        if ($this.is(':checked')) {
            $($this.data('icjia-child-id')).show();
        }
    });

    $('input[type="checkbox"][name="StaffHotlineColumnSelections"]').change(function () {
        var orderSelections = $('input[type="radio"][name="StaffHotlineOrderSelection"]');
        UpdateOrderSelectionAvailability($(this), orderSelections);
    });

    $('input[type="checkbox"][name="ClientDetailColumnSelections"]').change(function () {
        var orderSelections = $('input[type="radio"][name="ClientDetailOrderSelection"]');
        UpdateOrderSelectionAvailability($(this), orderSelections);
    });

    $('input[type="checkbox"][name="StaffClientServiceColumnSelections"]').change(function () {

        CheckIfServiceDateAndGroupBy($(this));
        UpdateStaffClientServiceGroupByColumns();
    });

    $('input[type="radio"][name="OtherStaffActivityRecordDetailOrderSelection"][data-icjia-name="GroupBy"]').change(function () {
        $('.OtherStaffActivity .moveColumnSelectionLeft').trigger("click");
    });
    $('input[type="radio"][name="OtherStaffActivityRecordDetailOrderSelection"][data-icjia-name="RecordDetail"]').change(function () {
        $('.OtherStaffActivity .moveColumnSelectionLeft').trigger("click");
    });


    $('input[type="checkbox"][name="OtherStaffActivityColumnSelections"]').change(function () {
        var $this = $(this);
        var groupBy = $('input[type="radio"][name="OtherStaffActivityRecordDetailOrderSelection"][data-icjia-name="GroupBy"]');
        var recordDetail = $('input[type="radio"][name="OtherStaffActivityRecordDetailOrderSelection"][data-icjia-name="RecordDetail"]');

        if ($this.data('icjia-name') === 'Date') {
            if ($this.is(':checked')) {
                groupBy.prop('disabled', true);
                groupBy.prop('checked', false);
                recordDetail.prop('checked', true);
                $('.OtherStaffActivity .moveColumnSelectionLeft').trigger("click");
            } else {
                groupBy.prop('disabled', false);
            }
        }
    });

    $('#newIncomeRange').click(function () {
        AddNewIncomeRange();
    });

    $('#resetIncomeRanges').click(function () {
    	$('.IncomeSource tbody tr:not(:first-child)').remove();
    	$('.IncomeSource tbody tr:first-child input').val("");
    	$("#newIncomeRange").prop('disabled', false);
    });

    $('.deleteIncomeRange').click(DeleteIncomeRange);

    ResetStaffClientServiceColumnOrderControlVisibility();
    UpdateStaffClientServiceGroupByColumns();

    $('.StaffClientService .moveColumnSelectionLeft').click(function () {
        MoveColumnSelectionLeft($(this));
        ResetStaffClientServiceColumnOrderControlVisibility();
        UpdateStaffClientServiceGroupByColumns();
        return false;
    });

    $('.StaffClientService .moveColumnSelectionRight').click(function () {
        MoveColumnSelectionRight($(this));
        ResetStaffClientServiceColumnOrderControlVisibility();
        UpdateStaffClientServiceGroupByColumns();
        return false;
    });

    ResetOtherStaffActivityColumnOrderControlVisibility();

    $('.OtherStaffActivity .moveColumnSelectionLeft').click(function () {
        MoveColumnSelectionLeft($(this));
        ResetOtherStaffActivityColumnOrderControlVisibility();
        return false;
    });

    $('.OtherStaffActivity .moveColumnSelectionRight').click(function () {
        MoveColumnSelectionRight($(this));
        ResetOtherStaffActivityColumnOrderControlVisibility();
        return false;
    });

    ResetCancellationColumnOrderControlVisibility();

    $('.CancellationNoShow .moveColumnSelectionLeft').click(function () {
        MoveColumnSelectionLeft($(this));
        ResetCancellationColumnOrderControlVisibility();
        return false;
    });

    $('.CancellationNoShow .moveColumnSelectionRight').click(function () {
        MoveColumnSelectionRight($(this));
        ResetCancellationColumnOrderControlVisibility();
        return false;
    });

    $(document.body).on('change', "input[id^='CenterSelection']", function () {
        ErrorClear("CenterIds", "CenterIds", "You must specify at least one Center.");
    });

    $(document.body).on('change', "input[name='ReportSelection']", function () {
        ErrorClear("ReportSelectionVal", "ReportSelection", "You must specify at least one Report.");
    });
});

function ValidateReportSelections() {
    //income source management report
    if ($('#selection30').is(':checked')) {
        //all income ranges' lower bound must be less than upper bound
        if (IncomeRangeLowerBoundExceedsUpperBound()) {
            return false;
        }
        //the inclusive bounds of one income range may not overlap another income range
        if (IncomeRangesOverlap()) {
            $('span[data-valmsg-for="IncomeSourceIncomeRanges"').css('color', '#a94442').html("Income ranges may not overlap.");
            return false;
        }
    }
    // staff/client service information report
    if ($('#selection31').is(':checked') && NoColumnsSelected()) {
        	ErrorFormat("StaffClientServiceColumnSelectionsVal", "StaffClientServiceColumnSelections", "You must specify at least one column.");
            return false;
    }

    var retval = true;

    if (!$('#Start').valid() || !$('#End').valid()) {
        return false;
    }

    var startDate = new Date($('#Start').val());
    var endDate = new Date($('#End').val());
    var minDate = new Date('01/01/1900');
    var maxDateFuture = new Date('01/01/' + ((new Date).getFullYear() + 100));

    if (startDate < minDate || startDate > new Date()) {
    	ErrorFormat("StartDate", "StartDate", "Start Date be between 1/1/1900 & today.");
    	retval = false;
    	$setFocusToError = null || $('#StartDate');
    }

    if (endDate < minDate || endDate > maxDateFuture) {
    	ErrorFormat("EndDate", "EndDate", "End Date must be between 1/1/1900 & 1/1/" + ((new Date).getFullYear() + 100));
    	retval = false;
    	$setFocusToError = null || $('#EndDate');
    }

    if ($('#End') && $('#Start') && startDate > endDate) {
        ErrorFormat("StartDate", "StartDate", "Start Date cannot be greater than End Date.");
        retval = false;
        $setFocusToError = null || $('#End');
    }

    if (!$('input[name="CenterIds"]:checked').length) {
        ErrorFormat("CenterIds", "CenterIds", "You must specify at least one Center.");
        retval = false;
        $setFocusToError = null || $('input[name="CenterIds"]:first');
    }

    if (!$('input[name="ReportSelection"]:checked').length) {
        ErrorFormat("ReportSelectionVal", "ReportSelection", "You must specify at least one Report.");
        retval = false;
        $setFocusToError = null || $('input[name="ReportSelection"]:first');
    }

    if (!retval) {
        setFocus = null || $setFocusToError.focus();
    }
    return retval;
}

function RefreshStaffList() {
    var centerIds = [];
    $('input[type="checkbox"][name="CenterIds"]').each(function (index, element) {
        if (element.checked) {
            centerIds.push(element.value);
        }
    });
    var startDate = $('#Start').val();
    var endDate = $('#End').val();
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
                $('.staffSelection').each(function () {
                    var selectedVals = $(this).val();
                    var options = '';
                    for (var i = 0; i < data.length; i++) {
                        var selected = '';
                        if ($.inArray(String(data[i].CodeId), selectedVals) > -1) {
                            selected = 'selected="selected"';
                        }
                        options += '<option class="reportStaff" value="' + data[i].CodeId + '" ' + selected + ' style="border-left-color:' + centerColors[data[i].CenterID] + '">' + data[i].Description + '</option>';
                    }
                    $(this).html(options);
                    $(this).trigger("chosen:updated");
                });
            }
            else {
                $('.staffSelection').each(function () {
                    $(this).html('');
                    $(this).trigger("chosen:updated");
                });
            }
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
    var startDate = $('#Start').val();
    var endDate = $('#End').val();
    $.ajax({
        url: $this.data('icjia-ajax-url'),
        method: 'GET',
        data: {
            centerIds: centerIds,
            startDate: startDate,
            endDate: endDate
        },
        traditional: true,
        success: function (data) {
            if (data && data.length) {
                var selectedVals = $this.val();
                var options = '';
                for (var i = 0; i < data.length; i++) {
                    var selected = '';
                    if ($.inArray(String(data[i].CodeId), selectedVals) > -1) {
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

function UpdateOrderSelectionAvailability(element, orderSelections) {
    $.each(orderSelections, function (index, item) {
        if ($(item).data('icjia-name') === element.data('icjia-name')) {
            if (element.is(':checked'))
                $(item).prop('disabled', false);
            else {
                $(item).prop('disabled', true);
                $(item).prop('checked', false);
            }
        }
    });
}

function AddNewIncomeRange() {
	if ($('.IncomeSource tbody tr').length < 10) {
		var priorRowUpperBound = $('.IncomeSource tbody tr:last-child input[name$="UpperBound"]').val();

		$.ajax({
			url: '/Report/AddNewIncomeRange',
			success: function (partialViewResult) {
				$('.IncomeSource tbody').append(partialViewResult);
				$('.IncomeSource tbody tr:last-child .deleteIncomeRange').click(DeleteIncomeRange);

				if ($.isNumeric(priorRowUpperBound)) {
					$('.IncomeSource tbody tr:last-child input[name$="LowerBound"]').val(parseInt(priorRowUpperBound) + 1);
					$('.IncomeSource tbody tr:last-child input[name$="UpperBound"]').focus();
				} else {
					$('.IncomeSource tbody tr:last-child input[name$="LowerBound"]').focus();
				}
				if ($('.IncomeSource tbody tr').length == 10) { $("#newIncomeRange").prop('disabled', true); }
			},
			complete: function () {
				reorderRowIndexes('newIncomeRangeRow', 'newIncomeRangeRowData');
				rescanUnobtrusiveValidation('#main');
			},
			error: function (xhr, ajaxOptions, thrownError) {
				//console.log(xhr);
				alert('There was an error while adding a new income range.');
			}
		});
	} 
}

function DeleteIncomeRange(event) {
    $(event.target).closest('.newIncomeRangeRow').remove();
    reorderRowIndexes('newIncomeRangeRow', 'newIncomeRangeRowData');
    $("#newIncomeRange").prop('disabled', false);
}

function IncomeRangeLowerBoundExceedsUpperBound() {
    var invalid = false;
    var lastIncomeRange = $('.newIncomeRangeRow').length;

    $('.newIncomeRangeRow').each(function (index, row) {
        var lowerBound = $('input[name$="LowerBound"]', row).val();
        var upperBound = $('input[name$="UpperBound"]', row).val();

        if (index === (lastIncomeRange - 1) && $.isNumeric(lowerBound) & !$.isNumeric(upperBound)) {
            return;
        }

        if ($.isNumeric(lowerBound) && $.isNumeric(upperBound) && parseFloat(lowerBound) < parseFloat(upperBound)) {
            return;
        }

        $('span[data-valmsg-for$="UpperBound"]', row).css('color', '#a94442').html("Ending income value must be greater than the starting income value.");
        invalid = true;
    });

    return invalid;
}

function IncomeRangesOverlap() {
    var valid = true;

    $('.newIncomeRangeRow').each(function (outerIndex, outerRow) {
        var outerRowLowerBound = parseFloat($('input[name$="LowerBound"]', outerRow).val());
        var outerRowUpperBound = parseFloat($('input[name$="UpperBound"]', outerRow).val());

        $('.newIncomeRangeRow').each(function (innerIndex, innerRow) {
            if (outerIndex === innerIndex) {
                return;
            }

            var innerRowLowerBound = parseFloat($('input[name$="LowerBound"]', innerRow).val());
            var innerRowUpperBound = parseFloat($('input[name$="UpperBound"]', innerRow).val());

            if (innerRowLowerBound > outerRowLowerBound && innerRowLowerBound < outerRowUpperBound) {
                valid = false;
            }

            if (innerRowLowerBound === outerRowLowerBound || innerRowLowerBound === outerRowUpperBound) {
                valid = false;
            }

            if (innerRowUpperBound > outerRowLowerBound && innerRowUpperBound < outerRowUpperBound) {
                valid = false;
            }

            if (innerRowUpperBound === outerRowLowerBound || innerRowUpperBound === outerRowUpperBound) {
                valid = false;
            }

            return valid;
        });

        return valid;
    });

    return !valid;
}

function NoColumnsSelected() {
    var invalid = true;

    $('input[name="StaffClientServiceColumnSelections"]').each(function (index, column) {
        if ($(column).is(':checked')) {
            invalid = false;
            return invalid;
        }
    });

    return invalid;
}

function ResetColumnOrderControlVisibility($columns, lastIndex) {
    $columns.each(function (index, column) {
        var $this = $(column);

        var left = $this.find('.moveColumnSelectionLeft').hide();
        var right = $this.find('.moveColumnSelectionRight').hide();

        if (index === 0) {
            right.show();
        } else if (index > 0 && index < lastIndex) {
            left.show();
            right.show();
        } else if (index === lastIndex) {
            left.show();
        } else {
            $this.remove();
        }
    });
}

function ResetOtherStaffActivityColumnOrderControlVisibility() {
    var controls = $('.OtherStaffActivity .columnOrderControl');
    var indexOfLastOrderableColumn = 1;

    ResetColumnOrderControlVisibility(controls, indexOfLastOrderableColumn);
}

function ResetStaffClientServiceColumnOrderControlVisibility() {
    var controls = $('.StaffClientService .columnOrderControl');
    var indexOfLastOrderableColumn = 2;

    ResetColumnOrderControlVisibility(controls, indexOfLastOrderableColumn);
}

function ResetCancellationColumnOrderControlVisibility() {
    var controls = $('.CancellationNoShow .columnOrderControl');
    var indexOfLastOrderableColumn = 2;

    ResetColumnOrderControlVisibility(controls, indexOfLastOrderableColumn);
}

function MoveColumnSelectionLeft($this) {
    var column = $this.closest('.columnSeletionOption');
    column.insertBefore(column.prev());
}

function MoveColumnSelectionRight($this) {
    var column = $this.closest('.columnSeletionOption');
    column.insertAfter(column.next());
}

function UpdateStaffClientServiceGroupByColumns() {
    var columns = '';

    $('.StaffClientService .columnSeletionOption:has(".columnOrderControl")').each(function (index, column) {
        var checkbox = $(column).find('input[type="checkbox"]');

        if (!checkbox.is(':checked')) { return; }

        if (columns) { columns += ', '; }

        columns += checkbox.data('icjia-name');
    });

    $('.staffClientServiceFieldGroupOrder').text(columns);
}

function CheckIfServiceDateAndGroupBy(checkbox) {
    var groupByRadio = $('input[type="radio"][name="StaffClientServiceRecordDetailOrderSelection"][value="1"]');
    var recordDetailRadio = $('input[type="radio"][name="StaffClientServiceRecordDetailOrderSelection"][value="0"]');

    if (checkbox.is(':checked') && checkbox.data('icjia-name') == "Service Date") {
        groupByRadio.prop('disabled', true);
        if (groupByRadio.is(':checked')) {
            recordDetailRadio.prop('checked', true);
        };
    } else if (!checkbox.is(':checked') && checkbox.data('icjia-name') == "Service Date") {
        groupByRadio.prop('disabled', false);
    }
}

$(document.body).on('change', '[name="StaffClientServiceColumnSelections"][type="checkbox"]', function () {
	if ($('[name="StaffClientServiceColumnSelections"][type="checkbox"]:checked').length > 0) {
		ErrorClear("StaffClientServiceColumnSelectionsVal", "StaffClientServiceColumnSelections", "You must specify at least one column.");
	} else {
		ErrorFormat("StaffClientServiceColumnSelectionsVal", "StaffClientServiceColumnSelections", "You must specify at least one column.");
	}
});

$(document.body).on('click', '#selection31', function () {
	if ($(this).is(':checked')) {
		ErrorClear("StaffClientServiceColumnSelectionsVal", "StaffClientServiceColumnSelections", "You must specify at least one column.");
	}
});