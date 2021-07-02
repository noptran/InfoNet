$(document).ready(function () {
    removePreventDuplicateRequestDisable();

    $(".reportSelectionRadio:checked").closest('fieldset').find('input[type="checkbox"]').prop('checked', true);

	$(".reportSelectionRadio").on('change', function () {
		var id = $(this).attr('id');
		$('.subSelection').each(function () {
		    if ($(this).hasClass('skipDefault') && $(this).hasClass(id)) {
				return; // Don't default the HUD/HMIS Service Report to selected
			}

			$(this).prop('checked', $(this).hasClass(id));
		});
	});

	$('.subSelection').change(function () {
		$('span[data-valmsg-for="SubReportSelections"]').html('');
		var parentId = $(this).data('icjia-parent-id');
		if(!$(parentId).is(':checked')){
			$(parentId).prop('checked', true);
			$(parentId).change();
		}
	});

	$('#Generate').click(function (e) {
	    e.preventDefault();
	    if (ValidateReportSelections()) {
			$('#main').submit();
		}
	});

	BindEventsForServiceProgramsSubReportSelections();

	
	$(document.body).on('change', "input[id^='CenterSelection']", function () {
	    ErrorClear("CenterIds", "CenterIds", "You must specify at least one Center.");
	});

	$(document.body).on('change', "input[name='SubReportSelections']", function () {
	    ErrorClear("SubReportSelections", "SubReportSelections", "You must specify at least one Subreport to run.");
	});
});

function ValidateReportSelections() {
	 //Start and end date entered
    var retval = true;

    if (!$('#Start').valid() || !$('#End').valid()) {
		return false;
	}

    var startDate =  new Date($('#Start').val());
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

	if (!$('input[name="SubReportSelections"]:checked').length) {
		ErrorFormat("SubReportSelectionsVal", "SubReportSelections", "You must specify at least one Subreport to run.");
		retval = false;
		$setFocusToError = null || $('input[name="SubReportSelections"]:first');
	}

	if ($('#providerId').val() == 2 && $('input[name="SubReportSelections"]:checked').length >= 1 && ($('[data-icjia-name="StdRptServiceProgramsNonClientCrisisInterventionDemographics"]').is(':checked') && !$('[data-icjia-name="StdRptServiceProgramsNonClientCrisisIntervention"]').is(':checked'))) {
		$('span[data-valmsg-for="SubReportSelections"]').css('color', '#a94442').html($.trim($("label[for=" + $('[data-icjia-name="StdRptServiceProgramsNonClientCrisisInterventionDemographics"]').prop('id') + "]").text()) + " cannot be run without selecting " + $.trim($("label[for=" + $('[data-icjia-name="StdRptServiceProgramsNonClientCrisisIntervention"]').prop('id') + "]").text()) + ".");
		return false;
	}
	
	if (!retval) {
	    setFocus = null || $setFocusToError.focus();
	}
	return retval;
}

function BindEventsForServiceProgramsSubReportSelections() {
    var subReportsParent = $('input[type="radio"][data-icjia-name="StdRptServicePrograms"]').closest('fieldset');

    var hudHmisSubReport = subReportsParent.find('input[type="checkbox"][name="SubReportSelections"][data-icjia-name="StdRptServiceProgramsHudHmisServiceReport"]');
    var otherSubReports = subReportsParent.find('input[type="checkbox"][name="SubReportSelections"][data-icjia-name!="StdRptServiceProgramsHudHmisServiceReport"]');

    hudHmisSubReport.change(function () {
        otherSubReports.prop('checked', $(this).is(':not(:checked)'));
    });

    otherSubReports.change(function () {
        if ($(this).is(':not(:checked)')) {
            return;
        }

        hudHmisSubReport.prop('checked', false);
    });
}

if ($('#providerId').val() == 2) {
	$(document).on('change', '[data-icjia-name="StdRptServiceProgramsNonClientCrisisIntervention"]', function () {
		if ($(this).not(':checked')) {
			$('[data-icjia-name="StdRptServiceProgramsNonClientCrisisInterventionDemographics"]').prop('checked', false);
		}
	});

	$(document).on('change', '[data-icjia-name="StdRptServiceProgramsNonClientCrisisInterventionDemographics"]', function () {
		if ($(this).is(':checked')) {
			$('[data-icjia-name="StdRptServiceProgramsNonClientCrisisIntervention"]').prop('checked', true);
		}
	});
}