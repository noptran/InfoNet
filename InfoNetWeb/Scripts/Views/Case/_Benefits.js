var $benefitsVisible = $('[data-icjia-role="benefits"] :checkbox:visible');
var $benefitsNoAndUnknown = $('[data-icjia-role="nobenefit"], [data-icjia-role="unknownbenefit"]');
var $benefitsNotNoAndUnknown = $benefitsVisible.not($benefitsNoAndUnknown);

var $healthInsuranceVisible = $('[data-icjia-role="healthinsurance"] :checkbox:visible');
var $healthInsuranceNoAndUnknown = $('[data-icjia-role="nohealthinsurance"], [data-icjia-role="unknownhealthinsurance"]');
var $healthInsuranceNotNoAndUnknown = $healthInsuranceVisible.not($healthInsuranceNoAndUnknown);

$(function () {
	benefitsHideCheckboxes($benefitsNotNoAndUnknown, $benefitsNoAndUnknown);
	benefitsHideCheckboxes($healthInsuranceNotNoAndUnknown, $healthInsuranceNoAndUnknown);
});

$(document.body).on('change', '[data-icjia-role="benefits"] :checkbox:visible', function () {
	benenfitsShowHideCheckboxes($(this), $benefitsVisible, $benefitsNoAndUnknown, 'benefits', NoOrUnknownBenefits);
});

$(document.body).on('change', '[data-icjia-role="healthinsurance"] :checkbox:visible', function () {
	benenfitsShowHideCheckboxes($(this), $healthInsuranceVisible, $healthInsuranceNoAndUnknown, 'healthinsurance', NoOrUnknownhealthInsurance);
});

function NoOrUnknownBenefits($this) {
	return typeof $this.data('icjia-role') !== 'undefined' && ($this.data('icjia-role') == 'nobenefit' || $this.data('icjia-role') == 'unknownbenefit');
}

function NoOrUnknownhealthInsurance($this) {
	return typeof $this.data('icjia-role') !== 'undefined' && ($this.data('icjia-role') == 'nohealthinsurance' || $this.data('icjia-role') == 'unknownhealthinsurance');
}

function benefitsHideCheckboxes($notNoAndUnknown, $noAndUnknown) {
	$notNoAndUnknown.is(':checked') && $noAndUnknown.prop('disabled', true);
	if ($noAndUnknown.is(':checked')) {
		$notNoAndUnknown.prop('disabled', true);
		$noAndUnknown.not(":checked").prop('disabled', true);
	}
}

function benenfitsShowHideCheckboxes($this, $visible, $noAndUnknown, dataRole, noOrUnknown) {
	$('#' + $this.attr("id") + ':hidden').val($this.prop('checked'));
	if ($this.is(':checked')) {
		noOrUnknown($this) ? $visible.not($this).prop('disabled', true) : $noAndUnknown.prop('disabled', true);
	} else {
		noOrUnknown($this) ? $visible.not($this).prop('disabled', false) : $('[data-icjia-role="'+ dataRole +'"] :checkbox:visible:checked').not($noAndUnknown).length == 0 && $noAndUnknown.prop('disabled', false);
	}
}