$(document).ready(function disableOrEnableSpecialNeedsData() {

	// Initialize both Textfield and Dropdown to disabled
	$("#ClientDisability_PrimaryLanguageID").prop("disabled", true);
	$("#ClientDisability_WhatOther").prop("disabled", true);

	// On Load
	// Primary Language Dropdown
	// Enables dropdown if chekced
	if ($("#ClientDisability_LimitedEnglish").is(":checked")) {
		$("#ClientDisability_PrimaryLanguageID").prop("disabled", false);
	}
	// Textfield
	// Enables textfield if chekced
	if ($("#ClientDisability_OtherDisability").is(":checked")) {
		$("#ClientDisability_WhatOther").prop("disabled", false);
	}

	// On Change
	// Primary Language Dropdown
	$("#ClientDisability_LimitedEnglish").change(function () {
		if ($(this).is(":checked")) {
			$("#ClientDisability_PrimaryLanguageID").prop("disabled", false);
			$("#ClientDisability_PrimaryLanguageID_Hidden").remove(); // Removes hidden field
		} else {
			$("#ClientDisability_PrimaryLanguageID").prop("disabled", true);
			$("#ClientDisability_PrimaryLanguageID").prop('selectedIndex', 0); // Resets display text back to "<Pick One>" when unchecked
			$("#specialNeeds").append('<input type="hidden" id="ClientDisability_PrimaryLanguageID_Hidden" name="ClientDisability.PrimaryLanguageID" value="">'); // Adds hidden field
		}
	});
	// Textfield
	$("#ClientDisability_OtherDisability").change(function () {
		if ($(this).is(":checked")) {
			$("#ClientDisability_WhatOther").prop("disabled", false);
			$("#ClientDisability_WhatOther_Hidden").remove(); // Removes hidden field
		} else {
			$("#ClientDisability_WhatOther").prop("disabled", true);
			// $("#ClientDisability_WhatOther").val(""); // Clears text field when unchecked.
			$("#specialNeeds").append('<input type="hidden" id="ClientDisability_WhatOther_Hidden" name="ClientDisability.WhatOther" value="">'); // Adds hidden field
		}
	});

	// Top 3 Selectors
	// Inilization / On Load
	// Check if one of these options are checked. If so, disable all other fields and checkboxes.
	if ($("#ClientDisability_NoSpecialNeeds").is(":checked")) {
		$("#icjia-special-needs-body input").prop("disabled", true);
		$("#ClientDisability_UnknownSpecialNeeds, #ClientDisability_NotReported").prop("disabled", true);
	} else if ($("#ClientDisability_UnknownSpecialNeeds").is(":checked")) {
		$("#icjia-special-needs-body input").prop("disabled", true);
		$("#ClientDisability_NoSpecialNeeds, #ClientDisability_NotReported").prop("disabled", true);
	} else if ($("#ClientDisability_NotReported").is(":checked")) {
		$("#icjia-special-needs-body input").prop("disabled", true);
		$("#ClientDisability_UnknownSpecialNeeds, #ClientDisability_NoSpecialNeeds").prop("disabled", true);
	}
	// Changing Top 3 Selectors
	// Enabling & Disabling the input fields.
	// Adding hidden fields for checkboxes in Special Needs panel and concatenating "_Hidden" to the IDs of all panel inputs
	$("#ClientDisability_NoSpecialNeeds, #ClientDisability_UnknownSpecialNeeds, #ClientDisability_NotReported").change(function () {
		// Disables
		if ($(this).is(":checked")) {
			$("#icjia-special-needs-body input").prop("disabled", true); // Disables all inputs within the #icjia-special-needs-body div
			$("#icjia-special-needs-body select").prop("disabled", true); // Disables the dropdown for primary language
			$("#ClientDisability_NoSpecialNeeds, #ClientDisability_UnknownSpecialNeeds, #ClientDisability_NotReported").not(this).prop("disabled", true);
			// Create and append hidden fields to the "speicalNeeds" panel ID
			// Checkboxes
			$("#icjia-special-needs-body :checkbox").each(function () {
				// Adds hidden field based on the input field ID and concatenates "_Hidden" to the end of the ID
				$("#specialNeeds").append('<input type="hidden" id="' + $(this).prop("id") + '_Hidden" name="' + $(this).prop("name") + '" value="false">');
			});
			// Text Fields
			$("#icjia-special-needs-body :text").each(function () {
				// Adds hidden field based on the input field ID and concatenates "_Hidden" to the end of the ID
				$("#specialNeeds").append('<input type="hidden" id="' + $(this).prop("id") + '_Hidden" name="' + $(this).prop("name") + '" value="">');
			});
			// Dropdown
			$("#icjia-special-needs-body select").each(function () {
				// Adds hidden field based on the input field ID and concatenates "_Hidden" to the end of the ID
				$("#specialNeeds").append('<input type="hidden" id="' + $(this).prop("id") + '_Hidden" name="' + $(this).prop("name") + '" selectedIndex="0">');
			});
		}
		// Enables
		else {
			$("#icjia-special-needs-body input").prop("disabled", false);
			$("#ClientDisability_NoSpecialNeeds, #ClientDisability_UnknownSpecialNeeds, #ClientDisability_NotReported").not(this).prop("disabled", false);

			// Selects all of the input fields via the div with ID "#icjia-special-needs-body"
			$("#icjia-special-needs-body input").each(function () {
				$("input[id$='_Hidden']").remove(); // Remove all inputs with an ID ending in "_Hidden"
				//$('"' + $(this).prop("id") + '_Hidden"').remove(); // Removes hidden field <- Figure out how to make this work *NEH*
			});
			// Text-Field
			$("#ClientDisability_WhatOther").prop("disabled", true); // Ensures the text field is disabled when any of the top three fields are unchecked

			// Ensures the dropdown is enabled, if checked, when any of the top three fields are unchecked
			if ($("#ClientDisability_LimitedEnglish").is(":checked")) {
				$("#ClientDisability_PrimaryLanguageID").prop("disabled", false);
			}
			// Ensures the text field is enabled, if checked, when any of the top three fields are unchecked
			if ($("#ClientDisability_OtherDisability").is(":checked")) {
				$("#ClientDisability_WhatOther").prop("disabled", false);
			}
		}
	});


	$("#icjia-special-needs-body input").change (function (){
	    console.log ("change");
	    var anyBelowChecked = false;
	    $("#icjia-special-needs-body input:checked").each (function (){ anyBelowChecked = true; }); 
	    $("#ClientDisability_NoSpecialNeeds, #ClientDisability_UnknownSpecialNeeds, #ClientDisability_NotReported").prop("disabled", anyBelowChecked);
	});


});