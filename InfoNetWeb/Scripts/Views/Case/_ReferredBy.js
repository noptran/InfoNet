$(document).ready(function () {
    disableInputForCheckbox($("#ClientReferralSource_Other"), $("#ClientReferralSource_WhatOther"));

    if ($("#ProviderID").val() == 1) {
        disableInputForCheckbox($("#ClientReferralSource_ToOther"), $("#ClientReferralSource_ToWhatOther"));
    }
});

$('label[for="ClientReferralSource_AgencyID"').tooltip({
	template: '<div class="tooltip" role="tooltip"><div class="tooltip-arrow"></div><div class="tooltip-inner" style="max-width: 450px;"></div></div>'
});

$(document.body).on('change', "#ClientReferralSource_Other", function () {
    checkboxHandleInputDisplayAndHidden($(this),"ClientReferralSource_WhatOther");
});


if ($("#ProviderID").val() == 1) {
    $(document.body).on('change', "#ClientReferralSource_ToOther", function () {
        checkboxHandleInputDisplayAndHidden($(this),"ClientReferralSource_ToWhatOther");
    });
}

function checkboxHandleInputDisplayAndHidden($checkbox, inputIdText) {
    var inputId = "#" + inputIdText;
    var $input = $(inputId);

    if ($checkbox.is(":checked")) {
        $input.prop("disabled", false);
        $(inputId + "_Hidden").remove();
    } else {
        $input.prop("disabled", true);
        $('<input type="hidden" id="' + inputIdText + "_Hidden" + '" name="' + inputIdText.replace("_", ".") + '" value="">').insertBefore(inputId);
    }
}

function disableInputForCheckbox($checkbox, $input) {
    $input.prop("disabled", !$checkbox.is(":checked"));
}