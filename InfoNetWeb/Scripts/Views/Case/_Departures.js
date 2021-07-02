$(function () {
	$(document).on('change', 'select[data-icjia="set-view-for"]', function () {
		if($("#" + this.id + " option:selected").val() != 0){
			$('[data-icjia-view-for="' + this.id + '"]').text($("#" + this.id + " option:selected").text());
		} else {
			$('[data-icjia-view-for="' + this.id + '"]').text("");
		}
	});
	$(document).on('keyup', 'input[data-icjia="set-view-for"]', function () {
		$('[data-icjia-view-for="' + this.id + '"]').text($("#" + this.id).val());
	});
	$(document).on('blur', 'input[data-icjia="set-view-for"]', function () {
		$('[data-icjia-view-for="' + this.id + '"]').text($("#" + this.id).val());
	});
	$(document).on('change', 'input[data-icjia="set-view-for"]', function () {
		$('[data-icjia-view-for="' + this.id + '"]').text($("#" + this.id).val());
	});

	var viewText = function (model, view) {
		$(view).text(model.value);
	}
	var viewSelectOne = function (model, view) {
		if (model.selectedIndex == -1 || model.value == '') {
			$(view).text('');
		} else {
			$(view).text(model.options[model.selectedIndex].text);
		}
	}
	$('[data-icjia-view-for][data-icjia-view-for!=""]').each(function () {
		var view = this;
		var model = document.getElementById($(this).data('icjia-view-for'));
		if (model.type == 'text') {
			viewText(model, view);
			$(model).on('change keyup blur', function () {
				viewText(this, view);
			});
		} else if (model.type == 'select-one') {
			viewSelectOne(model, view);
			$(model).on('change', function () {
				viewSelectOne(this, view);
			});
		}
	});

	$('.collapse:not(.in):has(.input-validation-error,.field-validation-error)').each(function () {
		$(this).collapse('show');
	});


	$(document).on('click', '[data-icjia-role="departure.destination.delete"]', function () {
		var $currentRow = $(this).closest('tr');
		var $mainRow = $currentRow.prev();

		$currentRow.find("[name$='.IsDeleted']").val(true).trigger('change');
		$mainRow.addClass('deleted');
		$(this).closest('.collapse').collapse('hide');

		$.when($mainRow.find('[data-icjia-role="departure.destination.expand"]').addClass('hide')).then(function (x) {
			$mainRow.find('[data-icjia-role="departure.destination.restore"]').removeClass('hide');
		});
	});


	$(document).on('click', '[data-icjia-role="departure.destination.restore"]', function () {
		var $mainRow = $(this).closest('tr');
		var $subRow = $mainRow.next();
		var $expandButton = $mainRow.find('[data-icjia-role="departure.destination.expand"]').removeClass('hide');

		$mainRow.removeClass('deleted');
		$subRow.find("[name$='.IsDeleted']").val('False').change();

		$.when($mainRow.find('[data-icjia-role="departure.destination.restore"]').addClass('hide')).then(function (x) {
			$mainRow.next().find('.collapse').collapse('show');
		});
		$expandButton.focus();
	});

	$(document).on('click', '[data-icjia-role="departure.destination.add"]', function () {
		var nextIndex = $("table#departureModify > tbody > tr.cntDeparturesAdd").length;

		$.ajax({
			url: '/Case/DepartureAdd',
			type: 'GET',
			data: { index: nextIndex },
			success: function (results) {
				var tBody = $("table#departureModify tbody");
				$.when($(tBody).append(results)).then(function (x) {
					$(tBody).find('.collapse:last').collapse('show');
					$(tBody).find('tr:last :input').first().focus();
					rescanUnobtrusiveValidation("#main");
					$("#main").dirtyForms('rescan');
				});
			}
		});
	});
})

