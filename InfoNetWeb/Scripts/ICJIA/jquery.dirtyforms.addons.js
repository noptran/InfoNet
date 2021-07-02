/* configure dirtyforms with a custom are-you-sure dialog and a helper allowing
   form dirty status to be forced programmatically. */
$(function () {

	/* create custom are-you-sure dialog for dirtyforms */
	$('<div id="icjia-dirty-dialog" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="icjia-dirty-title">' +
			'<div class="modal-dialog icjia-modal-danger" role="document">' +
				'<div class="modal-content">' +
					'<div class="modal-header">' +
						'<button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>' +
						'<h4 class="modal-title" id="icjia-dirty-title"></h4>' +
					'</div>' +
					'<div class="modal-body icjia-dirty-message"></div>' +
					'<div class="modal-footer">' +
						'<button type="button" class="icjia-dirty-proceed btn btn-danger" data-dismiss="modal"></button>' +
						'<button type="button" class="icjia-dirty-stay btn btn-default" data-dismiss="modal"></button>' +
					'</div>' +
				'</div>' +
			'</div>' +
		'</div>').appendTo('body');

	/* enable DirtyForms with above dialog */
	$('form[data-icjia-role="dirty.form"]').dirtyForms({
		message: "You've made changes on this page which aren't saved.<br/><br/>If you leave, those changes will be lost.",
		dialog: {
			dialogID: 'icjia-dirty-dialog',
			titleID: 'icjia-dirty-title',
			messageClass: 'icjia-dirty-message',
			proceedButtonClass: 'icjia-dirty-proceed',
			stayButtonClass: 'icjia-dirty-stay'
		},
		ignoreSelector: 'a[href^="#"], a:not([href])',
		helpers: [
			{
				isDirty: function ($node, index) {
					if ($node.is('form'))
						return $node.hasClass('icjia-make-dirty');
				},
				setClean: function ($node, index, excludeIgnored) {
					if ($node.is('form'))
						$node.removeClass('icjia-make-dirty');
				}
			}
		]
	});

});


/* force forms (or fragments) that load with validation errors to be dirty.
   assumes such errors resulted from posted but unsaved changes. */
function makeErrorsDirty(selector$) {
	var $selector = $(selector$);
	var $errors = $selector.find('.input-validation-error, .field-validation-error');
	if ($selector.is('.input-validation-error, .field-validation-error'))
		$errors.add($selector);
	$errors.closest('form[data-icjia-role="dirty.form"]').addClass('icjia-make-dirty').trigger('dirty.dirtyforms');
}

/* perform above check on page load */
$(function () {
	makeErrorsDirty('form[data-icjia-role="dirty.form"]');
});


/* add layer icjia events and classes on top of dirtyforms events and classes.
   the icjia events will distinguish between form and page level dirtyness.
   the icjia-dirty class will combine the dirty and icjia-make-dirty classes
   and be applied at both the form and page levels. */
(function () {

	/* refresh icjia-dirty status of specified 'this' form */
	var refreshFormIcjiaDirty = function () {
		var $form = $(this);
		var wasDirty = $form.hasClass('icjia-dirty');
		var isDirty = $form.hasClass('dirty') || $form.hasClass('icjia-make-dirty');
		if (isDirty != wasDirty) {
			$form.toggleClass('icjia-dirty', isDirty);
			$form.trigger(jQuery.Event(isDirty ? "dirty.form.icjia" : "clean.form.icjia"));
		}
	}

	/* refresh icjia-dirty status for entire page */
	var refreshPageIcjiaDirty = function () {
		var $page = $('html');
		var wasDirty = $page.hasClass('icjia-dirty');
		var isDirty = $('form.dirtylisten').hasClass('icjia-dirty');
		if (isDirty != wasDirty) {
			$page.toggleClass('icjia-dirty', isDirty);
			$page.trigger(jQuery.Event(isDirty ? "dirty.page.icjia" : "clean.page.icjia"));
		}
	}

	/* refresh icjia-dirty status for modal containing form(s) */
	var refreshModalIcjiaDirty = function () {
		var $modal = $(this);
		var wasDirty = $modal.hasClass('icjia-dirty');
		var isDirty = $modal.find('form.dirtylisten').hasClass('icjia-dirty');
		if (isDirty != wasDirty) {
			$modal.toggleClass('icjia-dirty', isDirty);
			$modal.trigger(jQuery.Event(isDirty ? "dirty.modal.icjia" : "clean.modal.icjia"));
		}
	}

	/* register events to refresh above as needed */
	var $document = $(document);
	$document.on('dirty.form.icjia clean.form.icjia', 'form.dirtylisten', refreshPageIcjiaDirty);
	$document.on('dirty.form.icjia clean.form.icjia', '.modal:has(form.dirtylisten)', refreshModalIcjiaDirty);
	$document.on('dirty.dirtyforms clean.dirtyforms', 'form.dirtylisten', refreshFormIcjiaDirty);

	/* refresh all forms when document ready */
	$(function () { $('form.dirtylisten').each(refreshFormIcjiaDirty); });

})();


/* add confirmation dialog to links but optionally skip when clean */
$(function () {

	/* add confirmation dialog to links */
	$('a[data-icjia-role="dirty.page.confirm"],a[data-icjia-role="dirty.page.confirm.always"]').each(function () {
		$(this).confirm({
			confirm: function (link) {
				$('form.dirtylisten').dirtyForms('setClean');
				window.location = $(link).attr('href');
			}
		});
	});

	/* skip confirmation dialog when page is clean */
	$(document).on('prompt-confirm.icjia', '[data-icjia-role="dirty.page.confirm"]', function (e) {
		if (!$('html').hasClass('icjia-dirty'))
			e.preventDefault();
	});

	/* add confirmation dialog to modal buttons */
	$('button[data-icjia-role="dirty.modal.confirm"],button[data-icjia-role="dirty.modal.confirm.always"]').each(function () {
		$(this).confirm({
			confirm: function (button) {
				var $modal = $(button).closest('form').closest('.modal');
				$modal.modal('hide');
				//KMS DO will this timeout cause problems?
				setTimeout(function () { $modal.find('form.dirtylisten').dirtyForms('setClean'); }, 300); /* .modal.fade .modal-dialog transition-duration */
			}
		});
	});

	/* skip confirmation dialog when modal is clean */
	$(document).on('prompt-confirm.icjia', '[data-icjia-role="dirty.modal.confirm"]', function (e) {
		var $modal = $(this).closest('form').closest('.modal');
		if (!$modal.hasClass('icjia-dirty')) {
			e.preventDefault();
			$modal.modal('hide');
			//KMS DO wouldn't be nice if confirm had an event...we wouldn't have to duplicate all of this...
		}
	});

});