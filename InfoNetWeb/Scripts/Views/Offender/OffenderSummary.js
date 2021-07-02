$(document).ready(function () {
	$('#saveButton').click(function () {
		if ($('#OffenderCode').hasClass('dirty')) {
			$.confirm({
				title: "Are you sure you want to do that?",
				text: "You are about to edit the Offender ID. This change connot be undone and will affect all cases for this client. Are you sure want to proceed?",
				confirm: function () {
					$('#main').submit();
				}
			});
		}
		else {
			$('#main').submit();
		}
	});
});