$(document).on('change', '.editRecord', function () {
    var $viewableRow = $(this).closest('tr').find("input[name$='DirectorEmail']").first().parent('div');
    var $changableRow = $viewableRow.next();
    if ($(this).is(':checked')) {
        $viewableRow.addClass('hidden');
        $changableRow.removeClass('hidden');
    } else {
        $changableRow.addClass('hidden');
        $viewableRow.removeClass('hidden');
        $(this).removeClass('input-validation-error').closest('.form-group').removeClass('has-error').find('span.help-block').html('');
    }
});

$(document.body).on('click', '#saveMain', function () {
	$("#main").submit();
});