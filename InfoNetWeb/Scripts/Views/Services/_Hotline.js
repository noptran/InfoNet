var prevDate;

$('#TimeOfDay').val(validateAndFormatTime($('#TimeOfDay').val()));

$(document).ready(function () {

	setTypeahead();

	$('#TimeOfDay').on('blur', function () {
		$(this).val(validateAndFormatTime($(this).val()));
	});

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

	$('#ZipCode').on('input', function () {
		if ($(this).hasClass('input-validation-error')) {
			$(this).removeClass('input-validation-error');
			$(this).closest('.form-group').removeClass('has-error');
			$(this).parent().next().removeClass('field-validation-error').html('');
		}
		if (!$(this).closest('.twitter-typeahead').length) {
			setZipCodeTypeahead();
			$(this).focus();
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


function roundToTwo(num) {
	return +(Math.round(num + "e+2") + "e-2");
}

function validateAndFormatTime(origTime) {
	var time = origTime;
	var hour, minute, tt;
	var errorMsg = false;

	if(time == "" || time === "undefined")
	{
		return origTime;
	}

	// Only Zero entered
	if (time == 0) {
		return '12:00 AM';
	}

	var reg1 = /^(\d{1,2})\s*?$/;

	// Check for only hour entered, nothing else
	if (regs1 = time.match(reg1)) {
		if (time > 0 && time < 12) {
			time = time + ":00 AM";
		}
		else {
			time = (time == 12) ? time + ":00 PM" : (time - 12) + ":00 PM";
		}
	}

	var reg2 = /^(\d{1,2})\s*([apAP][mM])?$/;

	// Check for 12-Hour format  (hour and AM/PM)
	if (regs2 = time.match(reg2)) {
		time = regs2[1] + ":00 " + regs2[2];
	}


	// regular expression to match required time format
	var re = /^(\d{1,2}\s*):(\d{1,2}\s*)(:00\s*)?([apAP][mM])?$/;

	if (time != '') {
		if (regs = time.match(re)) {
			if (regs[4]) {
				// 12-hour time format with am/pm
				if (regs[1] < 1 || regs[1] > 12) {
					errorMsg = true;
				}
				else {
					tt = regs[4].trim();
					hour = regs[1].trim();
				}
			} else {
				// 24-hour time format
				if (regs[1] > 23) {
					errorMsg = true;
				}
				else {
					if (regs[1] >= 12) {
						tt = "PM";
						hour = (regs[1] == 12) ? regs[1].trim() : (regs[1] - 12).toString().trim();
					}
					else {
						tt = "AM";
						hour = (regs[1] == 0) ? "12" : regs[1].trim();
					}
				}
			}
			if (!errorMsg && regs[2] > 59) {
				errorMsg = true;
			}
			else {
				minute = (regs[2] < 10 && regs[2].length == 1) ? "0" + regs[2].trim() : regs[2].trim();
			}
		} else {
			errorMsg = true;
		}
	}

	if (errorMsg) {
		//console.log("Invalid Time Entry: " + origTime);
		//PRC TODO show jquery unobtrusive error 
		return origTime;
	}
	else {
		return hour + ":" + minute + " " + tt.toUpperCase();
	}
}


function setTypeahead()
{
	setTownshipTypeahead();
	setTownTypeahead();
	setZipCodeTypeahead();
}

function setTownshipTypeahead() {
	$('#Township').typeahead({
		hint: true,
		limit: 5,
		minLength: 1
	},
	{
		name: 'Name',
		display: function (item) {
			return item.Name;
		},
		source: function (query, syncResults, asyncResults) {
			countyID = $('#CountyID').val();
			if (countyID == "") {
				countyID = 0;
			}
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

function setTownTypeahead() {
	$('#Town').typeahead({
		hint: true,
		limit: 5,
		minLength: 1
	},
	{
		name: 'Name',
		display: function (item) {
			return item.Name;
		},
		source: function (query, syncResults, asyncResults) {
			countyID = $('#CountyID').val();
			if (countyID == "") {
				countyID = 0;
			}
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

function setZipCodeTypeahead() {
	$('#ZipCode:not(.input-validation-error)').typeahead({
		hint: true,
		limit: 5,
		minLength: 1
	},
	{
		name: 'Name',
		display: function (item) {
			return item.Name;
		},
		source: function (query, syncResults, asyncResults) {
			countyID = $('#CountyID').val();
			if (countyID == "") {
				countyID = 0;
			}
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