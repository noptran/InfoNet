function ErrorClear($selector, dataValMsgFor, validationMessage) {
	if (!$selector.jquery)
		$selector = $('input[name=' + $selector + ']');

	$selector.closest('.form-group').removeClass('has-error');
	$selector.closest('.icjia-error-group').removeClass('has-group-error');

	if ($('span[data-valmsg-for="' + dataValMsgFor + '"]').html() == validationMessage)
		$('span[data-valmsg-for="' + dataValMsgFor + '"]').html("");
}

function ErrorFormat($selector, dataValMsgFor, validationMessage) {
	if (!$selector.jquery)
		$selector = $('input[name=' + $selector + ']');

	$selector.closest('.form-group').addClass('has-error');
	$selector.closest('.icjia-error-group').addClass('has-group-error');
	$('span[data-valmsg-for="' + dataValMsgFor + '"]').html(validationMessage);
}