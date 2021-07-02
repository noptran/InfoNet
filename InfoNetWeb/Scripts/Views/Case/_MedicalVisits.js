// Visited Medical Facility Enable & Disable on Selection Change
$(document).on('change', 'select[id$="MedicalVisitId"]', function () {
  	$(this).closest('.row').find('select[id$="MedicalTreatmentId"]').prop('disabled', $(this).val() != "1");
	$(this).closest('.row').find('select[id$="EvidKitId"]').prop('disabled', $(this).val() != "1");
	$(this).closest('.row').find('select[id$="SANETreatedId"]').prop('disabled', $(this).val() != "1");
});

// Medical Visit add button click
$(document).on('click', '[data-icjia-role="medicalvisit.add"]', function () {
	var $this = $(this);
	var params = $this.data('icjia-mustache-next');
	var $result = $this.find('[data-icjia-role="mustache"]').mustache(params).appendTo($this.closest('table').children('tbody'));
	params.key++;

	rescanUnobtrusiveValidation('#main');
	rescanTooltipsForSR($result.parent());
	$('#main').addClass('icjia-make-dirty').trigger('dirty.dirtyforms'); /* new record cannot be removed from page */
	$result.find('.well.collapse').collapse('show');
	window.setTimeout(function () {
		$result.find('select').first().focus();
	}, 350); /* rotate-90-if-collapsed transition ms */
});

// Medical Visit delete button clicks
$(document).on('click', '[data-icjia-role="medicalvisit.delete"]', function () {
	var $primaryRow = $(this).closest('tr').prev();

	$primaryRow.next().find('[data-icjia-role="medicalvisit.index"]').each(function () {
		var sign = this.value.substring(0, 1) == '=' ? '-' : '~';
		this.value = sign + this.value.substring(1);
		$(this).change();
	});
	$(this).closest('.collapse').collapse('hide');
	window.setTimeout(function () {
		$primaryRow.addClass('deleted');
		$primaryRow.find('[data-icjia-role="medicalvisit.expand"]').addClass('hide');
		$primaryRow.find('[data-icjia-role="medicalvisit.restore"]').removeClass('hide');
	}, 350); /* rotate-90-if-collapsed transition ms */
});

// Medical Visit restore
$(document).on('click', '[data-icjia-role="medicalvisit.restore"]', function () {
	var $primaryRow = $(this).closest('tr');

	$primaryRow.removeClass('deleted');
	$primaryRow.next().find('[data-icjia-role="medicalvisit.index"]').each(function () {
		var sign = this.value.substring(0, 1) == '-' ? '=' : '+';
		this.value = sign + this.value.substring(1);
		$(this).change();
	});
	var $expandButton = $primaryRow.find('[data-icjia-role="medicalvisit.expand"]').removeClass('hide');
	$primaryRow.find('[data-icjia-role="medicalvisit.restore"]').addClass('hide');
	$primaryRow.next().find('.collapse').collapse('show');
	$expandButton.focus();
});

// disable well collapse when MedicalVisit has been deleted
$(document).on('show.bs.collapse', 'tr:has([data-icjia-role="medicalvisit.index"]:regex(value, ^[-~])) .collapse', function (event) {
	event.preventDefault();
});