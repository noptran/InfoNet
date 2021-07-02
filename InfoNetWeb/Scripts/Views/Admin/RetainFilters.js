$(function () {
	copyFilters(true);
});

$('#Search').on('click', function () {
	copyFilters(false);
});

function copyFilters(createHiddenInputs) {
	$('#main :input:visible').not(':button,:submit').each(function () {
		var name = $(this).attr('name');
		if (createHiddenInputs) {
			$('<input>').attr({ 'type': 'hidden', 'name': name }).appendTo('#firstForm, #secondForm');
		}
		$('input:hidden[name="' + name + '"]').val($(this).val());
	});
}