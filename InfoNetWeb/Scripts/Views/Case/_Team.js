// Team add button click
$(document).on('click', '[data-icjia-role="mdteam.add"]', function () {
	var $this = $(this);
	var params = $this.data('icjia-mustache-next');
	var $result = $this.find('[data-icjia-role="mustache"]').mustache(params).appendTo($this.closest('table').find('tbody'));
	params.key++;
	setClientCaseIds($result);
	rescanUnobtrusiveValidation('#main');
	rescanTooltipsForSR($result.parent());
	$('#main').addClass('icjia-make-dirty').trigger('dirty.dirtyforms'); /* new record cannot be removed from page */
	$result.find('.well.collapse').collapse('show');
	window.setTimeout(function () {
		$result.find('select:enabled:not([readonly])').first().focus();
	}, 350); /* rotate-90-if-collapsed transition ms */
});

// Team delete button clicks
$(document).on('click', '[data-icjia-role="mdteam.delete"]', function () {
	var $primaryRow = $(this).closest('tr').prev();
	$primaryRow.find('[data-icjia-role="mdteam.index"]').each(function () {
		var sign = this.value.substring(0, 1) == '=' ? '-' : '~';
		this.value = sign + this.value.substring(1);
		$(this).change();
	});
	$(this).closest('.collapse').collapse('hide');
	window.setTimeout(function () {
		$primaryRow.addClass('deleted');
		$primaryRow.find('[data-icjia-role="mdteam.expand"]').addClass('hide');
		$primaryRow.find('[data-icjia-role="mdteam.restore"]').removeClass('hide');
	}, 350); /* rotate-90-if-collapsed transition ms */
});

// Team restore
$(document).on('click', '[data-icjia-role="mdteam.restore"]', function () {
	var $primaryRow = $(this).closest('tr');
	$primaryRow.removeClass('deleted');
	$primaryRow.find('[data-icjia-role="mdteam.index"]').each(function () {
		var sign = this.value.substring(0, 1) == '-' ? '=' : '+';
		this.value = sign + this.value.substring(1);
		$(this).change();
	});
	var $expandButton = $primaryRow.find('[data-icjia-role="mdteam.expand"]').removeClass('hide');
	$primaryRow.find('[data-icjia-role="mdteam.restore"]').addClass('hide');
	$primaryRow.next().find('.collapse').collapse('show');
	$expandButton.focus();
});

// disable well collapse when Team has been deleted
$(document).on('show.bs.collapse', 'tr:has([data-icjia-role="mdteam.index"]:regex(value, ^[-~])) + tr .collapse', function (event) {
	event.preventDefault();
});

function setClientCaseIds($element) {
	$element.find('input[name$=".ClientID"]').val($('#clientId').val());
	$element.find('input[name$=".CaseID"]').val($('#caseId').val());
}