$(document).ready(function disableOrEnablePreviousServiceUseData() {

	// Initialize both input date fields to disabled
	$("#PreviousServiceUse_PrevShelterDate").prop("disabled", true);
	$("#PreviousServiceUse_PrevServiceDate").prop("disabled", true);

	// On Load
	// If Previous Shelter Use is "Yes" enable date input field
	if ($("#PreviousServiceUse_PrevShelterUseId").val() == "1") {
		$("#PreviousServiceUse_PrevShelterDate").prop("disabled", false);
	}
	// If Previous Service Use is "Yes" enable date input field
	if ($("#PreviousServiceUse_PrevServiceUseId").val() == "1") {
		$("#PreviousServiceUse_PrevServiceDate").prop("disabled", false);
	}

	// On Change
	// Previous Shelter Use Enable & Disable on Selection Change
	$('#PreviousServiceUse_PrevShelterUseId').change(function () {
		if ($("#PreviousServiceUse_PrevShelterUseId").val() == "1") {
			$("#PreviousServiceUse_PrevShelterDate").prop("disabled", false); // Enables date inputfield.
			$("#PreviousServiceUse_PrevShelterDate_Hidden").remove(); // Remove hidden input
		} else {
			// Adds hidden field for PrevShelterDate input field to post an empty value to the DB when disabled.
			$("#PreviousServiceUse_PrevShelterDate").prop("disabled", true);
			// Prevents multipl hidden fields from being added if one already exists.
			if (!$("#PreviousServiceUse_PrevShelterDate_Hidden").length) {
				$("#previousServiceUse").append('<input type="hidden" id="PreviousServiceUse_PrevShelterDate_Hidden" name="PreviousServiceUse.PrevShelterDate" value="">'); // Adds hidden field
			}
		}
	});
	// Previous Service Use Enable & Disable on Selection Change
	$('#PreviousServiceUse_PrevServiceUseId').change(function () {
		if ($("#PreviousServiceUse_PrevServiceUseId").val() == "1") {
			$("#PreviousServiceUse_PrevServiceDate").prop("disabled", false); // Enables date inputfield.
			$("#PreviousServiceUse_PrevServiceDate_Hidden").remove(); // Remove hidden input
		} else {
			// Adds hidden field for PrevShelterDate input field to post an empty value to the DB when disabled.
			$("#PreviousServiceUse_PrevServiceDate").prop("disabled", true);
			// Prevents multipl hidden fields from being added if one already exists.
			if (!$("#PreviousServiceUse_PrevServiceDate_Hidden").length) {
				$("#previousServiceUse").append('<input type="hidden" id="PreviousServiceUse_PrevServiceDate_Hidden" name="PreviousServiceUse.PrevServiceDate" value="">'); // Adds hidden field
			}
		}
	});

});