$(document).ready(function disableOrEnableMedicalData() {   
	// Visited Medical Facility Enable & Disable on Selection Change
    $(document).on('change', 'select[id$="MedicalVisitId"]', function () {        
		$(this).closest('.row').find('select[id$="MedicalTreatmentId"]').prop('disabled', $(this).val() != "1");
		$(this).closest('.row').find('select[id$="InjuryId"]').prop('disabled', $(this).val() != "1");
		$(this).closest('.row').find('select[id$="MedWhereId"]').prop('disabled', $(this).val() != "1");
		$(this).closest('.row').find('select[id$="EvidKitId"]').prop('disabled', $(this).val() != "1");
		$(this).closest('.row').find('select[id$="SANETreatedId"]').prop('disabled', $(this).val() != "1");
		
	});

	$(document).on('change', 'select[id$="PhotosTakenId"]', function () {
		$(this).closest('.row').find('input[id$="WherePhotos"]').prop('disabled', $(this).val() != "1");
	});

	// On Change
	// Visited Medical Facility Enable & Disable on Selection Change
	$('select[id$="MedicalVisitId"]').change(function () {

		// Changes the Type of Medical Facility input value to "None", if "No" is selected on the Visited Medical Facility input.
		if ($(this).val() == "2") {
			// Set value of select to (value="6") to display "None"
			$('select[id$="MedWhereId"]').val('6');
			// Sets Hidden input value to 6 to save to DB
			$("input[name$='MedWhereId']").val('6');
		} else {
			$('select[id$="MedWhereId"]').val('');
			$("input[name$='MedWhereId']").val('');
		}

	});

});