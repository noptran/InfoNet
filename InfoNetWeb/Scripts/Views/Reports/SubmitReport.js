var SubmitReports = {};

$(document).ready(function () {
    SubmitReports.VAWAAll = "All VAWA";
    SubmitReports.VAWANone = "No VAWA";
    SubmitReports.VOCAAll = "All VOCA";
    SubmitReports.VOCANone = "No VOCA";
    SubmitReports.CentersAll = "All Centers";
    SubmitReports.CentersNone = "No Centers";
    SubmitReports.SatellitesAll = "All Satellites";
    SubmitReports.SatellitesNone = "No Satellites";
    SubmitReports.AggregateAllMessage = "When Aggregate All is chosen, only No Action is allowed.";
    SubmitReports.AtLeastOneCenter = "You must specify at least one Center.";
    SubmitReports.SubReportSelections = "You must specify at least one Subreport to run.";


    $("#fundingFilterIndividual").attr('disabled', true);
    cntJobsRefresh();

	$('.reportSelectionRadio').each(function () {
	    cntReportSelectionGroupRefresh($(this));
	});

	if ($("#providerId").val() == 1) {
		$("[data-icjia-parent-id='#selectionGroup3']").not('.skipDefault').on('change', function () {
			$(".skipDefault").prop('checked', false);
		});

		$(".skipDefault").on('change', function () {
			$("[data-icjia-parent-id='#selectionGroup3']").not('.skipDefault').prop('checked', false);
		});
	}

	//#region FundingFilter
	$("#fundingFilterSelectAll").on('click', function () {

		if ($(this).text() == "Select None") {
			$(this).text("Select All");
			$("#fundingFilterAllVAWA").text(SubmitReports.VAWAAll);
			$("#fundingFilterAllVOCA").text(SubmitReports.VOCAAll);
			$("[data-icjia-role='fundingFilter']").prop('checked', false);
		} else {
			$(this).text("Select None");
			$("#fundingFilterAllVAWA").text(SubmitReports.VAWANone);
			$("#fundingFilterAllVOCA").text(SubmitReports.VOCANone);
			$("[data-icjia-role='fundingFilter']").prop('checked', true);
		}
		cntJobsRefresh();
		cntFundingFilterRefresh();
	});

	$("#fundingFilterAllVAWA").on('click', function () {
		if ($(this).text() == SubmitReports.VAWAAll) {
			$(this).text(SubmitReports.VAWANone);
			$("[data-icjia-VOCAVAWA='VAWA']").prop('checked', true);
		} else {
			$(this).text(SubmitReports.VAWAAll);
			$("[data-icjia-VOCAVAWA='VAWA']").prop('checked', false);
		}
		cntJobsRefresh();
		cntFundingFilterRefresh();
	});

	$("#fundingFilterAllVOCA").on('click', function () {
		if ($(this).text() == SubmitReports.VOCAAll) {
			$(this).text(SubmitReports.VOCANone);
			$("[data-icjia-VOCAVAWA='VOCA']").prop('checked', true);
		} else {
			$(this).text(SubmitReports.VOCAAll);
			$("[data-icjia-VOCAVAWA='VOCA']").prop('checked', false);
		}
		cntJobsRefresh();
		cntFundingFilterRefresh();
	});

	$("[data-icjia-role='fundingFilter']").on('change', function () {
		cntFundingFilterRefresh();
		cntJobsRefresh();
	});
	//#endregion FundingFilter

	//#region CenterSelection
	$("#centerSelectionSelectAll").on('click', function () {

		if ($(this).text() == "Select None") {
			$(this).text("Select All");
			$("#centerSelectionAllCenters").text(SubmitReports.CentersAll);
			$("#centerSelectionAllSatellites").text(SubmitReports.SatellitesAll);
			$("[data-icjia-role='centerSelection']").prop('checked', false);
		} else {
			$(this).text("Select None");
			$("#centerSelectionAllCenters").text(SubmitReports.CentersNone);
			$("#centerSelectionAllSatellites").text(SubmitReports.SatellitesNone);
			$("[data-icjia-role='centerSelection']").prop('checked', true);
		}

		cntCenterSelectionRefresh();
		cntJobsRefresh();
	});

	$("#centerSelectionAllCenters").on('click', function () {
		if ($(this).text() == SubmitReports.CentersAll) {
			$(this).text(SubmitReports.CentersNone);
			$("[data-icjia-satellite='FALSE']").prop('checked', true);
		} else {
			$(this).text(SubmitReports.CentersAll);
			$("[data-icjia-satellite='FALSE']").prop('checked', false);
		}
		cntCenterSelectionRefresh();
		cntJobsRefresh();
	});

	$("#centerSelectionAllSatellites").on('click', function () {
		if ($(this).text() == SubmitReports.SatellitesAll) {
			$(this).text(SubmitReports.SatellitesNone);
			$("[data-icjia-satellite='TRUE']").prop('checked', true);
		} else {
			$(this).text(SubmitReports.SatellitesAll);
			$("[data-icjia-satellite='TRUE']").prop('checked', false);
		}
		cntCenterSelectionRefresh();
		cntJobsRefresh();
	});

	$("[data-icjia-role='centerSelection']").on('change', function () {
		cntCenterSelectionRefresh();
		cntJobsRefresh();
		ErrorClear($("[data-icjia-role='centerSelection']"),'Centers', SubmitReports.AtLeastOneCenter);
		$(this).prop('checked') ? $(this).closest("div").find("#CenterIds").val($(this).closest("div").find("[name$='CenterId']").val()) : $(this).closest("div").find("#CenterIds").val();
	});

	$("input[name=FundingFilterRadio]:radio").on('change', function () {
		cntJobsRefresh();
	});
	
	$("input[name=CenterSelectionRadio]:radio").on('change', function () {
	    cntJobsRefresh();
	    if ($("input[name = CenterSelectionRadio]:checked").val() != "aggregateall") {
	        ErrorClear("CenterSelectionRadio", "Centers", SubmitReports.AggregateAllMessage);
        }
	});

	$("input[name=CenterAction]:radio").on('change', function () {
	    if ($("input[name=CenterAction]:checked").val() == 0) {
	        ErrorClear("CenterSelectionRadio", "Centers", SubmitReports.AggregateAllMessage);
	    }
	});

	$('input[name="SubReportSelections"]').on('change', function () {
	    ErrorClear("valSubReport", 'SubReportSelections', SubmitReports.SubReportSelections);
	});

	function cntJobsRefresh() {
		cntFundingFilterRefresh();
		cntCenterSelectionRefresh();
		var fundingFilterCnt = parseInt($("#fundingFilterCount").text());
		var centerSelectionCnt = parseInt($("#centerSelectionCount").text());
		var parentCenters = new Set();

		$("[data-icjia-role='centerSelection']:checked").closest('div').find("[name$='ParentCenterId']").each(function() {
		    parentCenters.add($(this).val());
		});
		var cntParentCenters = parentCenters.size;
		var reportSelectionCnt = $('[class*="reportSelectionRadio"]:checked').length;
		var centerSelectionValue = $('input[name=CenterSelectionRadio]:radio:checked').val();

		if (centerSelectionValue == "individual") {
	        $('#totalJobCnt').text($('input[name=FundingFilterRadio]:radio:checked').val() == "individual" ? reportSelectionCnt * fundingFilterCnt * centerSelectionCnt : reportSelectionCnt * centerSelectionCnt);
		} else {
	        $('#totalJobCnt').text(centerSelectionCnt == 0 ? 0 : $('input[name=FundingFilterRadio]:radio:checked').val() == "individual" ? reportSelectionCnt * fundingFilterCnt * (centerSelectionValue == "aggregatebycenter" ? cntParentCenters : 1) : reportSelectionCnt * (centerSelectionValue == "aggregatebycenter" ? cntParentCenters : 1));
		}
	}

	function cntReportSelectionGroupRefresh($parentCheckbox) {
		$parentCheckbox.next('span').text(cntReportSelectionGroup($parentCheckbox));
		cntJobsRefresh();
	}

	function cntReportSelectionGroup($parentCheckbox) {
		return $("[class*='" + $parentCheckbox.prop('id') + "']:checkbox:checked").length;
	}

	function cntFundingFilterRefresh() {
		$("#fundingFilterCount").text($("[data-icjia-role='fundingFilter']:checked").length);

		if (parseInt($("#fundingFilterCount").text()) < 2) {
			$("#fundingFilterIndividual").attr('disabled', true);
			$("#fundingFilterAggregate").prop('checked', true);
		} else {
			$("#fundingFilterIndividual").attr('disabled', false);
		}
	}

	function cntCenterSelectionRefresh() {
		$("#centerSelectionCount").text($("[data-icjia-role='centerSelection']:checked").length);

		if (parseInt($("#centerSelectionCount").text()) < 2) {
			//$("#centerSelectionIndividual, #centerSelectionAggregateByCenter").attr('disabled', true);
			//$("#centerSelectionAggregateAll").prop('checked', true);
		} else {
			//$("#centerSelectionIndividual, #centerSelectionAggregateByCenter").attr('disabled', false);
		}
	}

	$('.reportSelectionRadio').change(function () {
		$("[class*='" + $(this).prop('id') + "']").not('.skipDefault').prop('checked', $(this).is(":checked"));
		if ($("#providerId").val() == 1 && $(this).prop('id') == 'selectionGroup3') {
			$(".skipDefault").prop('checked', false);
		}
        cntReportSelectionGroupRefresh($(this));
        ErrorClear("valSubReport", 'SubReportSelections', SubmitReports.SubReportSelections);
	});

	$('.subSelection').change(function () {
		$('span[data-valmsg-for="SubReportSelections"]').html('');

		var $parentCheckbox = $(this).closest("fieldset").find('input:checkbox:first');

		cntReportSelectionGroup($parentCheckbox) == 0 ? $parentCheckbox.prop('checked', false) : $parentCheckbox.prop('checked', true);
        cntReportSelectionGroupRefresh($parentCheckbox);       
	});

	$('#Submit').click(function () {
		$("#SubReportGroup1").val($('.selectionGroup1:checked').map(function () {
			return $(this).val();
		}).get());

		$("#SubReportGroup2").val($('.selectionGroup2:checked').map(function () {
			return $(this).val();
		}).get());

		$("#SubReportGroup3").val($('.selectionGroup3:checked').map(function () {
			return $(this).val();
		}).get());

		if (ValidateReportSelections()) {
			$('#main').submit();
		}
    });

    $("#ReportTitle").on('change', function () {
        if ($('#ReportTitle').val != '')            
            ErrorClear("Title", "Title", "The Batch Report Title field is required.");                  
    });
});

function ValidateReportSelections() {
  	var retval = true;
	var $setFocusToError = null;
	if ($('input[name=CenterSelectionRadio]:checked').val() == 'aggregateall') {
	    if ($('input[name=CenterAction]:checked').val() == 1) {
	        ErrorFormat("CenterSelectionRadio", "Centers", SubmitReports.AggregateAllMessage);
	        retval = false;
	        $setFocusToError = null || $('#centerSelectionAggregateAll');
	    }
	    if ($('input[name=CenterAction]:checked').val() == 2) {
	        ErrorFormat("CenterSelectionRadio", "Centers", SubmitReports.AggregateAllMessage);
	        retval = false;
	        $setFocusToError = null || $('#centerSelectionAggregateAll');
	    }
	}

	if (!$('#Start').val().trim()) {
	    ErrorFormat("StartDate", "StartDate", "The Start Date field is required.");
	    retval = false;
		$setFocusToError = null || $('#Start');
	}

	if (!$('#End').val().trim()) {
	    ErrorFormat("EndDate", "EndDate", "The End Date field is required.");
		retval = false;
		$setFocusToError = null || $('#End');
	}

	if ($('#End') && $('#Start') && new Date($('#Start').val()) > new Date($('#End').val())) {
	    ErrorFormat("StartDate", "StartDate", "Start Date cannot be greater than End Date.");
		retval = false;
		$setFocusToError = null || $('#End');
	}

    //For CDFSSAdmin only
    //1. Quarter dates is required when any report filter (city,race,client type) is applied.
    if ($('#loginCenterId').val() == -7) {
        if (IsReportFiltered() && (!IsQuarterFirstDate($('#Start').val()) || !IsQuarterLastDate($('#End').val()))) {
            ErrorFormat("StartDate", "StartDate", "Start Date and End Date need to be on quarter when filters are applied.");
            retval = false;
            $setFocusToError = null || $('#Start');
        }
    }

    if (!$('#ReportTitle').val().trim()) {
        ErrorFormat("Title", "Title", "The Batch Report Title field is required.");
        retval = false;
        $setFocusToError = null || $('#ReportTitle');
    }

	if (!$('input:checkbox[id^="Centers"]:checked').length) {
	    ErrorFormat("valCenters", "Centers", SubmitReports.AtLeastOneCenter);
		retval = false;
		$setFocusToError = null || $('input:checkbox[id^="Centers"]:first');
	}

	if (!$('input[name="SubReportSelections"]:checked').length) {
	    ErrorFormat("valSubReport", "SubReportSelections", SubmitReports.SubmitReportSelections);
		retval = false;
		$setFocusToError = null || $('input[name="SubReportSelections"]:first');
	}

	if (!retval) {
		setFocus = null || $setFocusToError.focus();
	}
	return retval;
}

function IsQuarterFirstDate(startDate) {
    var retval = true;
    var sDate = new Date(startDate);   
    var qtrFirstDate = GetQuarterFirstDate(startDate);
    retval = qtrFirstDate.toDateString() == sDate.toDateString();
    return retval;
}

function IsQuarterLastDate(endDate) {
    var retval = true;
    var eDate = new Date(endDate);
    var qtrFirstDate = GetQuarterFirstDate(endDate);
    var qtrLastDate = new Date(qtrFirstDate.getFullYear(), qtrFirstDate.getMonth() + 3, 0);
    retval = qtrLastDate.toDateString() == eDate.toDateString();
    return retval;
}

function GetQuarterFirstDate(aDate) {
    var sDate = new Date(aDate);
    var qtr = Math.floor((sDate.getMonth() / 3)),
        yyyy = sDate.getFullYear();
    var qtrFirstDate = new Date(yyyy, qtr * 3, 1);
    return qtrFirstDate;
}
function IsReportFiltered() {
    var retval = false;
    var selectedFilters = [];
    $.each($("select[data-icjia-role='chosen'] option:selected"), function () {
        selectedFilters.push($(this).val());
    });
    if (selectedFilters.length > 0)
        retval = true;
    return retval;
}

function RadionButtonSelectedValueSet(name, SelectdValue) {
	$('input[name="' + name+ '"][value="' + SelectdValue + '"]').prop('checked', true);
}


$(document.body).on('change', 'input:checkbox[id^="Centers"]', function () {
	if((this.checked) || $('input:checkbox[id^="Centers"]:checked').length > 1) {
		$('span[data-valmsg-for="Centers"]').html("");
	}
});

$(document.body).on('change', 'input:checkbox[id^="FundingFilter_"]', function () {
    if ((this.checked) || $('input:checkbox[id^="FundingFilter_"]:checked').length > 1) {
        ErrorClear("valFundingFilter", "FundingFilter", SubmitReports.AtLeastOneFunding);
    }
});
