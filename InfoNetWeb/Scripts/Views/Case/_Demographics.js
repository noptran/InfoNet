$(function () {
	$('#Client_ClientCode').focusout(function () {
		if ($(this).val() == $('#LastSavedClientCode').val()) {
			ErrorFormat($(this), $(this).attr('name'), "This Client ID is already in use.");
			return false;
		} else {
			if ($(this).hasClass('input-validation-error')) ErrorClear($(this), $(this).attr('name'), "This Client ID is already in use.");
		}
	});
});