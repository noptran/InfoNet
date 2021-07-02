$(document).ready(function () {
	$('#OutputType').change(function () {
		var $this = $(this);
		togglePdfOptions($this.val());
		toggleFormTarget($this.val(), $this.closest('form'));
	});
});

function togglePdfOptions(value) {
	if (value == 3) {
		$('#pdfModalToggle').removeClass('hide');
	} else {
		$('#pdfModalToggle').addClass('hide');
	}
}

function toggleFormTarget(value, $form) {
	if (value == 1) {
		$form.attr('target', '_blank');
	} else {
		$form.attr('target', '_self'); 
	}
}