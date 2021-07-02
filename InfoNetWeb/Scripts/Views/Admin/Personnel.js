var isNew = ($('input[type="hidden"][name$="SvId"]').val() == null || $('input[type="hidden"][name$="SvId"]').val() == 0);
var isVolunteer = $('input[name$="TypeId"][value="2"]').is(':checked');

// Set defaults for new Personnel
if(isNew)
{
	$('.radioType').toggle(true);
	$('input[name$="TypeId"][value="1"]').prop('checked', true);
	$('input[name$="Type"]').val('S');
}

// Show Volunteer fields if editing a Volunteer
if(isVolunteer)
{
	$('.supervisorID').show();
	$('.staffPersonnelTypes').hide().find('select[name$="PersonnelTypeId"]').prop('disabled', true);
	$('.volunteerPersonnelTypes').show().find('select[name$="PersonnelTypeId"]').prop('disabled', false);
}


$(function () {

	$('input[name$="TypeId"]').change(function () {
		var isVolunteer = $(this).val() == '2';
		$('.supervisorID').toggle(isVolunteer);
		$('.staffPersonnelTypes').toggle(!isVolunteer).find('select[name$="PersonnelTypeId"]').prop('disabled', isVolunteer);
		$('.volunteerPersonnelTypes').toggle(isVolunteer).find('select[name$="PersonnelTypeId"]').prop('disabled', !isVolunteer);
		$('input[name$="Type"]').val(isVolunteer ? 'S' : 'V');
	});

	$('#saveAddNewButton').click(function () {
		$('#saveAddNew').val('1');
		$('#main').submit();
	});

	$('#saveButton').click(function () {
		$('#saveAddNew').val('0');
		$('#main').submit();
	});

	$('#WorkPhone').on("blur", function formatNumber() {
		var numbers = this.value.replace(/\D/g, '');
		var char = { 0: '(', 3: ') ', 6: '-' }; // Determines where each character is placed within the string.
		this.value = '';
		for (var i = 0; i < numbers.length; i++) {
			this.value += (char[i] || '') + numbers[i];
		}
	});
});