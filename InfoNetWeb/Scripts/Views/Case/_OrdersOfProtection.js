$(document).ready(function () {
    $('textarea').each(function () {
        updateCountdown($(this));
    });
});

$('#ordersCollapse').on('change keyup', 'textarea', function () {
    updateCountdown($(this));
});

// Order Of Protection add button click
$(document).on('click', '[data-icjia-role="orderofprotection.add"]', function () {
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

// Order Of Protection delete button clicks
$(document).on('click', '[data-icjia-role="orderofprotection.delete"]', function () {
    var $primaryRow = $(this).closest('tr').prev();

    $primaryRow.next().find('[data-icjia-role="orderofprotection.index"]').each(function () {
        var sign = this.value.substring(0, 1) == '=' ? '-' : '~';
        this.value = sign + this.value.substring(1);
        $(this).change();
    });
    $(this).closest('.collapse').collapse('hide');
    window.setTimeout(function () {
        $primaryRow.addClass('deleted');
        $primaryRow.find('[data-icjia-role="orderofprotection.expand"]').addClass('hide');
        $primaryRow.find('[data-icjia-role="orderofprotection.restore"]').removeClass('hide');
    }, 350); /* rotate-90-if-collapsed transition ms */
});

// Order Of Protection restore
$(document).on('click', '[data-icjia-role="orderofprotection.restore"]', function () {
    var $primaryRow = $(this).closest('tr');

    $primaryRow.removeClass('deleted');
    $primaryRow.next().find('[data-icjia-role="orderofprotection.index"]').each(function () {
        var sign = this.value.substring(0, 1) == '-' ? '=' : '+';
        this.value = sign + this.value.substring(1);
        $(this).change();
    });
    var $expandButton = $primaryRow.find('[data-icjia-role="orderofprotection.expand"]').removeClass('hide');
    $primaryRow.find('[data-icjia-role="orderofprotection.restore"]').addClass('hide');
    $primaryRow.next().find('.collapse').collapse('show');
    $expandButton.focus();
});

// disable well collapse when OrderOfProtection has been deleted
$(document).on('show.bs.collapse', 'tr:has([data-icjia-role="orderofprotection.index"]:regex(value, ^[-~])) .collapse', function (event) {
	event.preventDefault();
});

// Order Of Protection Activity add button click
$(document).on('click', '[data-icjia-role="orderofprotection.activity.add"]', function () {
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

// Order Of Protection Activity delete button clicks
$(document).on('click', '[data-icjia-role="orderofprotection.activity.delete"]', function () {
    var $primaryRow = $(this).closest('tr');

    $primaryRow.find('[data-icjia-role="orderofprotection.activity.index"]').each(function () {
        var sign = this.value.substring(0, 1) == '=' ? '-' : '~';
        this.value = sign + this.value.substring(1);
        $(this).change();
    });

    $primaryRow.addClass('hidden');
    $primaryRow.next().removeClass('hidden');
});

// Order Of Protection activity restore
$(document).on('click', '[data-icjia-role="orderofprotection.activity.restore"]', function () {
    var $primaryRow = $(this).closest('tr');

    $primaryRow.prev().find('[data-icjia-role="orderofprotection.activity.index"]').each(function () {
        var sign = this.value.substring(0, 1) == '-' ? '=' : '+';
        this.value = sign + this.value.substring(1);
        $(this).change();
    });
    $primaryRow.addClass('hidden');
    $primaryRow.prev().removeClass('hidden');
});


$(document).on('click', '[data-icjia-role="orderofprotection.activity.delete"]', function () {
    var numOfActivities = $(this).closest("div[id$='_Collapse']").find("input[id$='OrderOfProtectionActivities']");
    var _newValue = numOfActivities.val() - 1;
    numOfActivities.val(_newValue);
    numOfActivities.change();
});

$(document).on('click', '[data-icjia-role="orderofprotection.activity.add"]', function () {
    var numOfActivities = $(this).closest("div[id$='_Collapse']").find("input[id$='OrderOfProtectionActivities']");
    var _newValue = parseInt(numOfActivities.val()) + 1;
    numOfActivities.val(_newValue);
    numOfActivities.change();
});

$(document).on('click', '[data-icjia-role="orderofprotection.activity.restore"]', function () {
    var numOfActivities = $(this).closest("div[id$='_Collapse']").find("input[id$='OrderOfProtectionActivities']");
    var _newValue = parseInt(numOfActivities.val()) + 1;
    numOfActivities.val(_newValue);
    numOfActivities.change();
});

function updateCountdown($this) {
    var numOfCharactersPlaceHolder = $this.closest('div').find('span[id$="charRemaining"]');
    // 300 is the max message length
    var remaining = 300 - jQuery($this).val().length;//
    jQuery(numOfCharactersPlaceHolder).text(remaining);
}


// delete confirmation dialog for Offenders, Police Charges, Trial Charges, and Sentences
(function () {

    var pluralize = function (noun, count) {
        if (noun.match("y$")) 
            return noun.replace('y', '') + (count > 1 ? 'ies' : 'y');
        else
            return noun + (count > 1 ? 's' : '');
    }

    var deletesConfirmed = false;

    $(document).on('submit-valid.icjia', '#main', function (e) {
        if (deletesConfirmed) {
            deletesConfirmed = false;
            return;
        }

        var $main = $(this);

        var $ordersOfProtectionDeleted = $main.find('.well:has([data-icjia-role="orderofprotection.index"][value^="-"])');
        var $ordersOfProtectionRetained = $main.find('.well:has([data-icjia-role="orderofprotection.index"][value^="="])');
        var ordersOfProtectionDeleted = $ordersOfProtectionDeleted.length;
        var deletedOrderOfProtectionActivities = $ordersOfProtectionDeleted.find('[data-icjia-role="orderofprotection.activity.index"]:regex(value, ^[\\-=])').length;

        var activitiesDeleted = $ordersOfProtectionRetained.find('[data-icjia-role="orderofprotection.activity.index"][value^="-"]').length;

        var $courtAppearancesDeleted = $('tr:has([data-icjia-role="courtAppearance.index"][value^="-"])');
        var courtAppearancesDeleted = $courtAppearancesDeleted.length;

        var total = ordersOfProtectionDeleted + activitiesDeleted + courtAppearancesDeleted;
        if (total == 0)
            return;

        var buffer = '';
        if (courtAppearancesDeleted > 0)
            buffer += '<li>' + courtAppearancesDeleted + pluralize(' Court Appearance', courtAppearancesDeleted) + '</li>';
        if (activitiesDeleted > 0)
            buffer += '<li>' + activitiesDeleted + pluralize(' Activity', activitiesDeleted) + '</li>';

        if (ordersOfProtectionDeleted > 0) {
            buffer += '<li>' + ordersOfProtectionDeleted + pluralize(' Order', ordersOfProtectionDeleted) + ' Of Protection';
            if (deletedOrderOfProtectionActivities > 0) {
                buffer += ' <span style="font-style: italic; font-weight: normal">(including ';
                if (deletedOrderOfProtectionActivities > 0)
                    buffer += deletedOrderOfProtectionActivities + pluralize(' Activity', deletedOrderOfProtectionActivities);
                buffer += ')</span>';
            }
            buffer += '</li>';
        }

        e.preventDefault();
        $.confirm({
            text: "You've marked the following for deletion: <ul class='text-danger' style='font-weight: bold; list-style: square; margin-top: 10px;'>" + buffer + "</ul> If you continue, these records will be <span style='font-weight: bold'>permanently deleted</span>.",
            confirmButtonClass: "btn-danger",
            dialogClass: "modal-dialog icjia-modal-danger",
            confirm: function () {
                deletesConfirmed = true;
                $main.submit();
            }
        });
    });

})();



