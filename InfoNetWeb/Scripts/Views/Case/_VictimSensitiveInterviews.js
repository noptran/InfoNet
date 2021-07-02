// Interview add button click
$(document).on('click', '[data-icjia-role="interview.add"]', function () {
	var $this = $(this);
	var params = $this.data('icjia-mustache-next');
	var $result = $this.find('[data-icjia-role="mustache"]').mustache(params).appendTo($this.closest('table').children('tbody'));
	params.key++;

	rescanUnobtrusiveValidation('#main');
	rescanTooltipsForSR($result.parent());
	$('#main').addClass('icjia-make-dirty').trigger('dirty.dirtyforms'); /* new record cannot be removed from page */
	$result.find('.well.collapse').collapse('show');
	window.setTimeout(function () {
		$result.find('.date input').first().focus();
	}, 350); /* rotate-90-if-collapsed transition ms */
});

// Interview delete button clicks
$(document).on('click', '[data-icjia-role="interview.delete"]', function () {
	var $primaryRow = $(this).closest('tr').prev();

	$primaryRow.next().find('[data-icjia-role="interview.index"]').each(function () {
		var sign = this.value.substring(0, 1) === '=' ? '-' : '~';
		this.value = sign + this.value.substring(1);
		$(this).change();
	});
	$(this).closest('.collapse').collapse('hide');
	window.setTimeout(function () {
		$primaryRow.addClass('deleted');
		$primaryRow.find('[data-icjia-role="interview.expand"]').addClass('hide');
		$primaryRow.find('[data-icjia-role="interview.restore"]').removeClass('hide');
	}, 350); /* rotate-90-if-collapsed transition ms */
});

// Interview restore
$(document).on('click', '[data-icjia-role="interview.restore"]', function () {
	var $primaryRow = $(this).closest('tr');

	$primaryRow.removeClass('deleted');
	$primaryRow.next().find('[data-icjia-role="interview.index"]').each(function () {
		var sign = this.value.substring(0, 1) === '-' ? '=' : '+';
		this.value = sign + this.value.substring(1);
		$(this).change();
	});
	var $expandButton = $primaryRow.find('[data-icjia-role="interview.expand"]').removeClass('hide');
	$primaryRow.find('[data-icjia-role="interview.restore"]').addClass('hide');
	$primaryRow.next().find('.collapse').collapse('show');
	$expandButton.focus();
});

// disable well collapse when Interview has been deleted
$(document).on('show.bs.collapse', 'tr:has([data-icjia-role="interview.index"]:regex(value, ^[-~])) .collapse', function (event) {
	event.preventDefault();
});

// Observer add button click
$(document).on('click', '[data-icjia-role="interview.observer.add"]', function () {
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


$(document).on('click', '[data-icjia-role="interview.observer.delete"]', function () {
    var numOfObservers = $(this).closest("div[id$='_Collapse']").find("input[id$='VSIObserversById']");
    var _newValue = numOfObservers.val() - 1;
    numOfObservers.val(_newValue);
    numOfObservers.change();

	var $primaryRow = $(this).closest('tr');
	$primaryRow.find('[data-icjia-role="interview.observer.index"]').each(function () {
		var sign = this.value.substring(0, 1) === '=' ? '-' : '~';
		this.value = sign + this.value.substring(1);
		$(this).change();
    });
    $(this).closest('td').addClass('hide');
    strikeOut($primaryRow, "staffInfo", '.restoreObserverRow');
	$primaryRow.addClass('deleted');
});

function strikeOut($killrow, killClass, restoreClass) {
    $killrow.find(restoreClass).removeClass('hide');
    $killrow.addClass("deleted");
    $killrow.removeClass(killClass);
    $killrow.css('cursor', 'not-allowed'); 
}

// Observer restore button clicks
$(document).on('click', '[data-icjia-role="observer.restore"]', function () {
    var $row = $(this).closest('tr');
    $row.removeClass("deleted ");
    $row.addClass("staffInfo");
    $row.find(".deleteObserverRow").removeClass("hide");
    $row.find(".restoreObserverRow").addClass("hide");
    $row.find('span.help-block-deleted').removeClass('help-block-deleted').addClass('help-block');
    $row.children().find("input[type=text],input[type=number],select").removeAttr("disabled");
    
    var numOfObservers = $(this).closest("div[id$='_Collapse']").find("input[id$='VSIObserversById']");
    var _newValue = Number(numOfObservers.val()) + 1;
    numOfObservers.val(_newValue);
    numOfObservers.change();

    $row.find('[data-icjia-role="interview.observer.index"]').each(function () {
        var sign = this.value.substring(0, 1) === '-' ? '=' : '~';
        this.value = sign + this.value.substring(1);
        $(this).change();
    });
});

// Observer restore
$(document).on('click', '[data-icjia-role="interview.observer.restore"]', function () {
	var $primaryRow = $(this).closest('tr');

	$primaryRow.prev().find('[data-icjia-role="interview.observer.index"]').each(function () {
		var sign = this.value.substring(0, 1) === '-' ? '=' : '+';
		this.value = sign + this.value.substring(1);
		$(this).change();
	});
	$primaryRow.addClass('hide');
	$primaryRow.prev().removeClass('hide');
});

// Increase or decrease the observer count for add/delete/restore
$(document).on('click', '[data-icjia-role="interview.observer.add"]', function () {
	var numOfObservers = $(this).closest("div[id$='_Collapse']").find("input[id$='VSIObserversById']");
	var _newValue = parseInt(numOfObservers.val()) + 1;
	numOfObservers.val(_newValue);
	numOfObservers.change();
});



$(document).on('click', '[data-icjia-role="interview.observer.restore"]', function () {
	var numOfObservers = $(this).closest("div[id$='_Collapse']").find("input[id$='VSIObserversById']");
	var _newValue = parseInt(numOfObservers.val()) + 1;
	numOfObservers.val(_newValue);
	numOfObservers.change();
});
// End Increase or decrease the observer count