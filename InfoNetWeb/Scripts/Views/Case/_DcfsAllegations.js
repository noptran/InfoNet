$(document).ready(function () {

	// Initialize Chosen plugin
	$('.allegationChosen').each(function () {
		var chosenID = this.id + "_chosen";
		chosenID = chosenID.replace(':', '_');
		$(this).chosen();
		if ($(this).hasClass('input-validation-error')) {
			$('#' + chosenID).find('ul.chosen-choices').addClass('input-validation-error').focus();
		}
	});

	$(document).on('change', 'select[name$="RespondentArray"]', function () {
		var $origSelect = $(this);
		var $errorGroup = $('#dcfsAllegations').find('.icjia-error-group').first();
		var $sideBarLink = $(document).find('a[href="#dcfsAllegations"]').parent();
		var $formGroup = $origSelect.closest('div.form-group');
		var $chosenUl = $formGroup.find('ul.chosen-choices');
		if($origSelect.valid() && $origSelect.find('option:selected').length > 0) {
			$chosenUl.removeClass('input-validation-error');
			$formGroup.removeClass('has-error');
			$formGroup.find('span.help-block').html('');
			$origSelect.removeClass('input-validation-error');
			var errorCount = $errorGroup.find('.input-validation-error').length;
			if (errorCount == 0) {
				$errorGroup.removeClass('has-group-error');
				$sideBarLink.removeClass('has-error');
			}
		}
	});
});

// Allegation add button click
$(document).on('click', '[data-icjia-role="allegation.add"]', function () {
	var $this = $(this);
	var params = $this.data('icjia-mustache-next');
	var $result = $this.find('[data-icjia-role="mustache"]').mustache(params).appendTo($this.closest('table').find('tbody'));
	params.key++;
	setClientCaseIds($result);
	$result.find('.allegationChosen').chosen();
	rescanUnobtrusiveValidation('#main');
	rescanTooltipsForSR($result.parent());
	$('#main').addClass('icjia-make-dirty').trigger('dirty.dirtyforms'); /* new record cannot be removed from page */
	$result.find('.well.collapse').collapse('show');
	window.setTimeout(function () {
		$result.find(':text:enabled:not([readonly])').first().focus();
	}, 350); /* rotate-90-if-collapsed transition ms */
});

// Allegation delete button clicks
$(document).on('click', '[data-icjia-role="allegation.delete"]', function () {
	var $primaryRow = $(this).closest('tr').prev();
	$primaryRow.find('[data-icjia-role="allegation.index"]').each(function () {
		var sign = this.value.substring(0, 1) == '=' ? '-' : '~';
		this.value = sign + this.value.substring(1);
		$(this).change();
	});
	$(this).closest('.collapse').collapse('hide');
	window.setTimeout(function () {
		$primaryRow.addClass('deleted');
		$primaryRow.find('[data-icjia-role="allegation.expand"]').addClass('hide');
		$primaryRow.find('[data-icjia-role="allegation.restore"]').removeClass('hide');
	}, 350); /* rotate-90-if-collapsed transition ms */
});

// Allegation restore
$(document).on('click', '[data-icjia-role="allegation.restore"]', function () {
	var $primaryRow = $(this).closest('tr');
	$primaryRow.removeClass('deleted');
	$primaryRow.find('[data-icjia-role="allegation.index"]').each(function () {
		var sign = this.value.substring(0, 1) == '-' ? '=' : '+';
		this.value = sign + this.value.substring(1);
		$(this).change();
	});
	var $expandButton = $primaryRow.find('[data-icjia-role="allegation.expand"]').removeClass('hide');
	$primaryRow.find('[data-icjia-role="allegation.restore"]').addClass('hide');
	$primaryRow.next().find('.collapse').collapse('show');
	$expandButton.focus();
});

// disable well collapse when Allegation has been deleted
$(document).on('show.bs.collapse', 'tr:has([data-icjia-role="allegation.index"]:regex(value, ^[-~])) + tr .collapse', function (event) {
	event.preventDefault();
});

function setClientCaseIds($element) {
	$element.find('input[name$=".ClientID"]').val($('#clientId').val());
	$element.find('input[name$=".CaseID"]').val($('#caseId').val());
}