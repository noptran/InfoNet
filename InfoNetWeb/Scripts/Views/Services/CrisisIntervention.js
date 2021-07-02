var prevDate;

$(document).ready(function () {
	setTypeahead();

	$('#main').on('focusin blur', '[data-provide="datepicker"] input', function (event) {
		//console.log('prevDate = ' + prevDate);
		//console.log('currentDate = ' + $(this).val());
		if (event.type === 'focusin') {
			prevDate = $(this).val();
		}
		else {
			if (prevDate != $(this).val()) {
				var ajaxURL = "/Service/GetStaff?serviceDate=" + $(this).val();

				$.ajax({
					url: ajaxURL,
					success: function (data) {
						//console.log(data);
						$('select[name$="SVID"]').each(function () {
							var selectList = this;
							var selectedOption = $(this).find('option:selected');
							var selectedValue = $(selectedOption).val();
							var isPresent = false;
							$(this).find('option').remove().end();
							$(this).append($('<option>').text("<Pick One>").attr('value', ""));
							$.each(data, function (i, staff) {
								if (selectedValue == data[i].SVID) {
									$(selectList).append($('<option>').text(data[i].EmployeeName).attr('value', data[i].SVID).attr('selected', 'selected'));
									isPresent = true;
								}
								else {
									$(selectList).append($('<option>').text(data[i].EmployeeName).attr('value', data[i].SVID));
								}
							});
							if (!isPresent && selectedValue != "") {
								$(selectList).find('option:selected').removeAttr('selected');
								$(selectedOption).attr('selected', 'selected');
								$(selectList).append(selectedOption);
							}
						});
					},
					error: function (xhr, ajaxOptions, thrownError) {
						//console.log(xhr.responseText);
					}
				});
			}
		}
	});

	// On County Changed
	$('select[name$="CountyID"]').change(function () {
		clearAddressFields();
		$('[data-icjia-county]').attr('data-icjia-county', $(this).val());
	});

	$('#ZipCode').on('input',function () {
		if ($(this).hasClass('input-validation-error')) {
			$(this).removeClass('input-validation-error');
			$(this).closest('.form-group').removeClass('has-error');
			$(this).parent().next().removeClass('field-validation-error').html('');
		}
		if (this.value.length > 5 && this.value.indexOf('-') == -1) {
			var numbers = this.value.replace('-', '');
			var char = { 5: '-' }; // Determines where each character is placed within the string.
			this.value = '';
			for (var i = 0; i < numbers.length; i++) {
				this.value += (char[i] || '') + numbers[i];
			}
		}
	});
});

function clearAddressFields() {
	$('input[id$="Zipcode"]').typeahead('val', '');
	$('input[id$="Town"]').typeahead('val', '');
	$('input[id$="Township"]').typeahead('val', '');
	$('input[id$="Zipcode"]').val('').change();
	$('input[id$="Town"]').val('').change();
	$('input[id$="Township"]').val('').change();
}

function setTypeahead() {
	setTownshipTypeahead($('#Township'));
	setTownTypeahead($('#Town'));
	setZipCodeTypeahead($('#ZipCode'));
}

function setTownshipTypeahead($target) {
	$target.typeahead({
		hint: true,
		limit: 5,
		minLength: 1
	},
	{
		name: 'Township',
		display: function (item) {
			return item.Name;
		},
		source: function (query, syncResults, asyncResults) {
			countyID = $('#CountyID').val();
			countyID = (countyID == "") ? 0 : countyID;
			$.ajax('/USPS/SearchTownshipName?input=' + query + '&StateID=14&CountyID=' + countyID,
			{
				dataType: 'json',
				success: function (data) {
					//console.log(data);
					asyncResults(data);
				}
			});
		},
		templates: {
			empty: [
				'<div class="empty-message tt-suggestion">No Results</div>'
			],
			suggestion: function (value) { return '<div>' + value.Name + '</div>'; }
		}
	});
}

function setTownTypeahead($target) {
	$target.typeahead({
		hint: true,
		limit: 5,
		minLength: 1
	},
	{
		name: 'Town',
		display: function (item) {
			return item.Name;
		},
		source: function (query, syncResults, asyncResults) {
			countyID = $('#CountyID').val();
			countyID = (countyID == "") ? 0 : countyID;
			$.ajax('/USPS/SearchCityName?input=' + query + '&StateID=14&CountyID=' + countyID,
			{
				dataType: 'json',
				success: function (data) {
					//console.log(data);
					asyncResults(data);
				}
			});
		},
		templates: {
			empty: [
				'<div class="empty-message tt-suggestion">No Results</div>'
			],
			suggestion: function (value) { return '<div>' + value.Name + '</div>'; }
		}
	});
}

function setZipCodeTypeahead($target) {
	var isInvalid = $target.hasClass('input-validation-error');
	$target.typeahead({
		hint: true,
		limit: 5,
		minLength: 1
	},
	{
		name: 'ZipCode',
		display: function (item) {
			return item.Name;
		},
		source: function (query, syncResults, asyncResults) {
			countyID = $('#CountyID').val();
			countyID = (countyID == "") ? 0 : countyID;
			$.ajax('/USPS/SearchZip?input=' + query + '&StateID=14&CountyID=' + countyID,
			{
				dataType: 'json',
				success: function (data) {
					//console.log(data);
					asyncResults(data);
				}
			});
		},
		templates: {
			empty: [
				'<div class="empty-message tt-suggestion">No Results</div>'
			],
			suggestion: function (value) { return '<div>' + value.Name + '</div>'; }
		}
	});
	if(isInvalid)
	{
		$target.addClass('input-validation-error');
		$target.closest('.form-group').addClass('has-error');
		$target.parent().next().addClass('field-validation-error').html('Invalid Zip Code for County');
	}
}

//Validate Form
function validateMyForm() {
	var response = true;
	return response;
}

function validateMyForm2() {
	$('#saveAddNew').val('1');
	return validateMyForm();
}