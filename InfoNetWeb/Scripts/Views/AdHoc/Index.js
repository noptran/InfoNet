//KMS DO no_results_text: "Oops, nothing found!"
//KMS DO write better validation messages

(function() {

	//#region json building

	function nullIfEmpty(s) {
		return s == '' ? null : s;
	} 

	$(document).on('change', '[data-icjia-role="data"],[data-icjia-role="pivot"]', function() {
		var isPivot = $(this).closest('.panel').find('[data-icjia-role="pivot"]:checked').length == 1;
		$('#data').toggleClass('hide', isPivot);
		$('#pivot').toggleClass('hide', !isPivot);
		pruneGroupErrors(true);
		styleExistingErrors('#main', true);
	});

	$(document).on('change', '[data-icjia-role="and"],[data-icjia-role="or"]', function() {
		var $panel = $(this).closest('[data-icjia-role="conjunction"]');
		var isAnd = buildConjunction($panel).operator == 'And';
		$panel.toggleClass('or-panel', !isAnd);
		$panel.children().children('.panel-body').children('.and-or').html(isAnd ? 'AND' : 'OR');
	});

	/* Takes a [data-icjia-role="predicate"] and returns a new predicate {field, condition, inputs...} */
	function buildPredicate(predicate$) {
		var $predicate = $(predicate$);
		var result = {};
		result.field = nullIfEmpty($predicate.find('[data-icjia-role="predicateField"] option:selected').val());
		result.condition = nullIfEmpty($predicate.find('[data-icjia-role="predicateCondition"] option:selected').val());
		$predicate.find('[data-icjia-role="predicateInput"] [data-icjia-name]:not([data-icjia-name=""]):input').each(function() {
			var $each = $(this);
			result[$each.data('icjia-name')] = nullIfEmpty($each.val());
		});
		return result;
	}

	/* Takes a [data-icjia-role="conjunction"] and returns a new conjunction {operator, predicates[], conjunctions[]} */
	function buildConjunction(conjunction$) {
		var $conjunction = $(conjunction$);
		var $panel = $conjunction.children('.panel-collapse');
		var $panelBody = $panel.children('.panel-body');
		var $panelFooter = $panel.children('.panel-footer');
		var isAnd = $panelFooter.find('[data-icjia-role="and"]:checked').length == 1;
		var result = { operator:  isAnd ? 'And' : 'Or', predicates: []};
		$panelBody.children('[data-icjia-role="predicate"],[data-icjia-role="conjunction"]').each(function() {
			if ($(this).data('icjia-role') == 'predicate')
				result.predicates.push(buildPredicate(this));
			else
				result.predicates.push(buildConjunction(this));
		});
		return result;
	}

	function buildQuery() {
		var isPivot = $('[data-icjia-role="pivot"]:checked').length == 1;

		var result = { perspective: $('#perspective').val() };
		var title = $('#title').val();
		if (title != '')
			result.title = title;
		if (isPivot) {
			result.select = {
				rows: $('#rows').getSelectionOrder(),
				columns: $('#columns').getSelectionOrder(),
				aggregate: nullIfEmpty($('#aggregate').val())
			};
		} else {
			var select = [];
			$('#data option:selected').each(function() {
				select.push(this.value);
			});
			result.select = select;
		}
		var where = buildConjunction('#where');
		if (where.predicates.length)
			result.where = where;
		return result;
	}

	//#endregion

	//#region ui building

	$(document).on('click', '[data-icjia-role="addPredicate"],[data-icjia-role="addConjunction"]', function() {
		var $this = $(this);
		var $panelBody = $this.closest('.panel-collapse').children('.panel-body');
		var isAnd = buildConjunction($this.closest('[data-icjia-role="conjunction"]')).operator == 'And';
		var isPredicate = $this.data('icjia-role') == 'addPredicate';

		$.ajax({
			url: isPredicate ? '/AdHoc/Predicate' : '/AdHoc/Conjunction',
			type: 'GET',
			data: isPredicate
				? { perspective: $('#perspective').val(), predicateJson: '{}' }
				: { perspective: $('#perspective').val(), conjunctionJson: JSON.stringify({ operator: isAnd ? 'Or' : 'And', predicates: [{}, {}] }) },
			beforeSend: function() {
				//KMS DO show spinner
			},
			success: function(data) {
				var $data = $(data);
				initializeChosen($data.find('select'));
				if ($panelBody.children().length) 
					$panelBody.append('<div class="and-or">' + (isAnd ? 'AND' : 'OR') + '</div>');
				$panelBody.append($data);
				rescanUnobtrusiveValidation('#main', true);
			},
			error: function() {
				systemGrowl('Oh Snap!', 'An error occurred while adding a new condition', 'danger');
			},
			complete: function() {
				//KMS DO show spinner
			}
		});
	});

	$(document).on('click', '[data-icjia-role="deletePredicate"],[data-icjia-role="deleteConjunction"]', function() {
		var $removable = $(this).closest('[data-icjia-role="predicate"],[data-icjia-role="conjunction"]');
		var first = true;
		while (first || $removable.length && !$removable.find('[data-icjia-role="predicate"],[data-icjia-role="conjunction"]').length) {
			first = false;
			var $parentRemovable = $removable.parent().closest('[data-icjia-role="conjunction"]:not(#where)');
			var $andOr = $removable.prev('.and-or');
			if (!$andOr.length)
				 $andOr = $removable.next('.and-or');
			$andOr.remove();
			$removable.remove();
			$removable = $parentRemovable;
		}
		pruneGroupErrors(true);
	});

	$(document).on('change', '[data-icjia-role="predicate"] [data-icjia-role="predicateField"] select', function() {
		var $this = $(this);
		var $row = $this.closest('.row');
		$row.find('[data-icjia-role="predicateCondition"],[data-icjia-role="predicateInput"]').remove();
		pruneGroupErrors(true);
		var fieldId = $this.val();
		if (fieldId == '')
			return;

		$.ajax({
			url: '/AdHoc/PredicateCondition',
			type: 'GET',
			data: { perspective: $('#perspective').val(), predicateJson: JSON.stringify({ field: fieldId }) },
			beforeSend: function() {
				//KMS DO show spinner
			},
			success: function(data) {
				var $data = $(data);
				initializeChosen($data.find('select'));
				$row.find('[data-icjia-role="deletePredicate"]').before($data);
				rescanUnobtrusiveValidation('#main', true);
			},
			error: function() {
				systemGrowl('Oh Snap!', 'An error occurred while selecting a field to filter', 'danger');
			},
			complete: function() {
				//KMS DO hide spinner
			}
		});
	});

	$(document).on('change', '[data-icjia-role="predicate"] [data-icjia-role="predicateCondition"] select', function() {
		var $this = $(this);
		var $row = $this.closest('.row');
		$row.find('[data-icjia-role="predicateInput"]').remove();
		pruneGroupErrors(true);
		var fieldId = $row.find('[data-icjia-role="predicateField"] select').val();
		var condition = $this.val();
		if (condition == '')
			return;

		$.ajax({
			url: '/AdHoc/PredicateInput',
			type: 'GET',
			data: { perspective: $('#perspective').val(), predicateJson: JSON.stringify({ field: fieldId, condition: condition }) },
			beforeSend: function() {
				//KMS DO spinner?
			},
			success: function(data) {
				var $data = $(data);
				initializeChosen($data.find('select'));
				$row.find('[data-icjia-role="deletePredicate"]').before($data);
				rescanUnobtrusiveValidation('#main', true);
			},
			error: function() {
				systemGrowl('Oh Snap!', 'An error occurred while selecting a condition', 'danger');
			},
			complete: function() {
				//KMS DO spinner?
			}
		});
	});

	//#endregion

	//#region run menu

	function validateMain() {
		var isValid = $('#main').valid();
		if (buildQuery().select.length == 0 ) {
			$('#dataHelp').addClass('field-validation-error').removeClass('hide');
			styleExistingErrors('#main', false); //KMS DO should this be true?
			isValid = false;
		}
		if (isValid)
			return true;
		
		$('#main .collapse:not(.in):has(.input-validation-error,.field-validation-error)').each(function () {
			$(this).collapse('show');
		});
		var $firstInvalid = $('#main .input-validation-error').first();
		if ($firstInvalid.data('chosen'))
			$firstInvalid.trigger('chosen:activate');
		else
			$firstInvalid.focus();
		return false;
	}

	$(document).on('click', '#runHtml,#runPdf', function(e) {
		e.preventDefault();
		if ($('.modal.in').length)
			return;

		if (!validateMain())
			return;

		var query = buildQuery();
		var isPdf = this.id == 'runPdf';
		var $form = $('<form method="post" action="/AdHoc/Run"' + (isPdf ? '' : ' target="_blank"') + '></form>');
		$('#id[value]:not([value=""])').clone().appendTo($form);
		if (isPdf)
			$form.append('<input type="hidden" name="isPdf" value="true"/>');
		var $hiddenJson = $('<input type="hidden" name="queryJson"/>');
		$hiddenJson.appendTo($form);
		$form.appendTo('body');
		$hiddenJson.val(JSON.stringify(query));
		$form.submit();
	});

	$(document).on('click', '#exportCsv', function(e) {
		e.preventDefault();

		if (!validateMain())
			return;

		var query = buildQuery();
		var $form = $('<form method="post" action="/AdHoc/ExportCsv"></form>');
		$('#id[value]:not([value=""])').clone().appendTo($form);
		var $hiddenJson = $('<input type="hidden" name="queryJson"/>');
		$hiddenJson.appendTo($form);
		$form.appendTo('body');
		$hiddenJson.val(JSON.stringify(query));
		$form.submit();
	});

	$(document).on('click', '#outputSql', function(e) {
		e.preventDefault();

		if (!validateMain())
			return;

		var query = buildQuery();
		$.ajax({
			url: '/AdHoc/OutputSql',
			type: 'POST',
			data: { queryJson: JSON.stringify(query) },
			beforeSend: function() {
				//KMS DO show spinner
			},
			success: function(data) {
				var $mainContent = $('#mainContent');
				$mainContent.append(data);
				var $modal = $mainContent.children().last();
				$modal.modal({ show: true, backdrop: true });
			},
			error: function() {
				systemGrowl('Oh Snap!', 'An error occurred while generating Query SQL', 'danger');
			},
			complete: function() {
				//KMS DO hide spinner
			}
		});
	});

	$(document).on('click', '#outputJson', function(e) {
		e.preventDefault();

		var query = buildQuery();
		$.ajax({
			url: '/AdHoc/OutputJson',
			type: 'POST',
			data: { queryJson: JSON.stringify(query) },
			beforeSend: function() {
				//KMS DO show spinner
			},
			success: function(data) {
				var mainContent = $('#mainContent');
				mainContent.append(data);
				var $modal = mainContent.children().last();
				$modal.modal({ show: true, backdrop: true });
			},
			error: function() {
				systemGrowl('Oh Snap!', 'An error occurred while generating Report JSON', 'danger');
			},
			complete: function() {
				//KMS DO hide spinner
			}
		});
	});

	//#endregion

	//#region query menu

	function saveQuery(asName) {
		var id = nullIfEmpty($('#id').val());
		var name = nullIfEmpty($('#name').val());
		if (asName != undefined && name != asName) {
			id = null;
			name = asName;
		}
		var query = buildQuery();
		$.ajax({
			url: '/AdHoc/Save',
			type: 'POST',
			data: { id: id, name: name, queryJson: JSON.stringify(query) },
			beforeSend: function() {
				//KMS DO show spinner
			},
			success: function(data) {
				$('#id').val(data.id);
				$('#name').val(data.name);
				$('title').text('Ad Hoc: ' + data.name);
				$('#pageHeaderText').html('Ad Hoc: <b>' + data.name + '</b>');
				systemGrowl('Hooray!', 'Your changes have been successfully saved.', 'success', 1500);
				$('#delete').closest('li').removeClass('disabled');
			},
			error: function() {
				systemGrowl('Oh Snap!', 'An error occurred while saving your changes.', 'danger');
			},
			complete: function() {
				//KMS DO hide spinner
			}
		});
	}

	function deleteQuery() {
		var id = nullIfEmpty($('#id').val());
		$.ajax({
			url: '/AdHoc/Delete',
			type: 'POST',
			data: { id: id },
			beforeSend: function() {
				//KMS DO show spinner
			},
			success: function() {
				window.location.href = '/AdHoc';
			},
			error: function() {
				systemGrowl('Oh Snap!', 'An error occurred while deleting the query.', 'danger');
			},
			complete: function() {
				//KMS DO hide spinner
			}
		});
	}

	$(document).on('click', '#new', function(e) {
		if ($('.modal.in').length)
			e.preventDefault();
	});

	$(document).on('click', '#open', function(e) {
		e.preventDefault();
		if ($('.modal.in').length)
			return;

		$.ajax({
			url: '/AdHoc/Open',
			type: 'GET',
			beforeSend: function() {
				//KMS DO show spinner
			},
			success: function(data) {
				var mainContent = $('#mainContent');
				mainContent.append(data);
				var $modal = mainContent.children().last();
				$modal.modal({ show: true, backdrop: true });
			},
			error: function() {
				systemGrowl('Oh Snap!', 'An error occurred while retrieving list of available queries.', 'danger');
			},
			complete: function() {
				//KMS DO hide spinner
			}
		});
	});

	$(document).on('click', '#save', function(e) {
		e.preventDefault();
		if ($('.modal.in').length)
			return;

		if ($('#name').val() == '')
			$('#saveAsModal').modal('show');
		else
			saveQuery();
	});

	$(document).on('click', '#saveAs', function(e) {
		e.preventDefault();
		if ($('.modal.in').length)
			return;

		$('#saveAsModal').modal('show');
	});

	$(document).on('click', '#saveAsName', function(e) {
		e.preventDefault();

		var $newName = $('#newName');
		if (!$newName.valid()) {
			$newName.focus();
			return;
		}
		$('#saveAsModal').modal('hide');
		saveQuery($newName.val());
	});

	$(document).on('keyup', '#newName', function(e) {
		if (e.which == 13) {
			e.preventDefault();
			$('#saveAsName').trigger('click');
		}
	});

	$(document).on('click', '#delete', function(e) {
		e.preventDefault();
		if ($('#id').val() == '')
			return;

		$.confirm({
			text: "If you continue, query <strong>" + $('#name').val() + "</strong> will be <strong>permanently deleted</strong>.",
			confirmButtonClass: "btn-danger",
			dialogClass: "modal-dialog icjia-modal-danger",
			confirm: deleteQuery
		});
	});

	//#endregion

	function initializeChosen(selector$) {
		var $chosenSelects = $(selector$);
		$chosenSelects.chosen({ allow_single_deselect: true, include_group_label_in_selected: true });
		$chosenSelects.on('change', function () {
			var $this = $(this);
			$this.valid();
			if ($this.closest('#data').length && $this.find('option:selected').length && $('#dataHelp').hasClass('field-validation-error')) {
					$('#dataHelp').removeClass('field-validation-error').addClass('hide');
					pruneGroupErrors(true);
				}
		});
	}

	$(function() {
		initializeChosen('select');
		$('select[data-icjia-chosen]').each(function() {
			var $this = $(this);
			$this.setSelectionOrder($this.data('icjia-chosen'), true);
		});

		$('#selectCollapse,#whereCollapse').collapse('show');

		$('#saveAsModal').modal({ show: false, backdrop: true })
			.on('show.bs.modal', function() {
					$('#newName').val($('#name').val());
				})
			.on('shown.bs.modal', function() {
					$('#newName').focus();
				});
	});
})();