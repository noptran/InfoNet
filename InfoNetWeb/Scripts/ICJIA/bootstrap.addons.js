$(document).tooltip({ selector: '[data-toggle="tooltip"]', delay: { "show": 600, "hide": 100 }, container: "body", placement: "auto top" });

function rescanTooltipsForSR(root$) {
	$(root$).find('[data-toggle="tooltip"]:not(:has([data-icjia-role="tooltip.sr-only"]))').each(function () {
		var $this = $(this);
		var tip = $this.attr('title');
		if (tip == undefined)
			tip = $this.attr('data-original-title');
		if (tip != undefined)
			$this.append('<span class="sr-only" data-icjia-role="tooltip.sr-only">' + tip + '</span>');
	});
}

// scroll further up when focus shifts upward to an element obscured by navbar-fixed-top
function ensureActiveVisible() {
	var navbarBottom = $('.navbar.navbar-fixed-top').get(0).getBoundingClientRect().bottom;
	var $html = $('html');
	if (navbarBottom != undefined) {
		var $active = $(document.activeElement);
		var navbarOffset = navbarBottom + 15; /* where 15 = form-group.margin-bottom */
		var $formGroup = $active.closest('.form-group');
		if ($formGroup.length == 0) {
			$formGroup = $active;
			navbarOffset = navbarBottom + 8; /* where 8 = .table td.padding-top */
		}
		if ($formGroup.offset().top - $(window).scrollTop() < navbarOffset)
			$html.scrollTop($formGroup.offset().top - navbarOffset);
	}
}

$(function () {
	rescanTooltipsForSR(document);
});

$(document) /* Needed when using multiple modals, otherwise loses scroll functionality.*/
    .on('shown.bs.modal', '.modal', function () {
    	$(document.body).addClass('modal-open');
    })
    .on('hidden.bs.modal', '.modal', function () {
    	$(document.body).removeClass('modal-open');
    });

$(document).on('click', 'a:regex(href, ^#\\S+$)', function () {
	$($(this).attr('href') + '.icjia-bookmark > .panel > .panel-collapse.collapse:not(.in)').collapse('show');
});

function bootstrapFindSize() {
	var envs = ['xs', 'sm', 'md', 'lg'];

	var $el = $('<div>');
	$el.appendTo($('body'));

	for (var i = envs.length - 1; i >= 0; i--) {
		var env = envs[i];

		$el.addClass('hidden-' + env);
		if ($el.is(':hidden')) {
			$el.remove();
			return env;
		}
	}
}

function refreshScrollspy() {
	$('[data-spy="scroll"]').each(function () {
		$(this).scrollspy('refresh');
	});
}

$(document).on('keydown', ':input', function (e) {
	if (e.keyCode === 112 && e.altKey) {
		$('.tooltip').hide();
		if ($(this).attr('data-icjia-tooltip-parent')) {
			$("[data-icjia-tooltip='" + $(this).attr('data-icjia-tooltip-parent') + "']").first().tooltip('toggle');
		} else {
			if ($(this).parents('table').length > 0) {
				var $td = $(this).closest('td');
				var $th = $td.closest('table').find('th').eq($td.index());
				if ($th.closest('table').find('th').eq($td.index()).length > 0) {
					$th.find("[data-toggle='tooltip']").first().tooltip('toggle');
				}
			};

			var $parentFormGroup = $(this).parents('.form-group').find("[data-toggle='tooltip']").first();

			var $type = $(this).attr('type');
			if ($type === 'checkbox') {
				var $legend = $(this).parents('fieldset').find('legend');
				if ($legend.length > 0) {
					$legend.tooltip('toggle');
				}
			} else if ($type === 'button') {
			} else if ($parentFormGroup.length > 0) {
				$parentFormGroup.tooltip('toggle');
			}
		}
	}
});
